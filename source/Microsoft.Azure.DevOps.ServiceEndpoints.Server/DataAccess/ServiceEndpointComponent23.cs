// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent23
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.ServiceEndpoints;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class ServiceEndpointComponent23 : ServiceEndpointComponent22
  {
    internal override void ShareServiceEndpoint(
      Guid endpointId,
      List<ServiceEndpointProjectReference> serviceEndpointProjectReferences)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (ShareServiceEndpoint)))
      {
        Guid projectId = serviceEndpointProjectReferences.Count == 1 ? serviceEndpointProjectReferences[0].ProjectReference.Id : Guid.Empty;
        string actionId = projectId != new Guid() ? ServiceConnectionAuditConstants.ServiceConnectionShared : ServiceConnectionAuditConstants.ServiceConnectionSharedWithMultipleProjects;
        List<string> excludeParameters = new List<string>()
        {
          "@authorizationParameters"
        };
        if (projectId == new Guid())
          excludeParameters.Add("@serviceEndpointProjectReferences");
        this.PrepareForAuditingAction(actionId, projectId: projectId, excludeParameters: (IEnumerable<string>) excludeParameters);
        this.PrepareStoredProcedure("Task.prc_ShareServiceEndpoint");
        this.BindGuid("@endpointId", endpointId);
        IEnumerable<SqlDataRecord> rows;
        if (serviceEndpointProjectReferences == null)
        {
          rows = (IEnumerable<SqlDataRecord>) null;
        }
        else
        {
          IEnumerable<ServiceEndpointProjectReference> source = serviceEndpointProjectReferences.Where<ServiceEndpointProjectReference>((Func<ServiceEndpointProjectReference, bool>) (x => x?.ProjectReference != null));
          rows = source != null ? source.Select<ServiceEndpointProjectReference, SqlDataRecord>(new Func<ServiceEndpointProjectReference, SqlDataRecord>(((ServiceEndpointComponent21) this).ConvertToSqlDataRecord)) : (IEnumerable<SqlDataRecord>) null;
        }
        this.BindTable("@serviceEndpointProjectReferences", "Task.typ_ServiceEndpointProjectReferenceTable", rows, false);
        this.ExecuteNonQuery();
      }
    }

    internal override void ShareServiceEndpoint(
      Guid endpointId,
      Guid fromProject,
      Guid withProject)
    {
      List<ServiceEndpointProjectReference> serviceEndpointProjectReferences = new List<ServiceEndpointProjectReference>()
      {
        new ServiceEndpointProjectReference()
        {
          Name = string.Empty,
          ProjectReference = new ProjectReference()
          {
            Id = withProject
          }
        }
      };
      this.ShareServiceEndpoint(endpointId, serviceEndpointProjectReferences);
    }
  }
}
