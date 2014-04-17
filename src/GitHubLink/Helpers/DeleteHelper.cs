﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteHelper.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink
{
    using System.IO;

    public static class DeleteHelper
    {
        #region Methods

        public static void DeleteGitRepository(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                return;
            }

            foreach (var fileName in Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories))
            {
                var fileInfo = new FileInfo(fileName)
                {
                    IsReadOnly = false
                };

                fileInfo.Delete();
            }

            Directory.Delete(directory, true);
        }

        #endregion
    }
}