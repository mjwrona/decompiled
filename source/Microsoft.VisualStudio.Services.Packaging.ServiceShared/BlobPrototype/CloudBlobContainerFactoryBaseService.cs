// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CloudBlobContainerFactoryBaseService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public abstract class CloudBlobContainerFactoryBaseService : 
    ICloudBlobContainerFactoryService,
    IVssFrameworkService
  {
    protected IFactory<IVssRequestContext, ICloudBlobContainerFactory> contextToContainerResolverFactory;
    protected ICloudBlobContainerFactory containerResolver;
    protected StrongBoxItemChangeHandler strongBoxConnectionStringChangeHandler;

    public static Guid ContainerDeletedNotificationGuid => new Guid("6d135210-f3c1-417c-a5b0-e1988233dfc0");

    public void ServiceStart(
      IVssRequestContext deploymentAndSystemRequestContext)
    {
      deploymentAndSystemRequestContext.CheckDeploymentRequestContext();
      if (this.contextToContainerResolverFactory == null)
        this.contextToContainerResolverFactory = (IFactory<IVssRequestContext, ICloudBlobContainerFactory>) new ByFuncInputFactory<IVssRequestContext, ICloudBlobContainerFactory>(new Func<IVssRequestContext, ICloudBlobContainerFactory>(this.RegisterKeyRotationEventsAndReturnResolverFactory));
      this.containerResolver = this.contextToContainerResolverFactory.Get(deploymentAndSystemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.strongBoxConnectionStringChangeHandler.OnStrongBoxItemChanged));

    private ICloudBlobContainerFactory RegisterKeyRotationEventsAndReturnResolverFactory(
      IVssRequestContext requestContext)
    {
      StrongBoxConnectionString accountConnectionString = this.ResolveStorageAccountConnectionString(requestContext);
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      AzureBlobContainerFactory containerFactory = new AzureBlobContainerFactory(accountConnectionString, CodeOnlyDeploymentsConstants.CodeOnlyContainerPrefix, "PrimaryThenSecondary", 32, (IRetryPolicy) new NoRetry());
      this.strongBoxConnectionStringChangeHandler = new StrongBoxItemChangeHandler((ISecretItemChangeListener) containerFactory);
      IVssRequestContext requestContext1 = requestContext;
      StrongBoxItemChangedCallback callback = new StrongBoxItemChangedCallback(this.strongBoxConnectionStringChangeHandler.OnStrongBoxItemChanged);
      string connectionStringDrawer = StorageAccountUtilities.StrongBoxConnectionStringDrawer;
      string[] filters = new string[1]
      {
        accountConnectionString.StrongBoxItemKey
      };
      service.RegisterNotification(requestContext1, callback, connectionStringDrawer, (IEnumerable<string>) filters);
      return (ICloudBlobContainerFactory) new AzureBlobContainerFactoryFacade(containerFactory, (ICancellationFacade) new CancellationFacade(requestContext));
    }

    protected abstract StrongBoxConnectionString ResolveStorageAccountConnectionString(
      IVssRequestContext requestContext);

    public IResolvedCloudBlobContainer GetContainer(
      IVssRequestContext requestContext,
      ContainerAddress containerAddress)
    {
      ICloudBlobContainer wrappedContainer = this.containerResolver.Get(BlobContainerUtils.ContainerAddressToName(containerAddress));
      return (IResolvedCloudBlobContainer) new ResolvedCloudBlobContainer(wrappedContainer, BlobContainerUtils.ContainerNameToAddress(wrappedContainer.Name));
    }

    public IEnumerable<IResolvedCloudBlobContainer> GetAllContainers(
      IVssRequestContext requestContext)
    {
      return (IEnumerable<IResolvedCloudBlobContainer>) this.containerResolver.Get().Select<ICloudBlobContainer, ResolvedCloudBlobContainer>((Func<ICloudBlobContainer, ResolvedCloudBlobContainer>) (c => new ResolvedCloudBlobContainer(c, BlobContainerUtils.ContainerNameToAddress(c.Name))));
    }

    public IEnumerable<IResolvedCloudBlobContainer> GetContainers(
      IVssRequestContext requestContext,
      ContainerAddress rootContainerAddress)
    {
      return (IEnumerable<IResolvedCloudBlobContainer>) this.containerResolver.GetByPrefix(BlobContainerUtils.ContainerAddressToName(rootContainerAddress)).Select<ICloudBlobContainer, ResolvedCloudBlobContainer>((Func<ICloudBlobContainer, ResolvedCloudBlobContainer>) (c => new ResolvedCloudBlobContainer(c, BlobContainerUtils.ContainerNameToAddress(c.Name)))).Where<ResolvedCloudBlobContainer>((Func<ResolvedCloudBlobContainer, bool>) (c => rootContainerAddress.IsAncestorOf(c.Address)));
    }
  }
}
