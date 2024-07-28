// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DataImportPrepareQueueVisitor
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DataImport;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class DataImportPrepareQueueVisitor : 
    DataImportVisitorBase,
    IFrameworkDataImportRequestVisitor<FrameworkDataImportRequest>
  {
    public DataImportPrepareQueueVisitor(
      IVssRequestContext deploymentRequestContext,
      FrameworkDataImportJobManager jobManager,
      DataImportSecurityManager securityManager)
      : base(deploymentRequestContext, jobManager, securityManager)
    {
    }

    public FrameworkDataImportRequest Visit(CreateCollectionDataImportRequest request) => (FrameworkDataImportRequest) request;

    public FrameworkDataImportRequest Visit(DatabaseDataImportRequest request)
    {
      ArgumentUtility.CheckForNull<DatabaseDataImportRequest>(request, nameof (request));
      ArgumentUtility.CheckForEmptyGuid(request.HostId, "HostId");
      new Microsoft.VisualStudio.Services.DataImport.ValidationHelper().CheckTargetCollectionHost(this.DeploymentRequestContext, request);
      request = new DatabaseDataImportRequest(request);
      SecurityHelper securityHelper = new SecurityHelper(request.RequestId);
      request.ConnectionString = securityHelper.SecureConnectionString(this.DeploymentRequestContext.Elevate(), request.ConnectionString);
      return (FrameworkDataImportRequest) request;
    }

    public FrameworkDataImportRequest Visit(FileCopyDataImportRequest request) => throw new InvalidOperationException("Request of type FileCopyDataImportRequest is not expected to be explicitly queued");

    public FrameworkDataImportRequest Visit(HostUpgradeDataImportRequest request) => (FrameworkDataImportRequest) request;

    public FrameworkDataImportRequest Visit(OnlinePostHostUpgradeDataImportRequest request) => (FrameworkDataImportRequest) request;

    public FrameworkDataImportRequest Visit(StopHostAfterUpgradeDataImportRequest request) => (FrameworkDataImportRequest) request;

    public FrameworkDataImportRequest Visit(ObtainDatabaseHoldDataImportRequest request) => (FrameworkDataImportRequest) request;

    public FrameworkDataImportRequest Visit(HostMoveDataImportRequest request) => (FrameworkDataImportRequest) request;

    public FrameworkDataImportRequest Visit(ActivateDataImportRequest request) => (FrameworkDataImportRequest) request;

    public FrameworkDataImportRequest Visit(RemoveDataImportRequest request) => (FrameworkDataImportRequest) request;

    public FrameworkDataImportRequest Visit(DataImportDehydrateRequest request)
    {
      this.ValidateDataImportRequest((FrameworkDataImportRequest) request);
      return (FrameworkDataImportRequest) request;
    }
  }
}
