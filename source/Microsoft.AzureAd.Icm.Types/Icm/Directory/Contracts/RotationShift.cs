// Decompiled with JetBrains decompiler
// Type: Microsoft.Icm.Directory.Contracts.RotationShift
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Collections.Generic;

namespace Microsoft.Icm.Directory.Contracts
{
  public class RotationShift
  {
    public RotationShift() => this.RotationLists = (IList<RotationList>) new List<RotationList>();

    public long Id { get; set; }

    public string Name { get; set; }

    public short Order { get; set; }

    public short BackupCount { get; set; }

    public short Length { get; set; }

    public IList<RotationList> RotationLists { get; set; }
  }
}
