// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.AgileEngagementServerResources
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
  public class AgileEngagementServerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal AgileEngagementServerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (AgileEngagementServerResources.resourceMan == null)
          AgileEngagementServerResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Agile.Resources.AgileEngagementServerResources", typeof (AgileEngagementServerResources).Assembly);
        return AgileEngagementServerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => AgileEngagementServerResources.resourceCulture;
      set => AgileEngagementServerResources.resourceCulture = value;
    }

    public static string GoodToExploreCancel => AgileEngagementServerResources.ResourceManager.GetString(nameof (GoodToExploreCancel), AgileEngagementServerResources.resourceCulture);

    public static string GoodToExploreTitle => AgileEngagementServerResources.ResourceManager.GetString(nameof (GoodToExploreTitle), AgileEngagementServerResources.resourceCulture);

    public static string KanbanFinishedContent2 => AgileEngagementServerResources.ResourceManager.GetString(nameof (KanbanFinishedContent2), AgileEngagementServerResources.resourceCulture);

    public static string KanbanFinishedOk => AgileEngagementServerResources.ResourceManager.GetString(nameof (KanbanFinishedOk), AgileEngagementServerResources.resourceCulture);

    public static string KanbanWaitForItem2Content => AgileEngagementServerResources.ResourceManager.GetString(nameof (KanbanWaitForItem2Content), AgileEngagementServerResources.resourceCulture);

    public static string Next => AgileEngagementServerResources.ResourceManager.GetString(nameof (Next), AgileEngagementServerResources.resourceCulture);

    public static string SettingsButton => AgileEngagementServerResources.ResourceManager.GetString(nameof (SettingsButton), AgileEngagementServerResources.resourceCulture);
  }
}
