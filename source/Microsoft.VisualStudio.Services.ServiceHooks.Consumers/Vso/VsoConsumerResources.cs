// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Vso.VsoConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Vso
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class VsoConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal VsoConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (VsoConsumerResources.resourceMan == null)
          VsoConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Vso.VsoConsumerResources", typeof (VsoConsumerResources).Assembly);
        return VsoConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => VsoConsumerResources.resourceCulture;
      set => VsoConsumerResources.resourceCulture = value;
    }

    internal static string ConsumerDescription => VsoConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), VsoConsumerResources.resourceCulture);

    internal static string ConsumerName => VsoConsumerResources.ResourceManager.GetString(nameof (ConsumerName), VsoConsumerResources.resourceCulture);

    internal static string MessageBusException => VsoConsumerResources.ResourceManager.GetString(nameof (MessageBusException), VsoConsumerResources.resourceCulture);

    internal static string NoopAction_PublishToServiceBus => VsoConsumerResources.ResourceManager.GetString(nameof (NoopAction_PublishToServiceBus), VsoConsumerResources.resourceCulture);

    internal static string SimulateBuildCompletionActionDescription => VsoConsumerResources.ResourceManager.GetString(nameof (SimulateBuildCompletionActionDescription), VsoConsumerResources.resourceCulture);

    internal static string SimulateBuildCompletionActionName => VsoConsumerResources.ResourceManager.GetString(nameof (SimulateBuildCompletionActionName), VsoConsumerResources.resourceCulture);

    internal static string SimulateCodePushActionDescription => VsoConsumerResources.ResourceManager.GetString(nameof (SimulateCodePushActionDescription), VsoConsumerResources.resourceCulture);

    internal static string SimulateCodePushActionName => VsoConsumerResources.ResourceManager.GetString(nameof (SimulateCodePushActionName), VsoConsumerResources.resourceCulture);

    internal static string SimulateCommentActionDescription => VsoConsumerResources.ResourceManager.GetString(nameof (SimulateCommentActionDescription), VsoConsumerResources.resourceCulture);

    internal static string SimulateCommentActionName => VsoConsumerResources.ResourceManager.GetString(nameof (SimulateCommentActionName), VsoConsumerResources.resourceCulture);

    internal static string SimulateImagePushActionDescription => VsoConsumerResources.ResourceManager.GetString(nameof (SimulateImagePushActionDescription), VsoConsumerResources.resourceCulture);

    internal static string SimulateImagePushActionName => VsoConsumerResources.ResourceManager.GetString(nameof (SimulateImagePushActionName), VsoConsumerResources.resourceCulture);

    internal static string SimulateImagePushActionNameInACR => VsoConsumerResources.ResourceManager.GetString(nameof (SimulateImagePushActionNameInACR), VsoConsumerResources.resourceCulture);

    internal static string SimulatePullRequestActionDescription => VsoConsumerResources.ResourceManager.GetString(nameof (SimulatePullRequestActionDescription), VsoConsumerResources.resourceCulture);

    internal static string SimulatePullRequestActionName => VsoConsumerResources.ResourceManager.GetString(nameof (SimulatePullRequestActionName), VsoConsumerResources.resourceCulture);
  }
}
