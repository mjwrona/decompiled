// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CloudBlobContainerFactoryServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class CloudBlobContainerFactoryServiceFacade : 
    IResolvedCloudBlobContainerFactory,
    IFactory<ContainerAddress, IResolvedCloudBlobContainer>,
    IResolvedCloudBlobContainerLister
  {
    private readonly IVssRequestContext requestContext;
    private readonly ICloudBlobContainerFactoryService service;

    public CloudBlobContainerFactoryServiceFacade(
      IVssRequestContext requestContext,
      ICloudBlobContainerFactoryService service)
    {
      this.requestContext = requestContext;
      this.service = service;
    }

    public IEnumerable<IResolvedCloudBlobContainer> GetFromRoot(
      ContainerAddress rootContainerAddress)
    {
      return this.service.GetContainers(this.requestContext, rootContainerAddress);
    }

    public IResolvedCloudBlobContainer Get(ContainerAddress containerAddress) => this.service.GetContainer(this.requestContext, containerAddress);

    public IEnumerable<IResolvedCloudBlobContainer> GetAllContainers() => this.service.GetAllContainers(this.requestContext);
  }
}
