// Decompiled with JetBrains decompiler
// Type: Microsoft.ServiceBus.Tracing.EventDefinitionResources
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Resources;

namespace Microsoft.ServiceBus.Tracing
{
  internal class EventDefinitionResources
  {
    private static ResourceManager resourceManager;
    private static CultureInfo resourceCulture;

    private EventDefinitionResources()
    {
    }

    internal static ResourceManager ResourceManager
    {
      get
      {
        if (EventDefinitionResources.resourceManager == null)
          EventDefinitionResources.resourceManager = new ResourceManager("Microsoft.ServiceBus.Tracing.EventDefinitionResources", typeof (EventDefinitionResources).Assembly);
        return EventDefinitionResources.resourceManager;
      }
    }

    [GeneratedCode("StrictResXFileCodeGenerator", "4.0.0.0")]
    internal static CultureInfo Culture
    {
      get => EventDefinitionResources.resourceCulture;
      set => EventDefinitionResources.resourceCulture = value;
    }

    internal static string keyword_Powershell => EventDefinitionResources.ResourceManager.GetString(nameof (keyword_Powershell), EventDefinitionResources.Culture);

    internal static string keyword_Host => EventDefinitionResources.ResourceManager.GetString(nameof (keyword_Host), EventDefinitionResources.Culture);

    internal static string keyword_Gateway => EventDefinitionResources.ResourceManager.GetString(nameof (keyword_Gateway), EventDefinitionResources.Culture);

    internal static string keyword_Client => EventDefinitionResources.ResourceManager.GetString(nameof (keyword_Client), EventDefinitionResources.Culture);

