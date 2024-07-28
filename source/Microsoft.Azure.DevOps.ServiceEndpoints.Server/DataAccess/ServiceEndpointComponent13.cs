// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent13
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
  internal class ServiceEndpointComponent13 : ServiceEndpointComponent12
  {
    public ServiceEndpointComponent13() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    internal override void UpdateServiceEndpoint(
      Guid scopeId,
      ServiceEndpoint serviceEndpoint,
      bool disallowDuplicateEndpointName)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (UpdateServiceEndpoint)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateServiceEndpoint");
        Guid parameterValue = serviceEndpoint.RemoveAuthConfigurationIfRequired();
        this.BindDataspaceId(scopeId);
        this.BindGuid("@id", serviceEndpoint.Id);
        this.BindString("@name", serviceEndpoint.Name.Trim(), 512, false, SqlDbType.NVarChar);
        this.BindString("@type", serviceEndpoint.Type.Trim(), 128, false, SqlDbType.NVarChar);
        this.BindString("@description", serviceEndpoint.Description, int.MaxValue, true, SqlDbType.NVarChar);
        this.BindString("@authorizationScheme", serviceEndpoint.Authorization.Scheme, 128, false, SqlDbType.NVarChar);
        this.BindString("@data", JsonUtility.ToString((object) serviceEndpoint.Data), int.MaxValue, true, SqlDbType.NVarChar);
        this.BindBoolean("@isReady", serviceEndpoint.IsReady);
        this.BindString("@operationStatus", JsonUtility.ToString<JToken>((IList<JToken>) serviceEndpoint.OperationStatus), int.MaxValue, true, SqlDbType.NVarChar);
        this.BindNullableGuid("@configurationId", parameterValue);
        this.BindString("@url", serviceEndpoint.Url?.OriginalString, int.MaxValue, true, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    internal override IList<ServiceEndpointOAuthConfigurationReference> GetServiceEndpointsByConfigurationId(
      IVssRequestContext requestContext,
      Guid configurationId)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (GetServiceEndpointsByConfigurationId)))
      {
        this.PrepareStoredProcedure("Task.prc_GetServiceEndpointsByConfigurationId");
        this.BindGuid("@configurationId", configurationId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpointOAuthConfigurationReference>((ObjectBinder<ServiceEndpointOAuthConfigurationReference>) new ServiceEndpointOAuthConfigurationBinder((ServiceEndpointComponent) this));
          return (IList<ServiceEndpointOAuthConfigurationReference>) resultCollection.GetCurrent<ServiceEndpointOAuthConfigurationReference>().Items;
        }
      }
    }

    internal override void MarkOAuth2EndpointsAsDirty(
      IVssRequestContext requestContext,
      Guid dataspaceId,
      IList<Guid> endpointsToBeMarkedDirty,
      string operationStatus)
    {
      this.PrepareStoredProcedure("Task.prc_MarkOAuth2EndpointsAsDirty");
      this.BindDataspaceId(dataspaceId);
      this.BindGuidTable("@endpointIds", (IEnumerable<Guid>) endpointsToBeMarkedDirty);
      this.BindString("@operationStatus", operationStatus, int.MaxValue, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }
  }
}
