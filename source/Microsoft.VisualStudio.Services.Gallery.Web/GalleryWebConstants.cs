// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.GalleryWebConstants
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public static class GalleryWebConstants
  {
    public const string PublishExtensionJobName = "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.PublishExtensionJob";
    public const string VSCodeExtensionInstallCommand = "ext install {0}";

    public static class Products
    {
      public const string VsCode = "vscode";
    }

    public static class WebFeatureFlags
    {
      public const string ShowVSItemLink = "Microsoft.VisualStudio.Services.Gallery.ShowVSItemLink";
      public const string ShowVs2017Banner = "Microsoft.VisualStudio.Services.Gallery.ShowVs2017Banner";
      public const string TileImpressionsHomepage = "Microsoft.VisualStudio.Services.Gallery.TileImpressionsHomePage";
      public const string ShowPublishExtensions = "Microsoft.VisualStudio.Services.Gallery.ShowPublishExtensions";
      public const string VsTrendingHomepage = "Microsoft.VisualStudio.Services.Gallery.VsTrendingHomepage";
      public const string Vs2019Homepage = "Microsoft.VisualStudio.Services.Gallery.Vs2019Homepage";
      public const string RemoveDuplicateHomepageExtensions = "Microsoft.VisualStudio.Services.Gallery.DedupeHomepageExtensions";
      public const string EnableVsForMac = "Microsoft.VisualStudio.Services.Gallery.EnableVsForMac";
      public const string EnableCertifiedPublisherUIChanges = "Microsoft.VisualStudio.Services.Gallery.EnableCertifiedPublisherUIChanges";
      public const string EnableHiddenFlagAddition = "Microsoft.VisualStudio.Services.Gallery.EnableHiddenFlagAddition";
      public const string EnableQueriesBasedOnHiddenFlags = "Microsoft.VisualStudio.Services.Gallery.EnableQueriesBasedOnHiddenFlags";
      public const string UsePublicAccessMappingMoniker = "VisualStudio.Services.WebPlatform.UsePublicAccessMappingMoniker";
      public const string MarketplaceBrandingChanges = "VisualStudio.Services.WebPlatform.UseNewBranding";
      public const string UseNewDomainUrlInShareDropdown = "Microsoft.VisualStudio.Services.Gallery.UseNewDomainUrlInShareDropdown";
      public const string EnableVersionHistoryDownload = "Microsoft.VisualStudio.Services.Gallery.EnableVersionHistoryDownload";
      public const string EnableItemDetailsPageServerSideRendering = "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsPageServerSideRendering";
      public const string EnableNoFilterSearchHomepageVSIDE = "Microsoft.VisualStudio.Services.Gallery.EnableNoFilterSearchHomepageVSIDE";
      public const string EnableItemDetailsAFDCaching = "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsAFDCaching";
      public const string EnableItemDetailsAFDCachingForVSCode = "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsAFDCachingForAllVSCode";
      public const string EnableItemDetailsSSRForVsCode = "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsSSRForVsCode";
      public const string EnableItemDetailsSSRForVsIDE = "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsSSRForVsIDE";
      public const string EnableItemDetailsSSRForAzDev = "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsSSRForAzDev";
      public const string EnableItemDetailsPageOverviewCache = "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsPageOverviewCache";
      public const string EnableDelayOverviewSSR = "Microsoft.VisualStudio.Services.Gallery.EnableDelayOverviewSSR";
      public const string EnableSSRForHomepageVSCode = "Microsoft.VisualStudio.Services.Gallery.EnableSSRForHomepageVSCode";
      public const string EnableSSRForHomepageVSIDE = "Microsoft.VisualStudio.Services.Gallery.EnableSSRForHomepageVSIDE";
      public const string EnableSSRForHomepageAzDev = "Microsoft.VisualStudio.Services.Gallery.EnableSSRForHomepageAzDev";
      public const string EnableSimpleMarkdownRenderingForSSR = "Microsoft.VisualStudio.Services.Gallery.EnableSimpleMarkdownRenderingForSSR";
      public const string EnableItemDetailsAFDCachingForVSCodeHomePage = "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsAFDCachingForVSCodeHomePage";
      public const string EnableRHSAsyncComponents = "Microsoft.VisualStudio.Services.Gallery.EnableRHSAsyncComponents";
      public const string EnableSSRForPaidAzDev = "Microsoft.VisualStudio.Services.Gallery.EnableSSRForPaidAzDev";
      public const string EnableSSRForConnectedContext = "Microsoft.VisualStudio.Services.Gallery.EnableSSRForConnectedContext";
      public const string EnableSSRForPrivateExtensions = "Microsoft.VisualStudio.Services.Gallery.EnableSSRForPrivateExtensions";
      public const string EnableSSRForDetailsTabs = "Microsoft.VisualStudio.Services.Gallery.EnableSSRForDetailsTabs";
    }

    public static class ConnectedInstallContext
    {
      public static readonly string ItemName = "itemName";
      public static readonly string ItemDetailsLink = "itemUrl";
    }

    public static class UrlParams
    {
      public const string ServerKey = "serverKey";
      public const string NewAzureSub = "newAzureSub";
    }

    public static class ProductExtensionConstants
    {
      public const int MaxItemPerCategory = 18;
      public const int MaxItemPerCategoryIncludeExtra = 24;
      public const int NumberOfItemsDisplayed = 6;
    }
  }
}
