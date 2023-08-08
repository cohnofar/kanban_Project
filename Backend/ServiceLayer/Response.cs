using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// This class converts all classes methods' return values to a JSON string with the following structure (see <see cref="System.Text.Json"/>):
    /// <code>
    /// {
    ///     "ErrorMessage": &lt;string&gt;,
    ///     "ReturnValue": &lt;<typeparamref name="T"/>gt;
    /// }
    /// </code>
    /// Where:
    /// <list type="bullet">
    ///     <item>
    ///         <term>ReturnValue</term>
    ///         <description>
    ///             The return value of the function.
    ///             <para>If the function does not return a value or an exception has occorred, then the field is undefined.</para>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>ErrorMessage</term>
    ///         <description>If an exception has occorred, then this field will contain a string of the error message.</description>
    ///     </item>
    /// </list>
    /// </para>
    /// </summary>
    
    public class Response<T>
    {
        public string ErrorMessage { get; set; }

        public T ReturnValue {get; set;}

        [JsonConstructor]
        public Response(string errorMessage, T returnValue)
        {
            this.ErrorMessage = errorMessage;
            this.ReturnValue = returnValue;
        }

        /// <summary>
        /// get the standard json string format
        /// </summary>
        /// <returns> The standard json string format of respond, composed of an Error message and a Return value </returns>
        public string ResponseToJson()
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            return JsonSerializer.Serialize(this, this.GetType(), options);
        }
     }
}
