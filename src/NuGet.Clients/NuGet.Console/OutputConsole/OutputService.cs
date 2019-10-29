using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.ServiceHub.Framework;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.RpcContracts.OutputChannel;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;
using TPLTask = System.Threading.Tasks.Task;

namespace NuGetConsole
{
    public class OutputService
    {
        public ServiceBrokerClient ServiceBrokerClient { get; private set; }
        private ServiceBrokerClient.Rental<IOutputChannelStore>? _channelStoreRental;

        [Import(typeof(SVsServiceProvider))]
        public IServiceProvider ServiceProvider { get; set; }

        public OutputService()
        {
            // We may or may not be on the main thread. Do our main thread work on the main thread
            var initTask = MainThreadInitializationAsync(ServiceProvider);
        }

        private async System.Threading.Tasks.Task MainThreadInitializationAsync(System.IServiceProvider serviceProvider)
        {
            Assumes.Present(serviceProvider);

            await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            
            IBrokeredServiceContainer serviceContainer = (IBrokeredServiceContainer)serviceProvider.GetService(typeof(SVsBrokeredServiceContainer));
            Assumes.Present(serviceContainer);

            IServiceBroker serviceBroker = serviceContainer.GetFullAccessServiceBroker();
            Assumes.NotNull(serviceBroker);

            ServiceBrokerClient = new ServiceBrokerClient(serviceBroker, ThreadHelper.JoinableTaskFactory);
            ServiceBrokerClient.Invalidated += AcquireServicesAsync;

            //Don't think we need this?
            //_refactoringFileAddedInfoBar = new InfoBarForRefactoringFileAdded(GetInfoBarForRefactoringFileAdded());

            //We might need this
            //_isVsHost = ErrorHandler.Succeeded(_shellService.GetProperty((int)__VSSPROPID11.VSSPROPID_ShellMode, out object shellModeObj))
              //  && Unbox.AsInt32(shellModeObj) == (int)__VSShellMode.VSSM_Server;
        }

        private async TPLTask AcquireServicesAsync(ServiceBrokerClient sender, BrokeredServicesChangedEventArgs args, CancellationToken cancellationToken)
        {
            await AcquireServicesWithSemaphoreAsync(cancellationToken);
        }

        internal async TPLTask AcquireServicesWithSemaphoreAsync(CancellationToken cancellationToken)
        {
            if (_channelStoreRental.HasValue)
            {
                _channelStoreRental.Value.Dispose();
            }

            _channelStoreRental = await ServiceBrokerClient.GetProxyAsync<IOutputChannelStore>(
                VisualStudioServices.VS2019_4.OutputChannelStore,
                cancellationToken);
            Assumes.Present(_channelStoreRental.Value.Proxy);
        }

        public void Write(string s)
        {
            _channelStoreRental.Value.Proxy?.WriteAsync(Resources.OutputConsolePaneName, s);
        }

    }
}
