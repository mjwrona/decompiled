// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent11
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class ServiceEndpointComponent11 : ServiceEndpointComponent10
  {
    public ServiceEndpointComponent11() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    internal override void AddServiceEndpoint(
      Guid scopeId,
      ServiceEndpoint serviceEndpoint,
      bool disallowDuplicateEndpointName)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (AddServiceEndpoint)))
      {
        this.PrepareStoredProcedure("Task.prc_AddServiceEndpoint");
        this.BindDataspaceId(scopeId);
        this.BindGuid("@id", serviceEndpoint.Id);
        this.BindString("@name", serviceEndpoint.Name.Trim(), 512, false, SqlDbType.NVarChar);
        this.BindString("@type", serviceEndpoint.Type.Trim(), 128, false, SqlDbType.NVarChar);
        this.BindString("@url", serviceEndpoint.Url?.OriginalString, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@description", serviceEndpoint.Description, int.MaxValue, true, SqlDbType.NVarChar);
        this.BindGuid("@createdBy", new Guid(serviceEndpoint.CreatedBy.Id));
        this.BindString("@authorizationScheme", serviceEndpoint.Authorization.Scheme, 128, false, SqlDbType.NVarChar);
        this.BindString("@data", JsonUtility.ToString((object) serviceEndpoint.Data), int.MaxValue, true, SqlDbType.NVarChar);
        this.BindBoolean("@isReady", serviceEndpoint.IsReady);
        this.BindString("@operationStatus", JsonUtility.ToString<JToken>((IList<JToken>) serviceEndpoint.OperationStatus), int.MaxValue, true, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }
  }
}
