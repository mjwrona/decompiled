// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.JiraLifecycleEventData
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  [DataContract]
  public class JiraLifecycleEventData
  {
    [DataMember]
    public string Key { get; set; }

    [DataMember]
    public string ClientKey { get; set; }

    [DataMember]
    public string SharedSecret { get; set; }

    [DataMember]
    public string BaseUrl { get; set; }

    [DataMember]
    public string EventType { get; set; }

    [DataMember]
    public Guid HostId { get; set; }

    [DataMember]
    public Guid ProjectId { get; set; }
  }
}
