// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineRequestTag
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachineRequestTag
  {
    private double m_waitTimeSlaMultiplier = 1.0;

    internal MachineRequestTag()
    {
    }

    public MachineRequestTag(
      string machineRequestType,
      string tagName,
      double slaMultiplier,
      string concurrencyCommerceMeterName)
    {
      this.ConcurrencyCommerceMeterName = concurrencyCommerceMeterName;
      this.MachineRequestType = machineRequestType;
      this.Name = tagName;
      this.WaitTimeSlaMultiplier = slaMultiplier;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ConcurrencyCommerceMeterName { get; set; }

    [DataMember(IsRequired = true)]
    public string MachineRequestType { get; set; }

    [DataMember(IsRequired = true)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double WaitTimeSlaMultiplier
    {
      get => this.m_waitTimeSlaMultiplier;
      set
      {
        if (value <= 0.0)
          this.m_waitTimeSlaMultiplier = 1.0;
        else
          this.m_waitTimeSlaMultiplier = value;
      }
    }
  }
}
