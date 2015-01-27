// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentParser.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Collections;
    using Catel.Logging;
    using Git;
    using GitLink.Providers;
    using LibGit2Sharp;

    public static class ArgumentParser
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static Context ParseArguments(string commandLineArguments)
        {
            return ParseArguments(commandLineArguments.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                new ProviderManager());
        }

        public static Context ParseArguments(params string[] commandLineArguments)
        {
            return ParseArguments(commandLineArguments.ToList(), new ProviderManager());
        }

        public static Context ParseArguments(List<string> commandLineArguments, IProviderManager providerManager)
        {
            var context = new Context(providerManager);

            if (commandLineArguments.Count == 0)
            {
                Log.ErrorAndThrowException<GitLinkException>("Invalid number of arguments");
            }

            var firstArgument = commandLineArguments.First();
            if (IsHelp(firstArgument))
            {
                context.IsHelp = true;
                return context;
            }

            if (commandLineArguments.Count < 3 && commandLineArguments.Count != 1)
            {
                Log.ErrorAndThrowException<GitLinkException>("Invalid number of arguments");
            }

            context.SolutionDirectory = firstArgument;

            var namedArguments = commandLineArguments.Skip(1).ToList();
            for (var index = 0; index < namedArguments.Count; index++)
            {
                var name = namedArguments[index];

                // First check everything without values
                if (IsSwitch("debug", name))
                {
                    context.IsDebug = true;
                    continue;
                }

                // After this point, all arguments should have a value
                index++;
                var valueInfo = GetValue(namedArguments, index);
                var value = valueInfo.Key;
                index = index + (valueInfo.Value - 1);

                if (IsSwitch("l", name))
                {
                    context.LogFile = value;
                    continue;
                }

                if (IsSwitch("c", name))
                {
                    context.ConfigurationName = value;
                    continue;
                }

                if (IsSwitch("p", name))
                {
                    context.PlatformName = value;
                    continue;
                }

                if (IsSwitch("u", name))
                {
                    context.TargetUrl = value;
                    continue;
                }

                if (IsSwitch("b", name))
                {
                    context.TargetBranch = value;
                    continue;
                }

                if (IsSwitch("s", name))
                {
                    context.ShaHash = value;
                    continue;
                }

                if (IsSwitch("f", name))
                {
                    context.SolutionFile = value;
                    continue;
                }

                if (IsSwitch("ignore", name))
                {
                    context.IgnoredProjects.AddRange(value.Split(new []{ ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                    continue;
                }

                Log.ErrorAndThrowException<GitLinkException>("Could not parse command line parameter '{0}'.", name);
            }

            if (string.IsNullOrEmpty(context.TargetUrl))
            {
                var gitDir = GitDirFinder.TreeWalkForGitDir(context.SolutionDirectory);
                if (gitDir != null)
                {
                    using (var repo = RepositoryLoader.GetRepo(gitDir))
                    {
                        var currentBranch = repo.Head;

                        if (string.IsNullOrEmpty(context.ShaHash))
                        {
                            context.ShaHash = currentBranch.Tip.Sha;
                        }

                        if (currentBranch.Remote == null || currentBranch.IsDetachedHead())
                        {
                            currentBranch = GetBranchesContainingCommit(repo, context.ShaHash).FirstOrDefault(b => b.Remote != null);
                        }

                        if (currentBranch != null && currentBranch.Remote != null)
                        {
                            var url = currentBranch.Remote.Url;
                            if (url.StartsWith("https://"))
                            {
                                context.TargetUrl = url.EndsWith(".git") ? url.Substring(0, url.Length - 4) : url;
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(context.TargetUrl))
            {
                context.Provider = providerManager.GetProvider(context.TargetUrl);
            }

            return context;
        }

        private static IEnumerable<Branch> GetBranchesContainingCommit(IRepository repository, string commitSha)
        {
            var directBranchHasBeenFound = false;
            foreach (var branch in repository.Branches)
            {
                if (branch.Tip.Sha != commitSha)
                {
                    continue;
                }

                directBranchHasBeenFound = true;
                yield return branch;
            }

            if (directBranchHasBeenFound)
            {
                yield break;
            }

            foreach (var branch in repository.Branches)
            {
                var commits = repository.Commits.QueryBy(new CommitFilter { Since = branch }).Where(c => c.Sha == commitSha);

                if (!commits.Any())
                {
                    continue;
                }

                yield return branch;
            }
        }

        private static KeyValuePair<string, int> GetValue(List<string> arguments, int index)
        {
            var totalCounter = 1;

            var value = arguments[index];

            while (value.StartsWith("\""))
            {
                if (value.EndsWith("\""))
                {
                    break;
                }

                index++;
                value += " " + arguments[index];

                totalCounter++;
            }

            value = value.Trim('\"');

            return new KeyValuePair<string, int>(value, totalCounter);
        }

        private static bool IsSwitch(string switchName, string value)
        {
            if (value.StartsWith("-"))
            {
                value = value.Remove(0, 1);
            }

            if (value.StartsWith("/"))
            {
                value = value.Remove(0, 1);
            }

            return (string.Equals(switchName, value));
        }

        private static bool IsHelp(string singleArgument)
        {
            return (singleArgument == "?") ||
                   IsSwitch("h", singleArgument) ||
                   IsSwitch("help", singleArgument) ||
                   IsSwitch("?", singleArgument);
        }
    }
}