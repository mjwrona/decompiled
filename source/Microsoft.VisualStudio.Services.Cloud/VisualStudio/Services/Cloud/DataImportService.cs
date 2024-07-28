// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DataImportService
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
  public class DataImportService : IDataImportService, IVssFrameworkService
  {
    private readonly FrameworkDataImportJobManager m_jobManager = new FrameworkDataImportJobManager();
    private readonly DataImportSecurityManager m_securityManager = new DataImportSecurityManager();
    internal const string Area = "DataImport";
    internal const string Layer = "DataImportService";

    public void ServiceStart(IVssRequestContext systemRequestContext) => DataImportService.CheckRequestContext(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual bool ValidateRequest(
      IVssRequestContext deploymentRequestContext,
      FrameworkDataImportRequest request)
    {
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentRequestContext, nameof (deploymentRequestContext));
        ArgumentUtility.CheckForNull<FrameworkDataImportRequest>(request, nameof (request));
        return request.Accept<bool>((IFrameworkDataImportRequestVisitor<bool>) new DataImportValidateVisitor(deploymentRequestContext, this.m_jobManager, this.m_securityManager));
      }
      catch (Exception ex)
      {
        deploymentRequestContext.TraceException(1006101, "DataImport", nameof (DataImportService), ex);
        throw;
      }
    }

    public virtual void QueueRequest(
      IVssRequestContext deploymentRequestContext,
      FrameworkDataImportRequest request)
    {
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentRequestContext, nameof (deploymentRequestContext));
        ArgumentUtility.CheckForNull<FrameworkDataImportRequest>(request, nameof (request));
        if (!this.ValidateRequest(deploymentRequestContext, request))
          throw new ApplicationException(request?.GetType()?.FullName + " returned false during validation, this request should not have been queued");
        request = request.Accept<FrameworkDataImportRequest>((IFrameworkDataImportRequestVisitor<FrameworkDataImportRequest>) new DataImportPrepareQueueVisitor(deploymentRequestContext, this.m_jobManager, this.m_securityManager));
        this.m_jobManager.QueueJob(deploymentRequestContext, request);
      }
      catch (Exception ex)
      {
        deploymentRequestContext.TraceException(1006102, "DataImport", nameof (DataImportService), ex);
        throw;
      }
    }

    public void CancelDataImportJob(IVssRequestContext deploymentRequestContext, Guid requestId)
    {
      ArgumentUtility.CheckForEmptyGuid(requestId, nameof (requestId));
      DataImportService.CheckRequestContext(deploymentRequestContext);
      this.m_securityManager.CheckPermission(deploymentRequestContext, ServicingOrchestrationPermission.Cancel);
      this.m_jobManager.StopJob(deploymentRequestContext, requestId);
    }

    public DataImportRequestStatus GetDataImportInfoFromId(
      IVssRequestContext deploymentRequestContext,
      Guid importId)
    {
      DataImportService.CheckRequestContext(deploymentRequestContext);
      ArgumentUtility.CheckForEmptyGuid(importId, nameof (importId));
      this.m_securityManager.CheckPermission(deploymentRequestContext, ServicingOrchestrationPermission.Read);
      return new DataImportRequestStatus(this.m_jobManager.GetJobStatus(deploymentRequestContext, importId));
    }

    private static void CheckRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckDeploymentRequestContext();
      requestContext.CheckHostedDeployment();
    }
  }
}
