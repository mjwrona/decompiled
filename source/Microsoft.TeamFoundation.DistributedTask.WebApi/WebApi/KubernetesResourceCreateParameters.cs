// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.KubernetesResourceCreateParameters
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [KnownType(typeof (KubernetesResourceCreateParametersExistingEndpoint))]
  [KnownType(typeof (KubernetesResourceCreateParametersNewEndpoint))]
  [JsonConverter(typeof (KubernetesResourceCreateParametersJsonConverter))]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public abstract class KubernetesResourceCreateParameters
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Tags")]
    private IList<string> m_tags;

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Namespace { get; set; }

    [DataMember]
    public string ClusterName { get; set; }

    public IList<string> Tags
    {
      get
      {
        if (this.m_tags == null)
          this.m_tags = (IList<string>) new List<string>();
        return this.m_tags;
      }
      set => this.m_tags = value;
    }
  }
}
