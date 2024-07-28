// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent25
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class ServiceEndpointComponent25 : ServiceEndpointComponent24
  {
    internal override IList<Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionRecord> AddServiceEndpointExecutionHistory(
      Guid scopeId,
      ServiceEndpointExecutionRecordsInput input)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (AddServiceEndpointExecutionHistory)))
      {
        this.PrepareForAuditingAction(ServiceConnectionAuditConstants.ServiceConnectionExecuted, projectId: scopeId, excludeParameters: (IEnumerable<string>) new string[3]
        {
          "@definition",
          "@owner",
          "@endpointIdList"
        });
        this.PrepareStoredProcedure("Task.prc_AddServiceEndpointExecutionHistory");
        this.BindDataspaceId(scopeId);
        this.BindGuidTable("@endpointIdList", (IEnumerable<Guid>) input.EndpointIds);
        this.BindString("@planType", input.Data.PlanType.Trim(), 128, false, SqlDbType.NVarChar);
        ServiceEndpointExecutionOwner definition = input.Data.Definition;
        this.BindNullableInt("@definitionId", definition != null ? definition.Id : 0, 0);
        this.BindBinary("@definition", JsonUtility.Serialize((object) input.Data.Definition, false), int.MaxValue, SqlDbType.VarBinary);
        ServiceEndpointExecutionOwner owner = input.Data.Owner;
        this.BindNullableInt("@ownerId", owner != null ? owner.Id : 0, 0);
        this.BindBinary("@owner", JsonUtility.Serialize((object) input.Data.Owner, false), int.MaxValue, SqlDbType.VarBinary);
        this.BindString("@ownerDetails", input.Data.OwnerDetails?.Trim(), 256, true, SqlDbType.NVarChar);
        DateTime? nullable = input.Data.StartTime;
        if (nullable.HasValue)
        {
          nullable = input.Data.StartTime;
          this.BindDateTime("@startTime", nullable.Value);
        }
        nullable = input.Data.FinishTime;
        if (nullable.HasValue)
        {
          nullable = input.Data.FinishTime;
          this.BindDateTime("@finishTime", nullable.Value);
        }
        ServiceEndpointExecutionResult? result = input.Data.Result;
        if (result.HasValue)
        {
          result = input.Data.Result;
          this.BindByte("@result", (byte) result.Value);
        }
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionRecord>((ObjectBinder<Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionRecord>) new ServiceEndpointExecutionRecordBinder());
          return (IList<Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionRecord>) resultCollection.GetCurrent<Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionRecord>().Items;
        }
      }
    }
  }
}
