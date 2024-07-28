// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Zapier.ZapierConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Zapier
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class ZapierConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ZapierConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ZapierConsumerResources.resourceMan == null)
          ZapierConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Zapier.ZapierConsumerResources", typeof (ZapierConsumerResources).Assembly);
        return ZapierConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ZapierConsumerResources.resourceCulture;
      set => ZapierConsumerResources.resourceCulture = value;
    }

    internal static string ConsumerDescription => ZapierConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), ZapierConsumerResources.resourceCulture);

    internal static string ConsumerName => ZapierConsumerResources.ResourceManager.GetString(nameof (ConsumerName), ZapierConsumerResources.resourceCulture);

    internal static string SendNotificationAction_InputUrlDescription => ZapierConsumerResources.ResourceManager.GetString(nameof (SendNotificationAction_InputUrlDescription), ZapierConsumerResources.resourceCulture);

    internal static string SendNotificationAction_InputUrlName => ZapierConsumerResources.ResourceManager.GetString(nameof (SendNotificationAction_InputUrlName), ZapierConsumerResources.resourceCulture);

    internal static string SendNotificationActionDescription => ZapierConsumerResources.ResourceManager.GetString(nameof (SendNotificationActionDescription), ZapierConsumerResources.resourceCulture);

    internal static string SendNotificationActionName => ZapierConsumerResources.ResourceManager.GetString(nameof (SendNotificationActionName), ZapierConsumerResources.resourceCulture);
  }
}
