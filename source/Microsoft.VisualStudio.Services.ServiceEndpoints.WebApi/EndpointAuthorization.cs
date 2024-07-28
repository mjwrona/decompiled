// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointAuthorization
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public sealed class EndpointAuthorization
  {
    private IDictionary<string, string> m_parameters;
    [DataMember(Name = "Parameters", EmitDefaultValue = false)]
    private IDictionary<string, string> m_serializedParameters;

    public EndpointAuthorization()
    {
    }

    private EndpointAuthorization(EndpointAuthorization authorizationToClone)
    {
      this.Scheme = authorizationToClone.Scheme;
      if (authorizationToClone.m_parameters == null || authorizationToClone.m_parameters.Count <= 0)
        return;
      this.m_parameters = (IDictionary<string, string>) new Dictionary<string, string>(authorizationToClone.m_parameters, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember]
    public string Scheme { get; set; }

    public IDictionary<string, string> Parameters
    {
      get
      {
        if (this.m_parameters == null)
          this.m_parameters = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_parameters;
      }
      set
      {
        if (value == null)
          return;
        this.m_parameters = (IDictionary<string, string>) new Dictionary<string, string>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
    }

    public EndpointAuthorization Clone() => new EndpointAuthorization(this);

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<string, string>(ref this.m_serializedParameters, ref this.m_parameters, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<string, string>(ref this.m_parameters, ref this.m_serializedParameters, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedParameters = (IDictionary<string, string>) null;

    public override bool Equals(object obj)
    {
      EndpointAuthorization authorization = obj as EndpointAuthorization;
      return authorization != null && string.Equals(authorization.Scheme, this.Scheme, StringComparison.InvariantCulture) && this.Parameters.Count == authorization.Parameters.Count && !this.Parameters.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (s1 => !authorization.Parameters.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (s2 => s2.Equals((object) s1)))));
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
