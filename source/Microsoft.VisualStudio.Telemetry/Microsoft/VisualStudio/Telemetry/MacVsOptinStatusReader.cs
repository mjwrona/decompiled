// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MacVsOptinStatusReader
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.IO;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class MacVsOptinStatusReader : ITelemetryOptinStatusReader
  {
    public bool ReadIsOptedInStatus(string productVersion)
    {
      try
      {
        string str = System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library"), "Preferences"), "VisualStudio");
        if (Directory.Exists(str))
        {
          string path = System.IO.Path.Combine(str, "TelemetryOptInState");
          if (File.Exists(path))
            return File.ReadAllText(path) != "0";
        }
      }
      catch
      {
      }
      return false;
    }

    public bool ReadIsOptedInStatus(TelemetrySession session) => this.ReadIsOptedInStatus(string.Empty);
  }
}
