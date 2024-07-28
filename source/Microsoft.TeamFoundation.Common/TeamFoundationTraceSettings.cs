// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TeamFoundationTraceSettings
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Eventing;
using System.Globalization;
using System.Threading;

namespace Microsoft.TeamFoundation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TeamFoundationTraceSettings
  {
    private Dictionary<string, TraceSwitch> m_traceSwitches;
    private Dictionary<string, TraceLevel> m_originalTraceLevels;
    private bool m_tracingEnabled;
    private TraceSwitch m_overrideSwitch;
    private ReaderWriterLock m_switchLock = new ReaderWriterLock();
    private long m_startTicks = DateTime.UtcNow.Ticks;
    private string m_globalKeyword = TraceKeywords.BuildKeyword("TFS.Tracing");
    private string m_overrideKeyword = TraceKeywords.BuildKeyword("TFS.All");
    internal static readonly string AppSettingTracingEnabled = "TFTrace.Writer";
    internal static readonly string AppSettingTraceDirectory = "TFTrace.DirectoryName";
    private static readonly Guid m_eventProviderGuid = new Guid("78EB1F5E-D85E-4FFF-AF60-8F3B4051E50D");
    private EventProvider m_eventProvider;
    private TeamFoundationTextWriterTraceListener m_textWriter;

    internal TeamFoundationTraceSettings()
    {
      this.InitializeTraceWriter();
      this.m_traceSwitches = new Dictionary<string, TraceSwitch>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_overrideSwitch = new TraceSwitch(this.m_overrideKeyword, this.m_overrideKeyword);
      List<string> fromConfiguration = this.GetTraceSwitchesFromConfiguration();
      this.m_switchLock.AcquireWriterLock(-1);
      try
      {
        foreach (string keyword in fromConfiguration)
          this.GetTraceSwitch(keyword, true);
        this.UpdateTracingEnabledFlag();
        this.PopulateOriginalTraceLevel();
      }
      finally
      {
        this.m_switchLock.ReleaseWriterLock();
      }
      try
      {
        this.m_eventProvider = new EventProvider(TeamFoundationTraceSettings.m_eventProviderGuid);
      }
      catch (Exception ex)
      {
        Trace.WriteLine("An error occurred while TeamFoundationTraceSettings was creating its EventProvider.  Error: " + ex.Message);
      }
    }

    internal bool IsTracingEnabled
    {
      get => this.m_tracingEnabled || this.IsEventProviderSessionActive;
      private set => this.m_tracingEnabled = value;
    }

    internal bool IsOverrideEnabled(TraceLevel traceLevel) => this.OverrideLevel >= traceLevel || this.m_eventProvider != null && this.m_eventProvider.IsEnabled(TeamFoundationTraceSettings.ConvertLevelToETW(traceLevel), 0L);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TraceLevel OverrideLevel
    {
      get => this.m_overrideSwitch.Level;
      set
      {
        if (this.m_overrideSwitch.Level == value)
          return;
        this.m_switchLock.AcquireWriterLock(-1);
        try
        {
          this.m_overrideSwitch.Level = value;
          this.UpdateTracingEnabledFlag();
        }
        finally
        {
          this.m_switchLock.ReleaseWriterLock();
        }
      }
    }

    public TraceLevel this[string keyword]
    {
      get
      {
        this.m_switchLock.AcquireReaderLock(-1);
        try
        {
          TraceSwitch traceSwitch = this.GetTraceSwitch(keyword, false);
          return traceSwitch != null ? traceSwitch.Level : TraceLevel.Off;
        }
        finally
        {
          this.m_switchLock.ReleaseReaderLock();
        }
      }
      set
      {
        if (keyword.Equals(this.m_overrideKeyword, StringComparison.OrdinalIgnoreCase))
        {
          this.OverrideLevel = value;
        }
        else
        {
          this.m_switchLock.AcquireWriterLock(-1);
          try
          {
            this.GetTraceSwitch(keyword, true).Level = value;
            this.UpdateTracingEnabledFlag();
          }
          finally
          {
            this.m_switchLock.ReleaseWriterLock();
          }
        }
      }
    }

    public TraceLevel OriginalTraceLevel(string keyword) => this.m_originalTraceLevels.ContainsKey(keyword) ? this.m_originalTraceLevels[keyword] : TraceLevel.Off;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public SortedDictionary<string, TraceLevel> GetEnabledKeywords()
    {
      SortedDictionary<string, TraceLevel> enabledKeywords = new SortedDictionary<string, TraceLevel>();
      this.m_switchLock.AcquireReaderLock(-1);
      try
      {
        foreach (KeyValuePair<string, TraceSwitch> traceSwitch in this.m_traceSwitches)
        {
          if (traceSwitch.Value.Level > TraceLevel.Off)
            enabledKeywords[traceSwitch.Key] = traceSwitch.Value.Level;
        }
      }
      finally
      {
        this.m_switchLock.ReleaseReaderLock();
      }
      return enabledKeywords;
    }

    private TraceSwitch GetTraceSwitch(string keyword, bool create)
    {
      TraceSwitch traceSwitch = (TraceSwitch) null;
      try
      {
        if (!this.m_traceSwitches.TryGetValue(keyword, out traceSwitch))
        {
          if (create)
          {
            traceSwitch = new TraceSwitch(keyword, "Team Foundation Trace Switch: " + keyword);
            int level = (int) traceSwitch.Level;
            this.m_traceSwitches[keyword] = traceSwitch;
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine("An error occurred while TeamFoundationTraceSettings was loading a switch: " + ex.Message);
      }
      return traceSwitch;
    }

    private void PopulateOriginalTraceLevel()
    {
      this.m_originalTraceLevels = new Dictionary<string, TraceLevel>();
      foreach (KeyValuePair<string, TraceSwitch> traceSwitch in this.m_traceSwitches)
        this.m_originalTraceLevels[traceSwitch.Key] = traceSwitch.Value.Level;
    }

    private List<string> GetTraceSwitchesFromConfiguration()
    {
      List<string> fromConfiguration = new List<string>();
      try
      {
        if (ConfigurationManager.GetSection("system.diagnostics") is ConfigurationSection section)
        {
          PropertyInformation property1 = section.ElementInformation.Properties["switches"];
          if (property1 != null)
          {
            if (property1.Value is ConfigurationElementCollection elementCollection)
            {
              foreach (ConfigurationElement configurationElement in elementCollection)
              {
                PropertyInformation property2 = configurationElement.ElementInformation.Properties["name"];
                if (property2 != null)
                {
                  string str = property2.Value as string;
                  if (!string.IsNullOrEmpty(str))
                    fromConfiguration.Add(str);
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine("An error occurred while TeamFoundationTraceSettings was preloading trace switches: " + ex.Message);
      }
      return fromConfiguration;
    }

    private void UpdateTracingEnabledFlag()
    {
      try
      {
        if (new TraceSwitch(this.m_globalKeyword, this.m_globalKeyword).Level > TraceLevel.Off)
        {
          this.IsTracingEnabled = true;
          return;
        }
        if (this.OverrideLevel > TraceLevel.Off)
        {
          this.IsTracingEnabled = true;
          return;
        }
        foreach (TraceSwitch traceSwitch in this.m_traceSwitches.Values)
        {
          if (traceSwitch.Level > TraceLevel.Off)
          {
            this.IsTracingEnabled = true;
            return;
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine("An error occurred while TeamFoundationTraceSettings was updating the tracing enabled flag: " + ex.Message);
      }
      this.IsTracingEnabled = false;
    }

    internal long MillisecondsElapsed => (DateTime.UtcNow.Ticks - this.m_startTicks) / 10000L;

    private void InitializeTraceWriter()
    {
      try
      {
        string appSetting1 = ConfigurationManager.AppSettings[TeamFoundationTraceSettings.AppSettingTracingEnabled];
        string appSetting2 = ConfigurationManager.AppSettings[TeamFoundationTraceSettings.AppSettingTraceDirectory];
        if (!string.IsNullOrEmpty(appSetting2))
          TeamFoundationTextWriterTraceListener.TraceFileDirectoryName = appSetting2;
        if (string.IsNullOrEmpty(appSetting1) || !appSetting1.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
          return;
        this.EnableLogging((string) null);
      }
      catch (Exception ex)
      {
        Trace.WriteLine("An error occurred while TeamFoundationTraceSettings was initializing the trace writer: " + ex.Message);
      }
    }

    internal void EnableLogging(string path)
    {
      this.DisableLogging();
      this.m_textWriter = string.IsNullOrEmpty(path) ? new TeamFoundationTextWriterTraceListener() : new TeamFoundationTextWriterTraceListener(path);
      this.m_textWriter.WriteLine(TFCommonResources.TraceStartMessage((object) "TFTrace", (object) DateTime.UtcNow.ToString((IFormatProvider) DateTimeFormatInfo.InvariantInfo)));
      Trace.Listeners.Add((TraceListener) this.m_textWriter);
    }

    internal void DisableLogging()
    {
      if (this.m_textWriter == null)
        return;
      Trace.Listeners.Remove((TraceListener) this.m_textWriter);
      this.m_textWriter.WriteLine(TFCommonResources.TraceStopMessage((object) "TFTrace", (object) DateTime.UtcNow.ToString((IFormatProvider) DateTimeFormatInfo.InvariantInfo)));
      this.m_textWriter.Dispose();
      this.m_textWriter = (TeamFoundationTextWriterTraceListener) null;
    }

    internal bool IsLogging => this.m_textWriter != null;

    internal string LogFileName => this.m_textWriter == null ? (string) null : this.m_textWriter.TraceFileName;

    private bool IsEventProviderSessionActive => this.m_eventProvider != null && this.m_eventProvider.IsEnabled();

    public EventProvider EventProvider => this.m_eventProvider;

    internal static byte ConvertLevelToETW(TraceLevel traceLevel) => (byte) (traceLevel + 1);
  }
}
