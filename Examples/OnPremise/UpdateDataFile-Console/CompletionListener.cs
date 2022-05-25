using FiftyOne.Pipeline.Engines.Services;
using System;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Examples.OnPremise.UpdateDataFile
{
    /// <summary>
    /// Helper class used to wait for data updates to complete and return the status of the update.
    /// </summary>
    public class CompletionListener
    {
        public DataUpdateCompleteArgs Result { get; private set; }
        public bool Complete { get; private set; }

        public CompletionListener(IDataUpdateService dataUpdateService)
        {
            dataUpdateService.CheckForUpdateComplete += DataUpdateService_CheckForUpdateComplete;
        }

        private void DataUpdateService_CheckForUpdateComplete(object sender, DataUpdateCompleteArgs e)
        {
            Result = e;
            Complete = true;
        }

        /// <summary>
        /// Blocks the calling thread until the data update is complete.
        /// Note - an update may have occurred before calling this method. Call <see cref="Reset"/>
        /// to wait for the *next* update.
        /// </summary>
        /// <seealso cref="Reset"/>
        /// <param name="timeout">
        /// The maximum time to wait.
        /// </param>
        /// <exception cref="TimeoutException">
        /// Thrown if the update does not complete before the timeout expires.
        /// </exception>
        public void WaitForComplete(TimeSpan timeout)
        {
            DateTime start = DateTime.UtcNow;
            while(Complete == false &&
                start.Add(timeout) > DateTime.UtcNow)
            {
                Task.Delay(100).Wait();
            }
            if(Complete == false)
            {
                throw new TimeoutException("Timed out waiting for data update to complete");
            }
        }

        /// <summary>
        /// Clear the 'complete' flag. This will cause the <see cref="WaitForComplete(TimeSpan)"/>
        /// method to block until a future data update completes.
        /// </summary>
        /// <seealso cref="WaitForComplete(TimeSpan)"/>
        public void Reset()
        {
            Complete = false;
            Result = null;
        }
    }
}
