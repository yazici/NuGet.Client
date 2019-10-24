using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;

namespace NuGetConsole
{
    public class OutputService
    {
        public IServiceProvider ServiceProvider { get; }

        public OutputService([Import(typeof(SVsServiceProvider))]System.IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            // We may or may not be on the main thread. Do our main thread work on the main thread
            var initTask = MainThreadInitializationAsync(serviceProvider);
        }

        private async System.Threading.Tasks.Task MainThreadInitializationAsync(System.IServiceProvider serviceProvider)
        {
            await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Microsoft.VisualStudio.Workspace.VSIntegration
                
            IBrokeredServiceContainer serviceContainer = (IBrokeredServiceContainer)serviceProvider.GetService(typeof(SVsBrokeredServiceContainer));
            Assumes.Present(serviceContainer);
            IServiceBroker serviceBroker = serviceContainer.GetFullAccessServiceBroker();
            Assumes.NotNull(serviceBroker);
            _serviceBrokerClient = new ServiceBrokerClient(serviceBroker, ThreadHelper.JoinableTaskFactory);
            _serviceBrokerClient.Invalidated += AcquireServicesAsync;

            _refactoringFileAddedInfoBar = new InfoBarForRefactoringFileAdded(GetInfoBarForRefactoringFileAdded());

            _isVsHost = ErrorHandler.Succeeded(_shellService.GetProperty((int)__VSSPROPID11.VSSPROPID_ShellMode, out object shellModeObj))
                && Unbox.AsInt32(shellModeObj) == (int)__VSShellMode.VSSM_Server;
        }
    }
}
