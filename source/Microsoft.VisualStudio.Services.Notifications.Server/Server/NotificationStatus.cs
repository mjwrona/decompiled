// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationStatus
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class NotificationStatus
  {
    public static readonly string Delivered = nameof (Delivered);
    public static readonly string DeliveredSome = nameof (DeliveredSome);
    public static readonly string SkippedOk = nameof (SkippedOk);
    public static readonly string PendingSuccess = "PendSuccess";
    public static readonly string BadDeliveryDetails = "BadDetails";
    public static readonly string AccountErrorSubscriber = "ErrSubscriber";
    public static readonly string AccountErrorSubscriberBindPending = "ErrSubsPending";
    public static readonly string AccountErrorRecipient = "ErrRecipient";
    public static readonly string AccountErrorAuthenticator = "ErrAuthId";
    public static readonly string AccountErrorNoRecipients = "Err0Recipients";
    public static readonly string Filtered = nameof (Filtered);
    public static readonly string Exception = nameof (Exception);
    public static readonly string ContentFormatError = "ErrFormat";
    public static readonly string Disabled = nameof (Disabled);
    public static readonly string SkippedNoDeliveryAddress = "SkipNoAddr";
    public static readonly string BadEventData = nameof (BadEventData);
    public static readonly string SkippedNoPublisherEvents = "SkipNoPubEvts";
    public static readonly string SkippedOptedOut = "SkipOptedOut";
    public static readonly string DefaultFailure = nameof (DefaultFailure);
    public static readonly string DeliveryFailed = "Failed";
    public static readonly string ExceptionRetry = nameof (ExceptionRetry);
    public static readonly string Pending = nameof (Pending);
    public static readonly string ContributionInFallbackMode = "ContrFallback";
    public static readonly string TerminalFailure = nameof (TerminalFailure);
    public static readonly string NoMoreRetries = nameof (NoMoreRetries);
    public static readonly string InvalidBody = nameof (InvalidBody);
    public static readonly string InvalidSubject = nameof (InvalidSubject);
    private static Dictionary<SendNotificationState, string> s_stateNames = new Dictionary<SendNotificationState, string>()
    {
      {
        SendNotificationState.__DeliverySucceeded,
        NotificationStatus.Delivered
      },
      {
        SendNotificationState.DeliveredSome,
        NotificationStatus.DeliveredSome
      },
      {
        SendNotificationState.SkippedOk,
        NotificationStatus.SkippedOk
      },
      {
        SendNotificationState.PendingSuccess,
        NotificationStatus.PendingSuccess
      },
      {
        SendNotificationState.__DeliveryFailed,
        NotificationStatus.BadDeliveryDetails
      },
      {
        SendNotificationState.__DeliveryFailedNoAlerting,
        NotificationStatus.AccountErrorSubscriber
      },
      {
        SendNotificationState.AccountErrorSubscriberBindPending,
        NotificationStatus.AccountErrorSubscriberBindPending
      },
      {
        SendNotificationState.AccountErrorRecipient,
        NotificationStatus.AccountErrorRecipient
      },
      {
        SendNotificationState.AccountErrorAuthenticator,
        NotificationStatus.AccountErrorAuthenticator
      },
      {
        SendNotificationState.AccountErrorNoRecipients,
        NotificationStatus.AccountErrorNoRecipients
      },
      {
        SendNotificationState.Filtered,
        NotificationStatus.Filtered
      },
      {
        SendNotificationState.Exception,
        NotificationStatus.Exception
      },
      {
        SendNotificationState.ContentFormatError,
        NotificationStatus.ContentFormatError
      },
      {
        SendNotificationState.Disabled,
        NotificationStatus.Disabled
      },
      {
        SendNotificationState.SkippedNoDeliveryAddress,
        NotificationStatus.SkippedNoDeliveryAddress
      },
      {
        SendNotificationState.BadEventData,
        NotificationStatus.BadEventData
      },
      {
        SendNotificationState.SkippedNoPublisherEvents,
        NotificationStatus.SkippedNoPublisherEvents
      },
      {
        SendNotificationState.SkippedOptedOut,
        NotificationStatus.SkippedOptedOut
      },
      {
        SendNotificationState.__DeliveryFailedCanRetry,
        NotificationStatus.DefaultFailure
      },
      {
        SendNotificationState.DeliveryFailed,
        NotificationStatus.DeliveryFailed
      },
      {
        SendNotificationState.ExceptionRetry,
        NotificationStatus.ExceptionRetry
      },
      {
        SendNotificationState.Pending,
        NotificationStatus.Pending
      },
      {
        SendNotificationState.ContributionInFallbackMode,
        NotificationStatus.ContributionInFallbackMode
      },
      {
        SendNotificationState.TerminalFailure,
        NotificationStatus.TerminalFailure
      },
      {
        SendNotificationState.NoMoreRetries,
        NotificationStatus.NoMoreRetries
      },
      {
        SendNotificationState.InvalidBody,
        NotificationStatus.InvalidBody
      },
      {
        SendNotificationState.InvalidSubject,
        NotificationStatus.InvalidSubject
      }
    };

    public static bool IsFailureState(this SendNotificationState state) => state >= SendNotificationState.__DeliveryFailed;

    public static bool IsFailedRetryState(this SendNotificationState state) => state >= SendNotificationState.__DeliveryFailedCanRetry;

    public static bool IsFailedNoAlertingState(this SendNotificationState state) => state >= SendNotificationState.__DeliveryFailedNoAlerting && state < SendNotificationState.__DeliveryFailedCanRetry;

    public static bool IsFailedAlertingState(this SendNotificationState state) => state >= SendNotificationState.__DeliveryFailed && state < SendNotificationState.__DeliveryFailedNoAlerting;

    public static bool IsSuccessState(this SendNotificationState state) => !state.IsFailureState();

    public static string GetUnmappedState(this SendNotificationState state) => string.Format("Unmapped{0}", (object) (int) state);

    private static string GetStateName(this SendNotificationState state)
    {
      string unmappedState;
      if (!NotificationStatus.s_stateNames.TryGetValue(state, out unmappedState))
        unmappedState = state.GetUnmappedState();
      return unmappedState;
    }

    public static string GetDbStateName(this SendNotificationState state) => state.GetStateName();

    public static string GetCiStateName(this SendNotificationState state) => state.GetStateName();

    public static void Increment(
      this Dictionary<SendNotificationState, int> dict,
      SendNotificationState state,
      int increment = 1)
    {
      int num;
      dict[state] = dict.TryGetValue(state, out num) ? num + increment : increment;
    }
  }
}
