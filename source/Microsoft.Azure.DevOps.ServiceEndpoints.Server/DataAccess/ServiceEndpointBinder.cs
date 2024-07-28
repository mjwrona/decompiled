// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointBinder
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  public class ServiceEndpointBinder : ObjectBinder<ServiceEndpoint>
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder TypeColumn = new SqlColumnBinder("Type");
    private SqlColumnBinder UrlColumn = new SqlColumnBinder("Url");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder CreatedByColumn = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder AuthorizationSchemeColumn = new SqlColumnBinder("AuthorizationScheme");
    private SqlColumnBinder DataColumn = new SqlColumnBinder("Data");

    protected override ServiceEndpoint Bind()
    {
      ServiceEndpoint serviceEndpoint = new ServiceEndpoint()
      {
        Id = this.IdColumn.GetGuid((IDataReader) this.Reader, false),
        Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
        Type = this.TypeColumn.GetString((IDataReader) this.Reader, false),
        Url = new Uri(this.UrlColumn.GetString((IDataReader) this.Reader, false)),
        Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true),
        CreatedBy = new IdentityRef()
        {
          Id = this.CreatedByColumn.GetGuid((IDataReader) this.Reader, false).ToString("D")
        },
        Authorization = new EndpointAuthorization()
        {
          Scheme = this.AuthorizationSchemeColumn.GetString((IDataReader) this.Reader, false)
        }
      };
      JsonUtility.Populate(this.DataColumn.GetString((IDataReader) this.Reader, true), (object) serviceEndpoint.Data);
      return serviceEndpoint;
    }
  }
}
