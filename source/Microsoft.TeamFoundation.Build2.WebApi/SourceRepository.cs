// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.SourceRepository
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class SourceRepository
  {
    [DataMember(Name = "Properties", EmitDefaultValue = false)]
    private IDictionary<string, string> m_serializedProperties;
    private IDictionary<string, string> m_properties;

    [DataMember(EmitDefaultValue = false)]
    public string Id { get; set; }

    [DataMember]
    public string SourceProviderName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string FullName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Uri Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DefaultBranch { get; set; }

    public IDictionary<string, string> Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_properties;
      }
      internal set => this.m_properties = (IDictionary<string, string>) new Dictionary<string, string>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<string, string>(ref this.m_serializedProperties, ref this.m_properties, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<string, string>(ref this.m_properties, ref this.m_serializedProperties, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedProperties = (IDictionary<string, string>) null;
  }
}
