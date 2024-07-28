// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.Container
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.WebApi
{
  [DataContract]
  public class Container
  {
    [JsonConstructor]
    public Container()
    {
    }

    [DataMember]
    public IDictionary<string, string> Environment { get; set; }

    [DataMember]
    public bool MapDockerSocket { get; set; }

    [DataMember]
    public string Image { get; set; }

    [DataMember]
    public string Options { get; set; }

    [DataMember]
    public IList<string> Volumes { get; set; }

    [DataMember]
    public IList<string> Ports { get; set; }
  }
}
