// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DataImportVisitorBase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DataImport;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class DataImportVisitorBase
  {
    protected readonly IVssRequestContext DeploymentRequestContext;
    protected readonly FrameworkDataImportJobManager JobManager;
    protected readonly DataImportSecurityManager SecurityManager;

    public DataImportVisitorBase(
      IVssRequestContext deploymentRequestContext,
      FrameworkDataImportJobManager jobManager,
      DataImportSecurityManager securityManager)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentRequestContext, nameof (deploymentRequestContext));
      ArgumentUtility.CheckForNull<FrameworkDataImportJobManager>(jobManager, nameof (jobManager));
      ArgumentUtility.CheckForNull<DataImportSecurityManager>(securityManager, nameof (securityManager));
      deploymentRequestContext.CheckDeploymentRequestContext();
      deploymentRequestContext.CheckHostedDeployment();
      this.DeploymentRequestContext = deploymentRequestContext;
      this.JobManager = jobManager;
      this.SecurityManager = securityManager;
      this.SecurityManager.CheckPermission(this.DeploymentRequestContext, ServicingOrchestrationPermission.Queue);
    }

    protected void ValidateDataImportRequest(FrameworkDataImportRequest import)
    {
      ArgumentUtility.CheckForNull<FrameworkDataImportRequest>(import, nameof (import));
      ArgumentUtility.CheckForEmptyGuid(import.RequestId, "RequestId");
      ArgumentUtility.CheckForEmptyGuid(import.ServicingJobId, "ServicingJobId");
      ArgumentUtility.CheckForEmptyGuid(import.HostId, "HostId");
      this.JobManager.ValidateRequest(this.DeploymentRequestContext, import);
    }
  }
}
