using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Calendar;
using Google.GData.Extensions;
using Google.GData.Client;
using System.Windows;
using L = HTCHome.Core.Logger;

namespace CalendarWidget
{
    public class CalendarHelper
    {
        public string ApplicationName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public static CalendarService GetService(string applicationName,
                    string userName, string password)
        {
            CalendarService service = new CalendarService(applicationName);
            service.setUserCredentials(userName, password);
            return service;
        }

        public static IEnumerable<EventEntry> GetAllEvents
        (CalendarService service, DateTime? startData)
        {
            // Create the query object:
            EventQuery query = new EventQuery();
            query.Uri = new Uri("http://www.google.com/calendar/feeds/" +
                  service.Credentials.Username + "/private/full");
            if (startData != null)
                query.StartTime = startData.Value;

            // Tell the service to query:
            try
            {
                EventFeed calFeed = service.Query(query);
                return calFeed.Entries.Cast<EventEntry>();
            }
            catch (Exception ex)
            {
                L.Log("Calendar: GetAllEvents caused exception!");
                L.Log(ex.ToString());
                return null;
            }
        }

        public static AtomEntry AddEvent(CalendarService service, string title,
            string contents, string location, DateTime startTime, DateTime endTime)
        {
            EventEntry entry = new EventEntry();

            // Set the title and content of the entry.
            entry.Title.Text = title;
            entry.Content.Content = contents;

            // Set a location for the event.
            Where eventLocation = new Where();
            eventLocation.ValueString = location;
            entry.Locations.Add(eventLocation);

            When eventTime = new When(startTime, endTime);
            entry.Times.Add(eventTime);
            Uri postUri = new Uri
            (string.Format("http://www.google.com/calendar/feeds/{0}/private/full", service.Credentials.Username));

            // Send the request and receive the response:
            AtomEntry insertedEntry = service.Insert(postUri, entry);
            return insertedEntry;
        }

        public static void RemoveEvent(CalendarService service, string url)
        {
            try
            {
                service.Delete(new Uri(url));
            }
            catch (Exception ex)
            {
                L.Log("Calendar: RemoveEvent caused exception!");
                L.Log(ex.ToString());
            }
        }

        public static string UpdateEvent(CalendarService service, string url, string title, DateTime startTime, DateTime endTime)
        {
            try
            {
                EventEntry entry = (EventEntry)service.Get(url);
                entry.Title = new AtomTextConstruct(AtomTextConstructElementType.Title, title);
                entry.Times.First().StartTime = startTime;
                entry.Times.First().EndTime = endTime;
                return service.Update(entry).EditUri.Content;
            }
            catch (Exception ex)
            {
                L.Log("Calendar: UpdateEvent caused exception!");
                L.Log(ex.ToString());
                return url;
            }
        }
    }
}
