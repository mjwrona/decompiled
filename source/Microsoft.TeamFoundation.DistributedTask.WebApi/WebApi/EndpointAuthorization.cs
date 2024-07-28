// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointAuthorization
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
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
    }

    public EndpointAuthorization Clone() => new EndpointAuthorization(this);

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<string, string>(ref this.m_serializedParameters, ref this.m_parameters, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<string, string>(ref this.m_parameters, ref this.m_serializedParameters, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedParameters = (IDictionary<string, string>) null;
  }
}
