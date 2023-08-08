using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// This class converts a Task to Json, so the method returns a JSON string with the following structure (see <see cref="System.Text.Json"/>):
    /// <code>
    /// {
    ///     "Id": &lt;int&gt;,
    ///     "CreationTime": &lt;DateTime&gt;
    ///     "Title": &lt;string&gt;
    ///     "Description": &lt;string&gt;
    ///     "DueDate": &lt;DateTime&gt;
    /// }
    /// </code>
    /// </summary>

    public class TaskToJson
    {
        public int Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

        public TaskToJson(IntroSE.Kanban.Backend.BusinessLayer.Task task)
        {
            Id = task.TASK_ID1;
            CreationTime = task.CREATION_DATE1;
            Title = task.Title;
            Description = task.Description;
            DueDate = task.DueDate;
        }


        /// <summary>
        /// get the json Test format
        /// </summary>
        /// <returns> json of the decided format for Task, composed of Id, CreationTime, Title, Description, DueDate</returns>
        public string toJson()
        { 
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            return JsonSerializer.Serialize(this, this.GetType(), options);
        }
    }
}
