using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.WinForms;

namespace BetaChromium
{
    public class BetaBrowser
    {
        public ChromiumWebBrowser chromeBrowser;
        private bool _pageIsLoaded;
        private object _obj = new object();

        public BetaBrowser()
        {
            CefSettings settings = new CefSettings() { RemoteDebuggingPort = 8088 };
            settings.CefCommandLineArgs.Add("renderer-process-limit", "1");

            Cef.EnableHighDPISupport();
            Cef.Initialize(settings);

            chromeBrowser = new ChromiumWebBrowser("https://www.google.com/");
            //chromeBrowser.AddressChanged += AddressChanged;
            chromeBrowser.LoadingStateChanged += LoadingStateChanged;
        }

        private void LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
                _pageIsLoaded = true;
        }

        private void AddressChanged(object sender, AddressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public async Task<JavascriptResponse> ExecuteJsWithResultAsync(string script, TimeSpan? timeout = null)
        {
            var jsResp = await chromeBrowser.EvaluateScriptAsync(script, timeout);
            return jsResp;
        }

        public async Task<bool> WaitUntilPageLoadedAsync(TimeSpan forceStopIn)
        {
            var startedAt = DateTime.UtcNow;
            var stopIn = DateTime.UtcNow.AddMilliseconds(forceStopIn.TotalMilliseconds);

            while (true)
            {
                if (!_pageIsLoaded)
                {
                    await WaitOnPageAsync(TimeSpan.FromMilliseconds(100));
                    continue;
                }

                if (startedAt > stopIn)
                    return false;

                var documentReady = (await ExecuteJsWithResultAsync("document.readyState")).Result;
                if (documentReady != null && documentReady.ToString() == "complete")
                {
                    Console.WriteLine("Ready");
                    break;
                }

                await WaitOnPageAsync(TimeSpan.FromMilliseconds(100));
                Console.WriteLine("NotReady");
            }

            return true;
        }

        public async Task<string> WaitUntilJsResponseNotEmpty(string script, TimeSpan timeout, string scriptCondiotion = null)
        {
            object response = null;
            while (true)
            {
                response = (await ExecuteJsWithResultAsync(script, timeout)).Result;
                if (response != null)
                {
                    if (string.IsNullOrEmpty(scriptCondiotion))
                        break;

                    var conditionAnswer = false;
                    var successParse = bool.TryParse(
                        (await ExecuteJsWithResultAsync($@"(function(){{ let e = '{response}'; return { scriptCondiotion }}})()", timeout)).Result.ToString()
                        , out conditionAnswer);
                    if (!successParse)
                        throw new InvalidOperationException();
                    if(conditionAnswer)
                        break;
                }
                await WaitOnPageAsync(TimeSpan.FromMilliseconds(100));
            }

            return response.ToString();
        }

        public async Task<string> GetSourceCodeAsync()
        {
            return await chromeBrowser.GetBrowser().MainFrame.GetSourceAsync();
        }

        public async Task ExecuteVoidJs(string script, TimeSpan timeout)
        {
            await chromeBrowser.EvaluateScriptAsync(script, timeout);
        }

        public async Task WaitOnPageAsync(TimeSpan wait)
        {
            //var desireed = DateTime.Now.AddMilliseconds(wait.TotalMilliseconds);
            //while (DateTime.Now < desireed)
            //{
            //    Thread.Sleep(1);
            //    System.Windows.Forms.Application.DoEvents();
            //}

            await Task.Delay(wait);//TODO_STAY ON THIS LINE WHILE BROWSER IS DOING ANYTHING
        }
        
        public async Task DoAsync(Action<BetaBrowser, StopEvaluationEventArgs> function, int times, TimeSpan interval, TimeSpan? forceStopIn = null)
        {
            var startedAt = DateTime.UtcNow;
            var stopIn = forceStopIn == null ? DateTime.UtcNow.AddYears(99) : DateTime.UtcNow.AddMilliseconds(forceStopIn.Value.TotalMilliseconds);

            for (int i = 0; i < times; i++)
            {
                await WaitOnPageAsync(interval);
                if (startedAt > stopIn)
                    break;

                try
                {
                    function(this, new StopEvaluationEventArgs());
                }
                catch (StopEvaluationException ex)
                {
                    break;
                }
            }
        }

        public void Navigate(string url)
        {
            _pageIsLoaded = false;
            chromeBrowser.Load(url);
        }
    }
}
