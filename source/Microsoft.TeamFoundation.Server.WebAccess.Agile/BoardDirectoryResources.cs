// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.BoardDirectoryResources
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
  public class BoardDirectoryResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal BoardDirectoryResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (BoardDirectoryResources.resourceMan == null)
          BoardDirectoryResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Agile.Resources.BoardsHub.BoardDirectoryResources", typeof (BoardDirectoryResources).Assembly);
        return BoardDirectoryResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => BoardDirectoryResources.resourceCulture;
      set => BoardDirectoryResources.resourceCulture = value;
    }

    public static string AllBoardsGrid_AriaLabel => BoardDirectoryResources.ResourceManager.GetString(nameof (AllBoardsGrid_AriaLabel), BoardDirectoryResources.resourceCulture);

    public static string Board => BoardDirectoryResources.ResourceManager.GetString(nameof (Board), BoardDirectoryResources.resourceCulture);

    public static string BoardFilteringAnnounceNoSearchResults => BoardDirectoryResources.ResourceManager.GetString(nameof (BoardFilteringAnnounceNoSearchResults), BoardDirectoryResources.resourceCulture);

    public static string BoardFilteringAnnounceSearchResultCount => BoardDirectoryResources.ResourceManager.GetString(nameof (BoardFilteringAnnounceSearchResultCount), BoardDirectoryResources.resourceCulture);

    public static string BoardHub_AllPivot => BoardDirectoryResources.ResourceManager.GetString(nameof (BoardHub_AllPivot), BoardDirectoryResources.resourceCulture);

    public static string Boards => BoardDirectoryResources.ResourceManager.GetString(nameof (Boards), BoardDirectoryResources.resourceCulture);

    public static string BoardsHub_MinePivot => BoardDirectoryResources.ResourceManager.GetString(nameof (BoardsHub_MinePivot), BoardDirectoryResources.resourceCulture);

    public static string BoardsNameColumnTitle => BoardDirectoryResources.ResourceManager.GetString(nameof (BoardsNameColumnTitle), BoardDirectoryResources.resourceCulture);

    public static string BoardsTeamColumnTitle => BoardDirectoryResources.ResourceManager.GetString(nameof (BoardsTeamColumnTitle), BoardDirectoryResources.resourceCulture);

    public static string DeletedBoardFavoriteInfoMessage => BoardDirectoryResources.ResourceManager.GetString(nameof (DeletedBoardFavoriteInfoMessage), BoardDirectoryResources.resourceCulture);

    public static string EmptyMyFavoritesGroupTextPrefix => BoardDirectoryResources.ResourceManager.GetString(nameof (EmptyMyFavoritesGroupTextPrefix), BoardDirectoryResources.resourceCulture);

    public static string EmptyMyFavoritesGroupTextSuffix => BoardDirectoryResources.ResourceManager.GetString(nameof (EmptyMyFavoritesGroupTextSuffix), BoardDirectoryResources.resourceCulture);

    public static string FavoritesGroupTitle => BoardDirectoryResources.ResourceManager.GetString(nameof (FavoritesGroupTitle), BoardDirectoryResources.resourceCulture);

    public static string KeywordFilter_PlaceholderText => BoardDirectoryResources.ResourceManager.GetString(nameof (KeywordFilter_PlaceholderText), BoardDirectoryResources.resourceCulture);

    public static string MyBoardsGrid_AriaLabel => BoardDirectoryResources.ResourceManager.GetString(nameof (MyBoardsGrid_AriaLabel), BoardDirectoryResources.resourceCulture);

    public static string TeamBoardsGroupTitle => BoardDirectoryResources.ResourceManager.GetString(nameof (TeamBoardsGroupTitle), BoardDirectoryResources.resourceCulture);

    public static string TeamFilter_PlaceholderText => BoardDirectoryResources.ResourceManager.GetString(nameof (TeamFilter_PlaceholderText), BoardDirectoryResources.resourceCulture);
  }
}
