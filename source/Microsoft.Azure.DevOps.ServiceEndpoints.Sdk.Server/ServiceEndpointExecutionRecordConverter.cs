// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionRecordConverter
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class ServiceEndpointExecutionRecordConverter
  {
    public static IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord> ToContract(
      this IEnumerable<ServiceEndpointExecutionRecord> executionRecords)
    {
      if (executionRecords == null)
        return (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord>) null;
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord> contract1 = new List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord>();
      foreach (ServiceEndpointExecutionRecord executionRecord in executionRecords)
      {
        Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord contract2 = executionRecord.ToContract();
        contract1.Add(contract2);
      }
      return (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord>) contract1;
    }

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord ToContract(
      this ServiceEndpointExecutionRecord executionRecord)
    {
      if (executionRecord == null)
        return (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord) null;
      ServiceEndpointExecutionData data = executionRecord.Data;
      return new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord()
      {
        EndpointId = executionRecord.EndpointId,
        Data = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionData()
        {
          Id = data.Id,
          PlanType = data.PlanType,
          Definition = data.Definition.Clone(),
          Owner = data.Owner.Clone(),
          StartTime = data.StartTime,
          FinishTime = data.FinishTime,
          Result = data.Result,
          OwnerDetails = data.OwnerDetails
        }
      };
    }

    public static ServiceEndpointExecutionData FromContract(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionData webApiExecutionData)
    {
      if (webApiExecutionData == null)
        return (ServiceEndpointExecutionData) null;
      return new ServiceEndpointExecutionData()
      {
        Id = webApiExecutionData.Id,
        PlanType = webApiExecutionData.PlanType,
        Definition = webApiExecutionData.Definition.Clone(),
        Owner = webApiExecutionData.Owner.Clone(),
        StartTime = webApiExecutionData.StartTime,
        FinishTime = webApiExecutionData.FinishTime,
        Result = webApiExecutionData.Result
      };
    }
  }
}
