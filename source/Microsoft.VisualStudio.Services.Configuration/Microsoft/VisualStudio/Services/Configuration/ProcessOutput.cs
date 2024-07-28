// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.ProcessOutput
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class ProcessOutput
  {
    private OutputLine[] m_outputLines;

    public ProcessOutput(int exitCode, OutputLine[] outputLines, TimeSpan runningTime)
    {
      this.ExitCode = exitCode;
      this.RunningTime = runningTime;
      this.m_outputLines = outputLines;
    }

    public string StdOut => this.GetOutput(OutputType.StdOut);

    public string StdErr => this.GetOutput(OutputType.StdErr);

    public int ExitCode { get; set; }

    public TimeSpan RunningTime { get; set; }

    public OutputLine[] GetOutputLines() => this.m_outputLines;

    public string[] GetMatchingLines(OutputType outputType) => this.m_outputLines == null || this.m_outputLines.Length == 0 ? Array.Empty<string>() : ((IEnumerable<OutputLine>) this.m_outputLines).Where<OutputLine>((Func<OutputLine, bool>) (l => l.OutputType == outputType)).Select<OutputLine, string>((Func<OutputLine, string>) (l => l.Data)).ToArray<string>();

    public string GetOutput(OutputType outputType)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string matchingLine in this.GetMatchingLines(outputType))
        stringBuilder.AppendLine(matchingLine);
      return stringBuilder.ToString();
    }
  }
}
