// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.MessagingExceptionHelper
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Tracing;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  internal static class MessagingExceptionHelper
  {
    private static readonly Dictionary<string, HttpStatusCode> ErrorCodes = new Dictionary<string, HttpStatusCode>()
    {
      {
        typeof (TimeoutException).UnderlyingSystemType.Name,
        HttpStatusCode.RequestTimeout
      },
      {
        MessagingExceptionDetail.ErrorLevelType.ServerError.ToString(),
        HttpStatusCode.InternalServerError
      },
      {
        MessagingExceptionDetail.ErrorLevelType.UserError.ToString(),
        HttpStatusCode.Unauthorized
      },
      {
        typeof (ArgumentException).UnderlyingSystemType.Name,
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentOutOfRangeException).UnderlyingSystemType.Name,
        HttpStatusCode.BadRequest
      },
      {
        typeof (MessagingEntityNotFoundException).UnderlyingSystemType.Name,
        HttpStatusCode.NotFound
      },
      {
        typeof (MessagingEntityAlreadyExistsException).UnderlyingSystemType.Name,
        HttpStatusCode.Conflict
      },
      {
        typeof (UnauthorizedAccessException).UnderlyingSystemType.Name,
        HttpStatusCode.Unauthorized
      },
      {
        typeof (TransactionSizeExceededException).UnderlyingSystemType.Name,
        HttpStatusCode.RequestEntityTooLarge
      },
      {
        typeof (QuotaExceededException).UnderlyingSystemType.Name,
        HttpStatusCode.BadRequest
      },
      {
        typeof (RequestQuotaExceededException).UnderlyingSystemType.Name,
        HttpStatusCode.BadRequest
      },
      {
        typeof (ServerBusyException).UnderlyingSystemType.Name,
        HttpStatusCode.ServiceUnavailable
      },
      {
        typeof (InvalidOperationException).UnderlyingSystemType.Name,
        HttpStatusCode.BadRequest
      },
      {
        typeof (EndpointNotFoundException).UnderlyingSystemType.Name,
        HttpStatusCode.InternalServerError
      },
      {
        typeof (MessagingEntityDisabledException).UnderlyingSystemType.Name,
        HttpStatusCode.Forbidden
      },
      {
        typeof (MessagingException).UnderlyingSystemType.Name,
        HttpStatusCode.InternalServerError
      },
      {
        typeof (MessagingCommunicationException).UnderlyingSystemType.Name,
        HttpStatusCode.InternalServerError
      },
      {
        typeof (InternalServerErrorException).UnderlyingSystemType.Name,
        HttpStatusCode.InternalServerError
      },
      {
        typeof (InvalidLinkTypeException).UnderlyingSystemType.Name,
        HttpStatusCode.BadRequest
      }
    };

    public static CommunicationException ConvertToCommunicationException(
      MessagingException exception)
    {
      return MessagingExceptionHelper.ConvertToCommunicationException(exception, out bool _);
    }

    public static CommunicationException ConvertToCommunicationException(
      MessagingException exception,
      out bool shouldFault)
    {
      shouldFault = false;
      if (exception is MessagingEntityNotFoundException)
      {
        shouldFault = true;
        return (CommunicationException) new EndpointNotFoundException(exception.Message, (Exception) exception);
      }
      if (exception is MessagingCommunicationException && exception.InnerException is EndpointNotFoundException innerException)
      {
        shouldFault = true;
        return (CommunicationException) innerException;
      }
      if (!(exception.InnerException is CommunicationException communicationException))
        communicationException = new CommunicationException(exception.Message, (Exception) exception);
      return communicationException;
    }

    public static Exception Unwrap(CommunicationException exception, bool isCancelling)
    {
      if (!isCancelling)
        return MessagingExceptionHelper.Unwrap(exception);
      return exception is CommunicationObjectAbortedException ? (Exception) new OperationCanceledException(SRClient.EntityClosedOrAborted, (Exception) exception) : (Exception) new OperationCanceledException(SRClient.EntityClosedOrAborted, MessagingExceptionHelper.Unwrap(exception));
    }

    public static HttpStatusCode ConvertStatusCodeFromDetail(string type)
    {
      HttpStatusCode httpStatusCode;
      if (MessagingExceptionHelper.ErrorCodes.TryGetValue(type, out httpStatusCode))
        return httpStatusCode;
      MessagingClientEtwProvider.Provider.EventWriteUnexpectedExceptionTelemetry(type);
      return HttpStatusCode.InternalServerError;
    }

    public static Exception Unwrap(CommunicationException exception)
    {
      switch (exception)
      {
        case null:
          return (Exception) null;
        case FaultException faultException:
          return MessagingExceptionHelper.Unwrap(faultException);
        case EndpointNotFoundException innerException:
          return (Exception) new MessagingCommunicationException(innerException.Message, (Exception) innerException);
        case CommunicationObjectAbortedException _:
          return (Exception) new OperationCanceledException(SRClient.EntityClosedOrAborted, (Exception) exception);
        case CommunicationObjectFaultedException _:
          return (Exception) new MessagingCommunicationException(SRClient.MessagingCommunicationError, (Exception) exception);
        default:
          return (Exception) new MessagingCommunicationException(exception.Message, (Exception) exception);
      }
    }

    public static bool IsWrappedExceptionTransient(this Exception exception)
    {
      bool flag;
      if (exception is CommunicationException exception1)
      {
        switch (MessagingExceptionHelper.Unwrap(exception1))
        {
          case MessagingException messagingException:
            flag = messagingException.IsTransient;
            break;
          case TimeoutException _:
            flag = true;
            break;
          default:
            flag = false;
            break;
        }
      }
      else
        flag = false;
      return flag;
    }

    private static Exception Unwrap(FaultException faultException)
    {
      switch (faultException)
      {
        case FaultException<ExceptionDetailNoStackTrace> exceptionDetailFaultException1 when exceptionDetailFaultException1.Detail != null:
          return MessagingExceptionHelper.ConvertExceptionFromDetail(exceptionDetailFaultException1.Detail.Type, (FaultException) exceptionDetailFaultException1);
        case FaultException<ExceptionDetail> exceptionDetailFaultException2 when exceptionDetailFaultException2.Detail != null:
          return MessagingExceptionHelper.ConvertExceptionFromDetail(exceptionDetailFaultException2.Detail.Type, (FaultException) exceptionDetailFaultException2);
        default:
          return MessagingExceptionHelper.ConvertExceptionFromStatusCode(faultException);
      }
    }

    private static Exception ConvertExceptionFromStatusCode(FaultException faultException)
    {
      if (faultException.Code.SubCode == null)
        return (Exception) new MessagingException(faultException.Message, (Exception) faultException);
      if (faultException.Code.SubCode.Name.Equals("ConnectionFailedFault", StringComparison.OrdinalIgnoreCase))
        return (Exception) new TimeoutException(faultException.Message, (Exception) faultException);
      if (faultException.Code.SubCode.Name.Equals("EndpointNotFoundFault", StringComparison.OrdinalIgnoreCase))
        return (Exception) new MessagingEntityNotFoundException(faultException.Message, (Exception) faultException);
      if (faultException.Code.SubCode.Name.Equals("AuthorizationFailedFault", StringComparison.OrdinalIgnoreCase) || faultException.Code.SubCode.Name.Equals("NoTransportSecurityFault", StringComparison.OrdinalIgnoreCase))
        return (Exception) new UnauthorizedAccessException(faultException.Message, (Exception) faultException);
      return faultException.Code.SubCode.Name.Equals("QuotaExceededFault", StringComparison.OrdinalIgnoreCase) ? (Exception) new QuotaExceededException(faultException.Message, (Exception) faultException) : (Exception) new MessagingException(faultException.Message, (Exception) faultException);
    }

    private static Exception ConvertExceptionFromDetail(
      string type,
      FaultException exceptionDetailFaultException)
    {
      if (string.Equals(type, typeof (TimeoutException).FullName, StringComparison.Ordinal))
        return (Exception) new TimeoutException(exceptionDetailFaultException.Message, (Exception) exceptionDetailFaultException);
      if (string.Equals(type, typeof (ArgumentException).FullName, StringComparison.Ordinal))
        return (Exception) new ArgumentException(exceptionDetailFaultException.Message, (Exception) exceptionDetailFaultException);
      if (string.Equals(type, typeof (ArgumentOutOfRangeException).FullName, StringComparison.Ordinal))
        return (Exception) new ArgumentOutOfRangeException(exceptionDetailFaultException.Message, (Exception) exceptionDetailFaultException);
      if (string.Equals(type, typeof (MessagingEntityNotFoundException).FullName, StringComparison.Ordinal))
        return (Exception) new MessagingEntityNotFoundException(exceptionDetailFaultException.Message, (Exception) exceptionDetailFaultException);
      if (string.Equals(type, typeof (MessagingEntityAlreadyExistsException).FullName, StringComparison.Ordinal))
        return (Exception) new MessagingEntityAlreadyExistsException(exceptionDetailFaultException.Message, (TrackingContext) null, (Exception) exceptionDetailFaultException);
      if (string.Equals(type, typeof (UnauthorizedAccessException).FullName, StringComparison.Ordinal))
        return (Exception) new UnauthorizedAccessException(exceptionDetailFaultException.Message, (Exception) exceptionDetailFaultException);
      if (string.Equals(type, typeof (TransactionSizeExceededException).FullName, StringComparison.Ordinal))
        return (Exception) new TransactionSizeExceededException(exceptionDetailFaultException.Message, (Exception) exceptionDetailFaultException);
      if (string.Equals(type, typeof (QuotaExceededException).FullName, StringComparison.Ordinal))
        return (Exception) new QuotaExceededException(exceptionDetailFaultException.Message, (Exception) exceptionDetailFaultException);
      if (string.Equals(type, typeof (ServerBusyException).FullName, StringComparison.Ordinal))
        return (Exception) new ServerBusyException(exceptionDetailFaultException.Message, (Exception) exceptionDetailFaultException);
      if (string.Equals(type, typeof (InvalidOperationException).FullName, StringComparison.Ordinal))
        return (Exception) new InvalidOperationException(exceptionDetailFaultException.Message.Replace("BR0012", string.Empty), (Exception) exceptionDetailFaultException);
      if (string.Equals(type, typeof (EndpointNotFoundException).FullName, StringComparison.Ordinal))
        return (Exception) new MessagingException(exceptionDetailFaultException.Message, (Exception) exceptionDetailFaultException);
      if (string.Equals(type, typeof (MessagingEntityDisabledException).FullName, StringComparison.Ordinal))
        return (Exception) new MessagingEntityDisabledException(exceptionDetailFaultException.Message, (Exception) exceptionDetailFaultException);
      if (string.Equals(type, typeof (MessagingCommunicationException).FullName, StringComparison.Ordinal))
        return (Exception) new MessagingCommunicationException(exceptionDetailFaultException.Message, (Exception) exceptionDetailFaultException);
      if (string.Equals(type, typeof (InternalServerErrorException).FullName, StringComparison.Ordinal))
        return (Exception) new MessagingException(exceptionDetailFaultException.Message, false, (Exception) exceptionDetailFaultException);
      return string.Equals(type, typeof (InvalidLinkTypeException).FullName, StringComparison.Ordinal) ? (Exception) new MessagingException(exceptionDetailFaultException.Message, false, (Exception) exceptionDetailFaultException) : (Exception) new MessagingException(exceptionDetailFaultException.Message, (Exception) exceptionDetailFaultException);
    }
  }
}
