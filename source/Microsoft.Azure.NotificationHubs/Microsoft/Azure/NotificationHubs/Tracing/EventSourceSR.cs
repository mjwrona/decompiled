// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.EventSourceSR
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Resources;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal class EventSourceSR
  {
    private static ResourceManager resourceManager;
    private static CultureInfo resourceCulture;

    private EventSourceSR()
    {
    }

    internal static ResourceManager ResourceManager
    {
      get
      {
        if (EventSourceSR.resourceManager == null)
          EventSourceSR.resourceManager = new ResourceManager("Microsoft.Azure.NotificationHubs.Tracing.EventSourceSR", typeof (EventSourceSR).Assembly);
        return EventSourceSR.resourceManager;
      }
    }

    [GeneratedCode("StrictResXFileCodeGenerator", "4.0.0.0")]
    internal static CultureInfo Culture
    {
      get => EventSourceSR.resourceCulture;
      set => EventSourceSR.resourceCulture = value;
    }

    internal static string Event_IllegalID => EventSourceSR.ResourceManager.GetString(nameof (Event_IllegalID), EventSourceSR.Culture);

    internal static string Event_IllegalOpcode => EventSourceSR.ResourceManager.GetString(nameof (Event_IllegalOpcode), EventSourceSR.Culture);

    internal static string Event_ListenerNotFound => EventSourceSR.ResourceManager.GetString(nameof (Event_ListenerNotFound), EventSourceSR.Culture);

    internal static string EventSource_UndefinedChannel(object param0, object param1) => string.Format((IFormatProvider) EventSourceSR.Culture, EventSourceSR.ResourceManager.GetString(nameof (EventSource_UndefinedChannel), EventSourceSR.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ArgumentOutOfRange_MaxArgExceeded(object param0) => string.Format((IFormatProvider) EventSourceSR.Culture, EventSourceSR.ResourceManager.GetString(nameof (ArgumentOutOfRange_MaxArgExceeded), EventSourceSR.Culture), new object[1]
    {
      param0
    });

    internal static string ArgumentOutOfRange_MaxStringsExceeded(object param0) => string.Format((IFormatProvider) EventSourceSR.Culture, EventSourceSR.ResourceManager.GetString(nameof (ArgumentOutOfRange_MaxStringsExceeded), EventSourceSR.Culture), new object[1]
    {
      param0
    });

    internal static string ProviderGuidNotSpecified(object param0) => string.Format((IFormatProvider) EventSourceSR.Culture, EventSourceSR.ResourceManager.GetString(nameof (ProviderGuidNotSpecified), EventSourceSR.Culture), new object[1]
    {
      param0
    });

    internal static string Event_EventNotReturnVoid(object param0) => string.Format((IFormatProvider) EventSourceSR.Culture, EventSourceSR.ResourceManager.GetString(nameof (Event_EventNotReturnVoid), EventSourceSR.Culture), new object[1]
    {
      param0
    });

    internal static string Event_FailedWithErrorCode(object param0) => string.Format((IFormatProvider) EventSourceSR.Culture, EventSourceSR.ResourceManager.GetString(nameof (Event_FailedWithErrorCode), EventSourceSR.Culture), new object[1]
    {
      param0
    });

    internal static string Event_IllegalEventArg(object param0, object param1, object param2) => string.Format((IFormatProvider) EventSourceSR.Culture, EventSourceSR.ResourceManager.GetString(nameof (Event_IllegalEventArg), EventSourceSR.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string Event_IllegalField(object param0, object param1) => string.Format((IFormatProvider) EventSourceSR.Culture, EventSourceSR.ResourceManager.GetString(nameof (Event_IllegalField), EventSourceSR.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string Event_KeywordValue(object param0, object param1) => string.Format((IFormatProvider) EventSourceSR.Culture, EventSourceSR.ResourceManager.GetString(nameof (Event_KeywordValue), EventSourceSR.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string Event_SourceWithUsedGuid(object param0) => string.Format((IFormatProvider) EventSourceSR.Culture, EventSourceSR.ResourceManager.GetString(nameof (Event_SourceWithUsedGuid), EventSourceSR.Culture), new object[1]
    {
      param0
    });

    internal static string Event_UndefinedKeyword(object param0, object param1) => string.Format((IFormatProvider) EventSourceSR.Culture, EventSourceSR.ResourceManager.GetString(nameof (Event_UndefinedKeyword), EventSourceSR.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string Event_UnsupportType(object param0) => string.Format((IFormatProvider) EventSourceSR.Culture, EventSourceSR.ResourceManager.GetString(nameof (Event_UnsupportType), EventSourceSR.Culture), new object[1]
    {
      param0
    });

    internal static string Event_UsedEventID(object param0, object param1) => string.Format((IFormatProvider) EventSourceSR.Culture, EventSourceSR.ResourceManager.GetString(nameof (Event_UsedEventID), EventSourceSR.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string Event_UsedEventName(object param0) => string.Format((IFormatProvider) EventSourceSR.Culture, EventSourceSR.ResourceManager.GetString(nameof (Event_UsedEventName), EventSourceSR.Culture), new object[1]
    {
      param0
    });
  }
}
