using Supabase.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Supabase.Core
{
    public static class Helpers
    {
        public static T GetPropertyValue<T>(object obj, string propName) => (T)obj.GetType().GetProperty(propName).GetValue(obj, null);
        public static T GetCustomAttribute<T>(object obj) where T : Attribute => (T)Attribute.GetCustomAttribute(obj.GetType(), typeof(T));
        public static T GetCustomAttribute<T>(Type type) where T : Attribute => (T)Attribute.GetCustomAttribute(type, typeof(T));

        public static MapToAttribute GetMappedToAttr(Enum obj)
        {
            var type = obj.GetType();
            var name = Enum.GetName(type, obj);

            return type.GetField(name).GetCustomAttributes(false).OfType<MapToAttribute>().SingleOrDefault();
        }
    }
}
