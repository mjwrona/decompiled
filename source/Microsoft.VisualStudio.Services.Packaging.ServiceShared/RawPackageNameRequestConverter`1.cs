// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RawPackageNameRequestConverter`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class RawPackageNameRequestConverter<TPackageName> : 
    IConverter<IRawPackageNameRequest, IPackageNameRequest<TPackageName>>,
    IHaveInputType<IRawPackageNameRequest>,
    IHaveOutputType<IPackageNameRequest<TPackageName>>
    where TPackageName : IPackageName
  {
    private readonly IConverter<string, TPackageName> nameConverter;

    public RawPackageNameRequestConverter(IConverter<string, TPackageName> nameConverter) => this.nameConverter = nameConverter;

    public IPackageNameRequest<TPackageName> Convert(IRawPackageNameRequest rawRequest) => (IPackageNameRequest<TPackageName>) new PackageNameRequest<TPackageName>((IFeedRequest) rawRequest, this.nameConverter.Convert(rawRequest.PackageName));
  }
}
