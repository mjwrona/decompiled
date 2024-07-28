// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SendNotificationState
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public enum SendNotificationState
  {
    Delivered = 0,
    __DeliverySucceeded = 0,
    DeliveredSome = 1,
    SkippedOk = 2,
    PendingSuccess = 3,
    BadDeliveryDetails = 1000, // 0x000003E8
    __DeliveryFailed = 1000, // 0x000003E8
    Exception = 1001, // 0x000003E9
    ContentFormatError = 1002, // 0x000003EA
    BadEventData = 1003, // 0x000003EB
    TerminalFailure = 1004, // 0x000003EC
    NoMoreRetries = 1005, // 0x000003ED
    InvalidBody = 1006, // 0x000003EE
    InvalidSubject = 1007, // 0x000003EF
    AccountErrorSubscriber = 2000, // 0x000007D0
    __DeliveryFailedNoAlerting = 2000, // 0x000007D0
    AccountErrorSubscriberBindPending = 2001, // 0x000007D1
    AccountErrorRecipient = 2002, // 0x000007D2
    AccountErrorAuthenticator = 2003, // 0x000007D3
    AccountErrorNoRecipients = 2004, // 0x000007D4
    Filtered = 2005, // 0x000007D5
    Disabled = 2006, // 0x000007D6
    SkippedNoDeliveryAddress = 2007, // 0x000007D7
    SkippedNoPublisherEvents = 2008, // 0x000007D8
    SkippedOptedOut = 2009, // 0x000007D9
    DefaultFailure = 3000, // 0x00000BB8
    __DeliveryFailedCanRetry = 3000, // 0x00000BB8
    DeliveryFailed = 3001, // 0x00000BB9
    ExceptionRetry = 3002, // 0x00000BBA
    Pending = 3003, // 0x00000BBB
    ContributionInFallbackMode = 3004, // 0x00000BBC
    __Last = 3005, // 0x00000BBD
  }
}
