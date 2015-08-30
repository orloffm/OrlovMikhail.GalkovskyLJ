using System;

namespace OrlovMikhail.LJ.Grabber.Client
{
    public class EventArgs<T> : EventArgs
    {
        public T Value { get; private set; }

        public EventArgs(T s)
        {
            this.Value = s;
        }
    }
}