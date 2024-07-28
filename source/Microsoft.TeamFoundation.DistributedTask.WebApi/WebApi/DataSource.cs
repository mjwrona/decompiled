// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DataSource
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class DataSource
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string EndpointUrl { get; set; }

    [DataMember]
    public string ResourceUrl { get; set; }

    [DataMember]
    public string ResultSelector { get; set; }

    [DataMember]
    public List<AuthorizationHeader> Headers { get; set; }

    [DataMember]
    public AuthenticationSchemeReference AuthenticationScheme { get; set; }
  }
}
