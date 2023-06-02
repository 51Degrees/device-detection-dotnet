/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2023 51 Degrees Mobile Experts Limited, Davidson House,
 * Forbury Square, Reading, Berkshire, United Kingdom RG1 3EU.
 *
 * This Original Work is licensed under the European Union Public Licence
 * (EUPL) v.1.2 and is subject to its terms as set out below.
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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using System;
using System.Diagnostics;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers
{
    internal class SwigFactory : ISwigFactory
    {
        public Func<string, IConfigSwigWrapper,
            IRequiredPropertiesConfigSwigWrapper, IEngineSwigWrapper> EngineFromFile;
        public Func<byte[], int, IConfigSwigWrapper,
            IRequiredPropertiesConfigSwigWrapper, IEngineSwigWrapper> EngineFromData;

        public Func<VectorStringSwig, IRequiredPropertiesConfigSwigWrapper> RequiredPropertiesFactory;

        public Func<IConfigSwigWrapper> ConfigFactory;

        public SwigFactory()
        {
            EngineFromFile = (fileName, config, requiredProperties) =>
            {
                Func<IEngineSwigWrapper> action = () =>
                {
                    var engine = new EngineHashSwig(fileName, config.Object, requiredProperties.Object);
                    return new EngineSwigWrapper(engine);
                };
                return DllExceptionHandler(action);
            };
            EngineFromData = (data, dataSize, config, requiredProperties) =>
            {
                Func<IEngineSwigWrapper> action = () =>
                {
                    var engine = new EngineHashSwig(data, dataSize, config.Object, requiredProperties.Object);
                    return new EngineSwigWrapper(engine);
                };
                return DllExceptionHandler(action);
            };
            RequiredPropertiesFactory = (properties) =>
            {
                Func<IRequiredPropertiesConfigSwigWrapper> action = () =>
                {
                    var instance = new RequiredPropertiesConfigSwig(properties);
                    return new RequiredPropertiesConfigSwigWrapper(instance);
                };
                return DllExceptionHandler(action);
            };
           
            ConfigFactory = () => {
                Func<IConfigSwigWrapper> action = () =>
                {
                    return new ConfigSwigWrapper(new ConfigHashSwig());
                };
                return DllExceptionHandler(action);
            };
        }

        public static T DllExceptionHandler<T>(Func<T> action) 
        {
            try
            {
                return action();
            }
            catch (TypeInitializationException ex)
            {
                throw new Exception(Messages.DLLException, ex);
            }
            catch (BadImageFormatException ex)
            {
                throw new Exception(string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    Messages.DLLException,
                    Environment.Is64BitProcess ? "64-bit" : "not 64-bit"), ex);                
            }
            catch (DllNotFoundException ex)
            {
                throw new Exception(Messages.DLLException, ex);
            }
        }

        public IEngineSwigWrapper CreateEngine(string fileName,
            IConfigSwigWrapper config,
            IRequiredPropertiesConfigSwigWrapper requiredProperties)
        {
            return EngineFromFile(fileName, config, requiredProperties);
        }
        public IEngineSwigWrapper CreateEngine(byte[] data, int dataSize,
            IConfigSwigWrapper config,
            IRequiredPropertiesConfigSwigWrapper requiredProperties)
        {
            return EngineFromData(data, dataSize, config, requiredProperties);
        }
        public IRequiredPropertiesConfigSwigWrapper CreateRequiredProperties(
            VectorStringSwig properties)
        {
            return RequiredPropertiesFactory(properties);
        }

        public IConfigSwigWrapper CreateConfig()
        {
            return ConfigFactory();
        }

    }
}
