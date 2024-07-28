// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.SourceRepository
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class SourceRepository
  {
    [DataMember(Name = "Properties", EmitDefaultValue = false)]
    private IDictionary<string, string> m_serializedProperties;
    private IDictionary<string, string> m_properties;

    public string Id { get; set; }

    public string SourceProviderName { get; set; }

    public string Name { get; set; }

    public string FullName { get; set; }

    public string DefaultBranch { get; set; }

    public Uri Url { get; set; }

    public IDictionary<string, string> Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_properties;
      }
      set => this.m_properties = (IDictionary<string, string>) new Dictionary<string, string>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<string, string>(ref this.m_serializedProperties, ref this.m_properties, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<string, string>(ref this.m_properties, ref this.m_serializedProperties, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedProperties = (IDictionary<string, string>) null;
  }
}
