// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.BoardViewResources
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
  public class BoardViewResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal BoardViewResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (BoardViewResources.resourceMan == null)
          BoardViewResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Agile.Resources.BoardsHub.BoardViewResources", typeof (BoardViewResources).Assembly);
        return BoardViewResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => BoardViewResources.resourceCulture;
      set => BoardViewResources.resourceCulture = value;
    }

    public static string AllTeamGroupHeader => BoardViewResources.ResourceManager.GetString(nameof (AllTeamGroupHeader), BoardViewResources.resourceCulture);

    public static string BacklogLevelLearningBubbleText => BoardViewResources.ResourceManager.GetString(nameof (BacklogLevelLearningBubbleText), BoardViewResources.resourceCulture);

    public static string BoardPivotDisplayName => BoardViewResources.ResourceManager.GetString(nameof (BoardPivotDisplayName), BoardViewResources.resourceCulture);

    public static string BoardsHeader_UnknownError => BoardViewResources.ResourceManager.GetString(nameof (BoardsHeader_UnknownError), BoardViewResources.resourceCulture);

    public static string BrowseAllBoards => BoardViewResources.ResourceManager.GetString(nameof (BrowseAllBoards), BoardViewResources.resourceCulture);

    public static string Directory_NoTeamsError => BoardViewResources.ResourceManager.GetString(nameof (Directory_NoTeamsError), BoardViewResources.resourceCulture);

    public static string EmbeddedBoardHeaderTitle => BoardViewResources.ResourceManager.GetString(nameof (EmbeddedBoardHeaderTitle), BoardViewResources.resourceCulture);

    public static string FavoriteGroupHeader => BoardViewResources.ResourceManager.GetString(nameof (FavoriteGroupHeader), BoardViewResources.resourceCulture);

    public static string LiveUpdates_Off => BoardViewResources.ResourceManager.GetString(nameof (LiveUpdates_Off), BoardViewResources.resourceCulture);

    public static string LiveUpdates_On => BoardViewResources.ResourceManager.GetString(nameof (LiveUpdates_On), BoardViewResources.resourceCulture);

    public static string LiveUpdates_Tooltip => BoardViewResources.ResourceManager.GetString(nameof (LiveUpdates_Tooltip), BoardViewResources.resourceCulture);

    public static string LoadingBoards => BoardViewResources.ResourceManager.GetString(nameof (LoadingBoards), BoardViewResources.resourceCulture);

    public static string MyTeamGroupHeader => BoardViewResources.ResourceManager.GetString(nameof (MyTeamGroupHeader), BoardViewResources.resourceCulture);

    public static string Navigation_PendingChanges => BoardViewResources.ResourceManager.GetString(nameof (Navigation_PendingChanges), BoardViewResources.resourceCulture);

    public static string SearchNoResultsText => BoardViewResources.ResourceManager.GetString(nameof (SearchNoResultsText), BoardViewResources.resourceCulture);

    public static string SearchResultsLoading => BoardViewResources.ResourceManager.GetString(nameof (SearchResultsLoading), BoardViewResources.resourceCulture);

    public static string SearchTextPlaceholder => BoardViewResources.ResourceManager.GetString(nameof (SearchTextPlaceholder), BoardViewResources.resourceCulture);

    public static string UnknownBoard => BoardViewResources.ResourceManager.GetString(nameof (UnknownBoard), BoardViewResources.resourceCulture);

    public static string ViewAsBacklog => BoardViewResources.ResourceManager.GetString(nameof (ViewAsBacklog), BoardViewResources.resourceCulture);
  }
}
