﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RunMethodsSequentially.LockAndRunCode;
using RunMethodsSequentially.TestHelpers;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RunMethodsSequentially
{
    /// <summary>
    /// This class will test your use of the <see cref="StartupExtensions.RegisterRunMethodsSequentially"/>
    /// by registering to the services and then running the code which would be run on the startup of your application
    /// </summary>
    public class RegisterRunMethodsSequentiallyTester
    {
        /// <summary>
        /// You need to register the <see cref="StartupExtensions.RegisterRunMethodsSequentially"/> with its options
        /// as found in your startup code.
        /// You also need to register any services, such as your application's DbContext, that your startup services need
        /// </summary>
        public ServiceCollection Services { get; } = new ServiceCollection();

        /// <summary>
        /// If you are using the <see cref="StartupExtensions.AddFileSystemLockAndRunMethods"/> then you can use this path to a directory
        /// </summary>
        public string LockFolderPath { get; } = Directory.GetCurrentDirectory();

        /// <summary>
        /// This holds the RunMethodsSequentially logs
        /// </summary>
        public List<LocalLogOutput> Logs = new List<LocalLogOutput>();

        /// <summary>
        /// Run this to check that your <see cref="StartupExtensions.RegisterRunMethodsSequentially"/> with its options work
        /// </summary>
        /// <returns></returns>
        public async Task RunHostStartupCodeAsync()
        {
            Services.AddSingleton<ILogger<GetLockAndThenRunServices>>(
                new Logger<GetLockAndThenRunServices>(new LoggerFactory(new[] { new LoggerProviderActionOut(Logs.Add) })));
            var serviceProvider = Services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<RunSequentiallyOptions>();
            if (options.RegisterAsHostedService)
            {
                var lockAndRun = serviceProvider.GetRequiredService<IHostedService>();
                await lockAndRun.StartAsync(default);
            }
            else
            {
                var lockAndRun = serviceProvider.GetRequiredService<IGetLockAndThenRunServices>();
                await lockAndRun.LockAndLoadAsync();
            }
        }

    }
}
