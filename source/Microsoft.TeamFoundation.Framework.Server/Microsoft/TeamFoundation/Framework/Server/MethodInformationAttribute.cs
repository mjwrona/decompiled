// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MethodInformationAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public sealed class MethodInformationAttribute : Attribute
  {
    public MethodInformationAttribute()
    {
      this.EstimatedCost = EstimatedMethodCost.Low;
      this.KeepsHostAwake = true;
      this.MethodType = MethodType.Normal;
    }

    [DefaultValue(EstimatedMethodCost.Low)]
    public EstimatedMethodCost EstimatedCost { get; set; }

    [DefaultValue(false)]
    public bool IsLongRunning { get; set; }

    [DefaultValue(true)]
    public bool KeepsHostAwake { get; set; }

    [DefaultValue(MethodType.Normal)]
    public MethodType MethodType { get; set; }

    [DefaultValue(0)]
    public long TimeoutSeconds
    {
      get => this.Timeout == new TimeSpan() ? 0L : (long) (int) this.Timeout.TotalSeconds;
      set
      {
        if (value == 0L)
          this.Timeout = new TimeSpan();
        else
          this.Timeout = TimeSpan.FromSeconds((double) value);
      }
    }

    public TimeSpan Timeout { get; set; }
  }
}
