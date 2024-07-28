// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DataImportValidateVisitor
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DataImport;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class DataImportValidateVisitor : 
    DataImportVisitorBase,
    IFrameworkDataImportRequestVisitor<bool>
  {
    public DataImportValidateVisitor(
      IVssRequestContext deploymentRequestContext,
      FrameworkDataImportJobManager jobManager,
      DataImportSecurityManager securityManager)
      : base(deploymentRequestContext, jobManager, securityManager)
    {
    }

    public bool Visit(CreateCollectionDataImportRequest request)
    {
      ArgumentUtility.CheckForNull<CreateCollectionDataImportRequest>(request, nameof (request));
      ArgumentUtility.CheckForEmptyGuid(request.RequestId, "RequestId");
      ArgumentUtility.CheckForEmptyGuid(request.ServicingJobId, "ServicingJobId");
      if (this.DeploymentRequestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS)
        throw new InvalidOperationException("Data Import Collection can only be created in SPS");
      return true;
    }

    public bool Visit(DatabaseDataImportRequest request)
    {
      ArgumentUtility.CheckForNull<DatabaseDataImportRequest>(request, nameof (request));
      ArgumentUtility.CheckForEmptyGuid(request.RequestId, "RequestId");
      Microsoft.VisualStudio.Services.DataImport.ValidationHelper validationHelper = new Microsoft.VisualStudio.Services.DataImport.ValidationHelper();
      if (!string.IsNullOrEmpty(request.ConnectionString))
        validationHelper.ValidateDataImportDatabase(this.DeploymentRequestContext, request);
      else
        validationHelper.ValidateSupportedMilestonesFromProperties(this.DeploymentRequestContext, request);
      validationHelper.ValidateDataImportAccountRegion(this.DeploymentRequestContext, request);
      validationHelper.ValidateDataImportAccountOwner(this.DeploymentRequestContext, request);
      this.DeploymentRequestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(this.DeploymentRequestContext, (object) request);
      return true;
    }

    public bool Visit(FileCopyDataImportRequest request) => throw new InvalidOperationException("Request of type FileCopyDataImportRequest is not expected to be sent via http request");

    public bool Visit(HostUpgradeDataImportRequest request)
    {
      this.ValidateDataImportRequest((FrameworkDataImportRequest) request);
      return new Microsoft.VisualStudio.Services.DataImport.ValidationHelper().CheckHostUpgradeRequest(this.DeploymentRequestContext, request);
    }

    public bool Visit(OnlinePostHostUpgradeDataImportRequest request)
    {
      this.ValidateDataImportRequest((FrameworkDataImportRequest) request);
      new Microsoft.VisualStudio.Services.DataImport.ValidationHelper().CheckOnlinePostHostUpgrade(this.DeploymentRequestContext, request);
      return true;
    }

    public bool Visit(StopHostAfterUpgradeDataImportRequest request)
    {
      this.ValidateDataImportRequest((FrameworkDataImportRequest) request);
      new Microsoft.VisualStudio.Services.DataImport.ValidationHelper().CheckStopHostAfterUpgrade(this.DeploymentRequestContext, request);
      return true;
    }

    public bool Visit(ObtainDatabaseHoldDataImportRequest request)
    {
      this.ValidateDataImportRequest((FrameworkDataImportRequest) request);
      new Microsoft.VisualStudio.Services.DataImport.ValidationHelper().CheckObtainDatabaseHold(this.DeploymentRequestContext, request);
      return true;
    }

    public bool Visit(HostMoveDataImportRequest request)
    {
      this.ValidateDataImportRequest((FrameworkDataImportRequest) request);
      new Microsoft.VisualStudio.Services.DataImport.ValidationHelper().CheckHostMoveRequest(this.DeploymentRequestContext, request);
      return true;
    }

    public bool Visit(ActivateDataImportRequest request)
    {
      this.ValidateDataImportRequest((FrameworkDataImportRequest) request);
      new Microsoft.VisualStudio.Services.DataImport.ValidationHelper().CheckActivateRequest(this.DeploymentRequestContext, request);
      return true;
    }

    public bool Visit(DataImportDehydrateRequest request) => true;

    public bool Visit(RemoveDataImportRequest request)
    {
      this.SecurityManager.CheckPermission(this.DeploymentRequestContext, ServicingOrchestrationPermission.Queue | ServicingOrchestrationPermission.Cancel);
      this.ValidateDataImportRequest((FrameworkDataImportRequest) request);
      return true;
    }
  }
}
