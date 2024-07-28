// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification.PackageGDPRDataStoreBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification
{
  public class PackageGDPRDataStoreBootstrapper : IBootstrapper<IPackageGdprDataStore>
  {
    private readonly IVssRequestContext deploymentContext;

    public PackageGDPRDataStoreBootstrapper(IVssRequestContext deploymentContext)
    {
      this.deploymentContext = deploymentContext;
      deploymentContext.CheckDeploymentRequestContext();
    }

    public IPackageGdprDataStore Bootstrap()
    {
      IResolvedCloudBlobContainerLister cloudBlobContainerFactory = (IResolvedCloudBlobContainerLister) new CloudBlobContainerFactoryServiceFacade(this.deploymentContext, (ICloudBlobContainerFactoryService) this.deploymentContext.GetService<CloudBlobContainerFactoryService>());
      return (IPackageGdprDataStore) new PackageGdprDataStore(BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(this.deploymentContext).Bootstrap(), cloudBlobContainerFactory, (ISerializer<PackageGdprData>) new JsonSerializer<PackageGdprData>(new JsonSerializerSettings()), new ContainerAddress((CollectionId) this.deploymentContext.ServiceHost.InstanceId, new Locator(new string[1]
      {
        "gdpr"
      })), (ITimeProvider) new DefaultTimeProvider());
    }
  }
}
