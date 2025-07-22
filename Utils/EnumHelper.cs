using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq;

namespace QontrolSystem.Utils
{
    // Helper class to retrieve enum values based on display names
    public class EnumHelper
    {
        public static T? GetValueFromDisplayName<T>(string displayName) where T : struct, Enum
        {
            foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attribute = field.GetCustomAttribute<DisplayAttribute>();
                if (attribute != null && attribute.Name == displayName)
                {
                    return (T)field.GetValue(null);
                }
            }
            return null;
        }
    }
}
