// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointPartialBinder
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Data;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  public class ServiceEndpointPartialBinder : ObjectBinder<ServiceEndpointPartial>
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder CreatedByColumn = new SqlColumnBinder("CreatedBy");

    protected override ServiceEndpointPartial Bind() => new ServiceEndpointPartial()
    {
      Id = this.IdColumn.GetGuid((IDataReader) this.Reader, false),
      Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
      CreatedBy = new IdentityRef()
      {
        Id = this.CreatedByColumn.GetGuid((IDataReader) this.Reader, false).ToString("D")
      }
    };
  }
}
