// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.HostedGalleryPageMetadataHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  internal class HostedGalleryPageMetadataHelper : IGalleryPageMetadataHelper
  {
    private string RegistryPathForAddingNoIndexTagTimeline_InDays = "/Configuration/Service/Gallery/ExtensionValidation/RegistryPathForAddingNoIndexTagTimeline_InDays";
    private int DefaultTimeLine_InDaysForNoIndexTag = 7;

    public HostedGalleryPageMetadataHelper() => this.MetadataHelperUtils = new PageMetadataHelperUtilities();

    public string GetHomePageTitle() => string.Format(GalleryCommonResources.HomePageTitle, (object) GalleryCommonResources.ExtensionsForVisualStudio, (object) GalleryCommonResources.VisualStudioMarketplace);

    public string GetHomePageMetadata(PageMetadataInputs pageMetadataInputs)
    {
      List<HtmlSelfClosingGenericControl> htmlGenericControls = new List<HtmlSelfClosingGenericControl>();
      if (pageMetadataInputs != null)
      {
        HtmlSelfClosingGenericControl closingGenericControl1 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl1.Attributes.Add("name", "description");
        closingGenericControl1.Attributes.Add("content", GalleryResources.HomePageMetaDescription);
        htmlGenericControls.Add(closingGenericControl1);
        HtmlSelfClosingGenericControl closingGenericControl2 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl2.Attributes.Add("name", "keywords");
        closingGenericControl2.Attributes.Add("content", GalleryCommonResources.Extension + ", " + GalleryCommonResources.VisualStudioMarketplace + ", " + GalleryCommonResources.VS_Header + ", " + GalleryCommonResources.VSCode_Header + ", " + GalleryCommonResources.Subs_Header + ", " + GalleryCommonResources.VSTS_Header + ", " + GalleryCommonResources.TeamServices + ", TFS, " + GalleryCommonResources.TeamFoundationServerExtensions);
        htmlGenericControls.Add(closingGenericControl2);
        HtmlSelfClosingGenericControl closingGenericControl3 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl3.Attributes.Add("property", "og:title");
        closingGenericControl3.Attributes.Add("content", GalleryCommonResources.VisualStudioMarketplace);
        htmlGenericControls.Add(closingGenericControl3);
        HtmlSelfClosingGenericControl closingGenericControl4 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl4.Attributes.Add("property", "og:type");
        closingGenericControl4.Attributes.Add("content", "website");
        htmlGenericControls.Add(closingGenericControl4);
        HtmlSelfClosingGenericControl closingGenericControl5 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl5.Attributes.Add("property", "og:url");
        closingGenericControl5.Attributes.Add("content", pageMetadataInputs.Url);
        htmlGenericControls.Add(closingGenericControl5);
        HtmlSelfClosingGenericControl closingGenericControl6 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl6.Attributes.Add("property", "og:image");
        closingGenericControl6.Attributes.Add("content", this.GetMarketplaceIconUrl(pageMetadataInputs.ResourcesPath));
        htmlGenericControls.Add(closingGenericControl6);
        HtmlSelfClosingGenericControl closingGenericControl7 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl7.Attributes.Add("property", "og:description");
        closingGenericControl7.Attributes.Add("content", GalleryResources.HomePageOpenGraphDescription);
        htmlGenericControls.Add(closingGenericControl7);
        HtmlSelfClosingGenericControl closingGenericControl8 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl8.Attributes.Add("property", "twitter:card");
        closingGenericControl8.Attributes.Add("content", "summary");
        htmlGenericControls.Add(closingGenericControl8);
        HtmlSelfClosingGenericControl closingGenericControl9 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl9.Attributes.Add("property", "twitter:site");
        closingGenericControl9.Attributes.Add("content", this.GetTwitterSiteHandleForProduct(pageMetadataInputs.Product));
        htmlGenericControls.Add(closingGenericControl9);
        HtmlSelfClosingGenericControl closingGenericControl10 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl10.Attributes.Add("property", "twitter:creator");
        closingGenericControl10.Attributes.Add("content", "@VisualStudio");
        htmlGenericControls.Add(closingGenericControl10);
      }
      return this.MetadataHelperUtils.GetHtmlStringFromHtmlGenericControls(htmlGenericControls);
    }

    public string GetItemDetailsPageTitle(PageMetadataInputs pageMetadataInputs)
    {
      string detailsPageTitle = GalleryCommonResources.VisualStudioMarketplace;
      if (pageMetadataInputs != null && pageMetadataInputs.Extension != null && !pageMetadataInputs.Extension.DisplayName.IsNullOrEmpty<char>())
        detailsPageTitle = string.Format(GalleryResources.ItemDetailsPageTitle, (object) pageMetadataInputs.Extension.DisplayName);
      return detailsPageTitle;
    }

    private bool AddNoIndexTag(IVssRequestContext tfsRequestContext, PublishedExtension extension) => extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Unpublished) || !(DateTime.UtcNow.AddDays((double) -tfsRequestContext.GetService<IVssRegistryService>().GetValue<int>(tfsRequestContext, (RegistryQuery) this.RegistryPathForAddingNoIndexTagTimeline_InDays, this.DefaultTimeLine_InDaysForNoIndexTag)) >= extension.LastUpdated);

    public string GetItemDetailsPageMetadata(
      PageMetadataInputs pageMetadataInputs,
      IVssRequestContext tfsRequestContext)
    {
      List<HtmlSelfClosingGenericControl> htmlGenericControls = new List<HtmlSelfClosingGenericControl>();
      if (pageMetadataInputs != null)
      {
        HtmlSelfClosingGenericControl closingGenericControl1 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl1.Attributes.Add("name", "description");
        closingGenericControl1.Attributes.Add("content", this.GetItemDetailsMetaDescriptionContentsFromExtension(pageMetadataInputs.Extension));
        htmlGenericControls.Add(closingGenericControl1);
        if (tfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.NoIndexTagForOverviewContent") && this.AddNoIndexTag(tfsRequestContext, pageMetadataInputs.Extension))
        {
          HtmlSelfClosingGenericControl closingGenericControl2 = new HtmlSelfClosingGenericControl("meta");
          closingGenericControl1.Attributes.Add("name", "robots");
          closingGenericControl1.Attributes.Add("content", "noindex");
          htmlGenericControls.Add(closingGenericControl2);
        }
        HtmlSelfClosingGenericControl closingGenericControl3 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl3.Attributes.Add("name", "keywords");
        closingGenericControl3.Attributes.Add("content", this.MetadataHelperUtils.GetCommaSeparatedTagsFromExtension(pageMetadataInputs.Extension));
        htmlGenericControls.Add(closingGenericControl3);
        HtmlSelfClosingGenericControl closingGenericControl4 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl4.Attributes.Add("property", "og:title");
        closingGenericControl4.Attributes.Add("content", this.GetItemDetailsPageTitle(pageMetadataInputs));
        htmlGenericControls.Add(closingGenericControl4);
        HtmlSelfClosingGenericControl closingGenericControl5 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl5.Attributes.Add("property", "og:type");
        closingGenericControl5.Attributes.Add("content", "website");
        htmlGenericControls.Add(closingGenericControl5);
        HtmlSelfClosingGenericControl closingGenericControl6 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl6.Attributes.Add("property", "og:url");
        closingGenericControl6.Attributes.Add("content", pageMetadataInputs.Url);
        htmlGenericControls.Add(closingGenericControl6);
        HtmlSelfClosingGenericControl closingGenericControl7 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl7.Attributes.Add("property", "og:image");
        closingGenericControl7.Attributes.Add("content", this.MetadataHelperUtils.GetExtensionIconUrlFromExtension(pageMetadataInputs.Extension));
        htmlGenericControls.Add(closingGenericControl7);
        HtmlSelfClosingGenericControl closingGenericControl8 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl8.Attributes.Add("property", "og:description");
        closingGenericControl8.Attributes.Add("content", this.GetItemDetailsMetaDescriptionContentsFromExtension(pageMetadataInputs.Extension));
        htmlGenericControls.Add(closingGenericControl8);
        HtmlSelfClosingGenericControl closingGenericControl9 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl9.Attributes.Add("property", "twitter:card");
        closingGenericControl9.Attributes.Add("content", "summary");
        htmlGenericControls.Add(closingGenericControl9);
        HtmlSelfClosingGenericControl closingGenericControl10 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl10.Attributes.Add("property", "twitter:site");
        closingGenericControl10.Attributes.Add("content", this.GetTwitterHandleFromExtension(pageMetadataInputs.Extension));
        htmlGenericControls.Add(closingGenericControl10);
        if (!string.IsNullOrEmpty(pageMetadataInputs.Url) && pageMetadataInputs.Extension != null)
        {
          HtmlSelfClosingGenericControl closingGenericControl11 = new HtmlSelfClosingGenericControl("link");
          closingGenericControl11.Attributes.Add("rel", "canonical");
          closingGenericControl11.Attributes.Add("href", this.GetCanonicalURLFromExtension(tfsRequestContext, pageMetadataInputs.Extension));
          htmlGenericControls.Add(closingGenericControl11);
        }
      }
      return this.MetadataHelperUtils.GetHtmlStringFromHtmlGenericControls(htmlGenericControls);
    }

    public string GetSearchPageTitle(PageMetadataInputs pageMetadataInputs)
    {
      string searchPageTitle = GalleryCommonResources.VisualStudioMarketplace;
      if (pageMetadataInputs != null && !pageMetadataInputs.Product.IsNullOrEmpty<char>() && !pageMetadataInputs.SearchTerm.IsNullOrEmpty<char>())
      {
        string fullNameFromProduct = this.GetProductFullNameFromProduct(pageMetadataInputs.Product);
        if (!fullNameFromProduct.IsNullOrEmpty<char>())
          searchPageTitle = string.Format(GalleryResources.SearchPageTitle, (object) pageMetadataInputs.SearchTerm, (object) fullNameFromProduct);
      }
      return searchPageTitle;
    }

    public string GetSearchPageMetadata(PageMetadataInputs pageMetadataInputs)
    {
      List<HtmlSelfClosingGenericControl> htmlGenericControls = new List<HtmlSelfClosingGenericControl>();
      if (pageMetadataInputs != null)
      {
        HtmlSelfClosingGenericControl closingGenericControl1 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl1.Attributes.Add("name", "description");
        closingGenericControl1.Attributes.Add("content", this.GetSearchMetaDescriptionContents(pageMetadataInputs.Product, pageMetadataInputs.SearchTerm));
        htmlGenericControls.Add(closingGenericControl1);
        HtmlSelfClosingGenericControl closingGenericControl2 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl2.Attributes.Add("name", "keywords");
        closingGenericControl2.Attributes.Add("content", this.GetSearchAndCategoryMetaKeywords(pageMetadataInputs.Product));
        htmlGenericControls.Add(closingGenericControl2);
        HtmlSelfClosingGenericControl closingGenericControl3 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl3.Attributes.Add("property", "og:title");
        closingGenericControl3.Attributes.Add("content", this.GetSearchPageOgTitleContents(pageMetadataInputs.Product, pageMetadataInputs.SearchTerm));
        htmlGenericControls.Add(closingGenericControl3);
        HtmlSelfClosingGenericControl closingGenericControl4 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl4.Attributes.Add("property", "og:type");
        closingGenericControl4.Attributes.Add("content", "website");
        htmlGenericControls.Add(closingGenericControl4);
        HtmlSelfClosingGenericControl closingGenericControl5 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl5.Attributes.Add("property", "og:url");
        closingGenericControl5.Attributes.Add("content", pageMetadataInputs.Url);
        htmlGenericControls.Add(closingGenericControl5);
        HtmlSelfClosingGenericControl closingGenericControl6 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl6.Attributes.Add("property", "twitter:card");
        closingGenericControl6.Attributes.Add("content", "summary");
        htmlGenericControls.Add(closingGenericControl6);
        HtmlSelfClosingGenericControl closingGenericControl7 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl7.Attributes.Add("property", "twitter:site");
        closingGenericControl7.Attributes.Add("content", this.GetTwitterSiteHandleForProduct(pageMetadataInputs.Product));
        htmlGenericControls.Add(closingGenericControl7);
        HtmlSelfClosingGenericControl closingGenericControl8 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl8.Attributes.Add("property", "twitter:creator");
        closingGenericControl8.Attributes.Add("content", "@VisualStudio");
        htmlGenericControls.Add(closingGenericControl8);
      }
      return this.MetadataHelperUtils.GetHtmlStringFromHtmlGenericControls(htmlGenericControls);
    }

    public string GetCategoryPageTitle(PageMetadataInputs pageMetadataInputs)
    {
      string categoryPageTitle = GalleryCommonResources.VisualStudioMarketplace;
      if (pageMetadataInputs != null && !pageMetadataInputs.Category.IsNullOrEmpty<char>())
        categoryPageTitle = string.Format(GalleryResources.CategoryTitle, (object) pageMetadataInputs.Category, (object) categoryPageTitle);
      return categoryPageTitle;
    }

    public string GetCategoryPageMetadata(PageMetadataInputs pageMetadataInputs)
    {
      List<HtmlSelfClosingGenericControl> htmlGenericControls = new List<HtmlSelfClosingGenericControl>();
      if (pageMetadataInputs != null)
      {
        HtmlSelfClosingGenericControl closingGenericControl1 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl1.Attributes.Add("name", "description");
        closingGenericControl1.Attributes.Add("content", this.GetCategoryMetaDescriptionContents(pageMetadataInputs.Product, pageMetadataInputs.Category));
        htmlGenericControls.Add(closingGenericControl1);
        HtmlSelfClosingGenericControl closingGenericControl2 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl2.Attributes.Add("name", "keywords");
        closingGenericControl2.Attributes.Add("content", this.GetSearchAndCategoryMetaKeywords(pageMetadataInputs.Product));
        htmlGenericControls.Add(closingGenericControl2);
        HtmlSelfClosingGenericControl closingGenericControl3 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl3.Attributes.Add("property", "og:title");
        closingGenericControl3.Attributes.Add("content", this.GetCategoryPageOgTitle(pageMetadataInputs.Product, pageMetadataInputs.Category));
        htmlGenericControls.Add(closingGenericControl3);
        HtmlSelfClosingGenericControl closingGenericControl4 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl4.Attributes.Add("property", "og:type");
        closingGenericControl4.Attributes.Add("content", "website");
        htmlGenericControls.Add(closingGenericControl4);
        HtmlSelfClosingGenericControl closingGenericControl5 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl5.Attributes.Add("property", "og:url");
        closingGenericControl5.Attributes.Add("content", pageMetadataInputs.Url);
        htmlGenericControls.Add(closingGenericControl5);
        HtmlSelfClosingGenericControl closingGenericControl6 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl6.Attributes.Add("property", "twitter:card");
        closingGenericControl6.Attributes.Add("content", "summary");
        htmlGenericControls.Add(closingGenericControl6);
        HtmlSelfClosingGenericControl closingGenericControl7 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl7.Attributes.Add("property", "twitter:site");
        closingGenericControl7.Attributes.Add("content", this.GetTwitterSiteHandleForProduct(pageMetadataInputs.Product));
        htmlGenericControls.Add(closingGenericControl7);
        HtmlSelfClosingGenericControl closingGenericControl8 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl8.Attributes.Add("property", "twitter:creator");
        closingGenericControl8.Attributes.Add("content", "@VisualStudio");
        htmlGenericControls.Add(closingGenericControl8);
      }
      return this.MetadataHelperUtils.GetHtmlStringFromHtmlGenericControls(htmlGenericControls);
    }

    public string GetPublisherProfilePageTitle(PageMetadataInputs pageMetadataInputs)
    {
      string profilePageTitle = GalleryCommonResources.VisualStudioMarketplace;
      if (pageMetadataInputs != null && pageMetadataInputs.Publisher != null)
        profilePageTitle = string.Format(GalleryResources.PublisherProfilePageTitle, (object) pageMetadataInputs.Publisher.DisplayName);
      return profilePageTitle;
    }

    public string GetPublisherProfilePageMetadata(
      IVssRequestContext tfsRequestContext,
      PageMetadataInputs pageMetadataInputs)
    {
      List<HtmlSelfClosingGenericControl> htmlGenericControls = new List<HtmlSelfClosingGenericControl>();
      if (pageMetadataInputs != null)
      {
        HtmlSelfClosingGenericControl closingGenericControl1 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl1.Attributes.Add("name", "description");
        closingGenericControl1.Attributes.Add("content", this.GetPublisherProfileMetaDescriptionContents(pageMetadataInputs.Publisher));
        htmlGenericControls.Add(closingGenericControl1);
        HtmlSelfClosingGenericControl closingGenericControl2 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl2.Attributes.Add("name", "keywords");
        closingGenericControl2.Attributes.Add("content", "publisher, extension");
        htmlGenericControls.Add(closingGenericControl2);
        HtmlSelfClosingGenericControl closingGenericControl3 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl3.Attributes.Add("property", "og:title");
        closingGenericControl3.Attributes.Add("content", this.GetPublisherProfilePageTitle(pageMetadataInputs));
        htmlGenericControls.Add(closingGenericControl3);
        HtmlSelfClosingGenericControl closingGenericControl4 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl4.Attributes.Add("property", "og:type");
        closingGenericControl4.Attributes.Add("content", "website");
        htmlGenericControls.Add(closingGenericControl4);
        HtmlSelfClosingGenericControl closingGenericControl5 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl5.Attributes.Add("property", "og:url");
        closingGenericControl5.Attributes.Add("content", pageMetadataInputs.Url);
        htmlGenericControls.Add(closingGenericControl5);
        HtmlSelfClosingGenericControl closingGenericControl6 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl6.Attributes.Add("property", "og:image");
        closingGenericControl6.Attributes.Add("content", this.GetPublisherIconUrl(pageMetadataInputs.Publisher));
        htmlGenericControls.Add(closingGenericControl6);
        HtmlSelfClosingGenericControl closingGenericControl7 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl7.Attributes.Add("property", "twitter:card");
        closingGenericControl7.Attributes.Add("content", "summary");
        htmlGenericControls.Add(closingGenericControl7);
        HtmlSelfClosingGenericControl closingGenericControl8 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl8.Attributes.Add("property", "twitter:site");
        closingGenericControl8.Attributes.Add("content", "@VisualStudio");
        htmlGenericControls.Add(closingGenericControl8);
        HtmlSelfClosingGenericControl closingGenericControl9 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl9.Attributes.Add("property", "twitter:creator");
        closingGenericControl9.Attributes.Add("content", "@VisualStudio");
        htmlGenericControls.Add(closingGenericControl9);
        HtmlSelfClosingGenericControl closingGenericControl10 = new HtmlSelfClosingGenericControl("meta");
        closingGenericControl10.Attributes.Add("property", "og:description");
        closingGenericControl10.Attributes.Add("content", pageMetadataInputs.Publisher?.LongDescription);
        htmlGenericControls.Add(closingGenericControl10);
      }
      return this.MetadataHelperUtils.GetHtmlStringFromHtmlGenericControls(htmlGenericControls);
    }

    internal string GetPublisherIconUrl(Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher)
    {
      string publisherIconUrl = string.Empty;
      if (publisher != null && publisher.Links != null)
      {
        object obj = (object) null;
        if (publisher.Links.Links.TryGetValue("logo", out obj))
          publisherIconUrl = ((ReferenceLink) obj).Href;
      }
      return publisherIconUrl;
    }

    internal string GetMarketplaceIconUrl(string resourcesPath)
    {
      string marketplaceIconUrl = string.Empty;
      if (!resourcesPath.IsNullOrEmpty<char>())
      {
        if (!resourcesPath.EndsWith("/"))
          resourcesPath += "/";
        marketplaceIconUrl = resourcesPath + "Header/access-marketplace.png";
      }
      return marketplaceIconUrl;
    }

    internal string GetItemDetailsMetaDescriptionContentsFromExtension(PublishedExtension extension)
    {
      string contentsFromExtension = string.Empty;
      if (extension != null && !extension.ShortDescription.IsNullOrEmpty<char>())
      {
        string nameForExtension = this.MetadataHelperUtils.GetProductNameForExtension(extension);
        if (!string.IsNullOrEmpty(nameForExtension))
          contentsFromExtension = !nameForExtension.Equals(GalleryCommonResources.Subs_Header, StringComparison.InvariantCultureIgnoreCase) ? string.Format(GalleryResources.ItemDetailsMetaDescription, (object) nameForExtension, (object) extension.ShortDescription) : extension.ShortDescription;
      }
      return contentsFromExtension;
    }

    internal string GetTwitterHandleFromExtension(PublishedExtension extension)
    {
      string handleFromExtension = string.Empty;
      if (extension != null)
      {
        string nameForExtension = this.MetadataHelperUtils.GetProductNameForExtension(extension);
        if (!nameForExtension.IsNullOrEmpty<char>())
          handleFromExtension = !nameForExtension.Equals(GalleryResources.VSCode, StringComparison.InvariantCultureIgnoreCase) ? "@VisualStudio" : "@Code";
      }
      return handleFromExtension;
    }

    internal string GetCanonicalURLFromExtension(
      IVssRequestContext tfsRequestContext,
      PublishedExtension extension)
    {
      string urlFromExtension = "";
      if (extension != null)
      {
        string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName);
        urlFromExtension = GalleryHtmlExtensions.GetGalleryItemDetailsUrl(tfsRequestContext, fullyQualifiedName);
      }
      return urlFromExtension;
    }

    internal string GetCanonicalURLFromPublisher(
      IVssRequestContext tfsRequestContext,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher)
    {
      string urlFromPublisher = "";
      if (publisher != null)
        urlFromPublisher = GalleryHtmlExtensions.GetGalleryPublisherProfileUrl(tfsRequestContext, publisher);
      return urlFromPublisher;
    }

    internal string GetProductFullNameFromProduct(string product)
    {
      string fullNameFromProduct = string.Empty;
      if (!product.IsNullOrEmpty<char>())
      {
        if (product.Equals("vs", StringComparison.InvariantCultureIgnoreCase))
          fullNameFromProduct = GalleryCommonResources.VS_Header;
        else if (product.Equals("vsts", StringComparison.InvariantCultureIgnoreCase))
          fullNameFromProduct = GalleryCommonResources.VSTS_Header;
        else if (product.Equals("vsformac", StringComparison.InvariantCultureIgnoreCase))
          fullNameFromProduct = GalleryCommonResources.VSForMac_Header;
        else if (product.Equals("vscode", StringComparison.InvariantCultureIgnoreCase))
          fullNameFromProduct = GalleryCommonResources.VSCode_Header;
        else if (product.Equals("subscriptions", StringComparison.InvariantCultureIgnoreCase))
          fullNameFromProduct = GalleryCommonResources.Subs_Header;
      }
      return fullNameFromProduct;
    }

    internal string GetOpenGraphDescriptionForHomePage(string product)
    {
      string descriptionForHomePage = string.Empty;
      if (!product.IsNullOrEmpty<char>())
      {
        string fullNameFromProduct = this.GetProductFullNameFromProduct(product);
        if (!fullNameFromProduct.IsNullOrEmpty<char>())
          descriptionForHomePage = !product.Equals("subscriptions", StringComparison.InvariantCultureIgnoreCase) ? string.Format(GalleryResources.HomePageOpenGraphDescription, (object) fullNameFromProduct) : GalleryResources.HomePageOpenGraphSubscriptionDescription;
      }
      return descriptionForHomePage;
    }

    internal string GetTwitterSiteHandleForProduct(string product)
    {
      string handleForProduct = string.Empty;
      if (!product.IsNullOrEmpty<char>())
        handleForProduct = !product.Equals("vscode", StringComparison.InvariantCultureIgnoreCase) ? "@VisualStudio" : "@Code";
      return handleForProduct;
    }

    internal string GetSearchMetaDescriptionContents(string product, string searchQuery)
    {
      string descriptionContents = string.Empty;
      if (!product.IsNullOrEmpty<char>() && !searchQuery.IsNullOrEmpty<char>())
      {
        string fullNameFromProduct = this.GetProductFullNameFromProduct(product);
        if (!fullNameFromProduct.IsNullOrEmpty<char>())
          descriptionContents = string.Format(GalleryResources.SearchPageMetaDescription, (object) searchQuery, (object) fullNameFromProduct);
      }
      return descriptionContents;
    }

    internal string GetSearchPageOgTitleContents(string product, string searchQuery)
    {
      string pageOgTitleContents = string.Empty;
      if (!product.IsNullOrEmpty<char>() && !searchQuery.IsNullOrEmpty<char>())
      {
        string fullNameFromProduct = this.GetProductFullNameFromProduct(product);
        if (!fullNameFromProduct.IsNullOrEmpty<char>())
          pageOgTitleContents = string.Format(GalleryResources.SearchPageOgTitle, (object) searchQuery, (object) fullNameFromProduct);
      }
      return pageOgTitleContents;
    }

    internal string GetSearchAndCategoryMetaKeywords(string product)
    {
      string categoryMetaKeywords = string.Empty;
      if (!product.IsNullOrEmpty<char>())
      {
        string fullNameFromProduct = this.GetProductFullNameFromProduct(product);
        if (!fullNameFromProduct.IsNullOrEmpty<char>())
          categoryMetaKeywords = string.Format(GalleryResources.SearchPageMetaKeywords, (object) fullNameFromProduct);
      }
      return categoryMetaKeywords;
    }

    internal string GetCategoryMetaDescriptionContents(string product, string category)
    {
      string descriptionContents = string.Empty;
      if (!product.IsNullOrEmpty<char>() && !category.IsNullOrEmpty<char>())
      {
        string fullNameFromProduct = this.GetProductFullNameFromProduct(product);
        if (!fullNameFromProduct.IsNullOrEmpty<char>())
          descriptionContents = string.Format(GalleryResources.CategoryPageMetaDescription, (object) category, (object) fullNameFromProduct);
      }
      return descriptionContents;
    }

    internal string GetCategoryPageOgTitle(string product, string category)
    {
      string categoryPageOgTitle = string.Empty;
      if (!product.IsNullOrEmpty<char>() && !category.IsNullOrEmpty<char>())
      {
        string fullNameFromProduct = this.GetProductFullNameFromProduct(product);
        if (!fullNameFromProduct.IsNullOrEmpty<char>())
          categoryPageOgTitle = string.Format(GalleryResources.CategoryPageOgTitle, (object) category, (object) fullNameFromProduct);
      }
      return categoryPageOgTitle;
    }

    internal string GetPublisherProfileMetaDescriptionContents(Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher)
    {
      string descriptionContents = string.Empty;
      if (publisher != null)
        descriptionContents = string.Format(GalleryResources.PublisherPageMetaDescription, (object) publisher.PublisherName, (object) publisher.DisplayName, (object) publisher.LongDescription);
      return descriptionContents;
    }

    public PageMetadataHelperUtilities MetadataHelperUtils { get; private set; }
  }
}
