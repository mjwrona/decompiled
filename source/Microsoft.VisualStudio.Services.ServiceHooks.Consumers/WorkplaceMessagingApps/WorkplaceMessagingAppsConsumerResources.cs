// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WorkplaceMessagingApps.WorkplaceMessagingAppsConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WorkplaceMessagingApps
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class WorkplaceMessagingAppsConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal WorkplaceMessagingAppsConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (WorkplaceMessagingAppsConsumerResources.resourceMan == null)
          WorkplaceMessagingAppsConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WorkplaceMessagingApps.WorkplaceMessagingAppsConsumerResources", typeof (WorkplaceMessagingAppsConsumerResources).Assembly);
        return WorkplaceMessagingAppsConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => WorkplaceMessagingAppsConsumerResources.resourceCulture;
      set => WorkplaceMessagingAppsConsumerResources.resourceCulture = value;
    }

    internal static string AzChatOpsActionTask_InvalidApiArea => WorkplaceMessagingAppsConsumerResources.ResourceManager.GetString(nameof (AzChatOpsActionTask_InvalidApiArea), WorkplaceMessagingAppsConsumerResources.resourceCulture);

    internal static string AzChatOpsActionTask_InvalidApiMethod => WorkplaceMessagingAppsConsumerResources.ResourceManager.GetString(nameof (AzChatOpsActionTask_InvalidApiMethod), WorkplaceMessagingAppsConsumerResources.resourceCulture);

    internal static string AzChatOpsActionTask_InvalidUrl => WorkplaceMessagingAppsConsumerResources.ResourceManager.GetString(nameof (AzChatOpsActionTask_InvalidUrl), WorkplaceMessagingAppsConsumerResources.resourceCulture);

    internal static string AzChatOpsActionTask_RequestTemplate => WorkplaceMessagingAppsConsumerResources.ResourceManager.GetString(nameof (AzChatOpsActionTask_RequestTemplate), WorkplaceMessagingAppsConsumerResources.resourceCulture);

    internal static string ConsumerDescription => WorkplaceMessagingAppsConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), WorkplaceMessagingAppsConsumerResources.resourceCulture);

    internal static string ConsumerName => WorkplaceMessagingAppsConsumerResources.ResourceManager.GetString(nameof (ConsumerName), WorkplaceMessagingAppsConsumerResources.resourceCulture);

    internal static string HttpRequestAction_InputUrlDescription => WorkplaceMessagingAppsConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_InputUrlDescription), WorkplaceMessagingAppsConsumerResources.resourceCulture);

    internal static string HttpRequestAction_InputUrlName => WorkplaceMessagingAppsConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_InputUrlName), WorkplaceMessagingAppsConsumerResources.resourceCulture);

    internal static string HttpRequestActionDescription => WorkplaceMessagingAppsConsumerResources.ResourceManager.GetString(nameof (HttpRequestActionDescription), WorkplaceMessagingAppsConsumerResources.resourceCulture);

    internal static string HttpRequestActionName => WorkplaceMessagingAppsConsumerResources.ResourceManager.GetString(nameof (HttpRequestActionName), WorkplaceMessagingAppsConsumerResources.resourceCulture);
  }
}
