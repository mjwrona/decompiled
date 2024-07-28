// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AzComm.Contracts.MeterUsage2GetResponse
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server.AzComm.Contracts
{
  [DataContract]
  public class MeterUsage2GetResponse
  {
    [DataMember(EmitDefaultValue = false)]
    public Guid MeterId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public MeterKind MeterKind { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool IsInTrial { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public double MaxQuantity { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public double CurrentQuantity { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public double AvailableQuantity { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public double FreeQuantity { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? LastUsageDateTime { get; set; }
  }
}
