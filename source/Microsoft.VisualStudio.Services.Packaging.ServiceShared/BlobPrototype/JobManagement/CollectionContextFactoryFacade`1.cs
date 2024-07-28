// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.CollectionContextFactoryFacade`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class CollectionContextFactoryFacade<TResponse> : IFactory<CollectionId, TResponse> where TResponse : class
  {
    private readonly IVssRequestContext deploymentContext;
    private readonly Func<IVssRequestContext, TResponse> converter;

    public CollectionContextFactoryFacade(
      IVssRequestContext deploymentContext,
      Func<IVssRequestContext, TResponse> converter)
    {
      this.deploymentContext = deploymentContext;
      this.converter = converter;
    }

    public TResponse Get(CollectionId collectionId) => this.converter(this.Map(collectionId));

    private IVssRequestContext Map(CollectionId collectionId) => this.deploymentContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(this.deploymentContext, collectionId.Guid, RequestContextType.SystemContext);
  }
}
