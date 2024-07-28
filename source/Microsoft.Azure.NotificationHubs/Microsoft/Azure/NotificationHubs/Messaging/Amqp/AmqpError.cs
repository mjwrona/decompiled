// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.AmqpError
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp
{
  internal static class AmqpError
  {
    private const int MaxSizeInInfoMap = 32768;
    private static Dictionary<string, Error> errors;
    public static Error InternalError = new Error()
    {
      Condition = (AmqpSymbol) "amqp:internal-error"
    };
    public static Error NotFound = new Error()
    {
      Condition = (AmqpSymbol) "amqp:not-found"
    };
    public static Error UnauthorizedAccess = new Error()
    {
      Condition = (AmqpSymbol) "amqp:unauthorized-access"
    };
    public static Error DecodeError = new Error()
    {
      Condition = (AmqpSymbol) "amqp:decode-error"
    };
    public static Error ResourceLimitExceeded = new Error()
    {
      Condition = (AmqpSymbol) "amqp:resource-limit-exceeded"
    };
    public static Error NotAllowed = new Error()
    {
      Condition = (AmqpSymbol) "amqp:not-allowed"
    };
    public static Error InvalidField = new Error()
    {
      Condition = (AmqpSymbol) "amqp:invalid-field"
    };
    public static Error NotImplemented = new Error()
    {
      Condition = (AmqpSymbol) "amqp:not-implemented"
    };
    public static Error ResourceLocked = new Error()
    {
      Condition = (AmqpSymbol) "amqp:resource-locked"
    };
    public static Error PreconditionFailed = new Error()
    {
      Condition = (AmqpSymbol) "amqp:precondition-failed"
    };
    public static Error ResourceDeleted = new Error()
    {
      Condition = (AmqpSymbol) "amqp:resource-deleted"
    };
    public static Error IllegalState = new Error()
    {
      Condition = (AmqpSymbol) "amqp:illegal-state"
    };
    public static Error FrameSizeTooSmall = new Error()
    {
      Condition = (AmqpSymbol) "amqp:frame-size-too-small"
    };
    public static Error ConnectionForced = new Error()
    {
      Condition = (AmqpSymbol) "amqp:connection:forced"
    };
    public static Error FramingError = new Error()
    {
      Condition = (AmqpSymbol) "amqp:connection:framing-error"
    };
    public static Error ConnectionRedirect = new Error()
    {
      Condition = (AmqpSymbol) "amqp:connection:redirect"
    };
    public static Error WindowViolation = new Error()
    {
      Condition = (AmqpSymbol) "amqp:session:window-violation"
    };
    public static Error ErrantLink = new Error()
    {
      Condition = (AmqpSymbol) "amqp:session-errant-link"
    };
    public static Error HandleInUse = new Error()
    {
      Condition = (AmqpSymbol) "amqp:session:handle-in-use"
    };
    public static Error UnattachedHandle = new Error()
    {
      Condition = (AmqpSymbol) "amqp:session:unattached-handle"
    };
    public static Error DetachForced = new Error()
    {
      Condition = (AmqpSymbol) "amqp:link:detach-forced"
    };
    public static Error TransferLimitExceeded = new Error()
    {
      Condition = (AmqpSymbol) "amqp:link:transfer-limit-exceeded"
    };
    public static Error MessageSizeExceeded = new Error()
    {
      Condition = (AmqpSymbol) "amqp:link:message-size-exceeded"
    };
    public static Error LinkRedirect = new Error()
    {
      Condition = (AmqpSymbol) "amqp:link:redirect"
    };
    public static Error Stolen = new Error()
    {
      Condition = (AmqpSymbol) "amqp:link:stolen"
    };
    public static Error TransactionUnknownId = new Error()
    {
      Condition = (AmqpSymbol) "amqp:transaction:unknown-id"
    };
    public static Error TransactionRollback = new Error()
    {
      Condition = (AmqpSymbol) "amqp:transaction:rollback"
    };
    public static Error TransactionTimeout = new Error()
    {
      Condition = (AmqpSymbol) "amqp:transaction:timeout"
    };

    static AmqpError() => AmqpError.errors = new Dictionary<string, Error>()
    {
      {
        AmqpError.InternalError.Condition.Value,
        AmqpError.InternalError
      },
      {
        AmqpError.NotFound.Condition.Value,
        AmqpError.NotFound
      },
      {
        AmqpError.UnauthorizedAccess.Condition.Value,
        AmqpError.UnauthorizedAccess
      },
      {
        AmqpError.DecodeError.Condition.Value,
        AmqpError.DecodeError
      },
      {
        AmqpError.ResourceLimitExceeded.Condition.Value,
        AmqpError.ResourceLimitExceeded
      },
      {
        AmqpError.NotAllowed.Condition.Value,
        AmqpError.NotAllowed
      },
      {
        AmqpError.InvalidField.Condition.Value,
        AmqpError.InvalidField
      },
      {
        AmqpError.NotImplemented.Condition.Value,
        AmqpError.NotImplemented
      },
      {
        AmqpError.ResourceLocked.Condition.Value,
        AmqpError.ResourceLocked
      },
      {
        AmqpError.PreconditionFailed.Condition.Value,
        AmqpError.PreconditionFailed
      },
      {
        AmqpError.ResourceDeleted.Condition.Value,
        AmqpError.ResourceDeleted
      },
      {
        AmqpError.IllegalState.Condition.Value,
        AmqpError.IllegalState
      },
      {
        AmqpError.FrameSizeTooSmall.Condition.Value,
        AmqpError.FrameSizeTooSmall
      },
      {
        AmqpError.ConnectionForced.Condition.Value,
        AmqpError.ConnectionForced
      },
      {
        AmqpError.FramingError.Condition.Value,
        AmqpError.FramingError
      },
      {
        AmqpError.ConnectionRedirect.Condition.Value,
        AmqpError.ConnectionRedirect
      },
      {
        AmqpError.WindowViolation.Condition.Value,
        AmqpError.WindowViolation
      },
      {
        AmqpError.ErrantLink.Condition.Value,
        AmqpError.ErrantLink
      },
      {
        AmqpError.HandleInUse.Condition.Value,
        AmqpError.HandleInUse
      },
      {
        AmqpError.UnattachedHandle.Condition.Value,
        AmqpError.UnattachedHandle
      },
      {
        AmqpError.DetachForced.Condition.Value,
        AmqpError.DetachForced
      },
      {
        AmqpError.TransferLimitExceeded.Condition.Value,
        AmqpError.TransferLimitExceeded
      },
      {
        AmqpError.MessageSizeExceeded.Condition.Value,
        AmqpError.MessageSizeExceeded
      },
      {
        AmqpError.LinkRedirect.Condition.Value,
        AmqpError.LinkRedirect
      },
      {
        AmqpError.Stolen.Condition.Value,
        AmqpError.Stolen
      },
      {
        AmqpError.TransactionUnknownId.Condition.Value,
        AmqpError.TransactionUnknownId
      },
      {
        AmqpError.TransactionRollback.Condition.Value,
        AmqpError.TransactionRollback
      },
      {
        AmqpError.TransactionTimeout.Condition.Value,
        AmqpError.TransactionTimeout
      }
    };

    public static Error GetError(AmqpSymbol condition)
    {
      Error error = (Error) null;
      if (!AmqpError.errors.TryGetValue(condition.Value, out error))
        error = AmqpError.InternalError;
      return error;
    }

    public static Error FromException(Exception exception, bool includeDetail = true)
    {
      if (exception is AmqpException)
        return ((AmqpException) exception).Error;
      Error error = new Error();
      error.Condition = !(exception is UnauthorizedAccessException) ? (!(exception is InvalidOperationException) ? (!(exception is TransactionAbortedException) ? (!(exception is NotImplementedException) ? AmqpError.InternalError.Condition : AmqpError.NotImplemented.Condition) : AmqpError.TransactionRollback.Condition) : AmqpError.NotAllowed.Condition) : AmqpError.UnauthorizedAccess.Condition;
      error.Description = exception.Message;
      if (includeDetail)
      {
        error.Info = new Fields();
        string str = exception.ToString();
        if (str.Length > 32768)
          str = str.Substring(0, 32768);
        error.Info.Add((AmqpSymbol) nameof (exception), (object) str);
      }
      return error;
    }
  }
}
