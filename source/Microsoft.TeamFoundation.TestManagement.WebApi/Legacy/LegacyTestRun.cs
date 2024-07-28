// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi.Legacy
{
  [DataContract]
  public sealed class LegacyTestRun
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string TeamProjectUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string TeamProject { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Title { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid Owner { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string OwnerName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string BuildUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string BuildNumber { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int BuildConfigurationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string BuildPlatform { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string BuildFlavor { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime CreationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime LastUpdated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime StartDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime CompleteDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Controller { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestPlanId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    [DefaultValue(0)]
    public int TestSettingsId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    [DefaultValue(0)]
    public int PublicTestSettingsId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid TestEnvironmentId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Revision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid LastUpdatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string LastUpdatedByName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Comment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int BugsCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte[] RowVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ServiceVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public LegacyTestRunStatistic[] TestRunStatistics { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string DropLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte PostProcessState { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime DueDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Iteration { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int IterationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestMessageLogId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string LegacySharePath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ErrorMessage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte Type { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsAutomated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Version { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsBvt { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TotalTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int IncompleteTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int NotApplicableTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int PassedTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int UnanalyzedTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ReleaseUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ReleaseEnvironmentUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public LegacyBuildConfiguration BuildReference { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public LegacyReleaseReference ReleaseReference { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public RunFilter Filter { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int[] ConfigurationIds { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public ShallowReference DtlTestEnvironment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public ShallowReference DtlAutEnvironment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public List<TestMessageLogDetails> TestMessageLogEntries { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public List<TestExtensionField> CustomFields { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string SourceWorkflow { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte Substate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TimeSpan RunTimeout { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string CsmParameters { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string CsmContent { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string SubscriptionName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string TestConfigurationsMapping { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool RunHasDtlEnvironment => ((uint) this.Type & 16U) > 0U;
  }
}
