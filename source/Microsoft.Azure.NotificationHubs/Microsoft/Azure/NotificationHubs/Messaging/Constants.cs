// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Constants
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  internal static class Constants
  {
    public const bool DefaultIsAnonymousAccessible = false;
    public static readonly TimeSpan DefaultOperationTimeout = TimeSpan.FromMinutes(1.0);
    public static readonly TimeSpan MaxOperationTimeout = TimeSpan.FromDays(1.0);
    public static readonly TimeSpan TokenRequestOperationTimeout = TimeSpan.FromMinutes(3.0);
    public const string DefaultOperationTimeoutString = "0.00:01:00.00";
    public static readonly int ServicePointMaxIdleTimeMilliSeconds = 50000;
    public static readonly int MaximumTagSize = 120;
    public static readonly int DefaultMaxDeliveryCount = 10;
    public static readonly TimeSpan DefaultRegistrationTtl = TimeSpan.MaxValue;
    public static readonly TimeSpan MinimumRegistrationTtl = TimeSpan.FromDays(1.0);
    public const string NotificationHub = "NotificationHub";
    public const string AuthClaimType = "net.windows.servicebus.action";
    public const string ManageClaim = "Manage";
    public const string SendClaim = "Send";
    public const string ListenClaim = "Listen";
    public static List<string> SupportedClaims = new List<string>()
    {
      "Manage",
      "Send",
      "Listen"
    };
    public const int SupportedClaimsCount = 3;
    public const string ClaimSeparator = ",";
    public const string PathDelimiter = "/";
    public const string SubQueuePrefix = "$";
    public const string EntityDelimiter = "|";
    public const string EmptyEntityDelimiter = "||";
    public const string ColonDelimiter = ":";
    public const string PartDelimiter = ":";
    public const string IsAnonymousAccessibleHeader = "X-MS-ISANONYMOUSACCESSIBLE";
    public const string ServiceBusSupplementartyAuthorizationHeaderName = "ServiceBusSupplementaryAuthorization";
    public const string ServiceBusDlqSupplementaryAuthorizationHeaderName = "ServiceBusDlqSupplementaryAuthorization";
    public const string ContinuationTokenHeaderName = "x-ms-continuationtoken";
    public const string ContinuationTokenQueryName = "continuationtoken";
    public const int MaxJobIdLength = 128;
    public const int NotificationHubNameMaximumLength = 260;
    public const string BrokerInvalidOperationPrefix = "BR0012";
    public const string InternalServiceFault = "InternalServiceFault";
    public const string ConnectionFailedFault = "ConnectionFailedFault";
    public const string EndpointNotFoundFault = "EndpointNotFoundFault";
    public const string AuthorizationFailedFault = "AuthorizationFailedFault";
    public const string NoTransportSecurityFault = "NoTransportSecurityFault";
    public const string QuotaExceededFault = "QuotaExceededFault";
    public const string PartitionNotOwnedFault = "PartitionNotOwnedException";
    public const string UndeterminableExceptionType = "UndeterminableExceptionType";
    public const string InvalidOperationFault = "InvalidOperationFault";
    public const string SessionLockLostFault = "SessionLockLostFault";
    public const string TimeoutFault = "TimeoutFault";
    public const string ArgumentFault = "ArgumentFault";
    public const string MessagingEntityDisabledFault = "MessagingEntityDisabledFault";
    public const string ServerBusyFault = "ServerBusyFault";
    public const int MaximumUserMetadataLength = 1024;
    public const int MaxNotificationHubPathLength = 290;
    public static readonly TimeSpan DefaultRetryMinBackoff = TimeSpan.FromSeconds(0.0);
    public static readonly TimeSpan DefaultRetryMaxBackoff = TimeSpan.FromSeconds(30.0);
    public static readonly TimeSpan DefaultRetryDeltaBackoff = TimeSpan.FromSeconds(3.0);
    public static readonly TimeSpan DefaultRetryTerminationBuffer = TimeSpan.FromSeconds(5.0);
    public const string HttpErrorSubCodeFormatString = "SubCode={0}";
    public const string HttpLocationHeaderName = "Location";
    public const bool DefaultHubIsDisabled = false;
  }
}
