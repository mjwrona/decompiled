// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Models.ItemBannerModel
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Models
{
  [DataContract]
  public class ItemBannerModel
  {
    [DataMember]
    public string BrandingColor;
    [DataMember]
    public string ImageUrl;
    [DataMember]
    public string ImageAlt;
    [DataMember]
    public string BrandingTheme;
    [DataMember]
    public string ItemName;
    [DataMember]
    public string PublisherDisplayName;
    [DataMember]
    public string PublisherLink;
    [DataMember]
    public string PublisherPageLinkDescription;
    [DataMember]
    public string BrandingThemeColor;
    [DataMember]
    public string SponsorThemeColor;
    [DataMember]
    public string VerifiedDomainText;
    [DataMember]
    public string DomainUrl;
    [DataMember]
    public string DomainName;
    [DataMember]
    public string InstallsText;
    [DataMember]
    public string InstallsHoverText;
    [DataMember]
    public string ItemPriceCategoryText;
    [DataMember]
    public string ItemDescription;
    [DataMember]
    public ItemActionModel ItemAction;
    [DataMember]
    public RnR RnRDetails;
    [DataMember]
    public string ExtensionTitleTag;
    [DataMember]
    public string ReportsLink;
    [DataMember]
    public string ReportsLinkDisplayName;
    [DataMember]
    public string GalleryItemEditLink;
    [DataMember]
    public string GalleryItemEditLinkDisplayName;
    [DataMember]
    public bool IsGetStartedType;
    [DataMember]
    public bool IsVssExtensionOrResource;
    [DataMember]
    public bool IsHelpTextVisible;
    [DataMember]
    public string HelpText;
    [DataMember]
    public string SponsorLink;
  }
}
