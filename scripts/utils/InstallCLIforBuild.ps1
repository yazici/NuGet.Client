# Download the CLI install script to Agent.TempDirectory

#Write-Host "Installing dotnet CLI into $Env:AGENT_TEMPDIRECTORY folder for building"

$InstallDir = Join-Path $Env:AGENT_TEMPDIRECTORY 'dotnet'

New-Item -ItemType Directory -Force -Path $InstallDir | Out-Null

$DotNetInstall = Join-Path $InstallDir 'dotnet-install.ps1'

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

Invoke-WebRequest 'https://dot.net/v1/dotnet-install.ps1' -OutFile $DotNetInstall


if ([Environment]::Is64BitOperatingSystem) 
{
    $arch = "x64";
}
else 
{
    $arch = "x86";
}

& $DotNetInstall -Channel master -i $InstallDir -Architecture $arch 

$Env:PATH

$Env:PATH = "$InstallDir;${Env:PATH}"

$Env:PATH

# Display build info
& dotnet --info