// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class TestPoint
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int Id;
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Url;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef AssignedTo;
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool Automated;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Comment;
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public ShallowReference Configuration;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string FailureType;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int LastResolutionStateId;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference LastTestRun;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference LastResult;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime LastUpdatedDate;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime LastResetToActive;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef LastUpdatedBy;
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Outcome;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Revision;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string State;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string LastResultState;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Suite;
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public WorkItemReference TestCase;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference TestPlan;
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public object[] WorkItemProperties;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public LastResultDetails LastResultDetails;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string LastRunBuildNumber;
  }
}
