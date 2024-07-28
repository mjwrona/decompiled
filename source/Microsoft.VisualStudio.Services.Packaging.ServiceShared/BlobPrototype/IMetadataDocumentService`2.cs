// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.IMetadataDocumentService`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public interface IMetadataDocumentService<TPackageIdentity, TMetadataEntry> : 
    IMetadataService<TPackageIdentity, TMetadataEntry>,
    IReadMetadataService<TPackageIdentity, TMetadataEntry>,
    IReadSingleVersionMetadataService<TPackageIdentity, TMetadataEntry>,
    IReadMetadataDocumentService<TPackageIdentity, TMetadataEntry>,
    IReadMetadataDocumentService
    where TPackageIdentity : IPackageIdentity
    where TMetadataEntry : class, IMetadataEntry<TPackageIdentity>
  {
  }
}
