﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Context.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using Catel;
    using Catel.IO;
    using Catel.Logging;
    using Providers;

    public class Context
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IProviderManager _providerManager;
        private IProvider _provider;

        public Context(IProviderManager providerManager)
        {
            Argument.IsNotNull(() => providerManager);

            _providerManager = providerManager;

            Authentication = new Authentication();
            ConfigurationName = "Release";
            PlatformName = "AnyCPU";
        }

        public bool IsHelp { get; set; }

        public bool IsDebug { get; set; }

        public string LogFile { get; set; }

        public string SolutionDirectory { get; set; }

        public string ConfigurationName { get; set; }

        public string PlatformName { get; set; }

        public Authentication Authentication { get; private set; }

        public IProvider Provider
        {
            get
            {
                if (_provider == null)
                {
                    _provider = _providerManager.GetProvider(TargetUrl);
                }

                return _provider;
            }
            set
            {
                _provider = value;
            }
        }

        public string TargetUrl { get; set; }

        public string TargetBranch { get; set; }

        public string ShaHash { get; set; }

        public string SolutionFile { get; set; }

        public void ValidateContext()
        {
            if (!string.IsNullOrWhiteSpace(SolutionDirectory))
            {
                SolutionDirectory = Path.GetFullPath(SolutionDirectory, Environment.CurrentDirectory);
            }

            if (string.IsNullOrEmpty(SolutionDirectory))
            {
                Log.ErrorAndThrowException<GitLinkException>("Solution directory is missing");
            }

            if (string.IsNullOrEmpty(ConfigurationName))
            {
                Log.ErrorAndThrowException<GitLinkException>("Configuration name is missing");
            }

            if (string.IsNullOrEmpty(PlatformName))
            {
                Log.ErrorAndThrowException<GitLinkException>("Platform name is missing");
            }

            if (string.IsNullOrEmpty(TargetUrl))
            {
                Log.ErrorAndThrowException<GitLinkException>("Target url is missing");
            }

            if (Provider == null)
            {
                Log.ErrorAndThrowException<GitLinkException>("Cannot determine git provider");
            }
        }
    }
}