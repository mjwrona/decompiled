// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestResult2
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class TestResult2
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid ProjectId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestCaseRefId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestResultId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime CreationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime LastUpdated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte Outcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Revision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime? DateStarted { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime? DateCompleted { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid? LastUpdatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid? RunBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ComputerName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte? FailureType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int? ResolutionStateId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid? Owner { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int? ResetCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int? AfnStripId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte? EffectivePointState { get; set; }
  }
}
