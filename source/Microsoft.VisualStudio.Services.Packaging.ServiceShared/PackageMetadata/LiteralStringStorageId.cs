// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.LiteralStringStorageId
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public sealed class LiteralStringStorageId : 
    IStorageId,
    IEquatable<IStorageId>,
    IEquatable<
    #nullable disable
    LiteralStringStorageId>
  {
    public LiteralStringStorageId(string value) => this.Value = value ?? throw new ArgumentNullException(nameof (value));

    public string Value { get; }

    public string ValueString => this.NonLegacyValueString;

    public string NonLegacyValueString => "literal:" + this.Value;

    public bool IsCacheable => true;

    public bool IsLocal => true;

    public bool? RepresentsSingleFile => new bool?(true);

    public override string ToString() => "LiteralStringStorageId(" + this.ValueString + ")";

    public bool Equals(LiteralStringStorageId other)
    {
      if (other == null)
        return false;
      return this == other || this.Value.Equals(other.Value);
    }

    public bool Equals(IStorageId other) => this.Equals(other as LiteralStringStorageId);

    public override bool Equals(object other) => this.Equals(other as LiteralStringStorageId);

    public override int GetHashCode() => this.Value.GetHashCode();
  }
}
