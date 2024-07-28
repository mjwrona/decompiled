// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
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
      this.CreatedBy = endpointToClone.CreatedBy;
      this.Type = endpointToClone.Type;
      this.Url = endpointToClone.Url;
      this.Description = endpointToClone.Description;
      this.GroupScopeId = endpointToClone.GroupScopeId;
      this.AdministratorsGroup = endpointToClone.AdministratorsGroup;
      this.ReadersGroup = endpointToClone.ReadersGroup;
      this.OperationStatus = endpointToClone.OperationStatus;
      this.IsReady = endpointToClone.IsReady;
      this.IsDisabled = endpointToClone.IsDisabled;
      this.Owner = endpointToClone.Owner;
      this.ServiceEndpointProjectReferences = (IList<ServiceEndpointProjectReference>) new List<ServiceEndpointProjectReference>((IEnumerable<ServiceEndpointProjectReference>) (endpointToClone.ServiceEndpointProjectReferences ?? (IList<ServiceEndpointProjectReference>) new List<ServiceEndpointProjectReference>()));
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
    public bool IsOutdated { get; set; }

    [DataMember(EmitDefaultValue = true)]
    [JsonConverter(typeof (EndpointIsReadyConverter<bool>))]
    public bool IsReady { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonConverter(typeof (EndpointIsDisabledConverter<bool>))]
    public bool IsDisabled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public JObject OperationStatus { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Owner { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public IList<ServiceEndpointProjectReference> ServiceEndpointProjectReferences { get; set; }

    public ServiceEndpoint Clone() => new ServiceEndpoint(this);

    public override bool Equals(object obj)
    {
      ServiceEndpoint endpoint = obj as ServiceEndpoint;
      return endpoint != null && endpoint.IsReady == this.IsReady && endpoint.IsDisabled == this.IsDisabled && string.Equals(endpoint.Description, this.Description, StringComparison.InvariantCulture) && string.Equals(endpoint.Name, this.Name, StringComparison.InvariantCulture) && string.Equals(endpoint.Type, this.Type, StringComparison.InvariantCultureIgnoreCase) && (!(this.Url == (Uri) null) || !(endpoint.Url != (Uri) null)) && (!(this.Url != (Uri) null) || this.Url.Equals((object) endpoint.Url)) && (this.Owner != null || endpoint.Owner == null) && (this.Owner == null || this.Owner.Equals(endpoint.Owner, StringComparison.InvariantCultureIgnoreCase)) && string.Equals(endpoint.OperationStatus?.ToString(), this.OperationStatus?.ToString(), StringComparison.InvariantCultureIgnoreCase) && (this.Authorization != null || endpoint.Authorization == null) && (this.Authorization == null || this.Authorization.Equals((object) endpoint.Authorization)) && this.Data.Count == endpoint.Data.Count && !this.Data.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (s1 => !endpoint.Data.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (s2 => string.Equals(s1.Key, s2.Key, StringComparison.InvariantCulture)))));
    }

    public override int GetHashCode() => this.Id.GetHashCode();
  }
}
