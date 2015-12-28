using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace OrlovMikhail.LJ.Grabber
{
    public class LJClient : ILJClient
    {
        static readonly ILog log = LogManager.GetLogger(typeof(LJClient));

        private HttpClient _client;

        public LJClient()
        {
            _client = new HttpClient();
        }

        public string GetContent(LiveJournalTarget target, ILJClientData data)
        {
            Uri address = target.WithStyleMine(true).WithCutExpand().GetUri();
            log.Info("Downloading " + address + "...");

            var cookieContainer = new CookieContainer();
            using (HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (HttpClient client = new HttpClient(handler) { BaseAddress = address })
            {
                LJClientCookieData cookieData = (data as LJClientCookieData);
                if (cookieData != null)
                {
                    Dictionary<string, string> cookies = cookieData.GetCookiesToUse();
                    foreach (var cookie in cookies)
                    {
                        log.DebugFormat("Using cookie {0}:{1}.", cookie.Key, cookie.Value);
                        cookieContainer.Add(address, new Cookie(cookie.Key, cookie.Value));
                    }
                }

                string result = DownloadString(client, address);
                return result;
            }
        }

        private static string DownloadString(HttpClient client, Uri address)
        {
            int count = 5;

            while (true)
            {
                Task<string> task = client.GetStringAsync(address);
                Task finalizer = task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        t.Exception.Handle(ex =>
                        {
                            log.Info("Error: " + ex.Message + " (" + ex.GetType() + ").");
                            return true;
                        });
                    }

                }, TaskContinuationOptions.None);
                finalizer.Wait();

                if (!(task.IsFaulted || task.IsCanceled))
                    return task.Result;

                if (--count > 0)
                {
                    log.Info("Will try again in a second...");
                    Task.Delay(1000).Wait();
                }
                else
                    break;
            }

            throw new Exception("Download of the address failed.");
        }

        public byte[] DownloadFile(Uri target)
        {
            log.Info("Downloading file from " + target + "...");

            Task<byte[]> task = _client.GetByteArrayAsync(target);
            Task finalizer = task.ContinueWith(t =>
                 {
                     if (t.IsFaulted)
                     {
                         t.Exception.Handle(ex =>
                            {
                                log.Error("Error: " + ex.Message + " (" + ex.GetType() + ").");
                                return true;
                            });
                     }

                 }, TaskContinuationOptions.None);
            finalizer.Wait();

            if (task.IsFaulted || task.IsCanceled)
                return null;

            return task.Result;
        }

        /// <summary>Creates a cookie container.</summary>
        public ILJClientData CreateDataObject(string input)
        {
            return LJClientCookieData.FromString(input);
        }
    }
}
