// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryLogSettingsProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryLogSettingsProvider : ITelemetryLogSettingsProvider
  {
    private static int fileVersion;
    private static int processId = -1;
    private static int appDomainId = -1;

    public string FileNameFormatString => "{0:yyyyMMdd_HHmmss}_{1}_{2}_{3}_{4}.txt";

    public IEnumerable<KeyValuePair<string, string>> MainIdentifiers { get; set; }

    public int ProcessId
    {
      get
      {
        if (TelemetryLogSettingsProvider.processId == -1)
        {
          using (Process currentProcess = Process.GetCurrentProcess())
            TelemetryLogSettingsProvider.processId = currentProcess.Id;
        }
        return TelemetryLogSettingsProvider.processId;
      }
    }

    public int AppDomainId
    {
      get
      {
        if (TelemetryLogSettingsProvider.appDomainId == -1)
          TelemetryLogSettingsProvider.appDomainId = AppDomain.CurrentDomain.Id;
        return TelemetryLogSettingsProvider.appDomainId;
      }
    }

    public string Path { get; set; }

    public string Folder { get; set; }

    public string FilePath
    {
      get
      {
        string path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.FileNameFormatString, (object) DateTime.Now, (object) string.Join("_", this.MainIdentifiers.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Value)).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x)))), (object) this.ProcessId, (object) this.AppDomainId, (object) this.GetNextUniqueId());
        return System.IO.Path.Combine(this.GetCreateFolderPath(), path2);
      }
    }

    public string GetCreateFolderPath()
    {
      string path = System.IO.Path.Combine(this.Path, this.Folder);
      if (!Directory.Exists(path))
        ReparsePointAware.CreateDirectory(path);
      return path;
    }

    public int GetNextUniqueId() => TelemetryLogSettingsProvider.fileVersion++;
  }
}
