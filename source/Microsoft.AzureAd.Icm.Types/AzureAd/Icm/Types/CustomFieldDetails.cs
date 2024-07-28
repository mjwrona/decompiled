// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.CustomFieldDetails
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  public class CustomFieldDetails
  {
    [Key]
    public long IncidentCustomFieldId { get; set; }

    public long CustomFieldId { get; set; }

    [IgnoreDataMember]
    public long IncidentId { get; set; }

    public long? TenantId { get; set; }

    public long? ServiceCategoryId { get; set; }

    [MaxLength(128)]
    public string Name { get; set; }

    [MaxLength(256)]
    public string Description { get; set; }

    public string StringValue { get; set; }

    public long? NumberValue { get; set; }

    public bool? BooleanValue { get; set; }

    public string EnumValue { get; set; }

    public DateTimeOffset? DateTimeOffsetValue { get; set; }

    [IgnoreDataMember]
    public DateTimeOffset CreatedTime { get; set; }

    [MaxLength(128)]
    [IgnoreDataMember]
    public string CreatedBy { get; set; }

    public DateTimeOffset ModifiedTime { get; set; }

    [MaxLength(128)]
    [IgnoreDataMember]
    public string ModifiedBy { get; set; }
  }
}
