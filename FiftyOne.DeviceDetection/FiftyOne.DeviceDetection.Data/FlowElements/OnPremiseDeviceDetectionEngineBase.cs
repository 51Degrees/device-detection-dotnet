/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2019 51 Degrees Mobile Experts Limited, 5 Charlotte Close,
 * Caversham, Reading, Berkshire, United Kingdom RG4 7BY.
 *
 * This Original Work is licensed under the European Union Public Licence (EUPL) 
 * v.1.2 and is subject to its terms as set out below.
 *
 * If a copy of the EUPL was not distributed with this file, You can obtain
 * one at https://opensource.org/licenses/EUPL-1.2.
 *
 * The 'Compatible Licences' set out in the Appendix to the EUPL (as may be
 * amended by the European Commission) shall be deemed incompatible for
 * the purposes of the Work and the provisions of the compatibility
 * clause in Article 5 of the EUPL shall not apply.
 * 
 * If using the Work as, or as part of, a network application, by 
 * including the attribution notice(s) required under Article 5 of the EUPL
 * in the end user terms of the application under an appropriate heading, 
 * such notice(s) shall fulfill the requirements of that article.
 * ********************************************************************* */
 
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FiftyOne.DeviceDetection.Shared.FlowElements
{
    public abstract class OnPremiseDeviceDetectionEngineBase<T> : 
        FiftyOneOnPremiseAspectEngineBase<T>, IOnPremiseDeviceDetectionEngine
        where T : IDeviceData
    {
        public OnPremiseDeviceDetectionEngineBase(ILogger<FiftyOneOnPremiseAspectEngineBase<T>> logger, 
            Func<IPipeline, FlowElementBase<T, IFiftyOneAspectPropertyMetaData>, T> aspectDataFactory, 
            string tempDataFilePath) :
            base(logger, aspectDataFactory, tempDataFilePath)
        {
        }

        public abstract event EventHandler<EventArgs> RefreshCompleted;

        /// <summary>
        /// Get the bytes from the specified stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected byte[] ReadBytesFromStream(Stream stream)
        {
            byte[] data = new byte[stream.Length];
            var memoryStream = stream as MemoryStream;
            if (memoryStream != null)
            {
                // Note that the buffer may be longer 
                // than it needs to be so we can't just copy the whole 
                // thing.
                // The stream 'Length' property will get the true number
                // of elements we need to take from the buffer.
                Array.Copy(memoryStream.GetBuffer(),
                    data, stream.Length);
            }
            else
            {
                int bytesRead = 1024;
                int counter = 0;
                while (bytesRead == 1024)
                {
                    bytesRead = stream.Read(data, 0 + counter * 1024, 1024);
                    counter++;
                }
            }

            return data;
        }

        /// <summary>
        /// Add the specified data file to the engine
        /// </summary>
        /// <param name="dataFile"></param>
        public override void AddDataFile(IAspectEngineDataFile dataFile)
        {
            if (DataFiles.Count > 0)
            {
                throw new Exception($"{GetType().Name} already " +
                    "has a configured data source.");
            }
            base.AddDataFile(dataFile);
            // Data files for OnPremise DeviceDetection engines are not 
            // processed by the 'ConfigureEngine' method in 
            // OnPremiseAspectEngineBuilderBase.
            // This is because they are added in the engine constructor 
            // instead.
            // Therefore, to avoid unnecessary memory usage, we need to 
            // check for and clear the data stream here instead.
            if (dataFile.Configuration.DataStream != null)
            {
                dataFile.Configuration.DataStream.Dispose();
                dataFile.Configuration.DataStream = null;
            }
        }

    }
}
