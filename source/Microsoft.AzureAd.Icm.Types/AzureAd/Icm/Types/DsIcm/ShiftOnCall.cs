// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.DsIcm.ShiftOnCall
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;

namespace Microsoft.AzureAd.Icm.Types.DsIcm
{
  public class ShiftOnCall
  {
    public ShiftOnCall() => this.Contacts = (IList<ShiftContactInfo>) new List<ShiftContactInfo>();

    public string ShiftName { get; set; }

    public TimeSpan ShiftStartTimeOfDayLocal { get; set; }

    public int ShiftLength { get; set; }

    public int ShiftOrder { get; set; }

    public IList<ShiftContactInfo> Contacts { get; set; }
  }
}
