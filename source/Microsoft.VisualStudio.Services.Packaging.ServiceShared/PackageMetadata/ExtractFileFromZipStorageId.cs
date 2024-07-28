// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.ExtractFileFromZipStorageId
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public sealed class ExtractFileFromZipStorageId : 
    IExtractingStorageId,
    IStorageId,
    IEquatable<IStorageId>,
    IEquatable<ExtractFileFromZipStorageId>
  {
    public IStorageId ExtractFrom { get; }

    public string Path { get; }

    public ExtractFileFromZipStorageId(IStorageId from, string path)
    {
      if (string.IsNullOrWhiteSpace(path))
        throw new ArgumentException(Resources.Error_ValueCannotBeNull((object) nameof (path)), nameof (path));
      this.ExtractFrom = from ?? throw new ArgumentNullException(nameof (from));
      this.Path = path;
    }

    public string ValueString => this.NonLegacyValueString;

    public string NonLegacyValueString => "zip-extract:" + this.ToJsonForm().Serialize<ExtractFileFromZipStorageId.JsonForm>(true);

    public bool IsCacheable => this.ExtractFrom.IsCacheable;

    public bool IsLocal => this.ExtractFrom.IsLocal;

    public bool? RepresentsSingleFile => new bool?(true);

    private ExtractFileFromZipStorageId.JsonForm ToJsonForm() => new ExtractFileFromZipStorageId.JsonForm(this.ExtractFrom.NonLegacyValueString, this.Path);

    private static ExtractFileFromZipStorageId FromJsonForm(
      ExtractFileFromZipStorageId.JsonForm jsonForm)
    {
      return new ExtractFileFromZipStorageId(StorageId.Parse(jsonForm.From), jsonForm.Path);
    }

    public static ExtractFileFromZipStorageId Parse(string valueStringAfterColon) => ExtractFileFromZipStorageId.FromJsonForm(JsonUtilities.Deserialize<ExtractFileFromZipStorageId.JsonForm>(valueStringAfterColon));

    public override string ToString() => "ExtractFileFromZipStorageId(" + this.ValueString + ")";

    public bool Equals(ExtractFileFromZipStorageId? other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return this.ExtractFrom.Equals(other.ExtractFrom) && this.Path.Equals(other.Path);
    }

    public bool Equals(IStorageId? other) => this.Equals(other as ExtractFileFromZipStorageId);

    public override bool Equals(object? other) => this.Equals(other as ExtractFileFromZipStorageId);

    public override int GetHashCode() => this.ExtractFrom.GetHashCode() * 397 ^ this.Path.GetHashCode();

    private record JsonForm(string From, string Path);
  }
}
