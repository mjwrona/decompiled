// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.WebApi.PolicyEvaluationRecord
// Assembly: Microsoft.TeamFoundation.Policy.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E2CB80F-05BD-43A4-BD5A-A4654EDC6268
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Policy.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Policy.WebApi
{
  [DataContract]
  public class PolicyEvaluationRecord : BaseSecuredObject
  {
    public PolicyEvaluationRecord()
    {
    }

    public PolicyEvaluationRecord(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public PolicyConfiguration Configuration { get; set; }

    [DataMember]
    public string ArtifactId { get; set; }

    [DataMember]
    public Guid EvaluationId { get; set; }

    [DataMember]
    public DateTime StartedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? CompletedDate { get; set; }

    [DataMember]
    public PolicyEvaluationStatus? Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public JObject Context { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }
  }
}
