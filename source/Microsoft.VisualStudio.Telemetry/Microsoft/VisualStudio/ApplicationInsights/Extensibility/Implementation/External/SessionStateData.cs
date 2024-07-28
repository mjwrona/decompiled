// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SessionStateData
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.CodeDom.Compiler;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  [GeneratedCode("gbc", "3.02")]
  internal class SessionStateData
  {
    public int ver { get; set; }

    public SessionState state { get; set; }

    public SessionStateData()
      : this("AI.SessionStateData", nameof (SessionStateData))
    {
    }

    protected SessionStateData(string fullName, string name)
    {
      this.ver = 2;
      this.state = SessionState.Start;
    }
  }
}
