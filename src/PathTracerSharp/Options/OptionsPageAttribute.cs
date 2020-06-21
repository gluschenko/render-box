using System;

namespace PathTracerSharp.Options
{
    public class OptionsPageAttribute : Attribute
    {
        public Type OptionsPageType { get; set; }

        public OptionsPageAttribute(Type optionsPageType)
        {
            OptionsPageType = optionsPageType;
        }
    }
}
