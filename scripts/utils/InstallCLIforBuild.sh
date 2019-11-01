#!/usr/bin/env bash

# Download the CLI install script to Agent.TempDirectory

env

echo "Installing dotnet CLI into ${AGENT_TEMPDIRECTORY} folder for building"

installDir="${AGENT_TEMPDIRECTORY}/dotnet"

mkdir -p $installDir

curl -o $installDir/dotnet-install.sh -L https://dot.net/v1/dotnet-install.sh

# Run install.sh for cli

chmod +x $installDir/dotnet-install.sh


# install master channel to get latest .NET 5 sdks 

$installDir/dotnet-install.sh -i $installDir -c master

echo "Add ${installDir} to PATH"
PATH=$PATH:${installDir}

echo $PATH

echo "Deleting .NET Core temporary files"
rm -rf "/tmp/"dotnet.*


# Display current version

dotnet --info



echo "================="

