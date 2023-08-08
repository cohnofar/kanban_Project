using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOS;
using IntroSE.Kanban.Backend.DataAccessLayer;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class Column
    {
        private readonly int BOARD_ID;
        private readonly int COLUMN_NUM;
        private Dictionary<int, Task> tasks;
        private int maxTasks;
        internal ColumnDTO columnDalDTO;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int COL_ID0 =0;
        private int COL_ID1 = 1;
        private int COL_ID2 = 2;

        public int COLUMN_NUM1 { get { return COLUMN_NUM; } }
        public int BOARD_ID1 { get { return BOARD_ID; } }
        public Dictionary<int, Task> Tasks { get { return tasks; } }

        public int MaxTasks { get { return maxTasks; }
            set
            {
                maxTasks = value;
            }
        }
        internal ColumnDTO ColumnDalDTO { get { return columnDalDTO; } }


        public Column(int columnNum, int boardId)
        {
            this.COLUMN_NUM = columnNum;
            this.tasks = new Dictionary<int, Task>();
            this.MaxTasks = -1;
            this.BOARD_ID = boardId;
            if(columnNum == COL_ID0)
                this.columnDalDTO = new ColumnDTO(-1, boardId, columnNum, "backlog" ,MaxTasks); // ***- enum for column name
            if (columnNum == COL_ID1)
                this.columnDalDTO = new ColumnDTO(-1, boardId, columnNum, "in progress", MaxTasks);
            if (columnNum == COL_ID2)
                this.columnDalDTO = new ColumnDTO(-1, boardId, columnNum, "done", MaxTasks);
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
        internal Column(ColumnDTO dto)
        {
            this.COLUMN_NUM = dto.ColumnNumber;
            this.MaxTasks = dto.TaskLimit;
            this.BOARD_ID = dto.BoardID;
            this.columnDalDTO = dto;
            columnDalDTO.IsPersistent = true;
            this.tasks = new Dictionary<int, Task>();
            foreach (TaskDTO taskDTO in columnDalDTO.Tasks)
            {
                tasks.Add(taskDTO.Id, new Task(taskDTO));
            }
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }


        // <summary>
        /// limit the number of tasks in this column
        /// </summary>
        /// <param name="limit"> the desired limit</param>
        public void limitTasksForColumn(int limit)
        {
            if (limit < -1)
            {
                log.Error($"error in limitTasksForColumn - '{limit}' is invalid");
                throw new ArgumentException("limit is invalid");
            }
            int inTasks = tasks.Count;
            if (limit < inTasks && limit != -1)
            {
                log.Error($"error in limitTasksForColumn - there more than '{limit}' tasks");
                throw new ArgumentException("Limit is lower than number of tasks in this column");
            }
            else
            {
                try
                {
                    columnDalDTO.limitTasksForColumnInDAL(limit);
                    columnDalDTO.IsPersistent = false;
                }
                catch (Exception)
                {
                    log.Warn($"Failed to limit tasks for column number '{columnDalDTO}' in board '{BOARD_ID}' to '{limit}'");
                    throw new Exception($"Failed to limit tasks for column number '{columnDalDTO}' in board '{BOARD_ID}'");
                }
                MaxTasks = limit;
                columnDalDTO.IsPersistent = true;
                log.Info("limitTasksForColumn done");
            }          
        }


        /// <summary>
        /// Remove a task from this column
        /// </summary>
        /// <param name="taskID">The ID of the task to remove</param>
        /// <returns>the removed Task object</returns>
        public Task removeTask(int taskID)
        {
            if (tasks.ContainsKey(taskID))
            {
                Task toReturn = tasks[taskID];
                if (tasks.Remove(taskID))
                {
                    log.Info("removeTask done");
                    return toReturn;
                }
            }
            log.Error($"error in removeTask - task '{taskID}' is not exist or couldnt remove from list in column");
            throw new Exception($"error in removeTask - task '{taskID}' is not exist or couldnt remove from list in column");
        }


        /// <summary>
        /// Create a new task and add to this column (performed only if the taskNum is 0)
        /// </summary>
        /// <param name="taskID">the task id acceppted from BoardService</param>
        /// <param name="title">title to the new task</param>
        /// <param name="description">descrption to the new task</param>
        /// <param name="dueDate">dueDate to the new task</param>
        public void createTask(int taskID, DateTime dueDate, string title, string description)
        {
            if (title == null || description == null || title.Equals(""))
            {
                log.Error($"error in createTask - title or description is null or empty");
                throw new ArgumentException("title or description is invalid");
            }
            if (this.COLUMN_NUM != 0)
            {
                log.Error($"error in createTask - can't add task to column number {COLUMN_NUM}");
                throw new ArgumentException("You can only add new tasks to the BackLog!");
            }
            if (dueDate<DateTime.Now)
            {
                log.Error($"error in createTask - due date is unvalid");
                throw new ArgumentException("due date is unvalid");
            }
            else
            {
                Task newTask = new Task(BOARD_ID1, taskID, dueDate, title, description);
                try
                {
                    columnDalDTO.addTaskToDAL(newTask.taskDalDTO);
                    columnDalDTO.IsPersistent = false;
                }
                catch (Exception)
                {
                    log.Warn($"Can't add task '{title}' to board '{BOARD_ID}'");
                    throw new Exception($"Can't add task '{taskID}' : '{title}' to board '{BOARD_ID}'");
                }
                addTask(newTask);
                columnDalDTO.IsPersistent = true;
                log.Info($"Task '{taskID}' added to board '{BOARD_ID}' successfully");
            }
        }


        /// <summary>
        /// Add a task to this column
        /// </summary>
        /// <param name="toAdd">The task we want to add</param>
        public void addTask(Task task) 
        {
            if (task == null)
            {
                log.Error("error in addTask - task is error");
                throw new Exception("task is null");
            }
            if (MaxTasks != -1)
            {
                int inTasks = tasks.Count;
                if (inTasks >= MaxTasks)
                {
                    log.Error($"error in addTask - number of tasks in column is {inTasks} and the maximum is {MaxTasks}");
                    throw new ArgumentException("Number of tasks in this column have reached to its Maximum");
                }
            }
            if (tasks.ContainsKey(task.TASK_ID1))
            {
                log.Error($"task {task.TASK_ID1} is already exist");
                throw new ArgumentException("This task already exists in this column!");
            }
            else
            {
                tasks.Add(task.TASK_ID1, task);
                task.ColumnNum = this.COLUMN_NUM;
                if (this.COLUMN_NUM == 2)
                    task.done();
                log.Info($"addTask {task.TASK_ID1} done");
            }
        }


        /// <summary>
        /// Get a task that has the given id
        /// </summary>
        /// <param name="taskID">id of a task to get</param>
        /// <returns>The task that has the given id</returns>
        public Task getTask(int taskID)
        {
            if (tasks.ContainsKey(taskID))
                return tasks[taskID];
            return null;

        }


        /// <summary>
        /// get a list with all the tasks of this column
        /// </summary>
        /// <returns>A list with all the tasks of this column</returns>
        public List<Task> getAllTasks()
        {
            List<Task> allTasks = tasks.Values.ToList();
            return allTasks;
        }


        /// <summary>
        /// Unassign all the tasks that are assigned to a certain user
        /// </summary>
        /// <param name="actorUser">userName of the user we wish to unassign</param>
        public void removeUser(string actorUser)
        {
            if (tasks != null)
            {

                foreach (KeyValuePair<int, Task> entry in tasks)
                {
                    if (entry.Value.isAssignee(actorUser))
                    {
                        entry.Value.editAssignee(null, actorUser);
                    }
                }
            }
            log.Info($"removeUser {actorUser} done");
        }
    }
}
