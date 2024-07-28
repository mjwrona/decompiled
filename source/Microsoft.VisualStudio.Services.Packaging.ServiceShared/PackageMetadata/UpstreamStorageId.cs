// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.UpstreamStorageId
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public sealed class UpstreamStorageId : 
    IStorageId,
    IEquatable<IStorageId>,
    IEquatable<
    #nullable disable
    UpstreamStorageId>
  {
    public UpstreamStorageId(UpstreamSourceInfo upstreamContentSource) => this.UpstreamContentSource = upstreamContentSource ?? throw new ArgumentNullException(nameof (upstreamContentSource));

    public UpstreamSourceInfo UpstreamContentSource { get; }

    public string ValueString => "upstream:" + this.UpstreamContentSource.Serialize<UpstreamSourceInfo>();

    public string NonLegacyValueString => this.ValueString;

    public bool IsCacheable => false;

    public bool IsLocal => false;

    public bool? RepresentsSingleFile => new bool?(false);

    public override string ToString() => "UpstreamStorageId(" + this.ValueString + ")";

    public bool Equals(UpstreamStorageId other)
    {
      if (other == null)
        return false;
      return this == other || this.UpstreamContentSource == other.UpstreamContentSource;
    }

    public bool Equals(IStorageId other) => this.Equals(other as UpstreamStorageId);

    public override bool Equals(object other) => this.Equals(other as UpstreamStorageId);

    public override int GetHashCode() => this.UpstreamContentSource.GetHashCode();
  }
}
