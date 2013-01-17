using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using FlixSharp.Constants;
using FlixSharp.Holders;
using FlixSharp.Queries;
using System.Collections;
using System.Threading;
using System.Collections.Concurrent;
using FlixSharp.Holders.Netflix;

namespace FlixSharp.Async
{
    internal class AsyncHelpers
    {
        public static Dictionary<LeakType, SlowLeak> Leaker = new Dictionary<LeakType, SlowLeak>();
        internal enum LeakType
        {
            Netflix,
            RottenTomatoes,
            IMDB,
            Redbox,
            Amazon
        }
        private static async Task<String> LoadStringAsync(String url, LeakType type)
        {
            if (!Leaker.ContainsKey(type))
            {
                Int32 rate = type == LeakType.Amazon ? 1 : 8; ///I believe everyone but Amazon can handle 8-10 per second
                Leaker[type] = new SlowLeak(rate, 1000);
            }
            using (WebClient wc = new WebClient())
            {
                Leaker[type].CheckLeak();
                return await wc.DownloadStringTaskAsync(url);
            }
        }

        private static async Task<XDocument> NetflixThrottleLoadXDocumentAsync(String url)
        {
            try
            {
                String xml = await LoadStringAsync(url, LeakType.Netflix);
                return XDocument.Parse(xml);
            }
            catch (WebException ex)
            {
                if (ex.Response.Headers["X-Mashery-Error-Code"] == "ERR_403_DEVELOPER_OVER_QPS")
                {
                    throw new NetflixThrottleException(ex);
                }
                else if (ex.Response.Headers["X-Mashery-Error-Code"] == "ERR_403_DEVELOPER_OVER_RATE")
                {
                    throw new NetflixOverQuotaException(ex, ex.Response.Headers["Retry-After"]);
                }
                throw;
            }
        }
        public static async Task<XDocument> NetflixLoadXDocumentAsync(String url)
        {
            Boolean retry = false;
            try
            {
                return await NetflixThrottleLoadXDocumentAsync(url);
            }
            catch (NetflixThrottleException)
            { retry = true; }
            if (retry)
            {
                Thread.Sleep(150); 
                return await NetflixLoadXDocumentAsync(url);
            }
            return null;
        }

       


        public static async Task<IEnumerable<Title>> GetExpandedMovieDetails(XDocument doc, Boolean OnUserBehalf = true)
        {
            List<Task<Title>> titles = new List<Task<Title>>();

            var movieids = from movie
                    in doc.Element("catalog_titles").Elements("catalog_title")
                    select movie.Element("id").Value;

            foreach (String id in movieids)
            {
                ///loop through and do a NetflixFill.whatever to fill the titles
                titles.Add(FlixSharp.Netflix.Fill.Titles.GetExpandedTitle(id, OnUserBehalf));
            }

            List<Title> awaitedtitles = new List<Title>();
            foreach (Task<Title> t in titles)
                awaitedtitles.Add(await t);
            return awaitedtitles;
        }

        public static async Task<IEnumerable<Title>> GetCompleteMovieDetails(XDocument doc, Boolean OnUserBehalf = true)
        {
            List<Task<Title>> titles = new List<Task<Title>>();
            var movieids = from movie
                    in doc.Element("catalog_titles").Elements("catalog_title")
                    select movie.Element("id").Value;
   
            foreach (String id in movieids)
            {
                titles.Add(FlixSharp.Netflix.Fill.Titles.GetCompleteTitle(id, OnUserBehalf));
            }

            List<Title> awaitedtitles = new List<Title>();
            foreach (Task<Title> t in titles)
                awaitedtitles.Add(await t);
            return awaitedtitles;
        }

        public static async Task<IEnumerable<Person>> GetCompletePersonDetails(XDocument doc, Boolean OnUserBehalf = true)
        {
            List<Task<Person>> people = new List<Task<Person>>();
            var personids = from person
                            in doc.Element("people").Elements("person")
                            select person.Element("id").Value;

            foreach (String id in personids)
            {
                people.Add(FlixSharp.Netflix.Fill.People.GetCompletePerson(id, OnUserBehalf));
            }

            List<Person> awaitedpeople = new List<Person>();
            foreach (Task<Person> p in people)
                awaitedpeople.Add(await p);
            return awaitedpeople;
        }
    }
   
    /// <summary>
    /// Thanks to Jack Leitch at http://www.pennedobjects.com/2010/10/better-rate-limiting-with-dot-net/ for original code for this
    /// Modified to simplify for my understanding, but the idea and structure was taken from Jack.
    /// </summary>
    internal class SlowLeak
    {
        private readonly SemaphoreSlim semaphore;
        private readonly ConcurrentQueue<Int32> exittimes;
        private readonly Timer exittimer;

        private Int32 rate = 0;
        private Int32 waittime = 0;

        public SlowLeak(Int32 Rate, Int32 WaitTimeInMs)
        {
            rate = Rate;
            waittime = WaitTimeInMs;

            semaphore = new SemaphoreSlim(rate, rate);
            exittimes = new ConcurrentQueue<Int32>();
            exittimer = new Timer(Leak, null, WaitTimeInMs, -1);
        }

        private void Leak(object state)
        {
            int exitTime;
            while (exittimes.TryPeek(out exitTime) && unchecked(exitTime - Environment.TickCount) <= 0)
            {
                semaphore.Release();
                exittimes.TryDequeue(out exitTime);
            }

            int timeUntilNextCheck;
            if (exittimes.TryPeek(out exitTime))
                timeUntilNextCheck = unchecked(exitTime - Environment.TickCount);
            else
                timeUntilNextCheck = waittime;

            exittimer.Change(timeUntilNextCheck, -1);
        }

        private Boolean CheckLeak(int millisecondsTimeout)
        {
            var entered = semaphore.Wait(millisecondsTimeout);
            if (entered)
            {
                var timeToExit = unchecked(Environment.TickCount + waittime);
                exittimes.Enqueue(timeToExit);
            }
            return entered;
        }

        public Boolean CheckLeak()
        {
            return CheckLeak(Timeout.Infinite);
        }

        public void Dispose()
        {
            semaphore.Dispose();
            exittimer.Dispose();
        }
    }

    public class NetflixOverQuotaException : NetflixException
    {
        public DateTime RetryAt { get; set; }

        public NetflixOverQuotaException(WebException ex, String retryafter = "")
            : base("ERR_403_DEVELOPER_OVER_RATE", ex)
        {
            Int32 retrytime;
            if (Int32.TryParse(retryafter, out retrytime))
            {
                this.RetryAt = DateTime.Now.AddSeconds(retrytime);
            }
            else
                this.RetryAt = new DateTime();
        }
    }
    public class NetflixThrottleException : NetflixException
    {
        public NetflixThrottleException(WebException ex) 
            : base("ERR_403_DEVELOPER_OVER_QPS", ex)
        {
        }
    }
    public class NetflixException : WebException
    {
        public NetflixException(WebException ex)
            : base(ex.Message, ex)
        {
        }
        public NetflixException(String Message, WebException ex)
            : base(Message, ex)
        {
        }
    }

    public class NetflixApiException : Exception
    {
        public NetflixApiException(NetflixException ex)
            : base("FlixSharp had some issues - Inner exception contains information", ex)
        {
        }
        public NetflixApiException(String message, NetflixException ex)
            : base("FlixSharp had some issues - " + message, ex)
        {
        }
    }
}
