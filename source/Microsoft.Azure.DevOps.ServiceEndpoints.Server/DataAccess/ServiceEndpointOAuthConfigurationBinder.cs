// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointOAuthConfigurationBinder
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Data;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal sealed class ServiceEndpointOAuthConfigurationBinder : 
    ObjectBinder<ServiceEndpointOAuthConfigurationReference>
  {
    private ServiceEndpointComponent m_component;
    private SqlColumnBinder m_endpointId = new SqlColumnBinder("ServiceEndpointId");
    private SqlColumnBinder m_configurationId = new SqlColumnBinder("ConfigurationId");
    private SqlColumnBinder m_endpointDataspaceId = new SqlColumnBinder("ServiceEndpointDataspaceId");

    public ServiceEndpointOAuthConfigurationBinder(ServiceEndpointComponent component) => this.m_component = component;

    protected override ServiceEndpointOAuthConfigurationReference Bind()
    {
      int int32 = this.m_endpointDataspaceId.GetInt32((IDataReader) this.Reader);
      return new ServiceEndpointOAuthConfigurationReference()
      {
        ServiceEndpointId = this.m_endpointId.GetGuid((IDataReader) this.Reader),
        ConfigurationId = this.m_configurationId.GetGuid((IDataReader) this.Reader),
        ServiceEndpointProjectId = int32 != 0 ? this.m_component.GetDataspaceIdentifier(int32) : Guid.Empty
      };
    }
  }
}
