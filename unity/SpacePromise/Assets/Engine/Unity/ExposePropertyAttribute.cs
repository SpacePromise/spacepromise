using System;

namespace Assets.Engine.Unity
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExposePropertyAttribute : Attribute
    {
        public int Row { get; private set; }


        public ExposePropertyAttribute()
        {
            this.Row = 0;
        }

        public ExposePropertyAttribute(int row)
        {
            this.Row = row;
        }
    }
}