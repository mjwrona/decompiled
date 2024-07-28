// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointBinder2
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Data;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class ServiceEndpointBinder2 : ServiceEndpointBinder
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder TypeColumn = new SqlColumnBinder("Type");
    private SqlColumnBinder UrlColumn = new SqlColumnBinder("Url");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder CreatedByColumn = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder AuthorizationSchemeColumn = new SqlColumnBinder("AuthorizationScheme");
    private SqlColumnBinder DataColumn = new SqlColumnBinder("Data");
    private SqlColumnBinder GroupScopeId = new SqlColumnBinder(nameof (GroupScopeId));
    private SqlColumnBinder AdministratorsGroupId = new SqlColumnBinder(nameof (AdministratorsGroupId));
    private SqlColumnBinder ReadersGroupId = new SqlColumnBinder(nameof (ReadersGroupId));

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
      Guid? nullableGuid1 = this.GroupScopeId.GetNullableGuid((IDataReader) this.Reader);
      if (nullableGuid1.HasValue)
        serviceEndpoint.GroupScopeId = nullableGuid1.Value;
      Guid? nullableGuid2 = this.AdministratorsGroupId.GetNullableGuid((IDataReader) this.Reader);
      if (nullableGuid2.HasValue)
        serviceEndpoint.AdministratorsGroup = new IdentityRef()
        {
          Id = nullableGuid2.Value.ToString("D")
        };
      Guid? nullableGuid3 = this.ReadersGroupId.GetNullableGuid((IDataReader) this.Reader);
      if (nullableGuid3.HasValue)
        serviceEndpoint.ReadersGroup = new IdentityRef()
        {
          Id = nullableGuid3.Value.ToString("D")
        };
      JsonUtility.Populate(this.DataColumn.GetString((IDataReader) this.Reader, true), (object) serviceEndpoint.Data);
      serviceEndpoint.IsReady = true;
      serviceEndpoint.OperationStatus = (JObject) null;
      return serviceEndpoint;
    }
  }
}
