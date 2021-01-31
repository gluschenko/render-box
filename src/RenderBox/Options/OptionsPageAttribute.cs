using System;

namespace RenderBox.Options
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
