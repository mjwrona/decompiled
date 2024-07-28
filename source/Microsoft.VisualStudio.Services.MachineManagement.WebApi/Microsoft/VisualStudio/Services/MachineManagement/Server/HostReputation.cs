// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.Server.HostReputation
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.Server
{
  [DataContract]
  public sealed class HostReputation
  {
    [DataMember(IsRequired = true)]
    public Guid HostId { get; set; }

    [DataMember(IsRequired = true)]
    public int Score { get; set; }

    [DataMember(IsRequired = false)]
    public bool IsConstant { get; set; }

    [DataMember(IsRequired = false)]
    public DateTime OwnerCreatedAt { get; set; }

    [DataMember(IsRequired = false)]
    public DateTime FirstDate { get; set; }

    [DataMember(IsRequired = false)]
    public DateTime EvaluationDate { get; set; }

    [DataMember(IsRequired = false)]
    public bool HasPrivateProject { get; set; }

    [DataMember(IsRequired = false)]
    public int MaxParallelism { get; set; }

    [DataMember(IsRequired = false)]
    public int RunCount { get; set; }

    [DataMember(IsRequired = false)]
    public int RunAverage { get; set; }

    [DataMember(IsRequired = false)]
    public int RunDeviation { get; set; }

    [DataMember(IsRequired = false)]
    public int RunDensity { get; set; }

    [DataMember(IsRequired = false)]
    public int MaliciousProcessDensity { get; set; }

    [DataMember(IsRequired = false)]
    public int AlteredHostsFileDensity { get; set; }

    [DataMember(IsRequired = false)]
    public int SuspiciousSourceDensity { get; set; }

    [DataMember(IsRequired = false)]
    public string TrustTier { get; set; }

    public override string ToString() => string.Format(string.Format("HostId:{0}, Score:{1}, IsConstant:{2}", (object) this.HostId, (object) this.Score, (object) this.IsConstant));
  }
}
