// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy.JobAuthorization
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy
{
  [DataContract]
  [JsonConverter(typeof (LegacyJsonConverter<JobAuthorization>))]
  public class JobAuthorization
  {
    [DataMember]
    public string ServicePrincipalId { get; set; }

    [DataMember]
    public string ServicePrincipalToken { get; set; }

    [DataMember]
    public Guid ServerId { get; set; }

    [DataMember]
    public Uri ServerUrl { get; set; }
  }
}
