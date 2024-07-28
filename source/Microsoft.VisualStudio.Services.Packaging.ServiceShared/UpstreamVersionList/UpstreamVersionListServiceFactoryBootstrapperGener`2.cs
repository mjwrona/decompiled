// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList.UpstreamVersionListServiceFactoryBootstrapperGenericizingAdapter`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList
{
  public class UpstreamVersionListServiceFactoryBootstrapperGenericizingAdapter<TPackageName, TPackageVersion> : 
    IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamVersionListService>>>
    where TPackageName : IPackageName
    where TPackageVersion : IPackageVersion
  {
    private readonly IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamVersionListService<TPackageName, TPackageVersion>>>> bootstrapperImplementation;

    public UpstreamVersionListServiceFactoryBootstrapperGenericizingAdapter(
      IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamVersionListService<TPackageName, TPackageVersion>>>> bootstrapperImplementation)
    {
      this.bootstrapperImplementation = bootstrapperImplementation;
    }

    public IFactory<IFeedRequest, Task<IUpstreamVersionListService>> Bootstrap() => this.bootstrapperImplementation.Bootstrap().ConvertBy<IFeedRequest, Task<IUpstreamVersionListService<TPackageName, TPackageVersion>>, Task<IUpstreamVersionListService>>((Func<Task<IUpstreamVersionListService<TPackageName, TPackageVersion>>, Task<IUpstreamVersionListService>>) (async x => (IUpstreamVersionListService) await x));
  }
}
