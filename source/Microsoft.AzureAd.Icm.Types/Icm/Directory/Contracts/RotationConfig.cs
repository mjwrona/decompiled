// Decompiled with JetBrains decompiler
// Type: Microsoft.Icm.Directory.Contracts.RotationConfig
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Icm.Directory.Contracts
{
  [Serializable]
  public class RotationConfig
  {
    public RotationConfig() => this.SubCycles = (IList<RotationSubCycle>) new List<RotationSubCycle>();

    public string Name { get; set; }

    public long TeamId { get; set; }

    public string Scheme { get; set; }

    public string State { get; set; }

    public short Length { get; set; }

    public DateTimeOffset SeedDate { get; set; }

    public DateTimeOffset EffectiveDate { get; set; }

    public TimeSpan TransitionTimeOfDay { get; set; }

    public IList<RotationSubCycle> SubCycles { get; set; }
  }
}
