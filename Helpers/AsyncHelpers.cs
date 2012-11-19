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

namespace FlixSharp.Async
{
    internal class AsyncHelpers
    {
        ///10 per second seems to be a liberal estimate by Netflix - they are strict....
        ///seems to trigger failures unless its throttled quite a bit?
        public static SlowLeak Leak = new SlowLeak(8, 1000); 

        public static async Task<XDocument> ThrottleLoadXDocumentAsync(String url)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("user-agent", "FilmTrove");
                ///wc.Headers.Add("accept-encoding", "gzip");
                try
                {
                    Leak.CheckLeak();
                    String xml = await wc.DownloadStringTaskAsync(url);
                    return XDocument.Parse(xml);
                }
                catch (WebException ex)
                {
                    if (ex.Response.Headers["X-Mashery-Error-Code"] == "ERR_403_DEVELOPER_OVER_QPS")
                    {
                        throw new NetflixThrottleException(ex);
                    }
                    ///possibly throw a throttled exception or something more valid?
                    throw;
                }
            }
        }
        public static async Task<XDocument> LoadXDocumentAsync(String url)
        {
            ///temporary
            Boolean retry = false;
            try
            {
                return await ThrottleLoadXDocumentAsync(url);
            }
            catch (NetflixThrottleException)
            { retry = true; }
            if (retry)
            {
                Thread.Sleep(150);
                return await LoadXDocumentAsync(url);
            }
            else
                throw new Exception("wat.");
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
                titles.Add(Netflix.Fill.Titles.GetExpandedTitle(id, OnUserBehalf));
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
                titles.Add(Netflix.Fill.Titles.GetCompleteTitle(id, OnUserBehalf));
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
                people.Add(Netflix.Fill.People.GetCompletePerson(id, OnUserBehalf));
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

    public class NetflixThrottleException : WebException
    {
        public NetflixThrottleException(WebException ex) 
            : base("ERR_403_DEVELOPER_OVER_QPS", ex)
        {
        }
    }
    public class NetflixApiException : Exception
    {
        public NetflixApiException(Exception ex)
            : base("FlixSharp had some issues - Inner exception contains information", ex)
        {
        }
        public NetflixApiException(String message, Exception ex)
            : base("FlixSharp had some issues - " + message, ex)
        {
        }
    }
}
