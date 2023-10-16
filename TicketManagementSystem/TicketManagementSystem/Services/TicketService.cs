using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.Json;
using EmailService;

namespace TicketManagementSystem
{
    public class TicketService
    {
        public int CreateTicket(string title, Priority priority, string assignedTo, string description, DateTime createdDate, bool isPayingCustomer)
        {
            // Check if t or desc are null or if they are invalid and throw exception
            if(string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
            {
                throw new InvalidTicketException("Title or description were null");
            }
            
            User assignedUser;
            User accountManager = null;
            double price = 0;
            using (UserService userService = new UserService())
            {               
                assignedUser = userService.GetUser(assignedTo, $"User {assignedTo} not found");
                               
                using PriorityService priorityService = new PriorityService();
                priority = priorityService.GetPriorityByCheckingDateAndTitle(createdDate, priority, title);

                // Send email 
                SendEmailToAdministrator(priority, title, assignedTo);

                if (isPayingCustomer)
                {
                    // Only paid customers have an account manager.
                    accountManager = userService.GetAccountManager();
                    price = priority == Priority.High ? 100 : 50;
                }
            }
                        
            int ticketId = TicketRepository.CreateTicket(new Ticket
            {
                Title = title,
                AssignedUser = assignedUser,
                Priority = priority,
                Description = description,
                Created = createdDate,
                PriceDollars = price,
                AccountManager = accountManager
            });

            // Return the id
            return ticketId;
        }


        public void AssignTicket(int ticketId, string username)
        {
            Ticket ticket = TicketRepository.GetTicket(ticketId);
            if (ticket == null)
            {
                throw new ApplicationException($"No ticket found for id {ticketId}");
            }

            using UserService userService = new UserService();
            ticket.AssignedUser = userService.GetUser(username, "User not found");

            TicketRepository.UpdateTicket(ticket);
        }

        private void WriteTicketToFile(Ticket ticket)
        {
            string ticketJson = JsonSerializer.Serialize(ticket);
            File.WriteAllText(Path.Combine(Path.GetTempPath(), $"ticket_{ticket.Id}.json"), ticketJson);
        }

        private void SendEmailToAdministrator(Priority priority, string ticketTitle, string assignedTo)
        {
            if (priority == Priority.High)
            {
                IEmailService emailService = new EmailServiceProxy();
                emailService.SendEmailToAdministrator(ticketTitle, assignedTo);
            }
        }
    }
   
}
