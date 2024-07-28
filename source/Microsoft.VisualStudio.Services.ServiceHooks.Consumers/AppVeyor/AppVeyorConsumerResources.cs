// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.AppVeyor.AppVeyorConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.AppVeyor
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class AppVeyorConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal AppVeyorConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (AppVeyorConsumerResources.resourceMan == null)
          AppVeyorConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.AppVeyor.AppVeyorConsumerResources", typeof (AppVeyorConsumerResources).Assembly);
        return AppVeyorConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => AppVeyorConsumerResources.resourceCulture;
      set => AppVeyorConsumerResources.resourceCulture = value;
    }

    internal static string AppVeyor_ConsumerDescription => AppVeyorConsumerResources.ResourceManager.GetString(nameof (AppVeyor_ConsumerDescription), AppVeyorConsumerResources.resourceCulture);

    internal static string AppVeyor_ConsumerName => AppVeyorConsumerResources.ResourceManager.GetString(nameof (AppVeyor_ConsumerName), AppVeyorConsumerResources.resourceCulture);

    internal static string AppVeyor_TriggerBuild_ActionDescription => AppVeyorConsumerResources.ResourceManager.GetString(nameof (AppVeyor_TriggerBuild_ActionDescription), AppVeyorConsumerResources.resourceCulture);

    internal static string AppVeyor_TriggerBuild_ActionName => AppVeyorConsumerResources.ResourceManager.GetString(nameof (AppVeyor_TriggerBuild_ActionName), AppVeyorConsumerResources.resourceCulture);

    internal static string AppVeyor_TriggerBuild_ProjectName_ActionInputDescription => AppVeyorConsumerResources.ResourceManager.GetString(nameof (AppVeyor_TriggerBuild_ProjectName_ActionInputDescription), AppVeyorConsumerResources.resourceCulture);

    internal static string AppVeyor_TriggerBuild_ProjectName_ActionInputName => AppVeyorConsumerResources.ResourceManager.GetString(nameof (AppVeyor_TriggerBuild_ProjectName_ActionInputName), AppVeyorConsumerResources.resourceCulture);

    internal static string AppVeyor_TriggerBuild_WebHookId_ActionInputDescription => AppVeyorConsumerResources.ResourceManager.GetString(nameof (AppVeyor_TriggerBuild_WebHookId_ActionInputDescription), AppVeyorConsumerResources.resourceCulture);

    internal static string AppVeyor_TriggerBuild_WebHookId_ActionInputName => AppVeyorConsumerResources.ResourceManager.GetString(nameof (AppVeyor_TriggerBuild_WebHookId_ActionInputName), AppVeyorConsumerResources.resourceCulture);
  }
}
