// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.SyncService
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;

namespace Microsoft.AzureAd.Icm.Types
{
  public class SyncService : SyncId
  {
    public string Name { get; set; }

    public string Description { get; set; }

    public Guid PublicServiceId { get; set; }

    public bool IsTombstoned { get; set; }

    public long ServiceSiloId { get; set; }

    public string PublicFriendlyId { get; set; }

    public string Keywords { get; set; }

    public long? ServiceCategoryId { get; set; }
  }
}
