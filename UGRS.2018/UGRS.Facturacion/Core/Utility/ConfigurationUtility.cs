// file:	Utility\ConfigurationUtility.cs
// summary:	Implements the configuration utility class

using System;
using System.Configuration;
using System.Linq;

namespace Core.Utility
{
    /// <summary> A configuration utility. </summary>
    /// <remarks> Ranaya, 24/05/2017. </remarks>

    public class ConfigurationUtility
    {
        /// <summary> Gets a value. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <exception cref="ConfigurationErrorsException">
        /// Thrown when a Configuration Errors error condition occurs.
        /// </exception>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="pStrKey"> The string key. </param>
        /// <returns> The value. </returns>

        public static T GetValue<T>(string pStrKey) where T : IConvertible
        {
            T lUknResultValue = default(T);

            if (ConfigurationManager.AppSettings.AllKeys.Contains(pStrKey))
            {
                string tmpValue = ConfigurationManager.AppSettings[pStrKey];

                if (typeof(T).IsEnum)
                {
                    lUknResultValue = (T)Enum.Parse(typeof(T), tmpValue, true);
                }
                else
                {
                    lUknResultValue = (T)Convert.ChangeType(tmpValue, typeof(T));
                }
            }
            else
            {
                throw new ConfigurationErrorsException(string.Format("{0} not found in app/webconfig", pStrKey));
            }

            return lUknResultValue;
        }
    }
}
