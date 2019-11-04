#!/usr/bin/env bash

# Download the CLI install script to Agent.TempDirectory

echo "Installing dotnet CLI into ${Agent.TempDirectory} folder for building"

installDir="${Agent.TempDirectory}/dotnet"

mkdir -p $installDir

curl -o $installDir/dotnet-install.sh -L https://dot.net/v1/dotnet-install.sh

# Run install.sh for cli

chmod +x $installDir/dotnet-install.sh


# install master channel to get latest .NET 5 sdks 

$installDir/dotnet-install.sh -i $installDir -c master

echo "Deleting .NET Core temporary files"
rm -rf "/tmp/"dotnet.*


# Display current version

dotnet --info



echo "================="

