using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOS;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class Task
    {
        private readonly int BOARD_ID;
        private readonly int TASK_ID;
        private int columnNum;
        private readonly DateTime CREATION_DATE; 
        private DateTime dueDate;
        private string title;   
        private string description;
        private Boolean editable;
        private string assignee;
        internal readonly TaskDTO taskDalDTO;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public Task (int boardID,int taskID, DateTime dueDate, string title, string description)
        {
            validateDescript(description);
            validateTitle(title);
            this.BOARD_ID = boardID;
            this.columnNum = 0;
            this.TASK_ID = taskID;
            this.CREATION_DATE = DateTime.Now;
            this.dueDate = dueDate;
            this.title = title;
            this.description = description;
            this.editable = true;
            this.assignee = null;
            this.taskDalDTO = new TaskDTO(TASK_ID, BOARD_ID, title, description, CREATION_DATE, dueDate, null);
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        internal Task(TaskDTO dto)
        {
            this.description = dto.Description;
            this.title = dto.Title;
            if (dto.Assignee.Equals("empty"))
                this.assignee = null;
            else
            {
                this.assignee = dto.Assignee;
            }
            this.TASK_ID = dto.Id;
            this.dueDate = dto.DueDate;
            this.editable = dto.IsEditable;
            this.columnNum = dto.ColumnNumber;
            this.BOARD_ID= dto.BoardID;
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        [JsonIgnore]
        public int ColumnNum { get { return columnNum; } set { columnNum = value; } }
        public int TASK_ID1 { get { return TASK_ID; } }
        [JsonIgnore]
        public int BOARD_ID1 { get { return BOARD_ID; } }
        public DateTime CREATION_DATE1 { get { return CREATION_DATE; } }
        public DateTime DueDate { get { return dueDate; } }
        public string Title { get { return title; } }
        public string Description { get { return description; } }
        [JsonIgnore]
        public string Assignee { get { return assignee; } } 


        /// <summary>
        /// Check if a given description is valid according to description's restrictions
        /// </summary>
        /// <param name="description">The description to check</param>
        private void validateDescript(string description)
        {
            if (description == null)
            {
                log.Error("error is validateDescript- description is null");
                throw new ArgumentNullException("description is null");
            }
            if (description.Length > 300)
            {
                log.Error("error in validateDescript- Description should be Max 300 characters");
                throw new ArgumentException("Description should be Max 300 characters");
            }
            log.Info("description is valid");
        }


        /// <summary>
        /// Check if a given title is valid according to title's restrictions
        /// </summary>
        /// <param name="description">The title to check</param>
        private void validateTitle(string title)
        {
            if (title == null|| title.Equals(""))
            {
                log.Error("error in validateTitle- title is invalid");
                throw new ArgumentNullException("title is invalid");
            }
            if ((title.Length > 50) || (title.Length < 1))
            {
                log.Error("error in validateTitle- Title in the length of 1-50 characters");
                throw new ArgumentException("Title in the length of 1-50 characters");
            }
            log.Info("title is valid");
        }


        /// <summary>
        /// Update the title of the task
        /// </summary>
        /// <param name="newTitle">The title we want to update to</param>
        /// <param name="actorUser"> The userName of the user that wants to update</param>
        public void editTitle (string actorUser, string newTitle)
        {
            if (!editable)
            {
                log.Error("error in editTitle - the task is in done column");
                throw new ArgumentException("Editing a done task is imposible");
            }
            if (!isAssignee(actorUser))
            {
                log.Error($"error in editTitle - {actorUser} is not assignee");
                throw new ArgumentException("Only the Task's assignee can update it");
            }
            else
            {
                validateTitle(newTitle);
                try
                {
                    taskDalDTO.editTitleInDAL(newTitle);
                    taskDalDTO.IsPersistent = false;
                }
                catch (Exception)
                {
                    log.Warn($"Failed to edit title in ask '{TASK_ID}' in board '{BOARD_ID}'");
                    throw new Exception($"Failed to edit title in ask '{TASK_ID}' in board '{BOARD_ID}'");
                }
                this.title = newTitle;
                taskDalDTO.IsPersistent = true;
                log.Info($"editTitle to {newTitle} done");
            }
        }


        /// <summary>
        /// Update the description of the task
        /// </summary>
        /// <param name="actorUser"> The userName of the user that wants to update</param>
        /// <param name="newDescript">The description we want to update to</param>
        public void editDescription(string actorUser, string newDescript)
        {
            if (!editable)
            {
                log.Error("error in editDescription- task is in done column");
                throw new ArgumentException("Editing a done task is imposible");
            }
            if (!isAssignee(actorUser))
            {
                log.Error($"error in editDescription- {actorUser} is not assignee");
                throw new ArgumentException("Only the Task's assignee can update it");
            }
            else
            {
                validateDescript(newDescript);
                try
                {
                    taskDalDTO.editDescriptionInDAL(newDescript);
                    taskDalDTO.IsPersistent = false;
                }
                catch (Exception)
                {
                    log.Warn($"Failed to edit description in task '{TASK_ID}' in board '{BOARD_ID}'");
                    throw new Exception($"Failed to edit description in task '{TASK_ID}' in board '{BOARD_ID}'");
                }
                this.description = newDescript;
                taskDalDTO.IsPersistent = true;
                log.Info($"editDescription to {newDescript} is done");
            }
        }


        /// <summary>
        /// Update the DueDate of the task
        /// </summary>
        /// <param name="actorUser"> The userName of the user that wants to update</param>
        /// <param name="newDate">The DueDate we want to update to</param>
        public void editDueDate(string actorUser, DateTime newDate)
        {
            if (!editable)
            {
                log.Error("error in editDueDate - task is in done column");
                throw new ArgumentException("Editing a done task is imposible");
            }
            if (!isAssignee(actorUser))
            {
                log.Error($"error in editDueDate -{actorUser} is not assignee");
                throw new ArgumentException("Only the Task's assignee can update it");
            } 
            if (newDate < DateTime.Now)
            {
                log.Error($"error in editDueDate -the new due date is invalid");
                throw new ArgumentException("new due date is invalid");
            }
            else
            {
                try
                {
                    taskDalDTO.editDueDateInDAL(newDate);
                    taskDalDTO.IsPersistent = false;
                }
                catch (Exception)
                {
                    log.Warn($"Failed to edit dueDate in task '{TASK_ID}' in board '{BOARD_ID}'");
                    throw new Exception($"Failed to edit dueDate in task '{TASK_ID}' in board '{BOARD_ID}'");
                }
                this.dueDate = newDate;
                taskDalDTO.IsPersistent = true;
                log.Info($"editDueDate to {newDate} is done");
            }
        }


        /// <summary>
        /// Update the assignee of the task
        /// </summary>
        /// <param name="newAssignee">The new assignee the current one wants to assign</param>
        /// <param name="actorUser">The current assignee</param>
        public void editAssignee(string newAssignee, string actorUser)
        {
            if (!editable)
            {
                log.Error("error in editAssignee - task is in done column");
                throw new ArgumentException("Editing a done task is imposible");
            }
            else
            {
                if ((assignee != null) && (!isAssignee(actorUser)))
                {
                    log.Error($"error in editAssignee -{actorUser} is not assignee");
                    throw new ArgumentException("Assigning an assigned task can be done only be the current assignee");
                }
                else
                {
                    try
                    {
                        taskDalDTO.editAssigneeInDAL(newAssignee);
                        taskDalDTO.IsPersistent = false;
                    }
                    catch (Exception)
                    {
                        log.Warn($"Failed to assign '{newAssignee}' to task '{TASK_ID}' in board '{BOARD_ID}'");
                        throw new Exception($"Failed to assign '{newAssignee}' to task '{TASK_ID}' in board '{BOARD_ID}'");
                    }
                    this.assignee = newAssignee;
                    taskDalDTO.IsPersistent = true;
                    log.Info($"editAssignee to {newAssignee} is done");
                }
            }
        }


        /// <summary>
        /// Change the editable field to false (to avoid users from editing tasks in "done" (2) column)
        /// </summary>
        public void done()
        {
            this.editable = false;
        }


        /// <summary>
        /// Check if a user is the assignee of a task
        /// </summary>
        /// <param name="actorUser">Username of the user we want to check</param>
        /// <returns>True if assignee, else False</returns>
        public Boolean isAssignee(string actorUser)
        {
            return (assignee == actorUser);
        }
    }
}
