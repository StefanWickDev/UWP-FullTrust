using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FullTrust_Background
{
    class Program
    {
        static AutoResetEvent done = new AutoResetEvent(false);
        static void Main(string[] args)
        {            
            Thread bgThread = new Thread(ThreadProc);
            bgThread.Start(done);
            done.WaitOne();
        }

        static void ThreadProc(object unused)
        {
            // keep this up ofr 60sec, just for demo purposes
            Thread.Sleep(60000);
            done.Set();
        }
    }
}
