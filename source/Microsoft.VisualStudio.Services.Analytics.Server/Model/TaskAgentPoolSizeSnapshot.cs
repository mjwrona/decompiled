// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.TaskAgentPoolSizeSnapshot
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
  [DisplayNameAnnotation("ENTITY_SET_NAME_TASKAGENTPOOLSIZESNAPSHOT")]
  [ODataHide(0, 3)]
  [Table("AnalyticsModel.vw_TaskAgentPoolSize")]
  [ModelTableMapping("Model.TaskAgentPoolSize")]
  [DatabaseHide(0, 54)]
  public class TaskAgentPoolSizeSnapshot : IPartitionScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [Key]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_SAMPLING_DATE", false)]
    [SuppressDateConsistencyCheck("Date navigation property exists.")]
    public virtual DateTimeOffset SamplingDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_POOL_ID", false)]
    public int PoolId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ONLINE_COUNT", false)]
    public int OnlineCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_OFFLINE_COUNT", false)]
    public int OfflineCount { get; set; }
  }
}
