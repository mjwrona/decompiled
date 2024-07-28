// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.TestSuite
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
  [DataContract]
  [DisplayNameAnnotation("ENTITY_SET_NAME_TEST_SUITES")]
  [ODataHide(0, 2)]
  [DatabaseHide(0, 44)]
  [Table("AnalyticsModel.vw_TestSuite")]
  [ModelTableMapping("Model.TestSuite")]
  public class TestSuite : IPartitionScoped, IProjectNavigate, IProjectScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public int TestSuiteSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_PLAN_ID", false)]
    public int TestPlanId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_ID", false)]
    public int TestSuiteId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_PLAN_TITLE", false)]
    public string TestPlanTitle { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_TITLE", false)]
    public string Title { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_ORDER_ID", false)]
    public int? OrderId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_1_ID", false)]
    public int? IdLevel1 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_1_TITLE", false)]
    public string TitleLevel1 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_2_ID", false)]
    public int? IdLevel2 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_2_TITLE", false)]
    public string TitleLevel2 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_3_ID", false)]
    public int? IdLevel3 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_3_TITLE", false)]
    public string TitleLevel3 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_4_ID", false)]
    public int? IdLevel4 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_4_TITLE", false)]
    public string TitleLevel4 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_5_ID", false)]
    public int? IdLevel5 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_5_TITLE", false)]
    public string TitleLevel5 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_6_ID", false)]
    public int? IdLevel6 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_6_TITLE", false)]
    public string TitleLevel6 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_7_ID", false)]
    public int? IdLevel7 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_7_TITLE", false)]
    public string TitleLevel7 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_8_ID", false)]
    public int? IdLevel8 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_8_TITLE", false)]
    public string TitleLevel8 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_9_ID", false)]
    public int? IdLevel9 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_9_TITLE", false)]
    public string TitleLevel9 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_10_ID", false)]
    public int? IdLevel10 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_10_TITLE", false)]
    public string TitleLevel10 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_11_ID", false)]
    public int? IdLevel11 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_11_TITLE", false)]
    public string TitleLevel11 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_12_ID", false)]
    public int? IdLevel12 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_12_TITLE", false)]
    public string TitleLevel12 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_13_ID", false)]
    public int? IdLevel13 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_13_TITLE", false)]
    public string TitleLevel13 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_14_ID", false)]
    public int? IdLevel14 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_14_TITLE", false)]
    public string TitleLevel14 { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_DEPTH", false)]
    public byte? Depth { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_TYPE", false)]
    public TestSuiteType Type { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_REQUIREMENT_WORK_ITEM_ID", false)]
    public int? RequirementWorkItemId { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ProjectSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PROJECT", false)]
    public Project Project { get; set; }

    [ForeignKey("PartitionId,TestSuiteId")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_SUITE_WORK_ITEM", false)]
    [NavigationRequiresPermissionCheck]
    public WorkItem TestSuiteWorkItem { get; set; }

    [ForeignKey("PartitionId,TestPlanId")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_PLAN_WORK_ITEM", false)]
    [NavigationRequiresPermissionCheck]
    public WorkItem TestPlanWorkItem { get; set; }

    [ForeignKey("PartitionId,RequirementWorkItemId")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REQUIREMENT_WORK_ITEM", false)]
    [NavigationRequiresPermissionCheck]
    public WorkItem RequirementWorkItem { get; set; }
  }
}
