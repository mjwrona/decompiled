// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.TeamPanelResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class TeamPanelResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal TeamPanelResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (TeamPanelResources.resourceMan == null)
          TeamPanelResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Agile.Resources.TeamPanelResources", typeof (TeamPanelResources).Assembly);
        return TeamPanelResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => TeamPanelResources.resourceCulture;
      set => TeamPanelResources.resourceCulture = value;
    }

    internal static string AdminGroupHeader => TeamPanelResources.ResourceManager.GetString(nameof (AdminGroupHeader), TeamPanelResources.resourceCulture);

    internal static string AllITemsFilterText => TeamPanelResources.ResourceManager.GetString(nameof (AllITemsFilterText), TeamPanelResources.resourceCulture);

    internal static string ShowTeamProfile => TeamPanelResources.ResourceManager.GetString(nameof (ShowTeamProfile), TeamPanelResources.resourceCulture);

    internal static string TeamMemberCountFormat => TeamPanelResources.ResourceManager.GetString(nameof (TeamMemberCountFormat), TeamPanelResources.resourceCulture);

    internal static string TeamPanel_Close => TeamPanelResources.ResourceManager.GetString(nameof (TeamPanel_Close), TeamPanelResources.resourceCulture);

    internal static string TeamPanel_ItemsPivot => TeamPanelResources.ResourceManager.GetString(nameof (TeamPanel_ItemsPivot), TeamPanelResources.resourceCulture);

    internal static string TeamPanel_Member => TeamPanelResources.ResourceManager.GetString(nameof (TeamPanel_Member), TeamPanelResources.resourceCulture);

    internal static string TeamSettingLabel => TeamPanelResources.ResourceManager.GetString(nameof (TeamSettingLabel), TeamPanelResources.resourceCulture);
  }
}
