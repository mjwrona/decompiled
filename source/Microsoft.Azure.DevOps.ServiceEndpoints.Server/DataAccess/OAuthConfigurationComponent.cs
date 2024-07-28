// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.OAuthConfigurationComponent
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class OAuthConfigurationComponent : ServiceEndpointsSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<OAuthConfigurationComponent>(1, true)
    }, "DistributedTaskOAuthConfiguration", "DistributedTask");

    public OAuthConfigurationComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public IList<OAuthConfiguration> GetOAuthConfigurations(string endpointType)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (GetOAuthConfigurations)))
      {
        this.PrepareStoredProcedure("Task.prc_GetOAuthConfigurations");
        this.BindDataspaceId(Guid.Empty);
        this.BindString("@endpointType", endpointType, 128, true, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<OAuthConfiguration>((ObjectBinder<OAuthConfiguration>) this.GetOAuthConfigurationBinder());
          return (IList<OAuthConfiguration>) resultCollection.GetCurrent<OAuthConfiguration>().Items;
        }
      }
    }

    internal virtual OAuthConfiguration GetOAuthConfiguration(Guid id)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (GetOAuthConfiguration)))
      {
        this.PrepareStoredProcedure("Task.prc_GetOAuthConfiguration");
        this.BindDataspaceId(Guid.Empty);
        this.BindGuid("@id", id);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<OAuthConfiguration>((ObjectBinder<OAuthConfiguration>) this.GetOAuthConfigurationBinder());
          return resultCollection.GetCurrent<OAuthConfiguration>().Items.FirstOrDefault<OAuthConfiguration>();
        }
      }
    }

    public IList<OAuthConfiguration> GetOAuthConfigurationsByIds(IList<Guid> configurationIds)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (GetOAuthConfigurationsByIds)))
      {
        this.PrepareStoredProcedure("Task.prc_GetOAuthConfigurationsByIds");
        this.BindDataspaceId(Guid.Empty);
        this.BindGuidTable("@configurationIds", configurationIds == null ? (IEnumerable<Guid>) null : (IEnumerable<Guid>) configurationIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<OAuthConfiguration>((ObjectBinder<OAuthConfiguration>) this.GetOAuthConfigurationBinder());
          return (IList<OAuthConfiguration>) resultCollection.GetCurrent<OAuthConfiguration>().Items;
        }
      }
    }

    internal virtual OAuthConfiguration AddOAuthConfiguration(
      IVssRequestContext requestContext,
      OAuthConfigurationParams configurationParams)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (AddOAuthConfiguration)))
      {
        this.PrepareStoredProcedure("Task.prc_AddOAuthConfiguration");
        this.BindDataspaceId(Guid.Empty);
        this.BindGuid("@id", Guid.NewGuid());
        this.BindString("@name", configurationParams.Name.Trim(), 512, false, SqlDbType.NVarChar);
        this.BindString("@url", configurationParams.Url.OriginalString, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@endpointType", configurationParams.EndpointType.Trim(), 128, false, SqlDbType.NVarChar);
        this.BindString("@clientId", configurationParams.ClientId, 128, false, SqlDbType.NVarChar);
        this.BindGuid("@createdBy", new Guid(this.GetUser(requestContext).Id));
        this.BindDateTime2("@createdOn", DateTime.UtcNow);
        this.BindGuid("@modifiedBy", new Guid(this.GetUser(requestContext).Id));
        this.BindDateTime2("@modifiedOn", DateTime.UtcNow);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<OAuthConfiguration>((ObjectBinder<OAuthConfiguration>) new OAuthConfigurationBinder());
          return resultCollection.GetCurrent<OAuthConfiguration>().FirstOrDefault<OAuthConfiguration>();
        }
      }
    }

    internal virtual OAuthConfiguration DeleteOAuthConfiguration(Guid configurationId)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (DeleteOAuthConfiguration)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteOAuthConfiguration");
        this.BindDataspaceId(Guid.Empty);
        this.BindGuid("@id", configurationId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<OAuthConfiguration>((ObjectBinder<OAuthConfiguration>) new OAuthConfigurationBinder());
          return resultCollection.GetCurrent<OAuthConfiguration>().FirstOrDefault<OAuthConfiguration>();
        }
      }
    }

    internal virtual OAuthConfiguration UpdateOAuthConfiguration(
      IVssRequestContext requestContext,
      Guid configurationId,
      OAuthConfigurationParams configurationParams)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (UpdateOAuthConfiguration)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateOAuthConfiguration");
        this.BindDataspaceId(Guid.Empty);
        this.BindGuid("@id", configurationId);
        this.BindString("@name", configurationParams.Name.Trim(), 512, false, SqlDbType.NVarChar);
        this.BindString("@url", (string) null, int.MaxValue, true, SqlDbType.NVarChar);
        this.BindString("@endpointType", (string) null, 128, true, SqlDbType.NVarChar);
        this.BindString("@clientId", (string) null, 128, true, SqlDbType.NVarChar);
        this.BindGuid("@modifiedBy", new Guid(this.GetUser(requestContext).Id));
        this.BindDateTime2("@modifiedOn", DateTime.UtcNow);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<OAuthConfiguration>((ObjectBinder<OAuthConfiguration>) new OAuthConfigurationBinder());
          return resultCollection.GetCurrent<OAuthConfiguration>().FirstOrDefault<OAuthConfiguration>();
        }
      }
    }

    private OAuthConfigurationBinder GetOAuthConfigurationBinder() => new OAuthConfigurationBinder();

    private IdentityRef GetUser(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return new IdentityRef()
      {
        Id = userIdentity.Id.ToString("D")
      };
    }
  }
}
