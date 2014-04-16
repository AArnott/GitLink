GitHubLink
==========

GitHubLink let's users step through your code hosted on GitHub! **This makes symbol servers obsolete** which saves you both time with uploading source files with symbols and the user no longer has to specify custom symbol servers (such as symbolsource.org).

 
The idea is based on the [SourceLink project](https://github.com/ctaggart/SourceLink "SourceLink project"). However it requires FAKE and no everyone likes to write code in F#. GitHubLink is available as console application and can be references as assembly as well to be used in other .NET assemblies.

The advantage of GitHubLink is that it is fully customized for GitHub. It also works with GitHub urls so it **does not require a local git repository to work**. This makes it perfectly usable in continuous integration servers such as [Continua CI](http://www.finalbuilder.com/Continua-CI "Continua CI").

Updating all the pdb files is very fast. A solution with over 85 projects will be handled in less than 30 seconds.

# Using GitHubLink #

Using GitHubLink is very simple:

1. Build the software (in release mode with pdb files enabled)
2. Run the console application with the right command line parameters

Below are a few examples.

## Running for the default branch ##

    GitHubLink.exe c:\source\catel -u https://github.com/catel/catel 

This will use the default branch (which is in most cases **master**). You can find out the default branch by checking what branch is loaded by default on the GitHub page.

## Running for a specific branch ##

    GitHubLink.exe c:\source\catel -u https://github.com/catel/catel -b develop

This will use the develop branch.

## Running for a specific branch and configuration ##

    GitHubLink.exe c:\source\catel -u https://github.com/catel/catel -b develop -c debug

This will use the develop branch and the debug configuration.

## Getting help ##

When you need help about GitHubLink, use the following command line:

    GitHubLink.exe -help

## Logging to a file ##

When you need to log the information to a file, use the following command line:

    GitHubLink.exe c:\source\catel -u https://github.com/catel/catel -b develop -l GitHubLinkLog.log


# How does it work #

The SrcSrv tool (Srcsrv.dll) enables a client to retrieve the exact version of the source files that were used to build an application. Because the source code for a module can change between versions and over the course of years, it is important to look at the source code as it existed when the version of the module in question was built.

For more information, see the [official documentation of SrcSrv](http://msdn.microsoft.com/en-us/library/windows/hardware/ff558791(v=vs.85).aspx).

GitHubLink creates a source index file and updates the PDB file so it will retrieve the files from the GitHub file handler.