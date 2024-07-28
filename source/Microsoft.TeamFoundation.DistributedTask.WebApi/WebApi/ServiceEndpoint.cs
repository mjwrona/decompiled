// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class ServiceEndpoint
  {
    [DataMember(EmitDefaultValue = false, Name = "Data")]
    private Dictionary<string, string> m_data;

    public ServiceEndpoint()
    {
      this.m_data = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.IsReady = true;
    }

    private ServiceEndpoint(ServiceEndpoint endpointToClone)
    {
      this.Id = endpointToClone.Id;
      this.Name = endpointToClone.Name;
      this.Type = endpointToClone.Type;
      this.Url = endpointToClone.Url;
      this.Description = endpointToClone.Description;
      this.GroupScopeId = endpointToClone.GroupScopeId;
      this.AdministratorsGroup = endpointToClone.AdministratorsGroup;
      this.ReadersGroup = endpointToClone.ReadersGroup;
      this.IsDisabled = endpointToClone.IsDisabled;
      if (endpointToClone.Authorization != null)
        this.Authorization = endpointToClone.Authorization.Clone();
      if (endpointToClone.m_data == null)
        return;
      this.m_data = new Dictionary<string, string>((IDictionary<string, string>) endpointToClone.m_data, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public static bool ValidateServiceEndpoint(ServiceEndpoint endpoint, ref string message)
    {
      if (endpoint == null)
      {
        message = "endpoint: null";
        return false;
      }
      if (endpoint.Id == Guid.Empty)
      {
        message = CommonResources.EmptyGuidNotAllowed((object) "endpoint.Id");
        return false;
      }
      if (string.IsNullOrEmpty(endpoint.Name))
      {
        message = string.Format("{0}:{1}", (object) CommonResources.EmptyStringNotAllowed(), (object) "endpoint.Name");
        return false;
      }
      if (endpoint.Url == (Uri) null)
      {
        message = "endpoint.Url: null";
        return false;
      }
      if (string.IsNullOrEmpty(endpoint.Type))
      {
        message = string.Format("{0}:{1}", (object) CommonResources.EmptyStringNotAllowed(), (object) "endpoint.Type");
        return false;
      }
      if (endpoint.Authorization != null)
        return true;
      message = "endpoint.Authorization: null";
      return false;
    }

    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Owner { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Uri Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef CreatedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public EndpointAuthorization Authorization { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid GroupScopeId { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef AdministratorsGroup { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ReadersGroup { get; internal set; }

    public IDictionary<string, string> Data
    {
      get => (IDictionary<string, string>) this.m_data;
      set
      {
        if (value == null)
          return;
        this.m_data = new Dictionary<string, string>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
    }

    [DataMember(EmitDefaultValue = true)]
    public bool IsShared { get; set; }

    [DataMember(EmitDefaultValue = true)]
    [JsonConverter(typeof (EndpointIsReadyConverter<bool>))]
    public bool IsReady { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDisabled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public JObject OperationStatus { get; set; }

    public ServiceEndpoint Clone() => new ServiceEndpoint(this);
  }
}
