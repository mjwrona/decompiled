// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.PythonString
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs
{
  public class PythonString
  {
    public string Value { get; }

    public PythonString(string value) => this.Value = value;

    public override string ToString()
    {
      char ch = '\'';
      if (this.Value.Contains<char>(ch))
        ch = '"';
      return ch.ToString() + this.Value + ch.ToString();
    }

    public string Dump(string indent, string newline) => "PythonString(" + this.Value + ")";
  }
}
