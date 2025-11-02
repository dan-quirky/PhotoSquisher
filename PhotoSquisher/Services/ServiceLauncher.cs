using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSquisher.Services
{
    internal static class ServiceLauncher
    {
        internal static void ScheduledRun()
        {
            //To be run periodically with an external scheduling service e.g. CRON on Ubuntu
        }
        internal static void ServiceStart()
        {
            /*To be run persistently in background, scan scheduling handled internally
            *Needs more infrastructure before implementing
            *   Scheduling/passive scanning of 
            *
            */
            throw new NotImplementedException();
        }
    }
}
