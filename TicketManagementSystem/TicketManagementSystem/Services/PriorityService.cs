using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketManagementSystem
{
    public class PriorityService : IDisposable
    {
        public Priority GetPriorityByCheckingDateAndTitle(DateTime createdDate, Priority defaultPriority, string title)
        {
            var priorityRaised = false;
            Priority actualPriority= defaultPriority;
            if (createdDate < DateTime.UtcNow - TimeSpan.FromHours(1))
            {
                actualPriority = GetTicketPriority(defaultPriority);
                priorityRaised = actualPriority!=defaultPriority;
            }

            ISet<string> ticketTypes = new HashSet<string>() { "Crash", "Important", "Failure" };
            if (ticketTypes.Any(t => title.Contains(t)) && !priorityRaised)
            {
                actualPriority = GetTicketPriority(actualPriority);
            }            

            return actualPriority;
        }
        private Priority GetTicketPriority(Priority defaultPriority)
        {           
            switch (defaultPriority)
            {
                case Priority.Low:
                    return Priority.Medium;
                case Priority.Medium:
                    return Priority.High;
                default: 
                    return defaultPriority;
            }
           
        }
        public void Dispose()
        {
             
        }
    }
}
