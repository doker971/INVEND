using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Terminal.Domain.Entities;
using System.Diagnostics;
using System.IO;
using Terminal.Domain.Concrete;
using Newtonsoft.Json;

namespace SignalRMvc.Hubs
{
    public class TerminalHub : Hub
    {
        private EFDbContext db = new EFDbContext();

        public void AddTicket()
        {
            List<Ticket> Tickets = db.Tickets.ToList();
            int number = Tickets.Count() != 0 ? (Tickets.Last().Number + 1): 1;
            int id = Tickets.Count() != 0 ? Tickets.Last().Id + 1 : 1;

            db.Tickets.Add(new Ticket { Id = id, Number = number, Status = (int)Statuses.New});
            db.SaveChanges();
            Clients.Caller.printNewTicket(number);
            Queue(null);
        }
        
        public void Queue(int? window)
        {
            List<Ticket> Tickets = db.Tickets.Where(t => t.Status == (int)Statuses.New).ToList();
            string list = JsonConvert.SerializeObject(Tickets);
            int count = Tickets.Count();

            if (window.HasValue) GetCurrentTicket(window.Value);
            Clients.Group("operators").getQueue(count, list);
        }

        public void GetCurrentTicket(int window)
        {
            var Ticket = db.Tickets.FirstOrDefault(t => t.Window == window && t.Status == (int)Statuses.Active);
            if (Ticket != null)
            {
                Clients.Group("operators").onNewCurTicket(Ticket.Number);
            }
            else Clients.Group("operators").noTicket();
        }

        public void Check(int window)
        {
            var last = db.Tickets.FirstOrDefault(t => t.Window == window && t.Status == (int)Statuses.Active);
            if (last != null)
            {
                last.Status = (int)Statuses.Closed;
                db.SaveChanges();
                Clients.Group("viewers").onCloseCall(last.Number);
            }
        }

        public void OperatorConnect(int window)
        {
            Groups.Add(Context.ConnectionId, "operators");

            Queue(window);
        }

        public void SelectFromList(int number, int window)
        {
            Check(window);
            var ticket = db.Tickets.FirstOrDefault(t => t.Number == number && t.Status == (int)Statuses.New);

            if (ticket != null)
            {
                ticket.Window = window;
                ticket.Status = (int)Statuses.Active;
                db.SaveChanges();
            }

            Clients.Group("viewers").onNewCall(number,window);
            Queue(window);
        }

        public void SelectNext(int window) {
            Check(window);
            var ticket = db.Tickets.FirstOrDefault(t => t.Status == (int)Statuses.New);

            if (ticket != null)
            {
                ticket.Window = window;
                ticket.Status = (int)Statuses.Active;
                db.SaveChanges();
            }

            Clients.Group("viewers").onNewCall(ticket.Number, window);
            Queue(window);
        }

        public void CloseTicket(int window)
        {
            Check(window);
            Queue(window);
        }

        public void DisplayConnect()
        {
            Groups.Add(Context.ConnectionId, "viewers");

            var find_active = db.Tickets.Where(t => t.Status == (int)Statuses.Active);
            if (find_active != null)
            {
                foreach (var ticket in find_active) {
                    Clients.Group("viewers").onNewCall(ticket.Number, ticket.Window);
                }
            }
        }
    }
}