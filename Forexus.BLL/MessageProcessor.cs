using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Forexus.DAL;
using Forexus.Entities;

namespace Forexus.BLL
{
    /// <summary>
    /// CSV files processor
    /// </summary>
    public class MessageProcessor
    {
        /// <summary>
        /// Gets csv files from folder and process them
        /// </summary>
        /// <param name="folder">Folder to find csv files</param>
        /// <returns></returns>
        public List<Message> Process(string folder)
        {
            //Get all csv files from folder:
            string[] csvFiles = GetCsvFilesToProcess(folder);

            //Process files in paralell mode:
            var messages = new List<Message>();
            Parallel.ForEach(csvFiles, file => ConvertFileToMessages(file, messages));

            return messages;
        }

        private void ConvertFileToMessages(string file, List<Message> messages)
        {
            string[] lines = File.ReadAllLines(file);
            //Remove first line in CSV file, since it is header
            lines = lines.Skip(1).ToArray();
            Parallel.ForEach(lines, line => ConvertLineToMessage(line, messages));
        }

        private void ConvertLineToMessage(string line, List<Message> messages)
        {
            try
            {
                string[] elements = line.Split('\t');

                //We expect that each line contains 7 elements of data. 
                //If not we log it to analize later and move to the next line
                if (elements.Length != 7)
                    throw new ApplicationException(String.Format("Line length is {0}", elements.Length));

                //Element 0 - Time. If we can not parse it we probably go away to next line
                //Element 1 - System
                //Element 2 - User
                //Element 3 - Event
                //Element 4 - Group
                //Element 5 - Viewers
                //Element 6 - Text Message. If empty, we do not save message

                DateTime time;
                if (!DateTime.TryParse(elements[0], out time))
                    throw new ApplicationException(String.Format("Failed to parse messege Date format '{0}'", elements[0]));

                if (String.IsNullOrWhiteSpace(elements[6]))
                    throw new ApplicationException("Message is empty");

                //Run elements handling in async mode:
                Task<Source> taskSource = Task.Run(() => CreateEntityOrFindExisted<Source>(elements[1]));
                Task<User> taskUser = Task.Run(() => CreateEntityOrFindExisted<User>(elements[2]));
                Task<Group> taskGroup = Task.Run(() => CreateEntityOrFindExisted<Group>(elements[4]));

                //It takse too long to process Viewers. If you comment this line whole file processing goes very fast.
                Task<List<User>> taskViewers = Task.Run(() =>
                {
                    if (string.IsNullOrWhiteSpace(elements[5])) return new List<User>();
                    string[] viewersNames = elements[5].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    var viewers = new List<User>();
                    Parallel.ForEach(viewersNames, name => viewers.Add(CreateEntityOrFindExisted<User>(name)));
                    //clear array
                    Array.Clear(viewersNames, 0, viewersNames.Length);
                    return viewers;
                });

                var message = new Message
                {
                    Time = time,
                    MessageText = elements[6],
                    Source = taskSource.Result,
                    User = taskUser.Result,
                    Event = elements[3],
                    Group = taskGroup.Result,
                    Viewers = taskViewers.Result
                };

                using (var repository = new MessageRepository())
                {
                    repository.Save(message);
                }

                //If message does not have Id after save, log an error but do not stop processing.
                if (message.Id == Guid.Empty) return;
                messages.Add(message);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(String.Format("LINE: {0} | MESSAGE: {1}", line, ex.Message));
            }
        }

        private T CreateEntityOrFindExisted<T>(string name) where T : Entity, new()
        {
            //If name is empty, just do nothing:
            if (String.IsNullOrWhiteSpace(name)) return null;
            using (var repository = new EntityRepository<T>())
            {
                var entity = new T { Name = name };
                return repository.Save(entity);
            }
        }

        private string[] GetCsvFilesToProcess(string folder)
        {
            try
            {
                //if you run test in debug mode your BaseDirectory will contain bin\debug. Need to drop them:
                string assemblyPath = AppDomain.CurrentDomain.BaseDirectory.Replace("bin", string.Empty).Replace("Debug", string.Empty);
                string folderWithCsvFiles = Path.Combine(assemblyPath, folder);
                return Directory.GetFiles(folderWithCsvFiles, "*.csv", SearchOption.AllDirectories);
            }
            catch
            {
                return new string[0];
            }
        }

    }
}
