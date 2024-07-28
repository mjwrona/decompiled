// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.HockeyApp.HockeyAppConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.HockeyApp
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class HockeyAppConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal HockeyAppConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (HockeyAppConsumerResources.resourceMan == null)
          HockeyAppConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.HockeyApp.HockeyAppConsumerResources", typeof (HockeyAppConsumerResources).Assembly);
        return HockeyAppConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => HockeyAppConsumerResources.resourceCulture;
      set => HockeyAppConsumerResources.resourceCulture = value;
    }

    internal static string ConsumerDescription => HockeyAppConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), HockeyAppConsumerResources.resourceCulture);

    internal static string ConsumerName => HockeyAppConsumerResources.ResourceManager.GetString(nameof (ConsumerName), HockeyAppConsumerResources.resourceCulture);

    internal static string SendNotificationAction_InputUrlDescription => HockeyAppConsumerResources.ResourceManager.GetString(nameof (SendNotificationAction_InputUrlDescription), HockeyAppConsumerResources.resourceCulture);

    internal static string SendNotificationAction_InputUrlName => HockeyAppConsumerResources.ResourceManager.GetString(nameof (SendNotificationAction_InputUrlName), HockeyAppConsumerResources.resourceCulture);

    internal static string SendNotificationActionDescription => HockeyAppConsumerResources.ResourceManager.GetString(nameof (SendNotificationActionDescription), HockeyAppConsumerResources.resourceCulture);

    internal static string SendNotificationActionName => HockeyAppConsumerResources.ResourceManager.GetString(nameof (SendNotificationActionName), HockeyAppConsumerResources.resourceCulture);
  }
}
