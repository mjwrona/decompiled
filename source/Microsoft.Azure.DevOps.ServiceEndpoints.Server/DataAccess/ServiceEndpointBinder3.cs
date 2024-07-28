// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointBinder3
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Data;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class ServiceEndpointBinder3 : ServiceEndpointBinder2
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder TypeColumn = new SqlColumnBinder("Type");
    private SqlColumnBinder UrlColumn = new SqlColumnBinder("Url");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder CreatedByColumn = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder AuthorizationSchemeColumn = new SqlColumnBinder("AuthorizationScheme");
    private SqlColumnBinder AuthorizationParametersColumn = new SqlColumnBinder("AuthorizationParameters");
    private SqlColumnBinder DataColumn = new SqlColumnBinder("Data");
    private SqlColumnBinder GroupScopeId = new SqlColumnBinder(nameof (GroupScopeId));
    private SqlColumnBinder AdministratorsGroupId = new SqlColumnBinder(nameof (AdministratorsGroupId));
    private SqlColumnBinder ReadersGroupId = new SqlColumnBinder(nameof (ReadersGroupId));
    private SqlColumnBinder IsReadyColumn = new SqlColumnBinder("IsReady");
    private SqlColumnBinder IsDisabledColumn = new SqlColumnBinder("IsDisabled");
    private SqlColumnBinder OperationStatusColumn = new SqlColumnBinder("OperationStatus");
    private SqlColumnBinder ConfigurationIdColumn = new SqlColumnBinder("ConfigurationId");
    private SqlColumnBinder OwnerColumn = new SqlColumnBinder("Owner");

    protected override ServiceEndpoint Bind()
    {
      ServiceEndpoint serviceEndpoint = new ServiceEndpoint()
      {
        Id = this.IdColumn.GetGuid((IDataReader) this.Reader, false),
        Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
        Type = this.TypeColumn.GetString((IDataReader) this.Reader, false),
        Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true) ?? string.Empty,
        CreatedBy = new IdentityRef()
        {
          Id = this.CreatedByColumn.GetGuid((IDataReader) this.Reader, false).ToString("D")
        },
        Authorization = new EndpointAuthorization()
        {
          Scheme = this.AuthorizationSchemeColumn.GetString((IDataReader) this.Reader, false)
        },
        IsReady = this.IsReadyColumn.GetBoolean((IDataReader) this.Reader, true),
        IsDisabled = this.IsDisabledColumn.GetBoolean((IDataReader) this.Reader, false, false),
        Owner = ((EndpointOwner) Enum.ToObject(typeof (EndpointOwner), this.OwnerColumn.GetInt32((IDataReader) this.Reader, 1, 1))).ToString()
      };
      string uriString = this.UrlColumn.GetString((IDataReader) this.Reader, true);
      if (uriString != null)
        serviceEndpoint.Url = new Uri(uriString);
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
      string toDeserialize1 = this.DataColumn.GetString((IDataReader) this.Reader, true);
      if (!string.IsNullOrEmpty(toDeserialize1))
        JsonUtility.Populate(toDeserialize1, (object) serviceEndpoint.Data);
      string toDeserialize2 = this.AuthorizationParametersColumn.GetString((IDataReader) this.Reader, (string) null);
      if (!string.IsNullOrEmpty(toDeserialize2))
        JsonUtility.Populate(toDeserialize2, (object) serviceEndpoint.Authorization.Parameters);
      string json = this.OperationStatusColumn.GetString((IDataReader) this.Reader, true);
      if (json != null)
      {
        JObject jobject = JObject.Parse(json);
        serviceEndpoint.OperationStatus = jobject;
      }
      Guid guid = this.ConfigurationIdColumn.ColumnExists((IDataReader) this.Reader) ? this.ConfigurationIdColumn.GetGuid((IDataReader) this.Reader, true) : Guid.Empty;
      if ((serviceEndpoint.Authorization.IsOauth2() || serviceEndpoint.Authorization.IsJiraConnectAppScheme()) && guid != Guid.Empty)
        serviceEndpoint.Authorization.Parameters["ConfigurationId"] = guid.ToString();
      if (serviceEndpoint.Type == "kubernetes")
        this.PatchKubernetesEndpoint(serviceEndpoint);
      return serviceEndpoint;
    }

    private void PatchKubernetesEndpoint(ServiceEndpoint serviceEndpoint)
    {
      if (serviceEndpoint.Authorization.Scheme != "Token")
      {
        serviceEndpoint.Authorization.Scheme = "Kubernetes";
      }
      else
      {
        string a;
        if (serviceEndpoint.Data.TryGetValue("authorizationType", out a))
        {
          if (!string.Equals(a, "KubeConfig", StringComparison.OrdinalIgnoreCase))
            return;
          serviceEndpoint.Authorization.Scheme = "Kubernetes";
        }
        else
          serviceEndpoint.Authorization.Scheme = "Kubernetes";
      }
    }
  }
}
