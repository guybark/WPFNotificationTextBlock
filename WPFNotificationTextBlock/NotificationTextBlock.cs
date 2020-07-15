using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace WPFNotificationTextBlock
{
    // This demo snippet was run with a NotificationTextBlock added
    // to the app's XAML using <local:NotificationTextBlock>.
    internal class NotificationTextBlock : TextBlock
    {
        // This control's AutomationPeer is the object that actually raises 
        // the UIA Notification event.
        private NotificationTextBlockAutomationPeer peer;

        // Assume the UIA Notification event is available until we learn otherwise.
        // If we learn that the UIA Notification event is not available, no instance 
        // of the NotificationTextBlock should attempt to raise it.
        static private bool notificationEventAvailable = true;

        public bool NotificationEventAvailable
        {
            get => notificationEventAvailable;
            set => notificationEventAvailable = value;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            this.peer = new NotificationTextBlockAutomationPeer(this);

            return this.peer;
        }

        public void RaiseNotificationEvent(
            string notificationText,
            string notificationId)
        {
            // Only attempt to raise the event if we already 
            // have an AutomationPeer.
            if (this.peer != null)
            {
                this.peer.RaiseNotificationEvent(notificationText, notificationId);
            }
        }

        private class NotificationTextBlockAutomationPeer :
            TextBlockAutomationPeer
        {
            private NotificationTextBlock notificationTextBlock;

            // The UIA Notification event requires the IRawElementProviderSimple 
            // associated with this AutomationPeer.
            private IRawElementProviderSimple reps;

            public NotificationTextBlockAutomationPeer(
                NotificationTextBlock owner) : base(owner)
            {
                this.notificationTextBlock = owner;
            }

            public void RaiseNotificationEvent(
                string notificationText,
                string notificationId)
            {
                // If we already know that the UIA Notification is not available, 
                // do not attempt to raise it.
                if (this.notificationTextBlock.NotificationEventAvailable)
                {
                    // Get the IRawElementProviderSimple for this 
                    // AutomationPeer if we don't have it already.
                    if (this.reps == null)
                    {
                        AutomationPeer peer =
                            FrameworkElementAutomationPeer.FromElement(
                                this.notificationTextBlock);
                        if (peer != null)
                        {
                            this.reps = ProviderFromPeer(peer);
                        }
                    }

                    if (this.reps != null)
                    {
                        try
                        {
                            // IMPORTANT: The NotificationKind and NotificationProcessing 
                            // values shown  here are sample values for the snippet. You should 
                            // use whatever values are appropriate for your scenarios.

                            Debug.WriteLine("NotificationTextBlock: Raise UIA Notification event with " +
                                "\"" + notificationText + "\"");

                            NativeMethods.UiaRaiseNotificationEvent(
                                this.reps,
                                /* SAMPLE */ NativeMethods.NotificationKind.NotificationKind_ActionCompleted,
                                /* SAMPLE */ NativeMethods.NotificationProcessing.NotificationProcessing_All,
                                notificationText,
                                notificationId);
                        }
                        catch (EntryPointNotFoundException)
                        {
                            // The UIA Notification event is not available, so don't attempt
                            // to raise it again.
                            notificationTextBlock.NotificationEventAvailable = false;
                        }
                    }
                }
            }
        }
    }

    internal class NativeMethods
    {
        // Documentation on NotificationProcessing is at:
        // https://docs.microsoft.com/en-us/windows/win32/api/uiautomationcore/ne-uiautomationcore-notificationprocessing
        public enum NotificationProcessing
        {
            NotificationProcessing_ImportantAll,
            NotificationProcessing_ImportantMostRecent,
            NotificationProcessing_All,
            NotificationProcessing_MostRecent,
            NotificationProcessing_CurrentThenMostRecent
        };


        // Documentation on NotificationKind is at:
        // https://docs.microsoft.com/en-us/windows/win32/api/uiautomationcore/ne-uiautomationcore-notificationkind
        public enum NotificationKind
        {
            NotificationKind_ItemAdded,
            NotificationKind_ItemRemoved,
            NotificationKind_ActionCompleted,
            NotificationKind_ActionAborted,
            NotificationKind_Other
        };


        // Documentation on UiaRaiseNotificationEvent is at:
        // https://docs.microsoft.com/en-us/windows/win32/api/uiautomationcoreapi/nf-uiautomationcoreapi-uiaraisenotificationevent
        [DllImport("UIAutomationCore.dll",
            EntryPoint = "UiaRaiseNotificationEvent", CharSet = CharSet.Unicode)]
        public static extern int UiaRaiseNotificationEvent(
            IRawElementProviderSimple provider,
            NotificationKind notificationKind,
            NotificationProcessing notificationProcessing,
            string notificationText,
            string notificationId);
    }
}
