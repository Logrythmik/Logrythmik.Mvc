using System;
using System.ComponentModel;


namespace Logrythmik.Data
{
    public static class EnumExtensions
    {
        public static string Description(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            if (!string.IsNullOrEmpty(name))
            {
                var field = type.GetField(name);
                if (field != null)
                {
                    var attr = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }

            return null;
        }
    }
}
