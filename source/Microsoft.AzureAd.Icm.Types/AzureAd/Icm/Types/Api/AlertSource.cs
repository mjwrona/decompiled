// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.Api.AlertSource
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AzureAd.Icm.Types.Api
{
  public class AlertSource
  {
    [Key]
    public Guid PublicId { get; set; }

    public string Name { get; set; }

    public bool IsEnabled { get; set; }

    public string IncidentLinkFormatString { get; set; }

    public long InstanceReverseSyncCategories { get; set; }

    public string TypeName { get; set; }

    public long TypeId { get; set; }

    public long AlertSourceTypeReverseSyncCategories { get; set; }
  }
}
