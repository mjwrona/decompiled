// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.DsIcm.RotationOnCall
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;

namespace Microsoft.AzureAd.Icm.Types.DsIcm
{
  public class RotationOnCall
  {
    public RotationOnCall() => this.ShiftOnCalls = (IList<ShiftOnCall>) new List<ShiftOnCall>();

    public long TeamId { get; set; }

    public DateTimeOffset RotationStartDate { get; set; }

    public int RotationLength { get; set; }

    public string TimeZoneId { get; set; }

    public IList<ShiftOnCall> ShiftOnCalls { get; set; }

    public RotationOnCall ShallowCopy() => (RotationOnCall) this.MemberwiseClone();
  }
}
