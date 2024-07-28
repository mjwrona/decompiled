// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.StackFrame
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.CodeDom.Compiler;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  [GeneratedCode("gbc", "3.02")]
  internal class StackFrame
  {
    public int level { get; set; }

    public string method { get; set; }

    public string assembly { get; set; }

    public string fileName { get; set; }

    public int line { get; set; }

    public StackFrame()
      : this("AI.StackFrame", nameof (StackFrame))
    {
    }

    protected StackFrame(string fullName, string name)
    {
      this.method = string.Empty;
      this.assembly = string.Empty;
      this.fileName = string.Empty;
    }
  }
}
