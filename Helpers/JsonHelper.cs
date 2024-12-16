//-----------------------------------------------------------------------
// <copyright file="JsonHelper.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization.Json;

namespace CoreSystem2024.Helpers
{
    /// <summary>
    /// The json helper.
    /// </summary>
    internal static class JsonHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// The deserialize.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <typeparam name="T">The type of the value to deserialize.</typeparam>
        /// <returns>
        /// The deserialized value.
        /// </returns>
        public static T Deserialize<T>(Stream stream) where T : class
        {
            //Requires.NotNull(stream, "stream");
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }

        #endregion
    }
}
