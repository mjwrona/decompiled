// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DataImport.DataImportJobManager`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DataImport
{
  public abstract class DataImportJobManager<T> : ServicingOrchestrationJobManager<T> where T : ServicingOrchestrationRequest
  {
    public override string Area => "DataImport";

    protected override void ThrowRequestNotFoundException(Guid requestId) => throw new DataImportEntryDoesNotExistException(requestId);

    protected override void ThrowRequestInProgressException(T request) => throw new DataImportInProgressException(string.Format("Expected job {0} to be stopped and fully disabled before scheduling next activity {1}", (object) request.RequestId, (object) request.GetType().Name));

    protected override void ValidateRequest(
      IVssRequestContext requestContext,
      T request,
      TeamFoundationJobDefinition jobDefinition)
    {
      requestContext.CheckDeploymentRequestContext();
    }

    public override ServicingOrchestrationRequestStatus GetJobStatus(
      IVssRequestContext requestContext,
      Guid requestId)
    {
      ServicingOrchestrationRequestStatus jobStatus = base.GetJobStatus(requestContext, requestId);
      this.ProcessRequestStatusData(requestContext, requestId, jobStatus);
      return jobStatus;
    }

    private void ProcessRequestStatusData(
      IVssRequestContext requestContext,
      Guid importId,
      ServicingOrchestrationRequestStatus status)
    {
      this.ProcessStatusProperties(requestContext, status.Properties, importId, "/Configuration/DataImport/{0}/Ci/**", "DataImportCi.");
      this.ProcessStatusProperties(requestContext, status.Properties, importId, "/Configuration/DataImport/{0}/ResultProperties/**", "DataImportResponse.");
    }

    private void ProcessStatusProperties(
      IVssRequestContext requestContext,
      PropertyCollection properties,
      Guid importId,
      string registryPath,
      string prefix)
    {
      ISqlRegistryService service = requestContext.GetService<ISqlRegistryService>();
      RegistryQuery registryQuery = new RegistryQuery(RegistryHelper.GetDataImportSpecificKey(importId, registryPath));
      IVssRequestContext requestContext1 = requestContext;
      ref RegistryQuery local = ref registryQuery;
      foreach (RegistryItem registryItem in service.Read(requestContext1, in local))
      {
        string name = prefix + ((IEnumerable<string>) registryItem.Path.Split('/')).LastOrDefault<string>();
        if (!string.IsNullOrEmpty(registryItem.Value) && string.IsNullOrEmpty(properties[name]))
          properties[name] = registryItem.Value;
      }
    }
  }
}
