// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestRun2
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class TestRun2
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid ProjectId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Title { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime CreationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime LastUpdated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid Owner { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int IncompleteTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestPlanId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int? IterationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string DropLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string BuildNumber { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ErrorMessage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime? StartDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime? CompleteDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte PostProcessState { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime? DueDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Controller { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestMessageLogId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string LegacySharePath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestSettingsId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int BuildConfigurationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Revision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid LastUpdatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte Type { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int? CoverageId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsAutomated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid TestEnvironmentId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Version { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int PublicTestSettingsId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsBvt { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Comment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TotalTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int PassedTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int NotApplicableTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int UnanalyzedTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsMigrated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ReleaseUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ReleaseEnvironmentUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestRunContextId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int? MaxReservedResultId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime? DeletedOn { get; set; }
  }
}
