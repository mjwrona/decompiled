// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.DropStorageId
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public sealed class DropStorageId : IStorageId, IEquatable<IStorageId>, IEquatable<
  #nullable disable
  DropStorageId>
  {
    public DropStorageId(string dropName) => this.DropName = dropName != null ? dropName : throw new ArgumentNullException(nameof (dropName));

    public string DropName { get; }

    public string ValueString => "drop:" + this.DropName;

    public string NonLegacyValueString => this.ValueString;

    public bool IsCacheable => true;

    public bool IsLocal => true;

    public bool? RepresentsSingleFile => new bool?(false);

    public override string ToString() => "DropStorageId(" + this.ValueString + ")";

    public bool Equals(DropStorageId other)
    {
      if (other == null)
        return false;
      return this == other || string.Equals(this.DropName, other.DropName, StringComparison.Ordinal);
    }

    public bool Equals(IStorageId obj) => this.Equals(obj as DropStorageId);

    public override bool Equals(object obj) => this.Equals(obj as DropStorageId);

    public override int GetHashCode() => this.DropName.GetHashCode();
  }
}
