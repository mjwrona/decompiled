// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.SyncEscalationPolicy
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AzureAd.Icm.Types
{
  public class SyncEscalationPolicy
  {
    [Required]
    [Range(1.0, 9.2233720368547758E+18)]
    public long TeamId { get; set; }

    public long ChainId { get; set; }

    public DateTimeOffset ModifiedDate { get; set; }

    public string Name { get; set; }

    [Required]
    [Range(1.0, 9.2233720368547758E+18)]
    public long TenantId { get; set; }

    [Required]
    public bool IsDefault { get; set; }

    [Required]
    public IList<SyncEscalationPolicy.Link> Links { get; set; }

    public class Link
    {
      public long? TargetId { get; set; }

      public int Position { get; set; }
    }
  }
}
