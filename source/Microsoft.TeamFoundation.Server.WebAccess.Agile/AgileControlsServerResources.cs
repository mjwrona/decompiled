// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.AgileControlsServerResources
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
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class AgileControlsServerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal AgileControlsServerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (AgileControlsServerResources.resourceMan == null)
          AgileControlsServerResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Agile.Resources.AgileControlsServerResources", typeof (AgileControlsServerResources).Assembly);
        return AgileControlsServerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => AgileControlsServerResources.resourceCulture;
      set => AgileControlsServerResources.resourceCulture = value;
    }

    public static string Board_InProgressLimitReached => AgileControlsServerResources.ResourceManager.GetString(nameof (Board_InProgressLimitReached), AgileControlsServerResources.resourceCulture);

    public static string Board_QueryLimitExceeded => AgileControlsServerResources.ResourceManager.GetString(nameof (Board_QueryLimitExceeded), AgileControlsServerResources.resourceCulture);

    public static string Board_Toolbar_Customize_Columns => AgileControlsServerResources.ResourceManager.GetString(nameof (Board_Toolbar_Customize_Columns), AgileControlsServerResources.resourceCulture);

    public static string BoardCard_ContextMenu_DeleteWorkItem => AgileControlsServerResources.ResourceManager.GetString(nameof (BoardCard_ContextMenu_DeleteWorkItem), AgileControlsServerResources.resourceCulture);

    public static string CAPACITY_ADD_PANEL_CLOSE_BUTTON_TEXT => AgileControlsServerResources.ResourceManager.GetString(nameof (CAPACITY_ADD_PANEL_CLOSE_BUTTON_TEXT), AgileControlsServerResources.resourceCulture);

    public static string Capacity_AddPanel_Add => AgileControlsServerResources.ResourceManager.GetString(nameof (Capacity_AddPanel_Add), AgileControlsServerResources.resourceCulture);

    public static string Capacity_AddPanel_Title => AgileControlsServerResources.ResourceManager.GetString(nameof (Capacity_AddPanel_Title), AgileControlsServerResources.resourceCulture);

    public static string Capacity_AssigningLicense => AgileControlsServerResources.ResourceManager.GetString(nameof (Capacity_AssigningLicense), AgileControlsServerResources.resourceCulture);

    public static string Card_Loading => AgileControlsServerResources.ResourceManager.GetString(nameof (Card_Loading), AgileControlsServerResources.resourceCulture);

    public static string InvalidChartOptions => AgileControlsServerResources.ResourceManager.GetString(nameof (InvalidChartOptions), AgileControlsServerResources.resourceCulture);

    public static string InvalidVelocityChartIterations => AgileControlsServerResources.ResourceManager.GetString(nameof (InvalidVelocityChartIterations), AgileControlsServerResources.resourceCulture);

    public static string IterationPathIsNull => AgileControlsServerResources.ResourceManager.GetString(nameof (IterationPathIsNull), AgileControlsServerResources.resourceCulture);
  }
}
