// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.TryAllUpstreamsStorageId
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public sealed class TryAllUpstreamsStorageId : 
    IStorageId,
    IEquatable<IStorageId>,
    IEquatable<
    #nullable disable
    TryAllUpstreamsStorageId>
  {
    public string ValueString => "upstreams:{}";

    public string NonLegacyValueString => this.ValueString;

    public bool IsCacheable => false;

    public bool IsLocal => false;

    public bool? RepresentsSingleFile => new bool?();

    public override string ToString() => "TryAllUpstreamsStorageId(" + this.ValueString + ")";

    public bool Equals(TryAllUpstreamsStorageId other)
    {
      if (other == null)
        return false;
      return this == other || this.ValueString == other.ValueString;
    }

    public bool Equals(IStorageId other) => this.Equals(other as TryAllUpstreamsStorageId);

    public override bool Equals(object other) => this.Equals(other as TryAllUpstreamsStorageId);

    public override int GetHashCode() => typeof (TryAllUpstreamsStorageId).GetHashCode();
  }
}
