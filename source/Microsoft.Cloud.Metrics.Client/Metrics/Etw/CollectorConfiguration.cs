// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.Etw.CollectorConfiguration
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Cloud.Metrics.Client.Metrics.Etw
{
  internal sealed class CollectorConfiguration
  {
    public CollectorConfiguration(string etwSessionsPrefix)
    {
      int processorCount = Environment.ProcessorCount;
      this.FlushTimerSec = 0;
      this.MinBufferCount = 2 * processorCount;
      this.MaxBufferCount = 2 * this.MinBufferCount;
      this.BufferSizeKB = 256;
      this.ClockType = ClockType.System;
      this.SessionType = SessionType.Realtime;
      this.DeprecatedCollector = (string) null;
      this.MaxFileSizeMB = 100;
      this.MaxFileTimeSpan = TimeSpan.FromMinutes(5.0);
      this.MaxFileCount = 1440;
      this.OriginalName = "Collector";
      this.Name = CollectorConfiguration.GetNormalizedSessionName(this.OriginalName, this.SessionType, etwSessionsPrefix);
      this.Providers = new Dictionary<Guid, ProviderConfiguration>();
    }

    public string Name { get; private set; }

    public string OriginalName { get; private set; }

    public string DeprecatedCollector { get; set; }

    public int FlushTimerSec { get; set; }

    public int MinBufferCount { get; set; }

    public int MaxBufferCount { get; set; }

    public int BufferSizeKB { get; set; }

    public ClockType ClockType { get; set; }

    public SessionType SessionType { get; set; }

    public int MaxFileSizeMB { get; set; }

    public TimeSpan MaxFileTimeSpan { get; set; }

    public int MaxFileCount { get; set; }

    public Dictionary<Guid, ProviderConfiguration> Providers { get; set; }

    public static string GetNormalizedSessionName(
      string originalName,
      SessionType sessionType,
      string etwSessionsPrefix)
    {
      if (originalName.Equals("NT Kernel Logger", StringComparison.OrdinalIgnoreCase))
        return originalName;
      string str;
      switch (sessionType)
      {
        case SessionType.File:
          str = "file-";
          break;
        case SessionType.Realtime:
          str = "live-";
          break;
        case SessionType.Private:
          str = "private-";
          break;
        case SessionType.FileAndRealtime:
          str = "file+live-";
          break;
        default:
          throw new InvalidDataException("The specified session type is not recognized: " + sessionType.ToString());
      }
      return etwSessionsPrefix + str + originalName;
    }
  }
}
