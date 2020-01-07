
# Problem

We have too many open issues, and a lack of consistently applied triaging guidelines. 

All of this leads to:
- Inconsistent labeling which leads to difficulty finding duplicates
- The number of issues blurs the visibility of our backlog
- Realistically we are never gonna fix all 1700 issues

At some point we brought the number of issues down to 800. There is a true and tried approach to decreasing the number of issues. Few hour sessions of everyone triaging can help us find duplicates, close old and not well understood issues and close issues that won't be fixing realistically. 

Bringing the issues down is only one part of it all. 
The second part is the more difficult one and that's maintaining our backlog clean. That will require constant effort and reevaluation. 
In this doc we are only addressing the latter. The first part has to be a brute-force approach.

## Solution

Categories of labels, clearly indicating required and optional labels. 

Each type/group of labels will have a different color. 
Most of the time they'll have the same prefix. 
Some label types are exclusive, but others are not.

When triaging we should ask ourselfs a list of questions, and this proposal covers that. 
Most label names are self-descriptive. Ones that are not will get an explanation.

Before going into labeling some ground rules:

- Every should label their own issues to the full extent of whatever we agree to in here.
  - that includes engineering and PMs See [Open Questions](#open-questions)
  - You created an issue, you label it
  - You moved an issue, you label it
- We jointly agree to follow a certain approach. As with coding styles, being consistent can be more important that being "correct". Being consistent could also make it easier to change our approach in the future with minimum friction :)

### What are the questions we should be asking ourselves when triaging?

