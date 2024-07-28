// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.AgileProductBacklogServerResources
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
  public class AgileProductBacklogServerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal AgileProductBacklogServerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (AgileProductBacklogServerResources.resourceMan == null)
          AgileProductBacklogServerResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Agile.Resources.AgileProductBacklogServerResources", typeof (AgileProductBacklogServerResources).Assembly);
        return AgileProductBacklogServerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => AgileProductBacklogServerResources.resourceCulture;
      set => AgileProductBacklogServerResources.resourceCulture = value;
    }

    public static string Backlog_Bugs => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (Backlog_Bugs), AgileProductBacklogServerResources.resourceCulture);

    public static string Backlog_InProgress_Hide => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (Backlog_InProgress_Hide), AgileProductBacklogServerResources.resourceCulture);

    public static string Backlog_InProgress_Show => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (Backlog_InProgress_Show), AgileProductBacklogServerResources.resourceCulture);

    public static string Backlog_OFF => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (Backlog_OFF), AgileProductBacklogServerResources.resourceCulture);

    public static string Backlog_ON => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (Backlog_ON), AgileProductBacklogServerResources.resourceCulture);

    public static string Backlog_Parents_Hide => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (Backlog_Parents_Hide), AgileProductBacklogServerResources.resourceCulture);

    public static string Backlog_Parents_Show => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (Backlog_Parents_Show), AgileProductBacklogServerResources.resourceCulture);

    public static string BacklogContextError_Message1 => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (BacklogContextError_Message1), AgileProductBacklogServerResources.resourceCulture);

    public static string BacklogContextError_Message2 => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (BacklogContextError_Message2), AgileProductBacklogServerResources.resourceCulture);

    public static string BacklogContextError_Message3 => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (BacklogContextError_Message3), AgileProductBacklogServerResources.resourceCulture);

    public static string BacklogContextError_Message4 => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (BacklogContextError_Message4), AgileProductBacklogServerResources.resourceCulture);

    public static string BacklogContextError_Title => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (BacklogContextError_Title), AgileProductBacklogServerResources.resourceCulture);

    public static string BugsOnBacklogProposedStateMappingDocLink => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (BugsOnBacklogProposedStateMappingDocLink), AgileProductBacklogServerResources.resourceCulture);

    public static string MissingProposedStateMappingForBugsWarning => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (MissingProposedStateMappingForBugsWarning), AgileProductBacklogServerResources.resourceCulture);

    public static string PortfolioManagementServiceStatusMessage => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (PortfolioManagementServiceStatusMessage), AgileProductBacklogServerResources.resourceCulture);

    public static string ProductBacklog_Reparent_Toast_2 => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (ProductBacklog_Reparent_Toast_2), AgileProductBacklogServerResources.resourceCulture);

    public static string ProductBacklogSearch => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (ProductBacklogSearch), AgileProductBacklogServerResources.resourceCulture);

    public static string ProductBacklogSearch_NotDoneYet => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (ProductBacklogSearch_NotDoneYet), AgileProductBacklogServerResources.resourceCulture);

    public static string ProductBacklogSearch_Over1000 => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (ProductBacklogSearch_Over1000), AgileProductBacklogServerResources.resourceCulture);

    public static string ProductBacklogSearchToolTip => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (ProductBacklogSearchToolTip), AgileProductBacklogServerResources.resourceCulture);

    public static string ReorderDialogError_ReorderWithChild => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (ReorderDialogError_ReorderWithChild), AgileProductBacklogServerResources.resourceCulture);

    public static string Title_BacklogFilter_Filter => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (Title_BacklogFilter_Filter), AgileProductBacklogServerResources.resourceCulture);

    public static string Tooltip_BacklogFilter_Filter_DrillDown => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (Tooltip_BacklogFilter_Filter_DrillDown), AgileProductBacklogServerResources.resourceCulture);

    public static string Tooltip_BacklogFilter_Filter_DrillDown_View => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (Tooltip_BacklogFilter_Filter_DrillDown_View), AgileProductBacklogServerResources.resourceCulture);

    public static string Tooltip_BacklogFilter_Filter_DrillUp => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (Tooltip_BacklogFilter_Filter_DrillUp), AgileProductBacklogServerResources.resourceCulture);

    public static string Tooltip_BacklogFilter_Filter_DrillUp_View => AgileProductBacklogServerResources.ResourceManager.GetString(nameof (Tooltip_BacklogFilter_Filter_DrillUp_View), AgileProductBacklogServerResources.resourceCulture);
  }
}
