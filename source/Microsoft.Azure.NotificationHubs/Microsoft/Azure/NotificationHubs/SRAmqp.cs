// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.SRAmqp
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Resources;

namespace Microsoft.Azure.NotificationHubs
{
  internal class SRAmqp
  {
    private static ResourceManager resourceManager;
    private static CultureInfo resourceCulture;

    private SRAmqp()
    {
    }

    internal static ResourceManager ResourceManager
    {
      get
      {
        if (SRAmqp.resourceManager == null)
          SRAmqp.resourceManager = new ResourceManager("Microsoft.Azure.NotificationHubs.SRAmqp", typeof (SRAmqp).Assembly);
        return SRAmqp.resourceManager;
      }
    }

    [GeneratedCode("StrictResXFileCodeGenerator", "4.0.0.0")]
    internal static CultureInfo Culture
    {
      get => SRAmqp.resourceCulture;
      set => SRAmqp.resourceCulture = value;
    }

    internal static string AmqpFramingError => SRAmqp.ResourceManager.GetString(nameof (AmqpFramingError), SRAmqp.Culture);

    internal static string AmqpFieldSessionId => SRAmqp.ResourceManager.GetString(nameof (AmqpFieldSessionId), SRAmqp.Culture);

    internal static string AmqpInvalidMessageBodyType => SRAmqp.ResourceManager.GetString(nameof (AmqpInvalidMessageBodyType), SRAmqp.Culture);

    internal static string AmqpUnopenObject => SRAmqp.ResourceManager.GetString(nameof (AmqpUnopenObject), SRAmqp.Culture);

    internal static string AmqpNotSupportMechanism => SRAmqp.ResourceManager.GetString(nameof (AmqpNotSupportMechanism), SRAmqp.Culture);

    internal static string AmqpDynamicTerminusNotSupported => SRAmqp.ResourceManager.GetString(nameof (AmqpDynamicTerminusNotSupported), SRAmqp.Culture);

    internal static string AmqpConnectionInactive => SRAmqp.ResourceManager.GetString(nameof (AmqpConnectionInactive), SRAmqp.Culture);

    internal static string AmqpInvalidRemoteIp => SRAmqp.ResourceManager.GetString(nameof (AmqpInvalidRemoteIp), SRAmqp.Culture);

    internal static string AmqpBufferAlreadyReclaimed => SRAmqp.ResourceManager.GetString(nameof (AmqpBufferAlreadyReclaimed), SRAmqp.Culture);

    internal static string AmqpCannotCloneSentMessage => SRAmqp.ResourceManager.GetString(nameof (AmqpCannotCloneSentMessage), SRAmqp.Culture);

    internal static string AmqpUnssuportedTokenType => SRAmqp.ResourceManager.GetString(nameof (AmqpUnssuportedTokenType), SRAmqp.Culture);

    internal static string AmqpInvalidCommand => SRAmqp.ResourceManager.GetString(nameof (AmqpInvalidCommand), SRAmqp.Culture);

    internal static string AmqpNoValidAddressForHost(object param0) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpNoValidAddressForHost), SRAmqp.Culture), new object[1]
    {
      param0
    });

    internal static string AmqpInvalidPerformativeCode(object param0) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpInvalidPerformativeCode), SRAmqp.Culture), new object[1]
    {
      param0
    });

    internal static string AmqpInvalidFormatCode(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpInvalidFormatCode), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpInvalidMessageSectionCode(object param0) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpInvalidMessageSectionCode), SRAmqp.Culture), new object[1]
    {
      param0
    });

    internal static string AmqpRequiredFieldNotSet(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpRequiredFieldNotSet), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpApplicationProperties(object param0) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpApplicationProperties), SRAmqp.Culture), new object[1]
    {
      param0
    });

    internal static string AmqpObjectAborted(object param0) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpObjectAborted), SRAmqp.Culture), new object[1]
    {
      param0
    });

    internal static string AmqpIllegalOperationState(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpIllegalOperationState), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpChannelNotFound(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpChannelNotFound), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpHandleNotFound(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpHandleNotFound), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpHandleInUse(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpHandleInUse), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpHandleExceeded(object param0) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpHandleExceeded), SRAmqp.Culture), new object[1]
    {
      param0
    });

    internal static string AmqpMessageSizeExceeded(object param0, object param1, object param2) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpMessageSizeExceeded), SRAmqp.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string AmqpTransferLimitExceeded(object param0) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpTransferLimitExceeded), SRAmqp.Culture), new object[1]
    {
      param0
    });

    internal static string AmqpProtocolVersionNotSupported(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpProtocolVersionNotSupported), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpProtocolVersionNotSet(object param0) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpProtocolVersionNotSet), SRAmqp.Culture), new object[1]
    {
      param0
    });

    internal static string AmqpInvalidType(object param0) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpInvalidType), SRAmqp.Culture), new object[1]
    {
      param0
    });

    internal static string AmqpOperationNotSupported(object param0) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpOperationNotSupported), SRAmqp.Culture), new object[1]
    {
      param0
    });

    internal static string AmqpInvalidReOpenOperation(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpInvalidReOpenOperation), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpTimeout(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpTimeout), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpDeliveryIDInUse(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpDeliveryIDInUse), SRAmqp.Culture), param0, param1, param2, param3);
    }

    internal static string AmqpEncodingTypeMismatch(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpEncodingTypeMismatch), SRAmqp.Culture), param0, param1, param2, param3);
    }

    internal static string AmqpDuplicateMemberOrder(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpDuplicateMemberOrder), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpUnknownDescriptor(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpUnknownDescriptor), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpInsufficientBufferSize(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpInsufficientBufferSize), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpErrorOccurred(object param0) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpErrorOccurred), SRAmqp.Culture), new object[1]
    {
      param0
    });

    internal static string AmqpInvalidLinkAttachAddress(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpInvalidLinkAttachAddress), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpGlobalOpaqueAddressesNotSupported(object param0) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpGlobalOpaqueAddressesNotSupported), SRAmqp.Culture), new object[1]
    {
      param0
    });

    internal static string AmqpInvalidLinkAttachScheme(object param0) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpInvalidLinkAttachScheme), SRAmqp.Culture), new object[1]
    {
      param0
    });

    internal static string AmqpIdleTimeoutNotSupported(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpIdleTimeoutNotSupported), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpInvalidSequenceNumberComparison(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpInvalidSequenceNumberComparison), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpInvalidPropertyType(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpInvalidPropertyType), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpInvalidPropertyValue(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpInvalidPropertyValue), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpMissingOrInvalidProperty(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpMissingOrInvalidProperty), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpMissingProperty(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpMissingProperty), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpPutTokenAudienceMismatch(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpPutTokenAudienceMismatch), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpPutTokenFailed(object param0, object param1) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpPutTokenFailed), SRAmqp.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AmqpCbsLinkAlreadyOpen(object param0) => string.Format((IFormatProvider) SRAmqp.Culture, SRAmqp.ResourceManager.GetString(nameof (AmqpCbsLinkAlreadyOpen), SRAmqp.Culture), new object[1]
    {
      param0
    });
  }
}
