// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent15
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class ServiceEndpointComponent15 : ServiceEndpointComponent14
  {
    internal override void ShareServiceEndpoint(
      Guid endpointId,
      Guid fromProject,
      Guid withProject)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (ShareServiceEndpoint)))
      {
        this.PrepareStoredProcedure("Task.prc_ShareServiceEndpoint");
        this.BindGuid("@endpointId", endpointId);
        this.BindInt("@fromDataspaceId", this.GetDataspaceId(fromProject));
        this.BindInt("@withDataspaceId", this.GetDataspaceId(withProject));
        this.ExecuteNonQuery();
      }
    }

    internal override async Task<List<ServiceEndpointProjectReferenceResult>> QueryServiceEndpointSharedProjectsAsync(
      IEnumerable<Guid> endpointIds,
      Guid project)
    {
      ServiceEndpointComponent15 component = this;
      List<ServiceEndpointProjectReferenceResult> items;
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) component, nameof (QueryServiceEndpointSharedProjectsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetServiceEndpointSharedProjects");
        component.BindGuidTable("@endpointIds", endpointIds);
        component.BindInt("@forDataspaceId", project.Equals(Guid.Empty) ? 0 : component.GetDataspaceId(project));
        component.DataspaceRlsEnabled = false;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpointProjectReferenceResult>((ObjectBinder<ServiceEndpointProjectReferenceResult>) new ServiceEndpointProjectReferenceBinder((ServiceEndpointComponent) component));
          items = resultCollection.GetCurrent<ServiceEndpointProjectReferenceResult>().Items;
        }
      }
      return items;
    }
  }
}
