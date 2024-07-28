// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.ParallelPipelineJobsSnapshot
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DataContract]
  [DisplayNameAnnotation("ENTITY_SET_NAME_PARALLELPIPELINEJOBSSNAPSHOT")]
  [ODataHide(0, 3)]
  [Table("AnalyticsModel.vw_ParallelPipelineJobsSnapshot")]
  [ModelTableMapping("Model.ParallelPipelineJobs")]
  [DatabaseHide(0, 54)]
  public class ParallelPipelineJobsSnapshot : IPartitionScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_SAMPLING_DATE", false)]
    [SuppressDateConsistencyCheck("SK column does not exists in the database at this point.")]
    [Key]
    public DateTimeOffset SamplingDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PARALLELISM_TAG", false)]
    [Key]
    public string ParallelismTag { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_HOSTED", false)]
    [Key]
    public bool IsHosted { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TOTAL_COUNT", false)]
    public int? TotalCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TOTAL_MINUTES", false)]
    public int? TotalMinutes { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_PREMIUM", false)]
    public bool IsPremium { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_FAILED_TO_REACH_ALL_PROVIDERS", false)]
    public bool? FailedToReachAllProviders { get; set; }
  }
}
