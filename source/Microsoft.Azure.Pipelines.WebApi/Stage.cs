// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.Stage
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
  public class Stage : BaseSecuredObject
  {
    [DataMember(Name = "_links", EmitDefaultValue = false)]
    private ReferenceLinks m_links;

    [JsonConstructor]
    private Stage()
    {
    }

    internal Stage(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public int Attempt { get; set; }

    [DataMember]
    public StageState State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public StageResult? Result { get; set; }

    public ReferenceLinks Links
    {
      get
      {
        if (this.m_links == null)
          this.m_links = new ReferenceLinks();
        return this.m_links;
      }
    }

    [DataMember]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }
  }
}
