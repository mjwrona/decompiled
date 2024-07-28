// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.User
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DataContract]
  [DisplayNameAnnotation("ENTITY_SET_NAME_USERS")]
  [ModelTableMapping("Model.User")]
  public class User : IPartitionScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public Guid UserSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_USER_ID", false)]
    public Guid? UserId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_USER_NAME", false)]
    [EuiiAnnotation]
    public string UserName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_USER_EMAIL", false)]
    [EuiiAnnotation]
    public string UserEmail { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    [DatabaseHide(0, 41)]
    [ODataHide(0, 2)]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }

    [LocalizedDisplayName("ENTITY_FIELD_NAME_USER_GITHUB_ID", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 46)]
    [ODataHide(0, 2)]
    public string GitHubUserId { get; set; }

    [LocalizedDisplayName("ENTITY_FIELD_NAME_USER_USERTYPE", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 46)]
    [ODataHide(0, 2)]
    public Microsoft.VisualStudio.Services.Analytics.Model.UserType? UserType { get; set; }

    [IgnoreDataMember]
    public int SkipToken { get; set; }
  }
}
