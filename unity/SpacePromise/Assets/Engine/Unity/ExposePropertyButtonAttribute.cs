using System;

namespace Assets.Engine.Unity
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExposePropertyButtonAttribute : ExposePropertyAttribute
    {
        public ExposePropertyButtonAttribute(int row) : base(row)
        {
        }
    }
}