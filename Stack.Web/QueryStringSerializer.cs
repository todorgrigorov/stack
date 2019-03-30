using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;

namespace Stack.Web
{
    internal class QueryStringSerializer
    {
        public string Serialize(object data)
        {
            Assure.NotNull(data, nameof(data));

            var values = new Dictionary<string, object>();
            CollectValuesRecursive(data, data.GetType(), string.Empty, false, values);
            return SerializeValues(values);
        }

        #region Private members
        private void CollectValuesRecursive(
            object obj,
            Type type,
            string prefix,
            bool inSquareBrackets,
            Dictionary<string, object> values)
        {
            foreach (var property in type.LoadProperties())
            {
                var val = property.Get<object>(obj);
                if (val != null)
                {
                    string propName = property.Name;
                    if (inSquareBrackets)
                    {
                        propName = '[' + propName + ']';
                    }
                    var fullName = prefix + propName;

                    if (IsSimple(property.PropertyType))
                    {
                        values.Add(fullName, val);
                    }
                    else if (property.PropertyType.Implements(typeof(IEnumerable)))
                    {
                        bool? isSimple = null;

                        var enumerable = (IEnumerable)val;
                        int index = 0;
                        var enumerator = enumerable.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            var enumeratedValue = enumerator.Current;
                            if (enumeratedValue != null)
                            {
                                if (!isSimple.HasValue)
                                {
                                    isSimple = IsSimple(enumeratedValue.GetType());
                                }

                                var name = fullName + '[' + index.ToString() + ']';
                                if (isSimple.GetValueOrDefault())
                                {
                                    values.Add(name, enumeratedValue);
                                }
                                else
                                {
                                    CollectValuesRecursive(
                                        enumeratedValue,
                                        enumeratedValue.GetType(),
                                        name,
                                        true,
                                        values);
                                }
                            }
                            index++;
                        }
                    }
                    else
                    {
                        CollectValuesRecursive(val, property.PropertyType, fullName, true, values);
                    }
                }
            }
        }
        private bool IsSimple(Type type)
        {
            return type.Implements(typeof(IConvertible)) ||
                        type == typeof(TimeSpan) ||
                            (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
        private string SerializeValues(Dictionary<string, object> values)
        {
            string result = null;
            foreach (var pair in values)
            {
                string item = null;
                if (pair.Value != null)
                {
                    item = JsonConvert.ToString(pair.Value).Trim('"');
                    item = HttpUtility.UrlEncode(item);
                }
                result += string.Format("{0}={1}&", pair.Key, item);
            }
            return result?.TrimEnd('&');
        }
        #endregion
    }
}
