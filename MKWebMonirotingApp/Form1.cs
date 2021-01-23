using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MKWebMonirotingApp
{
    public partial class Form1 : Form
    {
        public Thread t;
        private int loopInterval = 10000;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (t == null)
            {
                t = new Thread(MonitoringLoop);
                t.Start();
            }


        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (t != null)
            {
                t.Abort();
                t = null;
            }
        }

        private String MonitorSite(String uri)
        {
            String error = string.Empty;
            WebRequest request = System.Net.WebRequest.Create(uri);
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode.ToString();
                }
            }
            catch (Exception e)
            {
                error = "Error Message: " + e.Message + "<br />" +
                        "Inner Exception: " + (e.InnerException != null ? e.InnerException.Message : string.Empty) +
                        "<br />" + "Stack Trace: " + e.StackTrace;
                return error;
            }
        }
        private void MonitoringLoop()
        {
            while (true)
            {
                int counter = 0;
                string line, iniline;
            
                listView1.Items.Clear();
                DateTime localDate = DateTime.Now;

                List<String> sitesList = new List<string>();

                System.IO.StreamReader inifile =
                    new System.IO.StreamReader(@"app.conf");
                while((iniline = inifile.ReadLine()) != null)
                {
                    if (iniline.Contains("loop_interval="))
                    {
                        string inival = iniline.Substring(iniline.IndexOf("=")+1);
                        loopInterval = Convert.ToInt32(inival);
                    }
                }

                // Read the file and display it line by line.  
                System.IO.StreamReader file =
                    new System.IO.StreamReader(@"sites.conf");
                while ((line = file.ReadLine()) != null)
                {
                    sitesList.Add(line);
                    String returnCode = MonitorSite(line);
                    ListViewItem lvi = listView1.Items.Add(returnCode);
                    // add additional columns
                    lvi.SubItems.Add(line);
                    lvi.SubItems.Add(localDate.ToLongTimeString());

                }
                file.Close();
                Thread.Sleep(loopInterval);
            }
        }
            
    }
}
