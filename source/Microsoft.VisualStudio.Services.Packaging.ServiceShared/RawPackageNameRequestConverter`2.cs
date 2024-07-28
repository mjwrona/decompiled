// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RawPackageNameRequestConverter`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class RawPackageNameRequestConverter<TPackageName, TData> : 
    IConverter<IRawPackageNameRequest<TData>, IPackageNameRequest<TPackageName, TData>>,
    IHaveInputType<IRawPackageNameRequest<TData>>,
    IHaveOutputType<IPackageNameRequest<TPackageName, TData>>
    where TPackageName : IPackageName
    where TData : class
  {
    private IConverter<IRawPackageNameRequest, IPackageNameRequest<TPackageName>> nameRequestConverter;

    public RawPackageNameRequestConverter(
      IConverter<IRawPackageNameRequest, IPackageNameRequest<TPackageName>> nameRequestConverter)
    {
      this.nameRequestConverter = nameRequestConverter;
    }

    public IPackageNameRequest<TPackageName, TData> Convert(IRawPackageNameRequest<TData> input) => (IPackageNameRequest<TPackageName, TData>) new PackageNameRequest<TPackageName, TData>(this.nameRequestConverter.Convert((IRawPackageNameRequest) input), input.AdditionalData);
  }
}
