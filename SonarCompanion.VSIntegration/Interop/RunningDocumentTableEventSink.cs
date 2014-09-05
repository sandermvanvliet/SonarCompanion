using System;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using SonarCompanion_VSIntegration.MessageBus;
using SonarCompanion_VSIntegration.MessageBus.Messages;
using SonarCompanion_VSIntegration.Services;

namespace SonarCompanion_VSIntegration.Interop
{
    public class RunningDocumentTableEventSink : IVsRunningDocTableEvents, IDisposable
    {
        private uint _cookie;
        private bool _isDisposed;
        private readonly IVsRunningDocumentTable _runningDocumentTable;
        private readonly IMessageBus _messageBus;

        public RunningDocumentTableEventSink(IVsRunningDocumentTable runningDocumentTable, IMessageBus messageBus)
        {
            _runningDocumentTable = runningDocumentTable;
            _messageBus = messageBus;
            _runningDocumentTable.AdviseRunningDocTableEvents(this, out _cookie);
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining,
            uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining,
            uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            // Figure out which document this is
            uint pgrfRDTFlags;
            uint pdwReadLocks;
            uint pdwEditLocks;
            string pbstrMkDocument;
            IVsHierarchy ppHier;
            uint pitemid;
            IntPtr ppunkDocData;

            _runningDocumentTable.GetDocumentInfo(docCookie, out pgrfRDTFlags, out pdwReadLocks, out pdwEditLocks,
                out pbstrMkDocument, out ppHier, out pitemid, out ppunkDocData);

            // If it is a settings file queue a message
            if (Path.GetFileName(pbstrMkDocument) == SettingsService.SettingsFileName)
            {
                _messageBus.Push(new SettingsFileChanged());
            }

            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
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
                    _runningDocumentTable.UnadviseRunningDocTableEvents(_cookie);
                    _cookie = 0;
                }
            }
        }
    }
}