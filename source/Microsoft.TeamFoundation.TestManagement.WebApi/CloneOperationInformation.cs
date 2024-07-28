// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class CloneOperationInformation
  {
    [DataMember(Name = "opId")]
    public int OpId { get; set; }

    [DataMember(Name = "creationDate")]
    public DateTime CreationDate { get; set; }

    [DataMember(Name = "completionDate")]
    public DateTime CompletionDate { get; set; }

    [DataMember(Name = "state")]
    public CloneOperationState State { get; set; }

    [DataMember(Name = "message")]
    public string Message { get; set; }

    [DataMember(Name = "cloneStatistics")]
    public CloneStatistics CloneStatistics { get; set; }

    [DataMember(Name = "resultObjectType")]
    public ResultObjectType ResultObjectType { get; set; }

    [DataMember(Name = "destinationObject")]
    public ShallowReference DestinationObject { get; set; }

    [DataMember(Name = "sourceObject")]
    public ShallowReference SourceObject { get; set; }

    [DataMember(Name = "destinationPlan")]
    public ShallowReference DestinationPlan { get; set; }

    [DataMember(Name = "sourcePlan")]
    public ShallowReference SourcePlan { get; set; }

    [DataMember(Name = "destinationProject")]
    public ShallowReference DestinationProject { get; set; }

    [DataMember(Name = "sourceProject")]
    public ShallowReference SourceProject { get; set; }

    [DataMember(Name = "url")]
    public string Url { get; set; }
  }
}
