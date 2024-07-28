// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.ProcessTools
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Diagnostics;
using System.Text;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class ProcessTools : IProcessTools
  {
    public void RunCommand(string commandName, Action<string> onProcessComplete) => this.RunCommand(commandName, onProcessComplete, (string) null);

    public void RunCommand(
      string commandName,
      Action<string> onProcessComplete,
      string commandArgs)
    {
      StringBuilder processOutput = new StringBuilder();
      Process process = new Process();
      try
      {
        process.EnableRaisingEvents = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.FileName = commandName;
        process.StartInfo.Arguments = commandArgs ?? string.Empty;
        process.OutputDataReceived += (DataReceivedEventHandler) ((sender, e) => processOutput.AppendLine(e.Data));
        process.Exited += (EventHandler) ((sender, e) =>
        {
          process.WaitForExit();
          onProcessComplete(processOutput.ToString());
          process.Close();
          process.Dispose();
        });
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
      }
      catch (Exception ex)
      {
        process.Close();
        process.Dispose();
      }
    }
  }
}
