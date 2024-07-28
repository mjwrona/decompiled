// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.UserExtensionLicense
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class UserExtensionLicense : IEquatable<UserExtensionLicense>
  {
    [JsonIgnore]
    public Guid AccountId { get; set; }

    [JsonIgnore]
    public int InternalScopeId { get; set; }

    [JsonProperty(PropertyName = "userId")]
    public Guid UserId { get; set; }

    [JsonProperty(PropertyName = "extensionId")]
    public string ExtensionId { get; set; }

    [JsonProperty(PropertyName = "status")]
    public UserExtensionLicenseStatus Status { get; set; }

    [JsonProperty(PropertyName = "source")]
    public LicensingSource Source { get; set; }

    [JsonProperty(PropertyName = "assignmentSource")]
    public AssignmentSource AssignmentSource { get; set; }

    [JsonProperty(PropertyName = "assignmentDate")]
    public DateTimeOffset AssignmentDate { get; set; }

    [JsonProperty(PropertyName = "collectionId")]
    public Guid CollectionId { get; set; }

    [JsonProperty(PropertyName = "lastUpdated")]
    public DateTime? LastUpdated { get; set; }

    public UserExtensionLicense Transfer(Guid userId) => new UserExtensionLicense()
    {
      AccountId = this.AccountId,
      InternalScopeId = this.InternalScopeId,
      UserId = userId,
      ExtensionId = this.ExtensionId,
      Status = this.Status,
      Source = this.Source,
      AssignmentSource = this.AssignmentSource,
      AssignmentDate = this.AssignmentDate,
      CollectionId = this.CollectionId,
      LastUpdated = this.LastUpdated
    };

    public override bool Equals(object obj)
    {
      UserExtensionLicense extensionLicense = obj as UserExtensionLicense;
      return obj != null && extensionLicense != null && this.Equals(extensionLicense);
    }

    public bool Equals(UserExtensionLicense extensionLicense)
    {
      if (extensionLicense == null || !(this.ExtensionId == extensionLicense.ExtensionId) || this.Source != extensionLicense.Source || this.Status != extensionLicense.Status || this.AssignmentSource != extensionLicense.AssignmentSource || !(this.AssignmentDate == extensionLicense.AssignmentDate))
        return false;
      DateTime? lastUpdated1 = this.LastUpdated;
      DateTime? lastUpdated2 = extensionLicense.LastUpdated;
      if (lastUpdated1.HasValue != lastUpdated2.HasValue)
        return false;
      return !lastUpdated1.HasValue || lastUpdated1.GetValueOrDefault() == lastUpdated2.GetValueOrDefault();
    }

    public override int GetHashCode()
    {
      int num = 23 * (23 * (23 * (23 * (this.ExtensionId == null ? 0 : this.ExtensionId.GetHashCode()) + this.Source.GetHashCode()) + this.Status.GetHashCode()) + this.AssignmentSource.GetHashCode());
      DateTimeOffset assignmentDate = this.AssignmentDate;
      int hashCode = this.AssignmentDate.GetHashCode();
      return 23 * (num + hashCode) + (!this.LastUpdated.HasValue ? 0 : this.LastUpdated.GetHashCode());
    }
  }
}
