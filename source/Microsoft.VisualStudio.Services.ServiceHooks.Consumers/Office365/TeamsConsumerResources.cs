// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Office365.TeamsConsumerResources
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
  internal class TeamsConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal TeamsConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (TeamsConsumerResources.resourceMan == null)
          TeamsConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Office365.TeamsConsumerResources", typeof (TeamsConsumerResources).Assembly);
        return TeamsConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => TeamsConsumerResources.resourceCulture;
      set => TeamsConsumerResources.resourceCulture = value;
    }

    internal static string Teams_ConsumerDescription => TeamsConsumerResources.ResourceManager.GetString(nameof (Teams_ConsumerDescription), TeamsConsumerResources.resourceCulture);

    internal static string Teams_ConsumerName => TeamsConsumerResources.ResourceManager.GetString(nameof (Teams_ConsumerName), TeamsConsumerResources.resourceCulture);

    internal static string Teams_PostMessageToTeam_ActionDescription => TeamsConsumerResources.ResourceManager.GetString(nameof (Teams_PostMessageToTeam_ActionDescription), TeamsConsumerResources.resourceCulture);

    internal static string Teams_PostMessageToTeam_ActionName => TeamsConsumerResources.ResourceManager.GetString(nameof (Teams_PostMessageToTeam_ActionName), TeamsConsumerResources.resourceCulture);

    internal static string Teams_teamsUrl_ConsumerInputDescription => TeamsConsumerResources.ResourceManager.GetString(nameof (Teams_teamsUrl_ConsumerInputDescription), TeamsConsumerResources.resourceCulture);

    internal static string Teams_teamsUrl_ConsumerInputName => TeamsConsumerResources.ResourceManager.GetString(nameof (Teams_teamsUrl_ConsumerInputName), TeamsConsumerResources.resourceCulture);
  }
}
