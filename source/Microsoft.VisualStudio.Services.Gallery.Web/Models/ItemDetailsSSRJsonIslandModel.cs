// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Models.ItemDetailsSSRJsonIslandModel
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Models
{
  [DataContract]
  public class ItemDetailsSSRJsonIslandModel
  {
    [DataMember]
    public string GitHubLink;
    [DataMember]
    public string ReleaseDateString;
    [DataMember]
    public string LastUpdatedDateString;
    [DataMember]
    public string GalleryUrl;
    [DataMember]
    public List<string> Categories;
    [DataMember]
    public List<string> Tags;
    [DataMember]
    public IDictionary<string, string> ExtensionProperties;
    [DataMember]
    public ItemDetailsResourcesSectionModel Resources;
    [DataMember]
    public ItemDetailsMoreInfoModel MoreInfo;
    [DataMember]
    public string ResourcesPath;
    [DataMember]
    public string AssetUri;
    [DataMember]
    public string VsixManifestAssetType;
    [DataMember]
    public string StaticResourceVersion;
    [DataMember]
    public string AfdIdentifier;
    [DataMember]
    public string VsixId;
    [DataMember]
    public IEnumerable<string> WorksWith;
    [DataMember]
    public int ItemType;
    [DataMember]
    public bool IsMDPruned;
    [DataMember]
    public int PrunedMDLength;
    [DataMember]
    public int OverviewMDLength;
    [DataMember]
    public bool IsRHSAsyncComponentsEnabled;
    [DataMember]
    public string OfferDetails;
    [DataMember]
    public bool IsDetailsTabsEnabled;
    [DataMember]
    public bool ShowVersionHistory;
    [DataMember]
    public bool IsSeeMoreButtonOnVersionHistoryTab;
    [DataMember]
    public bool IsReferralLinkRedirectionWarningPopupEnabled;
    [DataMember]
    public List<ItemDetailsVersionModel> Versions;
    [DataMember]
    public bool IsCSRFeatureEnabled;
    [DataMember]
    public IDictionary<string, string> TargetPlatforms;
  }
}
