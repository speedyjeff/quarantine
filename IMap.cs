using System;

namespace quarantine
{
    abstract class IMap
    {
        public Space[][] Spaces { get; set; }

        public virtual string[] Information(int id)
        {
            return null;
        }
    }
}
