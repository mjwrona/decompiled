// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.MobileCenter.MobileCenterConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.MobileCenter
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class MobileCenterConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal MobileCenterConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (MobileCenterConsumerResources.resourceMan == null)
          MobileCenterConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.MobileCenter.MobileCenterConsumerResources", typeof (MobileCenterConsumerResources).Assembly);
        return MobileCenterConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => MobileCenterConsumerResources.resourceCulture;
      set => MobileCenterConsumerResources.resourceCulture = value;
    }

    internal static string ConsumerDescription => MobileCenterConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), MobileCenterConsumerResources.resourceCulture);

    internal static string ConsumerName => MobileCenterConsumerResources.ResourceManager.GetString(nameof (ConsumerName), MobileCenterConsumerResources.resourceCulture);

    internal static string SendNotificationAction_InputUrlDescription => MobileCenterConsumerResources.ResourceManager.GetString(nameof (SendNotificationAction_InputUrlDescription), MobileCenterConsumerResources.resourceCulture);

    internal static string SendNotificationAction_InputUrlName => MobileCenterConsumerResources.ResourceManager.GetString(nameof (SendNotificationAction_InputUrlName), MobileCenterConsumerResources.resourceCulture);

    internal static string SendNotificationActionDescription => MobileCenterConsumerResources.ResourceManager.GetString(nameof (SendNotificationActionDescription), MobileCenterConsumerResources.resourceCulture);

    internal static string SendNotificationActionName => MobileCenterConsumerResources.ResourceManager.GetString(nameof (SendNotificationActionName), MobileCenterConsumerResources.resourceCulture);
  }
}
