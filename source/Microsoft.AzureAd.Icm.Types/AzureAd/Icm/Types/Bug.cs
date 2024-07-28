// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.Bug
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  [Serializable]
  public class Bug : IEquatable<Bug>
  {
    [DataMember(Name = "id")]
    public long Id { get; set; }

    [DataMember(Name = "type")]
    public string BugType { get; set; }

    [DataMember(Name = "src")]
    public string BugSource { get; set; }

    [DataMember(Name = "repairDescription")]
    public string BugDescription { get; set; }

    [DataMember(Name = "bugId")]
    public string BugId { get; set; }

    [DataMember(Name = "owner")]
    public string BugOwner { get; set; }

    [DataMember(Name = "delivery")]
    public string BugDelivery { get; set; }

    public string BugValidationMessage { get; set; }

    public bool IsValid => !string.IsNullOrWhiteSpace(this.BugType) && string.Compare(this.BugType, "0", StringComparison.OrdinalIgnoreCase) != 0 && this.BugType.Length <= 128 && !string.IsNullOrWhiteSpace(this.BugSource) && string.Compare(this.BugSource, "0", StringComparison.OrdinalIgnoreCase) != 0 && this.BugSource.Length <= 128 && !string.IsNullOrWhiteSpace(this.BugId) && this.BugId.Length <= 128 && (string.IsNullOrWhiteSpace(this.BugOwner) || this.BugOwner.Length <= 128) && (string.IsNullOrWhiteSpace(this.BugDelivery) || this.BugDelivery.Length <= 128);

    public bool? IsValidated { get; set; }

    public bool Equals(Bug other)
    {
      if (other == null || (!string.IsNullOrEmpty(this.BugSource) || !string.IsNullOrEmpty(other.BugSource)) && !string.Equals(this.BugSource, other.BugSource, StringComparison.OrdinalIgnoreCase) || (!string.IsNullOrEmpty(this.BugType) || !string.IsNullOrEmpty(other.BugType)) && !string.Equals(this.BugType, other.BugType, StringComparison.OrdinalIgnoreCase) || (!string.IsNullOrEmpty(this.BugDescription) || !string.IsNullOrEmpty(other.BugDescription)) && !string.Equals(this.BugDescription, other.BugDescription, StringComparison.OrdinalIgnoreCase) || (!string.IsNullOrEmpty(this.BugId) || !string.IsNullOrEmpty(other.BugId)) && !string.Equals(this.BugId, other.BugId, StringComparison.OrdinalIgnoreCase) || (!string.IsNullOrEmpty(this.BugOwner) || !string.IsNullOrEmpty(other.BugOwner)) && !string.Equals(this.BugOwner, other.BugOwner, StringComparison.OrdinalIgnoreCase))
        return false;
      return string.IsNullOrEmpty(this.BugDelivery) && string.IsNullOrEmpty(other.BugDelivery) || string.Equals(this.BugDelivery, other.BugDelivery, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => (string.IsNullOrEmpty(this.BugSource) ? 0 : this.BugSource.GetHashCode()) ^ (string.IsNullOrEmpty(this.BugType) ? 0 : this.BugType.GetHashCode()) ^ (string.IsNullOrEmpty(this.BugId) ? 0 : this.BugId.GetHashCode());
  }
}
