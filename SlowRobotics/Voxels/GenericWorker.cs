using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SlowRobotics.Voxels
{
    class GenericWorker
    {

        public bool running = false;
        BackgroundWorker bw = new BackgroundWorker();
        public String progressText = "";


        public GenericWorker()
        {
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        public void Start()
        {
            if (bw.IsBusy != true)
            {
                running = true;
                bw.RunWorkerAsync();
            }
        }

        public void Abort()
        {
            if (bw.WorkerSupportsCancellation == true)
            {
                bw.CancelAsync();
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            run(worker);

        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                progressText = "Canceled!";
            }

            else if (!(e.Error == null))
            {
                progressText = ("Error: " + e.Error.Message);
            }

            else
            {
                running = false;
                progressText = "Done!";
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressText = (e.ProgressPercentage.ToString() + "%");
        }


        public virtual void run(BackgroundWorker worker)
        {

           // worker.ReportProgress(0);
        }
    }
}
