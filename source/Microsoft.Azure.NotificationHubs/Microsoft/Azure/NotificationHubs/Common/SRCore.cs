// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.SRCore
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Resources;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal class SRCore
  {
    private static ResourceManager resourceManager;
    private static CultureInfo resourceCulture;

    private SRCore()
    {
    }

    internal static ResourceManager ResourceManager
    {
      get
      {
        if (SRCore.resourceManager == null)
          SRCore.resourceManager = new ResourceManager("Microsoft.Azure.NotificationHubs.Common.SRCore", typeof (SRCore).Assembly);
        return SRCore.resourceManager;
      }
    }

    [GeneratedCode("StrictResXFileCodeGenerator", "4.0.0.0")]
    internal static CultureInfo Culture
    {
      get => SRCore.resourceCulture;
      set => SRCore.resourceCulture = value;
    }

    internal static string ActionItemIsAlreadyScheduled => SRCore.ResourceManager.GetString(nameof (ActionItemIsAlreadyScheduled), SRCore.Culture);

    internal static string AsyncCallbackThrewException => SRCore.ResourceManager.GetString(nameof (AsyncCallbackThrewException), SRCore.Culture);

    internal static string AsyncResultAlreadyEnded => SRCore.ResourceManager.GetString(nameof (AsyncResultAlreadyEnded), SRCore.Culture);

    internal static string AsyncTransactionException => SRCore.ResourceManager.GetString(nameof (AsyncTransactionException), SRCore.Culture);

    internal static string BufferIsNotRightSizeForBufferManager => SRCore.ResourceManager.GetString(nameof (BufferIsNotRightSizeForBufferManager), SRCore.Culture);

    internal static string InvalidAsyncResult => SRCore.ResourceManager.GetString(nameof (InvalidAsyncResult), SRCore.Culture);

    internal static string InvalidAsyncResultImplementationGeneric => SRCore.ResourceManager.GetString(nameof (InvalidAsyncResultImplementationGeneric), SRCore.Culture);

    internal static string InvalidNullAsyncResult => SRCore.ResourceManager.GetString(nameof (InvalidNullAsyncResult), SRCore.Culture);

    internal static string InvalidSemaphoreExit => SRCore.ResourceManager.GetString(nameof (InvalidSemaphoreExit), SRCore.Culture);

    internal static string MustCancelOldTimer => SRCore.ResourceManager.GetString(nameof (MustCancelOldTimer), SRCore.Culture);

    internal static string EndOfInnerExceptionStackTrace => SRCore.ResourceManager.GetString(nameof (EndOfInnerExceptionStackTrace), SRCore.Culture);

    internal static string AsyncSemaphoreExitCalledWithoutEnter => SRCore.ResourceManager.GetString(nameof (AsyncSemaphoreExitCalledWithoutEnter), SRCore.Culture);

    internal static string SharedAccessAuthorizationRuleKeyContainsInvalidCharacters => SRCore.ResourceManager.GetString(nameof (SharedAccessAuthorizationRuleKeyContainsInvalidCharacters), SRCore.Culture);

    internal static string SharedAccessAuthorizationRuleRequiresPrimaryKey => SRCore.ResourceManager.GetString(nameof (SharedAccessAuthorizationRuleRequiresPrimaryKey), SRCore.Culture);

    internal static string SharedAccessKeyShouldbeBase64 => SRCore.ResourceManager.GetString(nameof (SharedAccessKeyShouldbeBase64), SRCore.Culture);

    internal static string ArgumentNullOrEmpty(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (ArgumentNullOrEmpty), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string AsyncResultCompletedTwice(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (AsyncResultCompletedTwice), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string EtwAPIMaxStringCountExceeded(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (EtwAPIMaxStringCountExceeded), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string EtwMaxNumberArgumentsExceeded(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (EtwMaxNumberArgumentsExceeded), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string EtwRegistrationFailed(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (EtwRegistrationFailed), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string InvalidAsyncResultImplementation(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (InvalidAsyncResultImplementation), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string ShipAssertExceptionMessage(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (ShipAssertExceptionMessage), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string TimeoutInputQueueDequeue(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (TimeoutInputQueueDequeue), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string TimeoutMustBeNonNegative(object param0, object param1) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (TimeoutMustBeNonNegative), SRCore.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string TimeoutMustBePositive(object param0, object param1) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (TimeoutMustBePositive), SRCore.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string TimeoutOnOperation(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (TimeoutOnOperation), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string FailFastMessage(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (FailFastMessage), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string ResourceCountExceeded(object param0, object param1, object param2) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (ResourceCountExceeded), SRCore.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string ArgumentNullOrWhiteSpace(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (ArgumentNullOrWhiteSpace), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string UnsupportedEnumerationValue(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (UnsupportedEnumerationValue), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string UnsupportedOperation(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (UnsupportedOperation), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string UnsupportedTransport(object param0, object param1) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (UnsupportedTransport), SRCore.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AutoForwardToSelf(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (AutoForwardToSelf), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string NullOrEmptyConfigurationAttribute(object param0, object param1) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (NullOrEmptyConfigurationAttribute), SRCore.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string MultipleTransportSettingConfigurationElement(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (MultipleTransportSettingConfigurationElement), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string DictionaryKeyIsModified(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (DictionaryKeyIsModified), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string DictionaryKeyNotExist(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (DictionaryKeyNotExist), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string ArgumentStringTooBig(object param0, object param1) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (ArgumentStringTooBig), SRCore.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string SharedAccessAuthorizationRuleKeyNameTooBig(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (SharedAccessAuthorizationRuleKeyNameTooBig), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string SharedAccessAuthorizationRuleKeyTooBig(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (SharedAccessAuthorizationRuleKeyTooBig), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string SharedAccessRuleAllowsFixedLengthKeys(object param0) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (SharedAccessRuleAllowsFixedLengthKeys), SRCore.Culture), new object[1]
    {
      param0
    });

    internal static string ArgumentOutOfRange(object param0, object param1) => string.Format((IFormatProvider) SRCore.Culture, SRCore.ResourceManager.GetString(nameof (ArgumentOutOfRange), SRCore.Culture), new object[2]
    {
      param0,
      param1
    });
  }
}
