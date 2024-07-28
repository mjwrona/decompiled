// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Datadog.DatadogConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Datadog
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class DatadogConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal DatadogConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (DatadogConsumerResources.resourceMan == null)
          DatadogConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Datadog.DatadogConsumerResources", typeof (DatadogConsumerResources).Assembly);
        return DatadogConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => DatadogConsumerResources.resourceCulture;
      set => DatadogConsumerResources.resourceCulture = value;
    }

    internal static string AP1AccountTypeName => DatadogConsumerResources.ResourceManager.GetString(nameof (AP1AccountTypeName), DatadogConsumerResources.resourceCulture);

    internal static string ConsumerDescription => DatadogConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), DatadogConsumerResources.resourceCulture);

    internal static string ConsumerName => DatadogConsumerResources.ResourceManager.GetString(nameof (ConsumerName), DatadogConsumerResources.resourceCulture);

    internal static string EUAccountTypeName => DatadogConsumerResources.ResourceManager.GetString(nameof (EUAccountTypeName), DatadogConsumerResources.resourceCulture);

    internal static string GovAccountTypeName => DatadogConsumerResources.ResourceManager.GetString(nameof (GovAccountTypeName), DatadogConsumerResources.resourceCulture);

    internal static string PostEventInDatadogAction_ApiKeyInputDescription => DatadogConsumerResources.ResourceManager.GetString(nameof (PostEventInDatadogAction_ApiKeyInputDescription), DatadogConsumerResources.resourceCulture);

    internal static string PostEventInDatadogAction_ApiKeyInputName => DatadogConsumerResources.ResourceManager.GetString(nameof (PostEventInDatadogAction_ApiKeyInputName), DatadogConsumerResources.resourceCulture);

    internal static string PostEventInDatadogAction_InputAccountTypeDescription => DatadogConsumerResources.ResourceManager.GetString(nameof (PostEventInDatadogAction_InputAccountTypeDescription), DatadogConsumerResources.resourceCulture);

    internal static string PostEventInDatadogAction_InputAccountTypeName => DatadogConsumerResources.ResourceManager.GetString(nameof (PostEventInDatadogAction_InputAccountTypeName), DatadogConsumerResources.resourceCulture);

    internal static string PostEventInDatadogActionDescription => DatadogConsumerResources.ResourceManager.GetString(nameof (PostEventInDatadogActionDescription), DatadogConsumerResources.resourceCulture);

    internal static string PostEventInDatadogActionDetailedDescription => DatadogConsumerResources.ResourceManager.GetString(nameof (PostEventInDatadogActionDetailedDescription), DatadogConsumerResources.resourceCulture);

    internal static string PostEventInDatadogActionName => DatadogConsumerResources.ResourceManager.GetString(nameof (PostEventInDatadogActionName), DatadogConsumerResources.resourceCulture);

    internal static string StandardAccountTypeName => DatadogConsumerResources.ResourceManager.GetString(nameof (StandardAccountTypeName), DatadogConsumerResources.resourceCulture);

    internal static string US3AccountTypeName => DatadogConsumerResources.ResourceManager.GetString(nameof (US3AccountTypeName), DatadogConsumerResources.resourceCulture);

    internal static string US5AccountTypeName => DatadogConsumerResources.ResourceManager.GetString(nameof (US5AccountTypeName), DatadogConsumerResources.resourceCulture);
  }
}
