using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Core.Extension
{
    public static class ObjectExtension
    {
        public static T Copy<T>(this T lObjSource)
        {
            T lObjCopy = (T)Activator.CreateInstance(typeof(T));

            foreach (PropertyInfo lObjCopyProperty in lObjCopy.GetType().GetProperties())
            {
                lObjCopyProperty.SetValue
                (
                    lObjCopy,
                    lObjSource.GetType().GetProperties().Where(x => x.Name == lObjCopyProperty.Name).FirstOrDefault().GetValue(lObjSource)
                );
            }

            return lObjCopy;
        }

        public static T CopyWithoutVirtualProperties<T>(this T lObjSource)
        {
            T lObjClone = (T)Activator.CreateInstance(typeof(T));

            foreach (PropertyInfo lObjCloneProperty in lObjClone.GetType().GetProperties().Where(x => !x.GetMethod.IsVirtual))
            {
                lObjCloneProperty.SetValue
                (
                    lObjClone,
                    lObjSource.GetType().GetProperties().Where(x => x.Name == lObjCloneProperty.Name).FirstOrDefault().GetValue(lObjSource)
                );
            }

            return lObjClone;
        }

        public static object GetPropertyValue(this object pObjCurrentObject, string pStrPropertyName)
        {
            return pObjCurrentObject.GetType().GetProperty(pStrPropertyName).GetValue(pObjCurrentObject, null);
        }

        public static object GetPropertyValueByAttribute<T>(this object pObjCurrentObject) where T : Attribute
        {
            return pObjCurrentObject.GetPropertyByAttribute<T>().GetValue(pObjCurrentObject, null);
        }

        public static PropertyInfo GetPropertyByAttribute<T>(this object pObjCurrentObject) where T : Attribute
        {
            return pObjCurrentObject.GetType().GetProperties().Where(p => p.IsDefined(typeof(T), false)).FirstOrDefault();
        }

        public static T Parse<T>(this object UnkObject) where T : class
        {
            T lUnkResult = (T)Activator.CreateInstance(typeof(T));

            foreach (PropertyInfo lObjProperty in lUnkResult.GetType().GetProperties())
            {
                lObjProperty.SetValue(lUnkResult, UnkObject.GetType().GetProperties().FirstOrDefault(x => x.Name == lObjProperty.Name).GetValue(UnkObject));
            }

            return lUnkResult;
        }

        public static IList<T> ParseList<T>(this IList<object> pLstUnkObject) where T : class
        {
            IList<T> lLstUnkResult = new List<T>();

            foreach (object lUnkObject in pLstUnkObject)
            {
                lLstUnkResult.Add(lLstUnkResult.Parse<T>());
            }

            return lLstUnkResult;
        }

        public static T ParseBySerialize<T>(this object pUnkObject) where T : class
        {
            return pUnkObject.JsonSerialize().JsonDeserialize<T>();
        }

        public static string JsonSerialize(this object pUnkObject)
        {
            return JsonConvert.SerializeObject(pUnkObject, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public static T JsonDeserialize<T>(this string pStrSource) where T : class
        {
            return JsonConvert.DeserializeObject<T>(pStrSource);
        }

        public static T DeepConvert<T>(this object pUnkSource)
        {
            var IsNotSerializable = !typeof(T).IsSerializable;
            if (IsNotSerializable)
                throw new ArgumentException("The type must be serializable.", "source");

            var SourceIsNull = ReferenceEquals(pUnkSource, null);
            if (SourceIsNull)
                return default(T);

            var lObjFormatter = new BinaryFormatter();
            using (var lObjStream = new MemoryStream())
            {
                lObjFormatter.Serialize(lObjStream, pUnkSource);
                lObjStream.Seek(0, SeekOrigin.Begin);
                return (T)lObjFormatter.Deserialize(lObjStream);
            }
        }
    }
}
