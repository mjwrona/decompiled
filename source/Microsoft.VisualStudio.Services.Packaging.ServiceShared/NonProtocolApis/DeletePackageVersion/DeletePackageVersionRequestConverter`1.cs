// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion.DeletePackageVersionRequestConverter`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion
{
  public class DeletePackageVersionRequestConverter<TPackageIdentity> : 
    IConverter<IPackageRequest<TPackageIdentity>, IPackageRequest<TPackageIdentity, DeleteRequestAdditionalData>>,
    IHaveInputType<IPackageRequest<TPackageIdentity>>,
    IHaveOutputType<IPackageRequest<TPackageIdentity, DeleteRequestAdditionalData>>
    where TPackageIdentity : IPackageIdentity
  {
    private readonly ITimeProvider timeProvider;

    public DeletePackageVersionRequestConverter(ITimeProvider timeProvider) => this.timeProvider = timeProvider;

    public IPackageRequest<TPackageIdentity, DeleteRequestAdditionalData> Convert(
      IPackageRequest<TPackageIdentity> input)
    {
      return (IPackageRequest<TPackageIdentity, DeleteRequestAdditionalData>) input.WithData<TPackageIdentity, DeleteRequestAdditionalData>(new DeleteRequestAdditionalData(this.timeProvider.Now));
    }
  }
}
