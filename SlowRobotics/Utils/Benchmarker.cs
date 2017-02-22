using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Utils
{
    public class UpdateEventArgs : EventArgs
    {
        public string Name { get; set; }
        public long Duration { get; set; }

        public UpdateEventArgs(string _Name, long _Duration)
        {
            Name = _Name;
            Duration = _Duration;
        }

    }

    public class Benchmarker
    {
        private string log = "";

        public Benchmarker()
        {

        }

        private void updateLogger(object sender, UpdateEventArgs e)
        {
            log += "Benchmarking object: " + e.Name + ", update time: " + e.Duration + "\n";
        }

        public string Print()
        {
            return log;
        }

        public void clearLog()
        {
            log = "";
        }

        public void Add(IBenchmark o)
        {
            o.OnUpdate += updateLogger;
        }

    }
}
