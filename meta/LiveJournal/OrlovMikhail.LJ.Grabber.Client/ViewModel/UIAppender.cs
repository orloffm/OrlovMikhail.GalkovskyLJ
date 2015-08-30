using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;

namespace OrlovMikhail.LJ.Grabber.Client
{
    public sealed class UIAppender : IAppender
    {
        public event EventHandler<EventArgs<string>> StringAdded;

        public UIAppender()
        {
            Name = "UI appender.";
        }

        public void Close()
        {

        }

        public void DoAppend(LoggingEvent loggingEvent)
        {
            if (loggingEvent.Level <= Level.Debug)
                return;

            if (StringAdded != null)
            {
                string s = loggingEvent.MessageObject.ToString();
                StringAdded(this, new EventArgs<string>(s));
            }
        }

        public string Name { get; set; }
    }
}
