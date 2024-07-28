// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.RotationShift
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Collections;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class RotationShift
  {
    [DataMember(Name = "ShiftId")]
    public long Id { get; set; }

    [DataMember(Name = "ShiftName")]
    public string Name { get; set; }

    [DataMember(Name = "ShiftDuration")]
    public int Duration { get; set; }

    [DataMember(Name = "RotationList")]
    public ArrayList RotationList { get; set; }
  }
}
