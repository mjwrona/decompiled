// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.MyGet.MyGetConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.MyGet
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class MyGetConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal MyGetConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (MyGetConsumerResources.resourceMan == null)
          MyGetConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.MyGet.MyGetConsumerResources", typeof (MyGetConsumerResources).Assembly);
        return MyGetConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => MyGetConsumerResources.resourceCulture;
      set => MyGetConsumerResources.resourceCulture = value;
    }

    internal static string MyGet_ConsumerDescription => MyGetConsumerResources.ResourceManager.GetString(nameof (MyGet_ConsumerDescription), MyGetConsumerResources.resourceCulture);

    internal static string MyGet_ConsumerName => MyGetConsumerResources.ResourceManager.GetString(nameof (MyGet_ConsumerName), MyGetConsumerResources.resourceCulture);

    internal static string MyGet_PublishPackage_ActionDescription => MyGetConsumerResources.ResourceManager.GetString(nameof (MyGet_PublishPackage_ActionDescription), MyGetConsumerResources.resourceCulture);

    internal static string MyGet_PublishPackage_ActionName => MyGetConsumerResources.ResourceManager.GetString(nameof (MyGet_PublishPackage_ActionName), MyGetConsumerResources.resourceCulture);

    internal static string MyGet_PublishPackage_FeedId_ActionInputDescription => MyGetConsumerResources.ResourceManager.GetString(nameof (MyGet_PublishPackage_FeedId_ActionInputDescription), MyGetConsumerResources.resourceCulture);

    internal static string MyGet_PublishPackage_FeedId_ActionInputName => MyGetConsumerResources.ResourceManager.GetString(nameof (MyGet_PublishPackage_FeedId_ActionInputName), MyGetConsumerResources.resourceCulture);

    internal static string Myget_PublishPackage_PackageSourceId_ActionInputDescritption => MyGetConsumerResources.ResourceManager.GetString(nameof (Myget_PublishPackage_PackageSourceId_ActionInputDescritption), MyGetConsumerResources.resourceCulture);

    internal static string Myget_PublishPackage_PackageSourceId_ActionInputName => MyGetConsumerResources.ResourceManager.GetString(nameof (Myget_PublishPackage_PackageSourceId_ActionInputName), MyGetConsumerResources.resourceCulture);

    internal static string MyGet_TriggerBuild_ActionDescription => MyGetConsumerResources.ResourceManager.GetString(nameof (MyGet_TriggerBuild_ActionDescription), MyGetConsumerResources.resourceCulture);

    internal static string MyGet_TriggerBuild_ActionName => MyGetConsumerResources.ResourceManager.GetString(nameof (MyGet_TriggerBuild_ActionName), MyGetConsumerResources.resourceCulture);

    internal static string Myget_TriggerBuild_BuildSourceId_ActionInputDescritption => MyGetConsumerResources.ResourceManager.GetString(nameof (Myget_TriggerBuild_BuildSourceId_ActionInputDescritption), MyGetConsumerResources.resourceCulture);

    internal static string Myget_TriggerBuild_BuildSourceId_ActionInputName => MyGetConsumerResources.ResourceManager.GetString(nameof (Myget_TriggerBuild_BuildSourceId_ActionInputName), MyGetConsumerResources.resourceCulture);

    internal static string MyGet_TriggerBuild_FeedId_ActionInputDescription => MyGetConsumerResources.ResourceManager.GetString(nameof (MyGet_TriggerBuild_FeedId_ActionInputDescription), MyGetConsumerResources.resourceCulture);

    internal static string MyGet_TriggerBuild_FeedId_ActionInputName => MyGetConsumerResources.ResourceManager.GetString(nameof (MyGet_TriggerBuild_FeedId_ActionInputName), MyGetConsumerResources.resourceCulture);
  }
}
