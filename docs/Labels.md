
# Problem

Too many issues. Lack of consistent triaging guidelines. 

Leads to inconsistent labeling, which in turn 
leads to difficulty finding duplicates.
The number of issues blurs the visibility of the backlog.

Solution: Categories of labels, clearly indicating required and optional labels. 

Each type/group of labels will have a different color. Some are exclusive, others are not. 
For some if you need more than 1 label, it might suggest that we need more than one issue, etc

When triaging we should ask ourselfs a list of questions, and this proposal should cover that. 

We have 13-14 issue categories each with a different color. 

1. What "kind" of an issue is this? 
Is this a product bug? New feature? DCR? A docs request?

1. If this is a product issue, which product does it fact? 
It could be one of the ones we actively maintain, nuget.exe, dotnet cli, pm console, other VS functionality.
Or maybe VS 2015. Or is it maybe a product tracked in our repo like NuGetizer or BuildTasks or a special case like MSBuildSDKResolver which is maintained by partner teams in our repo.

1. If this is a product related issue, which functionality does it tackle? 
Restore,List, list package? Pack? Symbols? 

1. Any more specific sub areas. These labels are very subjective, so we'll keep it simple and discuss internally when we need to add new ones. 

1. Does this issue fall under a specific tenet? 
Performance, Reliability, Security? 

1. Is this a specific platform issue? 
Mono/Docker/XPLAT. No platform implies windows or irrelevant usually :) It's not a fully fleshed out story.

1. Is the package management style relevant here? If yes, label it!
The package management style is relevant for restore/pack (install/uninstall due to source availability), and many feature asks. 

1. Is this an ask from a partner team (for a partner feature, not just filed by a partner team member :) )

1. If this issue is not appropriate and will not require team action (not a bug, duplicate etc), make sure to use the resolution labels. 

1. The triage/cla/release/sprint are something we are getting rid of soon, so don't bother too much until asked.

### Issue type

- Type:Bug
- Type:DCR
- Type:DataAnalysis
- Type:Docs
- Area:Contributing => Type:DeveloperDocs
- Type:Feature
- Type:Spec
- Area:Test => Type:Test
- Test Failure => Type:TestFailure (vendor failures, should we label them as such?) (consider unifying these.)
- Type:Engineering (new label, infrastructure, or simple refactorings etc.)
- Area:Release Process => Type:Docs (or developer docs, either way, no need for a special label)

### Product

- Area:CommandLine => Product:NuGetCommandline
- Area:Dotnet CLI => Product:DotnetCLI
- Area:MSBuildSDKResolver Product:MSBuildSDKResolver
- Area:NuGetizer => Product:NuGetizer
- Area:VS.PMConsole => Product => VS.PMConsole
- Area:VS.Client => Product:VS.Client
- Visual Studio 2015 => Product:VS2015
- Area:Build Task => Product:BuildTasks

### Functionality

- Area:Install => Functionality:Install
- Area:List(Search) => Functionality:List(Search)
- Area:ListPackageCommand => Functionality:ListPackage
- Area:Locals => Functionality:Locals
- Area:PCtoPRMigrator => Functionality:PCtoPRMigrator
- Area:Pack => Functionality:Pack
- Area:PackageSigning => Functionality:Signing
- Area:Push => Functionality:Push
- Area:Restore => Functionality:Restore
- Area:Updates => Functionality:Update
- NuGet Visual Studio UI => Functionality:NuGetVisualStudioUI
- Symbols => Functionality:Symbols
- Search => Functionality:Search

### Area - sub area, sub part of a functionality/product

- Area:Auth => Area:Authentication
- Area:ContentFiles => Area:ContentFiles
- Area:Error Handling => Area:ErrorHandling
- Area:HttpCaching => Area:HttpCaching
- Area:HttpCommunication => Area:HttpCommunication
- Area:Logging => Area:Logging
- Area:NewFrameworks => Area:NewFrameworks
- Area:PackageDefinition => Area:PackageDefinition
- Area:RepeatableBuild => Area:RepeatableBuild
- Area:RestoreNoOp => Area:RestoreNoOp
- Area:ToolRestore => Area:ToolRestore
- Area:Plugin => Area:Plugin
- Area:Settings => Area:Config
- Area:SDK => Area:SDK (Challenge here is that there are lots of issues that will be affected, Might require some work to actually call the GitHub APIs)
- NuGet API => Area:SDK
- Area:Engineering Improvements => Area:Engineering improvements (refactorings, process changes, what makes the NuGet dev better)
- Area:Infrastructure => new labels, infrastructure, tests failing, machines broken, RPS failing

### Tenet

- Area:Accessibility => Tenet:Accessibility
- Area:Acquisition => Tenet:Acquisition
- Security => Tenet:Security
- Area:Compliance => Tenet:Compliance
- Area:Perf => Tenet:Performance
- Area:Reliability => Tenet:Reliability
- Needs loc => Tenet:WorldReady

### Platform 

- Area:Docker => Platform:Docker
- Area:Mono => Platform:Mono
- XPLAT => Platform:XPLAT

### PackageManagement style (Applies to install, restore, pack )

- Style:PackageReference => Style:PackageReference
- Style:Packages.Config => Style:Packages.Config
- Style:Project.json => Style:Project.json

### Partners

- Partner:VS-Other
- Partner:1ES
- Partner:AzureDevOps
- Partner:C++
- Partner:CLI-SDK
- Partner:MSBuild
- Partner:Project-System
- Partner:Roslyn
- Partner:Runtime
- Partner:VS4Mac
- Partner:VSPlatform
- Partner:Xamarin

### Resolution labels

- Resolution:BlockedByExternal
- Resolution:ByDesign
- Resolution:Duplicate
- Resolution:External
- Resolution:Invalid
- Resolution:NotABug
- Resolution:NotEnoughInfo
- Resolution:NotRepro
- Resolution:Question
- Resolution:WontFix

### Triaging

- Priority:0
- Priority:1
- Priority:2

### Sprint tracking

Sprint130
....
Sprint165

### Release Tracking 

- Preview1
- Preview2
- Preview3
- Preview4
- Preview5
- RTM

### CLA related labels.

- cla-already-signed
- cla-not-required
- cla-required
- cla-signed


### Status labels

- Discussions
- Epic
- Investigate
- NeedsMoreDetails
- NeedsRepro
- NeedsTriageDiscussion
- RegressionDuringThisVersion
- RegressionFromPreviousRTM
- Up for Grabs
- WaitingForCustomer