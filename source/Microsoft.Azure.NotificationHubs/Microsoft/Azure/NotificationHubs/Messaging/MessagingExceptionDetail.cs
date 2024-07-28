// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.MessagingExceptionDetail
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  [Serializable]
  public sealed class MessagingExceptionDetail
  {
    private MessagingExceptionDetail(int errorCode, string message)
      : this(errorCode, message, MessagingExceptionDetail.ErrorLevelType.UserError)
    {
    }

    private MessagingExceptionDetail(
      int errorCode,
      string message,
      MessagingExceptionDetail.ErrorLevelType errorLevel)
    {
      this.ErrorCode = errorCode;
      this.Message = message;
      this.ErrorLevel = errorLevel;
    }

    public int ErrorCode { get; private set; }

    public string Message { get; private set; }

    public MessagingExceptionDetail.ErrorLevelType ErrorLevel { get; private set; }

    public static MessagingExceptionDetail UnknownDetail(string message) => new MessagingExceptionDetail(60000, message);

    public static MessagingExceptionDetail EntityGone(string message) => new MessagingExceptionDetail(41000, message);

    public static MessagingExceptionDetail EntityNotFound(string message) => new MessagingExceptionDetail(40400, message);

    public static MessagingExceptionDetail EntityConflict(string message) => new MessagingExceptionDetail(40900, message);

    public static MessagingExceptionDetail ServerBusy(string message) => new MessagingExceptionDetail(50004, message, MessagingExceptionDetail.ErrorLevelType.ServerError);

    public static MessagingExceptionDetail StoreLockLost(string message) => new MessagingExceptionDetail(40500, message);

    public static MessagingExceptionDetail UnspecifiedInternalError(string message) => new MessagingExceptionDetail(50000, message, MessagingExceptionDetail.ErrorLevelType.ServerError);

    public static MessagingExceptionDetail SqlFiltersExceeded(string message) => new MessagingExceptionDetail(40501, message);

    public static MessagingExceptionDetail CorrelationFiltersExceeded(string message) => new MessagingExceptionDetail(40502, message);

    public static MessagingExceptionDetail SubscriptionsExceeded(string message) => new MessagingExceptionDetail(40503, message);

    public static MessagingExceptionDetail EventHubAtFullCapacity(string message) => new MessagingExceptionDetail(40505, message, MessagingExceptionDetail.ErrorLevelType.ServerError);

    public static MessagingExceptionDetail EntityUpdateConflict(string entityName) => new MessagingExceptionDetail(40504, SRClient.MessagingEntityUpdateConflict((object) entityName));

    public static MessagingExceptionDetail EntityConflictOperationInProgress(string entityName) => new MessagingExceptionDetail(40901, SRClient.MessagingEntityRequestConflict((object) entityName));

    public static MessagingExceptionDetail ReconstructExceptionDetail(
      int errorCode,
      string message,
      MessagingExceptionDetail.ErrorLevelType errorLevel)
    {
      return new MessagingExceptionDetail(errorCode, message, errorLevel);
    }

    public enum ErrorLevelType
    {
      UserError,
      ServerError,
    }
  }
}
