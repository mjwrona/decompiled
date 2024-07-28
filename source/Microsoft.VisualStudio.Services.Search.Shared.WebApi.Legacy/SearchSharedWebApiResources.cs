// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.SearchSharedWebApiResources
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class SearchSharedWebApiResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal SearchSharedWebApiResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (SearchSharedWebApiResources.resourceMan == null)
          SearchSharedWebApiResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.SearchSharedWebApiResources", typeof (SearchSharedWebApiResources).Assembly);
        return SearchSharedWebApiResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => SearchSharedWebApiResources.resourceCulture;
      set => SearchSharedWebApiResources.resourceCulture = value;
    }

    public static string DuplicateFilterNameMessage => SearchSharedWebApiResources.ResourceManager.GetString(nameof (DuplicateFilterNameMessage), SearchSharedWebApiResources.resourceCulture);

    public static string EmptySortOptionInOrderByMessage => SearchSharedWebApiResources.ResourceManager.GetString(nameof (EmptySortOptionInOrderByMessage), SearchSharedWebApiResources.resourceCulture);

    public static string EmptySortOrderGivenInSortOptionsMessageFormat => SearchSharedWebApiResources.ResourceManager.GetString(nameof (EmptySortOrderGivenInSortOptionsMessageFormat), SearchSharedWebApiResources.resourceCulture);

    public static string EmptyVersionNotAllowed => SearchSharedWebApiResources.ResourceManager.GetString(nameof (EmptyVersionNotAllowed), SearchSharedWebApiResources.resourceCulture);

    public static string Error_InvalidVersion => SearchSharedWebApiResources.ResourceManager.GetString(nameof (Error_InvalidVersion), SearchSharedWebApiResources.resourceCulture);

    public static string Error_PackageVersionExceedsMaximumLength => SearchSharedWebApiResources.ResourceManager.GetString(nameof (Error_PackageVersionExceedsMaximumLength), SearchSharedWebApiResources.resourceCulture);

    public static string ExtensionNotActiveMessage => SearchSharedWebApiResources.ResourceManager.GetString(nameof (ExtensionNotActiveMessage), SearchSharedWebApiResources.resourceCulture);

    public static string FieldSpecifiedMoreThanOnceInTheSortOptionsMessageFormat => SearchSharedWebApiResources.ResourceManager.GetString(nameof (FieldSpecifiedMoreThanOnceInTheSortOptionsMessageFormat), SearchSharedWebApiResources.resourceCulture);

    public static string InvalidFilterValueMessage => SearchSharedWebApiResources.ResourceManager.GetString(nameof (InvalidFilterValueMessage), SearchSharedWebApiResources.resourceCulture);

    public static string InvalidTakeResultsMessage => SearchSharedWebApiResources.ResourceManager.GetString(nameof (InvalidTakeResultsMessage), SearchSharedWebApiResources.resourceCulture);

    public static string InvalidVisibilityFilterValueMessageFormat => SearchSharedWebApiResources.ResourceManager.GetString(nameof (InvalidVisibilityFilterValueMessageFormat), SearchSharedWebApiResources.resourceCulture);

    public static string NullFilterValuesMessage => SearchSharedWebApiResources.ResourceManager.GetString(nameof (NullFilterValuesMessage), SearchSharedWebApiResources.resourceCulture);

    public static string NullOrEmptyEntityTypeMessage => SearchSharedWebApiResources.ResourceManager.GetString(nameof (NullOrEmptyEntityTypeMessage), SearchSharedWebApiResources.resourceCulture);

    public static string NullOrEmptyFilterNameMessage => SearchSharedWebApiResources.ResourceManager.GetString(nameof (NullOrEmptyFilterNameMessage), SearchSharedWebApiResources.resourceCulture);

    public static string NullOrEmptySearchTextMessage => SearchSharedWebApiResources.ResourceManager.GetString(nameof (NullOrEmptySearchTextMessage), SearchSharedWebApiResources.resourceCulture);

    public static string ParentFilterHasMultipleValuesMessageFormat => SearchSharedWebApiResources.ResourceManager.GetString(nameof (ParentFilterHasMultipleValuesMessageFormat), SearchSharedWebApiResources.resourceCulture);

    public static string ParentFilterNotFoundMessageFormat => SearchSharedWebApiResources.ResourceManager.GetString(nameof (ParentFilterNotFoundMessageFormat), SearchSharedWebApiResources.resourceCulture);

    public static string SortOptionFieldNameShouldNotBeEmptyOrNullMessage => SearchSharedWebApiResources.ResourceManager.GetString(nameof (SortOptionFieldNameShouldNotBeEmptyOrNullMessage), SearchSharedWebApiResources.resourceCulture);

    public static string UnknownFilterMessageFormat => SearchSharedWebApiResources.ResourceManager.GetString(nameof (UnknownFilterMessageFormat), SearchSharedWebApiResources.resourceCulture);

    public static string UnsupportedEntityMesssage => SearchSharedWebApiResources.ResourceManager.GetString(nameof (UnsupportedEntityMesssage), SearchSharedWebApiResources.resourceCulture);
  }
}
