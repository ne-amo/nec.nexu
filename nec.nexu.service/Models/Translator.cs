using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Threading;

namespace nec.nexu.Models
{
    public static class Translator
    {
        public static void Translate<T>(ref T source)
        {
            Type type = source.GetType();

            if (type.BaseType != null && type.Namespace == "System.Data.Entity.DynamicProxies")
            {
                type = type.BaseType;
            }
            foreach (PropertyInfo prop in type.GetProperties())
            {
                TranslationFor attr = prop.GetCustomAttribute<TranslationFor>();
                if (attr == null)
                    continue;
                string targetProperty = attr.GetTarget();
                PropertyInfo dictProp = type.GetProperty("Localized" + targetProperty);
                MultilingualString dictionary = (MultilingualString)dictProp.GetValue(source, null);
                string lang = GetLanguage();
                string translated = dictionary[lang];
                if (!string.IsNullOrEmpty(translated))
                {
                    type.GetProperty(targetProperty).SetValue(source, translated);
                }
            }
        }
        private static string GetLanguage()
        {
            return Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
        }
    }
}