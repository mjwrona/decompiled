// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.OAuthConfigurationBinder
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
  internal class OAuthConfigurationBinder : ObjectBinder<OAuthConfiguration>
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder UrlColumn = new SqlColumnBinder("Url");
    private SqlColumnBinder EndpointTypeColumn = new SqlColumnBinder("EndpointType");
    private SqlColumnBinder ClientIdColumn = new SqlColumnBinder("ClientId");
    private SqlColumnBinder CreatedByColumn = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder CreatedOnColumn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder ModifiedByColumn = new SqlColumnBinder("ModifiedBy");
    private SqlColumnBinder ModifiedOnColumn = new SqlColumnBinder("ModifiedOn");

    protected override OAuthConfiguration Bind() => new OAuthConfiguration()
    {
      Id = this.IdColumn.GetGuid((IDataReader) this.Reader, false),
      Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
      Url = new Uri(this.UrlColumn.GetString((IDataReader) this.Reader, false)),
      EndpointType = this.EndpointTypeColumn.GetString((IDataReader) this.Reader, false),
      ClientId = this.ClientIdColumn.GetString((IDataReader) this.Reader, false),
      CreatedBy = new IdentityRef()
      {
        Id = this.CreatedByColumn.GetGuid((IDataReader) this.Reader, false).ToString("D")
      },
      CreatedOn = this.CreatedOnColumn.GetDateTime((IDataReader) this.Reader),
      ModifiedBy = new IdentityRef()
      {
        Id = this.ModifiedByColumn.GetGuid((IDataReader) this.Reader, false).ToString("D")
      },
      ModifiedOn = this.ModifiedOnColumn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
