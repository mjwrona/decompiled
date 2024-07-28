// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.TestPointCommon
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  public abstract class TestPointCommon : IPartitionScoped, IProjectNavigate, IProjectScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public int TestPointSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestSuiteSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_PLAN_ID", false)]
    public int TestPlanId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_ID", false)]
    public int TestSuiteId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_POINT_ID", false)]
    public int TestPointId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestConfigurationSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_CONFIGURATION_ID", false)]
    public int TestConfigurationId { get; set; }

    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_CASE_ID", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? TestCaseId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? TesterUserSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? AssignedToUserSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PRIORITY", false)]
    public int? Priority { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AUTOMATION_STATUS", false)]
    public string AutomationStatus { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ProjectSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PROJECT", false)]
    public Project Project { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,TestSuiteSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_POINT_TEST_SUITE", false)]
    [NavigationRequiresPermissionCheck]
    public TestSuite TestSuite { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,TestConfigurationSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_POINT_TEST_CONFIGURATION", false)]
    [NavigationRequiresPermissionCheck]
    public TestConfiguration TestConfiguration { get; set; }

    [ForeignKey("PartitionId,TestCaseId")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_POINT_TEST_CASE", false)]
    [NavigationRequiresPermissionCheck]
    public WorkItem TestCase { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ForeignKey("PartitionId,TesterUserSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_POINT_TESTER", false)]
    public User Tester { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ForeignKey("PartitionId,AssignedToUserSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ASSIGNED_TO", false)]
    public User AssignedTo { get; set; }
  }
}
