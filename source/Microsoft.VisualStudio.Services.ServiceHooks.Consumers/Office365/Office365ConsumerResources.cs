// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Office365.Office365ConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Office365
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Office365ConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Office365ConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (Office365ConsumerResources.resourceMan == null)
          Office365ConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Office365.Office365ConsumerResources", typeof (Office365ConsumerResources).Assembly);
        return Office365ConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Office365ConsumerResources.resourceCulture;
      set => Office365ConsumerResources.resourceCulture = value;
    }

    internal static string Office365_ConsumerDescription => Office365ConsumerResources.ResourceManager.GetString(nameof (Office365_ConsumerDescription), Office365ConsumerResources.resourceCulture);

    internal static string Office365_ConsumerName => Office365ConsumerResources.ResourceManager.GetString(nameof (Office365_ConsumerName), Office365ConsumerResources.resourceCulture);

    internal static string Office365_OfficeConnectorUrl_ConsumerInputDescription => Office365ConsumerResources.ResourceManager.GetString(nameof (Office365_OfficeConnectorUrl_ConsumerInputDescription), Office365ConsumerResources.resourceCulture);

    internal static string Office365_OfficeConnectorUrl_ConsumerInputName => Office365ConsumerResources.ResourceManager.GetString(nameof (Office365_OfficeConnectorUrl_ConsumerInputName), Office365ConsumerResources.resourceCulture);

    internal static string Office365_PostMessageToGroup_ActionDescription => Office365ConsumerResources.ResourceManager.GetString(nameof (Office365_PostMessageToGroup_ActionDescription), Office365ConsumerResources.resourceCulture);

    internal static string Office365_PostMessageToGroup_ActionName => Office365ConsumerResources.ResourceManager.GetString(nameof (Office365_PostMessageToGroup_ActionName), Office365ConsumerResources.resourceCulture);
  }
}
