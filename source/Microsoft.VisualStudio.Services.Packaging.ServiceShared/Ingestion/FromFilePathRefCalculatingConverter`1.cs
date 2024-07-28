// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.FromFilePathRefCalculatingConverter`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion
{
  public class FromFilePathRefCalculatingConverter<TPackageId> : 
    IConverter<IPackageFileRequest<TPackageId>, string>,
    IHaveInputType<IPackageFileRequest<TPackageId>>,
    IHaveOutputType<string>
    where TPackageId : IPackageIdentity
  {
    public string Convert(IPackageFileRequest<TPackageId> input) => string.Format("feed/{0}/{1}.{2}.{3}", (object) input.Feed.Id, (object) input.PackageId.Name.NormalizedName, (object) input.PackageId.Version.NormalizedVersion, (object) input.FilePath);
  }
}
