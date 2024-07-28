// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ControlOptions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ControlOptions : IEquatable<ControlOptions>
  {
    [DataMember]
    public bool AlwaysRun { get; set; }

    [DataMember]
    public bool Enabled { get; set; }

    [DataMember]
    public bool ContinueOnError { get; set; }

    public ControlOptions()
    {
      this.AlwaysRun = false;
      this.Enabled = true;
      this.ContinueOnError = false;
    }

    public bool Equals(ControlOptions other) => other != null && this.AlwaysRun == other.AlwaysRun && this.ContinueOnError == other.ContinueOnError && this.Enabled == other.Enabled;

    public ControlOptions Clone() => new ControlOptions()
    {
      AlwaysRun = this.AlwaysRun,
      Enabled = this.Enabled,
      ContinueOnError = this.ContinueOnError
    };
  }
}
