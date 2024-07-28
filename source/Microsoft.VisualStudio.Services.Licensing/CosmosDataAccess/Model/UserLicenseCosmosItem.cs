// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess.Model.UserLicenseCosmosItem
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess.Model
{
  public class UserLicenseCosmosItem
  {
    [JsonIgnore]
    private List<UserExtensionLicense> _extensionLicenses;

    [JsonProperty(PropertyName = "userId")]
    public Guid UserId { get; set; }

    [JsonProperty(PropertyName = "license")]
    public UserLicense License { get; set; }

    [JsonProperty(PropertyName = "previousLicense")]
    public UserLicense PreviousLicense { get; set; }

    [JsonProperty(PropertyName = "extensionLicenses")]
    public List<UserExtensionLicense> ExtensionLicenses
    {
      get => this._extensionLicenses ?? (this._extensionLicenses = new List<UserExtensionLicense>());
      set => this._extensionLicenses = value;
    }

    [JsonProperty(PropertyName = "licensedIdentity")]
    public LicensedIdentity LicensedIdentity { get; set; }

    [JsonProperty(PropertyName = "previousUserId")]
    public Guid? PreviousUserId { get; set; }

    [JsonProperty(PropertyName = "isCollectionLevel")]
    public bool IsCollectionLevel { get; set; }

    public UserLicenseCosmosItem()
    {
    }

    public UserLicenseCosmosItem(Guid userId) => this.UserId = userId;

    public UserLicenseCosmosItem(
      Guid userId,
      UserLicense license,
      LicensedIdentity licensedIdentity,
      UserLicense previousLicense = null,
      List<UserExtensionLicense> extensions = null)
    {
      this.UserId = userId;
      this.License = license;
      this.PreviousLicense = previousLicense;
      this.ExtensionLicenses = extensions;
      this.LicensedIdentity = licensedIdentity;
    }
  }
}
