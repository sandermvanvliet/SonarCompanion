using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using SonarCompanion_VSIntegration.MessageBus;
using SonarCompanion_VSIntegration.MessageBus.Messages;

namespace SonarCompanion_VSIntegration.Interop
{
    public class SolutionEventsSink : IVsSolutionEvents, IDisposable
    {
        private readonly IMessageBus _messageBus;
        private readonly IVsSolution _solution;
        private uint _cookie;
        private bool _isDisposed;

        public SolutionEventsSink(IMessageBus messageBus, IVsSolution solution)
        {
            _messageBus = messageBus;
            _solution = solution;
            solution.AdviseSolutionEvents(this, out _cookie);
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            _messageBus.Push(new SolutionLoaded(null));

            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            _messageBus.Push(new SolutionClosed());

            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                if (_cookie != 0)
                {
                    _solution.UnadviseSolutionEvents(_cookie);
                    _cookie = 0;
                }
            }
        }
    }
}