    internal static string event_MessageSendingTransfer => EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageSendingTransfer), EventDefinitionResources.Culture);

    internal static string event_SetStateTransfer => EventDefinitionResources.ResourceManager.GetString(nameof (event_SetStateTransfer), EventDefinitionResources.Culture);

    internal static string event_GetStateTransfer => EventDefinitionResources.ResourceManager.GetString(nameof (event_GetStateTransfer), EventDefinitionResources.Culture);

    internal static string event_MessageReceiveTransfer => EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageReceiveTransfer), EventDefinitionResources.Culture);

    internal static string event_MessagePeekTransfer => EventDefinitionResources.ResourceManager.GetString(nameof (event_MessagePeekTransfer), EventDefinitionResources.Culture);

    internal static string event_RenewSessionLockTransfer => EventDefinitionResources.ResourceManager.GetString(nameof (event_RenewSessionLockTransfer), EventDefinitionResources.Culture);

    internal static string event_RelayChannelConnectionTransfer => EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayChannelConnectionTransfer), EventDefinitionResources.Culture);

    internal static string event_HybridConnectionManagerStarting => EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionManagerStarting), EventDefinitionResources.Culture);

    internal static string event_HybridConnectionManagerStopping => EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionManagerStopping), EventDefinitionResources.Culture);

    internal static string event_HybridConnectionManagerConfigSettingsChanged => EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionManagerConfigSettingsChanged), EventDefinitionResources.Culture);

    internal static string event_HybridConnectionClientServiceStarting => EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionClientServiceStarting), EventDefinitionResources.Culture);

    internal static string event_HybridConnectionClientServiceStopping => EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionClientServiceStopping), EventDefinitionResources.Culture);

    internal static string event_HybridConnectionClientConfigSettingsChanged => EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionClientConfigSettingsChanged), EventDefinitionResources.Culture);

    internal static string EventSource_UndefinedChannel(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (EventSource_UndefinedChannel), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ArgumentOutOfRange_MaxArgExceeded(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (ArgumentOutOfRange_MaxArgExceeded), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string ArgumentOutOfRange_MaxStringsExceeded(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (ArgumentOutOfRange_MaxStringsExceeded), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_MessageAbandon(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageAbandon), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_MessageComplete(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageComplete), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_MessageReceived(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageReceived), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_MessageSending(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageSending), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_ChannelFaulted(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_ChannelFaulted), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_ChannelReceiveContextAbandon(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_ChannelReceiveContextAbandon), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_ChannelReceiveContextComplete(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_ChannelReceiveContextComplete), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_ChannelSendingMessage(object param0, object param1, object param2) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_ChannelSendingMessage), EventDefinitionResources.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string event_ChannelReceivedMessage(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_ChannelReceivedMessage), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_MessageSuspend(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageSuspend), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_MessageDefer(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageDefer), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_LogOperation(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_LogOperation), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_LogOperationWarning(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_LogOperationWarning), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_RetryOperation(object param0, object param1, object param2) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RetryOperation), EventDefinitionResources.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string event_ThreadNeutralSemaphoreEnterFailed(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_ThreadNeutralSemaphoreEnterFailed), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_RuntimeChannelCreated(
      object param0,
      object param1,
      object param2,
      object param3,
      object param4)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RuntimeChannelCreated), EventDefinitionResources.Culture), param0, param1, param2, param3, param4);
    }

    internal static string event_RuntimeChannelAborting(
      object param0,
      object param1,
      object param2,
      object param3,
      object param4)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RuntimeChannelAborting), EventDefinitionResources.Culture), param0, param1, param2, param3, param4);
    }

    internal static string event_RuntimeChannelFaulting(
      object param0,
      object param1,
      object param2,
      object param3,
      object param4,
      object param5)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RuntimeChannelFaulting), EventDefinitionResources.Culture), param0, param1, param2, param3, param4, param5);
    }

    internal static string event_RuntimeChannelPingFailed(
      object param0,
      object param1,
      object param2,
      object param3,
      object param4,
      object param5)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RuntimeChannelPingFailed), EventDefinitionResources.Culture), param0, param1, param2, param3, param4, param5);
    }

    internal static string event_RuntimeChannelPingIncorrectState(
      object param0,
      object param1,
      object param2,
      object param3,
      object param4,
      object param5)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RuntimeChannelPingIncorrectState), EventDefinitionResources.Culture), param0, param1, param2, param3, param4, param5);
    }

    internal static string event_RuntimeChannelStopPingWithIncorrectState(
      object param0,
      object param1,
      object param2,
      object param3,
      object param4,
      object param5,
      object param6)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RuntimeChannelStopPingWithIncorrectState), EventDefinitionResources.Culture), param0, param1, param2, param3, param4, param5, param6);
    }

    internal static string event_RetryPolicyIteration(
      object param0,
      object param1,
      object param2,
      object param3,
      object param4,
      object param5,
      object param6)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RetryPolicyIteration), EventDefinitionResources.Culture), param0, param1, param2, param3, param4, param5, param6);
    }

    internal static string event_RetryPolicyStreamNotSeekable(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RetryPolicyStreamNotSeekable), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_RetryPolicyStreamNotClonable(
      object param0,
      object param1,
      object param2,
      object param3,
      object param4)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RetryPolicyStreamNotClonable), EventDefinitionResources.Culture), param0, param1, param2, param3, param4);
    }

    internal static string event_AppDomainUnload(object param0, object param1, object param2) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AppDomainUnload), EventDefinitionResources.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string event_ShipAssertExceptionMessage(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_ShipAssertExceptionMessage), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_ThrowingExceptionVerbose(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_ThrowingExceptionVerbose), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_ThrowingExceptionInformational(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_ThrowingExceptionInformational), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_ThrowingExceptionWarning(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_ThrowingExceptionWarning), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_ThrowingExceptionError(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_ThrowingExceptionError), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_UnhandledException(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_UnhandledException), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_TraceCodeEventLogCritical(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_TraceCodeEventLogCritical), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_TraceCodeEventLogError(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_TraceCodeEventLogError), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_TraceCodeEventLogInformational(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_TraceCodeEventLogInformational), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_TraceCodeEventLogVerbose(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_TraceCodeEventLogVerbose), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_TraceCodeEventLogWarning(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_TraceCodeEventLogWarning), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_HandledExceptionWarning(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HandledExceptionWarning), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_SingletonManagerLoadSucceeded(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_SingletonManagerLoadSucceeded), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_ExceptionAsWarning(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_ExceptionAsWarning), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_ExceptionAsInformation(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_ExceptionAsInformation), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_HandledExceptionWithEntityName(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HandledExceptionWithEntityName), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_LogAsWarning(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_LogAsWarning), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_ThrowingExceptionWithEntityName(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_ThrowingExceptionWithEntityName), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_HandledExceptionWithFunctionName(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HandledExceptionWithFunctionName), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_NonSerializableException(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_NonSerializableException), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_MessageRenew(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageRenew), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_BatchManagerExecutingBatchedObject(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_BatchManagerExecutingBatchedObject), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_BatchManagerException(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_BatchManagerException), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_BatchManagerTransactionInDoubt(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_BatchManagerTransactionInDoubt), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_FailFastOccurred(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_FailFastOccurred), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_AcceptSessionRequestBegin(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AcceptSessionRequestBegin), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_AcceptSessionRequestEnd(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AcceptSessionRequestEnd), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_AcceptSessionRequestFailed(
      object param0,
      object param1,
      object param2,
      object param3,
      object param4)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AcceptSessionRequestFailed), EventDefinitionResources.Culture), param0, param1, param2, param3, param4);
    }

    internal static string event_AmqpLogOperation(object param0, object param1, object param2) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpLogOperation), EventDefinitionResources.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string event_AmqpLogOperationVerbose(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpLogOperationVerbose), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_AmqpLogError(object param0, object param1, object param2) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpLogError), EventDefinitionResources.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string event_AmqpAddSession(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpAddSession), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_AmqpRemoveSession(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpRemoveSession), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_AmqpDispose(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpDispose), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_AmqpDeliveryNotFound(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpDeliveryNotFound), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_AmqpStateTransition(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpStateTransition), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_AmqpAttachLink(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpAttachLink), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_AmqpRemoveLink(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpRemoveLink), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_AmqpReceiveMessage(object param0, object param1, object param2) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpReceiveMessage), EventDefinitionResources.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string event_AmqpSettle(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpSettle), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_AmqpUpgradeTransport(object param0, object param1, object param2) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpUpgradeTransport), EventDefinitionResources.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string event_AmqpInsecureTransport(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpInsecureTransport), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_AmqpOpenEntityFailed(
      object param0,
      object param1,
      object param2,
      object param3,
      object param4)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpOpenEntityFailed), EventDefinitionResources.Culture), param0, param1, param2, param3, param4);
    }

    internal static string event_AmqpOpenEntitySucceeded(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpOpenEntitySucceeded), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_DetectConnectivityModeFailed(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_DetectConnectivityModeFailed), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_DetectConnectivityModeSucceeded(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_DetectConnectivityModeSucceeded), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_AmqpManageLink(object param0, object param1, object param2) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpManageLink), EventDefinitionResources.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string event_AmqpListenSocketAcceptError(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpListenSocketAcceptError), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_AmqpDynamicReadBufferSizeChange(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpDynamicReadBufferSizeChange), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_AmqpMissingHandle(object param0, object param1, object param2) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpMissingHandle), EventDefinitionResources.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string event_UnexpectedExceptionTelemetry(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_UnexpectedExceptionTelemetry), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_PairedNamespaceTransferQueueCreateError(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceTransferQueueCreateError), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_PairedNamespaceTransferQueueCreateFailure(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceTransferQueueCreateFailure), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_PairedNamespaceDeadletterException(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceDeadletterException), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_PairedNamespaceDestinationSendException(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceDestinationSendException), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_PairedNamespaceCouldNotCreateMessageSender(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceCouldNotCreateMessageSender), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_PairedNamespaceMessageNoPathInBacklog(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceMessageNoPathInBacklog), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_PairedNamespaceMessagePumpReceiveFailed(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceMessagePumpReceiveFailed), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_PairedNamespaceSendToBacklogFailed(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceSendToBacklogFailed), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_PairedNamespacePingException(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespacePingException), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_PairedNamespaceMessagePumpProcessQueueFailed(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceMessagePumpProcessQueueFailed), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_RelayListenerRelayedConnectReceived(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayListenerRelayedConnectReceived), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_RelayListenerClientAccepted(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayListenerClientAccepted), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_RelayClientConnecting(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayClientConnecting), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_RelayClientConnected(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayClientConnected), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_RelayClientDisconnected(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayClientDisconnected), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_RelayLogVerbose(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayLogVerbose), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_RelayChannelOpening(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayChannelOpening), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_GetRuntimeEntityDescriptionCompleted(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_GetRuntimeEntityDescriptionCompleted), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_GetRuntimeEntityDescriptionFailed(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_GetRuntimeEntityDescriptionFailed), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_GetRuntimeEntityDescriptionStarted(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_GetRuntimeEntityDescriptionStarted), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_AmqpInputSessionChannelMessageReceived(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_AmqpInputSessionChannelMessageReceived), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_PairedNamespaceMessagePumpProcessCloseSenderFailed(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceMessagePumpProcessCloseSenderFailed), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_PairedNamespaceReceiveMessageFromSecondary(
      object param0,
      object param1)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceReceiveMessageFromSecondary), EventDefinitionResources.Culture), new object[2]
      {
        param0,
        param1
      });
    }

    internal static string event_PairedNamespaceSendingMessage(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceSendingMessage), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_PairedNamespaceSendMessageFailure(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceSendMessageFailure), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_PairedNamespaceSendMessageSuccess(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceSendMessageSuccess), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_FramingOuputPumpPingException(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_FramingOuputPumpPingException), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_FramingOuputPumpRunException(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_FramingOuputPumpRunException), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_WebStreamConnectionAbort(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamConnectionAbort), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_WebStreamConnectionClose(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamConnectionClose), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_WebStreamConnectionShutdown(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamConnectionShutdown), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_WebStreamAbort(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamAbort), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_WebStreamClose(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamClose), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_WebStreamDispose(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamDispose), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_WebStreamReset(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamReset), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_WebStreamShutdown(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamShutdown), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_RelayClientConnectFailed(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayClientConnectFailed), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_PerformanceCounterCreationFailed(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PerformanceCounterCreationFailed), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_PerformanceCounterInstanceCreated(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PerformanceCounterInstanceCreated), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_PerformanceCounterInstanceRemoved(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PerformanceCounterInstanceRemoved), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_MessageReceivePumpUserCallbackException(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageReceivePumpUserCallbackException), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_MessageReceivePumpFailedToComplete(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageReceivePumpFailedToComplete), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_MessageReceivePumpFailedToAbandon(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageReceivePumpFailedToAbandon), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_MessageReceivePumpUnexpectedException(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageReceivePumpUnexpectedException), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_MessageReceivePumpReceiveException(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageReceivePumpReceiveException), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_MessageReceivePumpStopped(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageReceivePumpStopped), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_MessageReceivePumpBackoff(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageReceivePumpBackoff), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_RelayClientConnectRedirected(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayClientConnectRedirected), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_RelayClientFailedToAcquireToken(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayClientFailedToAcquireToken), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_WebStreamConnectCompleted(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamConnectCompleted), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_WebStreamConnectFailed(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamConnectFailed), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_WebStreamConnecting(object param0, object param1, object param2) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamConnecting), EventDefinitionResources.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string event_WebStreamFramingInputPumpSlowRead(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamFramingInputPumpSlowRead), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_WebStreamFramingInputPumpSlowReadWithException(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamFramingInputPumpSlowReadWithException), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_WebStreamFramingOuputPumpPingSlow(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamFramingOuputPumpPingSlow), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_WebStreamFramingOuputPumpPingSlowException(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamFramingOuputPumpPingSlowException), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_WebStreamFramingOuputPumpSlow(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamFramingOuputPumpSlow), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_WebStreamFramingOuputPumpSlowException(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamFramingOuputPumpSlowException), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_WebStreamReadStreamCompleted(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamReadStreamCompleted), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_WebStreamReturningZero(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamReturningZero), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_WebStreamWriteStreamCompleted(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebStreamWriteStreamCompleted), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_RelayChannelAborting(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayChannelAborting), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_RelayChannelClosing(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayChannelClosing), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_RelayChannelFaulting(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayChannelFaulting), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_RelayClientConnectivityModeDetected(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayClientConnectivityModeDetected), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_RelayClientPingFailed(object param0, object param1, object param2) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayClientPingFailed), EventDefinitionResources.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string event_RelayListenerClientAcceptFailed(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayListenerClientAcceptFailed), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_RelayListenerFailedToDispatchMessage(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayListenerFailedToDispatchMessage), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_PairedNamespaceStartSyphon(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceStartSyphon), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_PairedNamespaceStopSyphon(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_PairedNamespaceStopSyphon), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_MessageCanceling(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_MessageCanceling), EventDefinitionResources.Culture), param0, param1, param2, param3);
    }

    internal static string event_WebSocketConnectionAbort(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebSocketConnectionAbort), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_WebSocketConnectionClose(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebSocketConnectionClose), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_WebSocketConnectionShutdown(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebSocketConnectionShutdown), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_WebSocketConnectionEstablished(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebSocketConnectionEstablished), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_NullReferenceErrorOccurred(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_NullReferenceErrorOccurred), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_WebSocketTransportAborted(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebSocketTransportAborted), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_WebSocketTransportClosed(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebSocketTransportClosed), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_WebSocketTransportEstablished(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebSocketTransportEstablished), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_WebSocketTransportShutdown(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_WebSocketTransportShutdown), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_HybridConnectionStarted(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionStarted), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_HybridConnectionStopped(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionStopped), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_HybridConnectionFailedToReadResourceDescriptionMetaData(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionFailedToReadResourceDescriptionMetaData), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_HybridConnectionFailedToStart(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionFailedToStart), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_HybridConnectionSecurityException(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionSecurityException), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_HybridConnectionFailedToStop(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionFailedToStop), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_RelayClientGoingOnline(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayClientGoingOnline), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_RelayClientReconnecting(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayClientReconnecting), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_RelayClientStopConnecting(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_RelayClientStopConnecting), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_HybridConnectionInvalidConnectionString(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionInvalidConnectionString), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_HybridConnectionManagerConfigurationFileError(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionManagerConfigurationFileError), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_HybridConnectionManagerManagementServerError(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionManagerManagementServerError), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_HybridConnectionManagerManagementServiceStarting(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionManagerManagementServiceStarting), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_HybridConnectionManagerManagementServiceStopping(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionManagerManagementServiceStopping), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_UnexpectedScheduledNotificationIdFormat(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_UnexpectedScheduledNotificationIdFormat), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_FailedToCancelNotification(object param0, object param1) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_FailedToCancelNotification), EventDefinitionResources.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string event_HybridConnectionClientFailedToStart(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionClientFailedToStart), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_HybridConnectionClientFailedToStop(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionClientFailedToStop), EventDefinitionResources.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string event_HybridConnectionClientProxyError(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionClientProxyError), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_HybridConnectionClientStarted(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionClientStarted), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_HybridConnectionClientStopped(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionClientStopped), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_HybridConnectionClientTrace(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionClientTrace), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });

    internal static string event_HybridConnectionClientConfigurationFileError(object param0) => string.Format((IFormatProvider) EventDefinitionResources.Culture, EventDefinitionResources.ResourceManager.GetString(nameof (event_HybridConnectionClientConfigurationFileError), EventDefinitionResources.Culture), new object[1]
    {
      param0
    });
  }
}
