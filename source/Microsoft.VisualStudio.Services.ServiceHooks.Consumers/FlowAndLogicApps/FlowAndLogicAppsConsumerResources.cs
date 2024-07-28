// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.FlowAndLogicApps.FlowAndLogicAppsConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.FlowAndLogicApps
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class FlowAndLogicAppsConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal FlowAndLogicAppsConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (FlowAndLogicAppsConsumerResources.resourceMan == null)
          FlowAndLogicAppsConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.FlowAndLogicApps.FlowAndLogicAppsConsumerResources", typeof (FlowAndLogicAppsConsumerResources).Assembly);
        return FlowAndLogicAppsConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => FlowAndLogicAppsConsumerResources.resourceCulture;
      set => FlowAndLogicAppsConsumerResources.resourceCulture = value;
    }

    internal static string ConsumerDescription => FlowAndLogicAppsConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), FlowAndLogicAppsConsumerResources.resourceCulture);

    internal static string ConsumerName => FlowAndLogicAppsConsumerResources.ResourceManager.GetString(nameof (ConsumerName), FlowAndLogicAppsConsumerResources.resourceCulture);

    internal static string HttpRequestAction_InputUrlDescription => FlowAndLogicAppsConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_InputUrlDescription), FlowAndLogicAppsConsumerResources.resourceCulture);

    internal static string HttpRequestAction_InputUrlName => FlowAndLogicAppsConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_InputUrlName), FlowAndLogicAppsConsumerResources.resourceCulture);

    internal static string HttpRequestActionDescription => FlowAndLogicAppsConsumerResources.ResourceManager.GetString(nameof (HttpRequestActionDescription), FlowAndLogicAppsConsumerResources.resourceCulture);

    internal static string HttpRequestActionName => FlowAndLogicAppsConsumerResources.ResourceManager.GetString(nameof (HttpRequestActionName), FlowAndLogicAppsConsumerResources.resourceCulture);
  }
}
