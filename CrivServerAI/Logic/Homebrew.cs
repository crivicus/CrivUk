using CrivServer.AI.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrivServer.AI.Logic
{
    public class Homebrew : IDisposable
    {
        private List<string> _tempMemory;

        public IEnumerable<string> RetrieveFromMemory(string input)
        {
            Task.Run(() => SearchMemoryAsync(input)).Wait();
            return _tempMemory;
        }

        public async Task SearchMemoryAsync(string input)
        {
            List<string> tempMemory = new List<string>();
            List<Task> tasksList = new List<Task>();

            tasksList.Add(FixedMemoryAsync());
            tempMemory.AddRange(FixedMemoryAsync().Result);

            using (DatabaseMemory db = new DatabaseMemory()) {
                Task<List<string>> dbtask = db.FindRelatedStringAsync(input);
                tasksList.Add(dbtask);
                tempMemory.AddRange(dbtask.Result);
            }

            using (FileMemory fm = new FileMemory()) {
                Task<List<string>> fmtask = fm.FindInFilesAsync(input);
                tasksList.Add(fmtask);
                tempMemory.AddRange(fmtask.Result);
            }

            await Task.WhenAll(tasksList);

            _tempMemory = tempMemory;
        }

        private async Task<List<string>> FixedMemoryAsync()
        {
            return new List<string> { "1", "2", "3" };
        }

        #region Dispose elements
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                _tempMemory = null;
            }
            // free native resources if there are any.
        }
        #endregion
    }
}
