// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Models.GalleryPublisherViewData
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Models
{
  [DataContract]
  public class GalleryPublisherViewData
  {
    [DataMember(Name = "publisherTenants")]
    public List<PublisherTenant> PublisherTenants { get; set; }

    [DataMember(Name = "publisherSelected")]
    public Publisher PublisherSelected { get; set; }

    [DataMember(Name = "migratedExtensions")]
    public List<string> MigratedExtensions { get; set; }

    [DataMember(Name = "accountsDomain")]
    public string AccountsDomain { get; set; }

    [DataMember(Name = "userDomain")]
    public string UserDomain { get; set; }

    [DataMember(Name = "userDomainId")]
    public Guid UserDomainId { get; set; }

    [DataMember(Name = "userAddress")]
    public string UserAddress { get; set; }

    [DataMember(Name = "hasCreatePublisherPermission")]
    public bool HasCreatePublisherPermission { get; set; }

    [DataMember(Name = "hasEditPublisherSettingsPermission")]
    public bool HasEditPublisherSettingsPermission { get; set; }

    [DataMember(Name = "hasCertificateDownloadPermission")]
    public bool HasCertificateDownloadPermission { get; set; }

    [DataMember(Name = "hasViewPublisherPermissionsPermission")]
    public bool HasViewPublisherPermissionsPermission { get; set; }

    [DataMember(Name = "hasPublisherPrivateReadPermission")]
    public bool HasPublisherPrivateReadPermission { get; set; }

    [DataMember(Name = "maxPackageSize")]
    public long MaxPackageSize { get; set; }

    [DataMember(Name = "hasPublisherPublishExtensionPermission")]
    public bool HasPublisherPublishExtensionPermission { get; set; }
  }
}