1. What "kind" of an issue is this? 
Is this a product bug? New feature? DCR? A docs request? See [Issue type](#issue-type)

1. If this is a product issue, which product does it affect? 
It could be one of the ones we actively maintain, nuget.exe, dotnet cli, pm console, other VS functionality. See [Product](#product)

1. If this is a product related issue, which functionality does it affect? 
Restore,List, list package? Pack? Symbols? See [Functionality](#functionality)

1. Any more specific sub areas? See [Area](#area)

1. Does this issue fall under a specific tenet? See [Tenet](#tenet)

1. Is this a specific platform issue? See [Platform](#platform)

1. Is the package management style relevant here? If yes, label it! See [PackageManagement Style](#packagemanagement-style)

1. Is this an ask from a partner team? Affect a partner team? See [Partners](#partners)

1. If  we have determined that issue is not actionable, make sure to add a resolution label. See [Resolution](#resolution)

1. Finally, if necessary use the status labels such as WaitingForCustomer, Regression etc. See [Status labels](#status-labels)
These labels usually indicate that these issues should be actively monitored, not just by the person labeling them, but in the future as well.

### Issue type

- Type:Bug
- Type:DCR
- Type:Feature
- Type:Spec
- Type:DataAnalysis
- Type:Engineering
- Type:Docs <= Type:Docs & Area:Release Process
- Type:DeveloperDocs <= Area:Contributing
- Type:Test <= Area:Test
- Type:TestFailure <= Test Failure

Type:Docs - This refers to official NuGet docs and blogs etc. 
Type:DeveloperDocs - We are starting to build up our product knowledge base. That consists of a docs folder in the root of the client repo.
Type:Test - This refers to any automation test related issues. In the past we have had Area:Test and TestFailure which used to refer to manual vendor test issues. 

#### How often should it be used?
[1*]
Ideally one per issue. Epics are excluded, in some situations more than one is reasonable, although consider splitting the issue up if necessary.

### Product

All the products for which issues are created in this repository. 

- Product:NuGetCommandline <= Area:CommandLine
- Product:DotnetCLI <= Area:Dotnet CLI
- Product:MSBuildSDKResolver <= Area:MSBuildSDKResolver 
- Product:NuGetizer <= Area:NuGetizer
- Product:VS.PMConsole <= Area:VS.PMConsole
- Product:VS.Client <= Area:VS.Client
- Product:VS2015 <= Visual Studio 2015
- Product:BuildTasks <= Area:Build Task

NuGetizer is a 1st party alternative to pack, which for "reasons" is tracked in this repo. At this point NuGetizer is not actively developed anymore.
BuildTask refers to the build tasks that consume the assets file in old csproj PackageReference (or called LegacyPackageReference sometimes). The repo has now moved to https://github.com/dotnet/NuGet.BuildTasks. See [Additional proposals](additional-proposals)

#### How often should it be used?
[0..n]
However many are needed. If say there's a feature ask for restore, that'd affect all restore scenarios, nuget.exe, dotnetcli, and VS, add *all* the labels. 
The cons of overlabeling are better than the cons of not labeling and simply not knowing what the bugs are. 

### Functionality

This covers things like commands, experience funnels etc that are not products themselves. 

- Functionality:Install <= Area:Install
- Functionality:List(Search) <= Area:List(Search)
- Functionality:ListPackage <= Area:ListPackageCommand
- Functionality:Locals <= Area:Locals
- Functionality:PCtoPRMigrator <= Area:PCtoPRMigrator
- Functionality:Pack <= Area:Pack
- Functionality:Signing <= Area:PackageSigning
- Functionality:Push <= Area:Push
- Functionality:Restore <= Area:Restore
- Functionality:Update <= Area:Updates
- Functionality:NuGetVisualStudioUI <= NuGet Visual Studio UI
- Functionality:Symbols <= Symbols
- Functionality:Searc <= Search

For example anything that has to do with a VS gesture, needs a Functionality:NuGetVisualStudioUI label. 

#### How often should it be used?
[0..n]

### Area

- Area:Authentication <= Area:Auth
- Area:ContentFiles <= Area:ContentFiles
- Area:ErrorHandling <= Area:Error Handling
- Area:HttpCaching <= Area:HttpCaching
- Area:HttpCommunication <= Area:HttpCommunication
- Area:Logging <= Area:Logging
- Area:NewFrameworks <= Area:NewFrameworks
- Area:PackageDefinition <= Area:PackageDefinition
- Area:RepeatableBuild <= Area:RepeatableBuild
- Area:RestoreNoOp <= Area:RestoreNoOp
- Area:ToolRestore <= Area:ToolRestore
- Area:Plugin <= Area:Plugin
- Area:Settings <= Area:Settings
- Area:SDK <= Area:SDK
- NuGet API <= Area:SDK [See Open Questions](#open-questions)
- Area:Engineering Improvements <= Area:Engineering improvements
- Area:Infrastructure  (NEW!)

Some concepts are throughout all of the product, but without a specific user functionality. A perfect example is authentication. 
It's relevant for every scenario that involves remoting.

Infrastructure is a new area. Covers CI/insertion issues. 
Engineering improvements should have a reduced scope. Currently Engineering Improvements refers to things like CI issues to things like refactoring. It should be all about what makes a NuGet dev better. 

#### How often should it be used?
[0..n]

### Tenet

- Tenet:Accessibility <= Area:Accessibility
- Tenet:Acquisition <= Area:Acquisition
- Tenet:Security <= Security
- Tenet:Compliance <= Area:Compliance
- Tenet:Performance <= Area:Perf
- Tenet:Reliability <= Area:Reliability
- Tenet:WorldRead <= Needs loc

Most of us are tenet owners, and this helps categorize issues based on that. 

#### How often should it be used?
[1*]
Ideally one, but exceptions are understandable.

### Platform 

- Platform:Docker <= Area:Docker
- Platform:Mono <= Area:Mono
- Platform:XPLA <= XPLAT

Is this an issue only with a specific platform?
No platform label usually means affects all appropriate for product. We don't have a windows specific one, as we've never had the need to look at issue that way.

#### How often should it be used?
[1*]
Ideally one, but exceptions are understandable.
 
### PackageManagement style

- Style:PackageReference <= Style:PackageReference
- Style:Packages.Config <= Style:Packages.Config
- Style:Project.json <= Style:Project.json

In restore, package install/uninstall, config reading, pack scenarios, the project style matters!

#### How often should it be used?
[1..n]
 
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

#### How often should it be used?
[1..n]

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

Most of this labels indicate that we won't be taking an action on this specific issue. Usually means this issue will be closed very soon.

#### How often should it be used?
[1..n]

### Triaging

- Priority:0
- Priority:1
- Priority:2

Used for prioritizing purposes. 

#### How often should it be used?
[1]

### Sprint tracking

Sprint130
....
Sprint165

#### How often should it be used?
[1..n]

### Release Tracking 

- Preview1
- Preview2
- Preview3
- Preview4
- Preview5
- RTM

#### How often should it be used?
[1]

### CLA related labels

- cla-already-signed
- cla-not-required
- cla-required
- cla-signed

Used by the contribution bot. 

#### How often should it be used?
[1]

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

#### How often should it be used? 
[1..n]

#### Open Questions

- Should we merge Area:Test and Area:TestFailure?

- Should NuGet API be renamed to Area:SDK. Currently we don't have a way to distinguish between library APIs and Visual Studio contracts, IVs API, which are the only APIs we commit to 100% support. This change has a large footprint, as lots of issues have it.

- Do we expect the PMs to follow these guidelines?

#### Additional proposals 

A list of the proposals that go beyond issue labeling/categories, labeling additions etc. 

- The Area:BuildTask issues should be moved to the https://github.com/dotnet/NuGet.BuildTasks repo. Along those lines the label should be removed.

#### Not covered 

- The prioritization/planning part of the labeling is not covered here. It's being overhauled by @zkat. The specific categories of labels are still included for reference.

- Sprint labels cleanup. Refer to the above.