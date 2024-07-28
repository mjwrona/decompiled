// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestCaseResult
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi.Legacy
{
  [DataContract]
  public sealed class LegacyTestCaseResult
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestCaseId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int ConfigurationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ConfigurationName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestPointId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte FailureType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int ResolutionStateId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ComputerName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid Owner { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string OwnerName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid RunBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string RunByName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte Priority { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string TestCaseTitle { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string TestCaseArea { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string TestCaseAreaUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string AreaUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestCaseRevision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int AfnStripId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int ResetCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string AutomatedTestName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string AutomatedTestStorage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string AutomatedTestType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string AutomatedTestTypeId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string AutomatedTestId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Revision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string BuildNumber { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestPlanId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestSuiteId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string SuiteName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int SequenceId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestCaseReferenceId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TestExtensionField StackTrace { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public List<TestExtensionField> CustomFields { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public LegacyBuildConfiguration BuildReference { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public LegacyReleaseReference ReleaseReference { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public FailingSince FailingSince { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsRerun { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int SubResultCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public ResultGroupType ResultGroupType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string TestRunTitle { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int AreaId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public LegacyTestCaseResultIdentifier Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestResultId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime CreationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte Outcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ErrorMessage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Comment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid LastUpdatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string LastUpdatedByName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime LastUpdated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime DateStarted { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime DateCompleted { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public long Duration { get; set; }
  }
}
