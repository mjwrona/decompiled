// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification.GdprDataWriterBootstrapper`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification
{
  public class GdprDataWriterBootstrapper<TReq> : 
    IBootstrapper<IAsyncHandler<(TReq, ICommitLogEntry), bool>>
    where TReq : IFeedRequest
  {
    private IVssRequestContext requestContext;

    public GdprDataWriterBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<(TReq, ICommitLogEntry), bool> Bootstrap()
    {
      Guid instanceId = this.requestContext.ServiceHost.InstanceId;
      IVssRequestContext deploymentContext = this.requestContext.To(TeamFoundationHostType.Deployment);
      CollectionId collectionId = (CollectionId) instanceId;
      IPackageGdprDataStore gdprDataStore = new PackageGDPRDataStoreBootstrapper(deploymentContext).Bootstrap();
      PackageGdprData packageGdprData = new PackageGdprData();
      packageGdprData.IpAddress = this.requestContext.RemoteIPAddress();
      IFeatureFlagService featureFlagFacade = this.requestContext.GetFeatureFlagFacade();
      IExecutionEnvironment environmentFacade = this.requestContext.GetExecutionEnvironmentFacade();
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      return (IAsyncHandler<(TReq, ICommitLogEntry), bool>) new GdprDataWriter<TReq>(collectionId, gdprDataStore, packageGdprData, featureFlagFacade, environmentFacade, tracerFacade);
    }
  }
}
