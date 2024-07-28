// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.Job
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.WebApi
{
  [DataContract]
  public class Job : BaseSecuredObject
  {
    [DataMember(Name = "_links", EmitDefaultValue = false)]
    private ReferenceLinks m_links;

    [JsonConstructor]
    public Job(Guid JobId) => this.Id = JobId;

    internal Job(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public Guid Id { get; private set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public int Attempt { get; set; }

    [DataMember]
    public JobState State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public JobResult? Result { get; set; }

    [DataMember]
    public DateTime? StartTime { get; set; }

    [DataMember]
    public DateTime? FinishTime { get; set; }

    public ReferenceLinks Links
    {
      get
      {
        if (this.m_links == null)
          this.m_links = new ReferenceLinks();
        return this.m_links;
      }
    }
  }
}
