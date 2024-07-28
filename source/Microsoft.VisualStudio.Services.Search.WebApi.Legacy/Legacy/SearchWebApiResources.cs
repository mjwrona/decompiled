// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchWebApiResources
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Legacy
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class SearchWebApiResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal SearchWebApiResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (SearchWebApiResources.resourceMan == null)
          SearchWebApiResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchWebApiResources", typeof (SearchWebApiResources).Assembly);
        return SearchWebApiResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => SearchWebApiResources.resourceCulture;
      set => SearchWebApiResources.resourceCulture = value;
    }

    public static string CustomRepositoryNotRegistered => SearchWebApiResources.ResourceManager.GetString(nameof (CustomRepositoryNotRegistered), SearchWebApiResources.resourceCulture);

    public static string CustomRepositoryShadowNotRegistered => SearchWebApiResources.ResourceManager.GetString(nameof (CustomRepositoryShadowNotRegistered), SearchWebApiResources.resourceCulture);

    public static string ElasticsearchUnavailableErrorMessage => SearchWebApiResources.ResourceManager.GetString(nameof (ElasticsearchUnavailableErrorMessage), SearchWebApiResources.resourceCulture);

    public static string InvalidCollectionProjectRepositoryCombination => SearchWebApiResources.ResourceManager.GetString(nameof (InvalidCollectionProjectRepositoryCombination), SearchWebApiResources.resourceCulture);

    public static string InvalidProjectFiltersCountMessage => SearchWebApiResources.ResourceManager.GetString(nameof (InvalidProjectFiltersCountMessage), SearchWebApiResources.resourceCulture);

    public static string InvalidProjectFilterValueMessage => SearchWebApiResources.ResourceManager.GetString(nameof (InvalidProjectFilterValueMessage), SearchWebApiResources.resourceCulture);

    public static string InvalidSkipResultsMessage => SearchWebApiResources.ResourceManager.GetString(nameof (InvalidSkipResultsMessage), SearchWebApiResources.resourceCulture);

    public static string MaxSearchTextLengthExceptionMessageFormat => SearchWebApiResources.ResourceManager.GetString(nameof (MaxSearchTextLengthExceptionMessageFormat), SearchWebApiResources.resourceCulture);

    public static string NullQueryMessage => SearchWebApiResources.ResourceManager.GetString(nameof (NullQueryMessage), SearchWebApiResources.resourceCulture);

    public static string RequiredMetadataMissing => SearchWebApiResources.ResourceManager.GetString(nameof (RequiredMetadataMissing), SearchWebApiResources.resourceCulture);

    public static string SearchNotConfiguredMessage => SearchWebApiResources.ResourceManager.GetString(nameof (SearchNotConfiguredMessage), SearchWebApiResources.resourceCulture);

    public static string SearchTextIsNotAdvancedMessage => SearchWebApiResources.ResourceManager.GetString(nameof (SearchTextIsNotAdvancedMessage), SearchWebApiResources.resourceCulture);

    public static string UnexpectedSearchErrorMessage => SearchWebApiResources.ResourceManager.GetString(nameof (UnexpectedSearchErrorMessage), SearchWebApiResources.resourceCulture);

    public static string UnsupportedCodeElementFilterMessageFormat => SearchWebApiResources.ResourceManager.GetString(nameof (UnsupportedCodeElementFilterMessageFormat), SearchWebApiResources.resourceCulture);
  }
}
