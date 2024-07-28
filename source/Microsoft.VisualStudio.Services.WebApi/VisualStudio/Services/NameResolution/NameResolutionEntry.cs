// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.NameResolutionEntry
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NameResolution
{
  [DataContract]
  public class NameResolutionEntry
  {
    public NameResolutionEntry()
    {
    }

    public NameResolutionEntry(string @namespace, string name)
    {
      this.Namespace = @namespace;
      this.Name = name;
    }

    [DataMember]
    public string Namespace { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public Guid Value { get; set; }

    [DataMember]
    public bool IsPrimary { get; set; }

    [DataMember]
    public bool IsEnabled { get; set; }

    [DataMember]
    public int? TTL
    {
      get => !this.ExpiresOn.HasValue ? new int?() : new int?(Math.Max((int) ((this.ExpiresOn.Value - DateTime.UtcNow).TotalSeconds + 0.5), 0));
      set
      {
        if (!value.HasValue)
          this.ExpiresOn = new DateTime?();
        else
          this.ExpiresOn = new DateTime?(DateTime.UtcNow.AddSeconds((double) value.Value));
      }
    }

    [DataMember]
    public int Revision { get; set; }

    [IgnoreDataMember]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DateTime? ExpiresOn { get; set; }

    [IgnoreDataMember]
    public bool HasExpiration => this.ExpiresOn.HasValue;

    [IgnoreDataMember]
    public bool IsExpired => this.HasExpiration && this.ExpiresOn.Value <= DateTime.UtcNow;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [IgnoreDataMember]
    public bool Force { get; set; }

    public NameResolutionEntry Clone() => new NameResolutionEntry()
    {
      Namespace = this.Namespace,
      Name = this.Name,
      Value = this.Value,
      IsPrimary = this.IsPrimary,
      IsEnabled = this.IsEnabled,
      ExpiresOn = this.ExpiresOn,
      Revision = this.Revision
    };

    public override bool Equals(object obj)
    {
      if (!(obj is NameResolutionEntry nameResolutionEntry))
        return false;
      if (nameResolutionEntry == this)
        return true;
      return StringComparer.OrdinalIgnoreCase.Equals(this.Namespace, nameResolutionEntry.Namespace) && StringComparer.OrdinalIgnoreCase.Equals(this.Name, nameResolutionEntry.Name);
    }

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(this.Namespace) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(this.Name);

    public override string ToString() => string.Format("=> Namespace: {0} Name: {1} Value: {2} IsPrimary: {3} IsEnabled: {4} ExpiresOn: {5} Revision: {6}", (object) this.Namespace, (object) this.Name, (object) this.Value, (object) this.IsPrimary, (object) this.IsEnabled, (object) this.ExpiresOn, (object) this.Revision);
  }
}
