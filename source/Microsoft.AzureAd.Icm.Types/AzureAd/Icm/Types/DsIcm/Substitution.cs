// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.DsIcm.Substitution
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;

namespace Microsoft.AzureAd.Icm.Types.DsIcm
{
  public class Substitution
  {
    public DateTime Date { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public long TeamId { get; set; }

    public short? OnCallPosition { get; set; }

    public long? ShiftId { get; set; }

    public string ShiftName { get; set; }

    public long OriginalContactId { get; set; }

    public long SubstituteContactId { get; set; }
  }
}
