using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BetaChromium
{
    public partial class mForm : Form
    {
        
        public mForm()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            //betaBrowser.ExecuteAsyncJsWithResultAsync();//to implement

            var betaBrowser = new BetaBrowser();

            Controls.Add(betaBrowser.chromeBrowser);
            betaBrowser.chromeBrowser.Dock = DockStyle.Fill;

            var links = new List<string>
            {
                "https://stackoverflow.com/questions/26959655/is-there-an-application-doevents-for-webbrowser",
                "https://stackoverflow.com/questions/20930414/how-to-dynamically-generate-html-code-using-nets-webbrowser-or-mshtml-htmldocu/20934538#20934538",
                "https://www.googleadservices.com/pagead/aclk?sa=L&ai=DChcSEwjwvZbx25TnAhVYqZoKHbJKDgUYABAAGgJsbQ&ohost=www.google.com&cid=CAESQOD2x0WImYNULU3NY1bOFNkrdZusR61NQND9Aii3f6mCmVqgz_t54iWzy6X4_vavjM_5gR8hSCY2fa5-LgKiZm0&sig=AOD64_2xtadh0ayE77tx53jqMiTrAheBFw&q=&ved=2ahUKEwjeqo3x25TnAhUSxYsKHSolAwMQ0Qx6BAgMEAE&adurl="
            };

            var times = links.Count();
            var interval = TimeSpan.FromSeconds(1);

            foreach (var url in links)
            {
                await betaBrowser.DoAsync(async (b, _evnt) =>
                {
                    b.Navigate(url);

                    await b.WaitUntilPageLoadedAsync(TimeSpan.FromSeconds(10)); //TODO_STAY Implement this line

                    //await b.ExecuteVoidJs(Scripts.ClickOnPhonesJs, TimeSpan.FromSeconds(2));
                    //await b.WaitUntilJsResponseNotEmpty(Scripts.GetPhonesFunc, TimeSpan.FromSeconds(5), "(function(e){ return e.indexOf('xx') == -1; })(e)");
                    //Do smth else
                    //Then continue to next url
                }, times, interval);
            }
        }
    }
}
