// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Bamboo.BambooConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Bamboo
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class BambooConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal BambooConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (BambooConsumerResources.resourceMan == null)
          BambooConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Bamboo.BambooConsumerResources", typeof (BambooConsumerResources).Assembly);
        return BambooConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => BambooConsumerResources.resourceCulture;
      set => BambooConsumerResources.resourceCulture = value;
    }

    internal static string BambooConsumer_PasswordInputDescription => BambooConsumerResources.ResourceManager.GetString(nameof (BambooConsumer_PasswordInputDescription), BambooConsumerResources.resourceCulture);

    internal static string BambooConsumer_PasswordInputName => BambooConsumerResources.ResourceManager.GetString(nameof (BambooConsumer_PasswordInputName), BambooConsumerResources.resourceCulture);

    internal static string BambooConsumer_ServerBaseUrlInputDescription => BambooConsumerResources.ResourceManager.GetString(nameof (BambooConsumer_ServerBaseUrlInputDescription), BambooConsumerResources.resourceCulture);

    internal static string BambooConsumer_ServerBaseUrlInputName => BambooConsumerResources.ResourceManager.GetString(nameof (BambooConsumer_ServerBaseUrlInputName), BambooConsumerResources.resourceCulture);

    internal static string BambooConsumer_UsernameInputDescription => BambooConsumerResources.ResourceManager.GetString(nameof (BambooConsumer_UsernameInputDescription), BambooConsumerResources.resourceCulture);

    internal static string BambooConsumer_UsernameInputName => BambooConsumerResources.ResourceManager.GetString(nameof (BambooConsumer_UsernameInputName), BambooConsumerResources.resourceCulture);

    internal static string ConsumerDescription => BambooConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), BambooConsumerResources.resourceCulture);

    internal static string ConsumerName => BambooConsumerResources.ResourceManager.GetString(nameof (ConsumerName), BambooConsumerResources.resourceCulture);

    internal static string QueueBuildAction_DetailedDescriptionFormat => BambooConsumerResources.ResourceManager.GetString(nameof (QueueBuildAction_DetailedDescriptionFormat), BambooConsumerResources.resourceCulture);

    internal static string QueueBuildAction_PlanNameInputDescription => BambooConsumerResources.ResourceManager.GetString(nameof (QueueBuildAction_PlanNameInputDescription), BambooConsumerResources.resourceCulture);

    internal static string QueueBuildAction_PlanNameInputName => BambooConsumerResources.ResourceManager.GetString(nameof (QueueBuildAction_PlanNameInputName), BambooConsumerResources.resourceCulture);

    internal static string QueueBuildAction_QueryError_ExceptionFormat => BambooConsumerResources.ResourceManager.GetString(nameof (QueueBuildAction_QueryError_ExceptionFormat), BambooConsumerResources.resourceCulture);

    internal static string QueueBuildAction_QueryError_ResponseFailureFormat => BambooConsumerResources.ResourceManager.GetString(nameof (QueueBuildAction_QueryError_ResponseFailureFormat), BambooConsumerResources.resourceCulture);

    internal static string QueueBuildAction_QueryError_SuppliedCredentialsNotAuthorized => BambooConsumerResources.ResourceManager.GetString(nameof (QueueBuildAction_QueryError_SuppliedCredentialsNotAuthorized), BambooConsumerResources.resourceCulture);

    internal static string QueueBuildActionDescription => BambooConsumerResources.ResourceManager.GetString(nameof (QueueBuildActionDescription), BambooConsumerResources.resourceCulture);

    internal static string QueueBuildActionName => BambooConsumerResources.ResourceManager.GetString(nameof (QueueBuildActionName), BambooConsumerResources.resourceCulture);
  }
}
