// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointProjectReferenceBinder
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class ServiceEndpointProjectReferenceBinder : 
    ObjectBinder<ServiceEndpointProjectReferenceResult>
  {
    private ServiceEndpointComponent m_component;
    private SqlColumnBinder EndpointIdColumn = new SqlColumnBinder("EndpointId");
    private SqlColumnBinder DataspaceIdColumn = new SqlColumnBinder("DataspaceId");

    public ServiceEndpointProjectReferenceBinder(ServiceEndpointComponent component) => this.m_component = component;

    protected override ServiceEndpointProjectReferenceResult Bind() => new ServiceEndpointProjectReferenceResult()
    {
      ServiceEndpointId = this.EndpointIdColumn.GetGuid((IDataReader) this.Reader),
      ProjectId = this.m_component.GetDataspaceIdentifier(this.DataspaceIdColumn.GetInt32((IDataReader) this.Reader, 0, 0))
    };
  }
}
