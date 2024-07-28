// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.Etw.ActiveCollector
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace Microsoft.Cloud.Metrics.Client.Metrics.Etw
{
  internal sealed class ActiveCollector
  {
    private const int ExitCodeSuccess = 0;
    private static readonly object LogId = Logger.CreateCustomLogId(nameof (ActiveCollector));
    private readonly string baseEtlLocation;
    private bool isFileCollector;
    private string etlBaseName;
    private int maxFileCount;
    private int maxFileSizeKB;
    private TimeSpan maxFileTimeSpan;
    private DateTime lastRotationTime;
    private string currentEtlSessionFile;

    public ActiveCollector(string sessionName, string baseEtlLocation = ".")
    {
      if (string.IsNullOrEmpty(sessionName))
        throw new ArgumentException("sessionName cannot be null or empty.", nameof (sessionName));
      if (string.IsNullOrEmpty(baseEtlLocation))
        throw new ArgumentException("baseEtlLocation cannot be null or empty.", nameof (baseEtlLocation));
      this.Name = sessionName;
      this.lastRotationTime = DateTime.MaxValue;
      this.baseEtlLocation = baseEtlLocation;
    }

    public string Name { get; private set; }

    public string EtlLogsDirectory { get; private set; }

    public static bool StopCollector(string collectorName)
    {
      if (string.IsNullOrEmpty(collectorName))
        throw new ArgumentException("collectorName cannot be null or empty.", nameof (collectorName));
      Logger.Log(LoggerLevel.Info, ActiveCollector.LogId, nameof (StopCollector), "Attempting to stop ETW session [{0}]", (object) collectorName);
      if (!EtwSessionManager.Stop(collectorName))
      {
        Logger.Log(LoggerLevel.Error, ActiveCollector.LogId, nameof (StopCollector), "Failed to stop ETW session [{0}]", (object) collectorName);
        return false;
      }
      Logger.Log(LoggerLevel.Info, ActiveCollector.LogId, nameof (StopCollector), "ETW session [{0}] was stopped", (object) collectorName);
      return true;
    }

    public static bool TryUpdateProviders(CollectorConfiguration config)
    {
      bool flag = true;
      foreach (ProviderConfiguration providerConfiguration in config.Providers.Values)
      {
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        object[] objArray1 = new object[5]
        {
          (object) config.Name,
          null,
          null,
          null,
          null
        };
        Guid id = providerConfiguration.Id;
        objArray1[1] = (object) id.ToString("B");
        objArray1[2] = (object) providerConfiguration.KeywordsAny.ToString("X16");
        objArray1[3] = (object) providerConfiguration.KeywordsAll.ToString("X16");
        objArray1[4] = (object) ((int) providerConfiguration.Level).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        int num = ActiveCollector.RunCommand("logman", string.Format((IFormatProvider) invariantCulture, "update \"{0}\" -p {1} 0x{2},0x{3} {4} -ets", objArray1));
        if (num != 0)
        {
          object logId = ActiveCollector.LogId;
          object[] objArray2 = new object[3];
          id = providerConfiguration.Id;
          objArray2[0] = (object) id.ToString("B");
          objArray2[1] = (object) config.Name;
          objArray2[2] = (object) num.ToString("X8");
          Logger.Log(LoggerLevel.Error, logId, "UpdateProviders", "Failed to set provider {0} on collector [{1}]. Error code: 0x{2}", objArray2);
          flag = false;
        }
      }
      return flag;
    }

    public List<string> StartCollector(CollectorConfiguration config)
    {
      List<string> source = new List<string>();
      if (!string.IsNullOrEmpty(config.DeprecatedCollector))
        ActiveCollector.StopCollector(config.DeprecatedCollector);
      ClockType clockType = config.ClockType == ClockType.Default ? ClockType.Perf : config.ClockType;
      bool flag = false;
      NativeMethods.EventTraceProperties traceProperties;
      if (EtwSessionManager.TryGetSessionProperties(this.Name, out traceProperties))
      {
        if ((int) clockType == (int) traceProperties.Wnode.ClientContext)
          flag = true;
        else if (!ActiveCollector.StopCollector(this.Name))
        {
          Logger.Log(LoggerLevel.Error, ActiveCollector.LogId, nameof (StartCollector), "Failed to stop existing trace session [{0}]. Cannot proceed to correctly update session.", (object) this.Name);
          return source;
        }
      }
      StringBuilder stringBuilder1 = new StringBuilder(256);
      StringBuilder stringBuilder2 = stringBuilder1;
      CultureInfo invariantCulture = CultureInfo.InvariantCulture;
      object[] objArray = new object[7]
      {
        flag ? (object) "update" : (object) "start",
        (object) config.Name,
        (object) config.MinBufferCount.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        null,
        null,
        null,
        null
      };
      int num1 = config.MaxBufferCount;
      objArray[3] = (object) num1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      num1 = config.BufferSizeKB;
      objArray[4] = (object) num1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      num1 = config.FlushTimerSec;
      objArray[5] = (object) num1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      objArray[6] = (object) clockType;
      stringBuilder2.AppendFormat((IFormatProvider) invariantCulture, "{0} \"{1}\" -nb {2} {3} -bs {4} -ft {5} -ct {6} -ets ", objArray);
      if (config.SessionType == SessionType.Realtime)
      {
        stringBuilder1.Append("-rt");
      }
      else
      {
        this.isFileCollector = true;
        this.maxFileCount = config.MaxFileCount;
        this.maxFileSizeKB = config.MaxFileSizeMB * 1024;
        this.maxFileTimeSpan = config.MaxFileTimeSpan;
        this.etlBaseName = config.OriginalName;
        this.EtlLogsDirectory = Path.Combine(this.baseEtlLocation, config.OriginalName);
        if (!ActiveCollector.ProtectedIO((Action) (() => Directory.CreateDirectory(this.EtlLogsDirectory)), (Action<Exception>) (e =>
        {
          Logger.Log(LoggerLevel.Error, ActiveCollector.LogId, nameof (StartCollector), "Failed to create directory [{0}] for collector [{1}]. Exception: {2}", (object) this.EtlLogsDirectory, (object) config.Name, (object) e);
          Logger.Log(LoggerLevel.Error, ActiveCollector.LogId, nameof (StartCollector), "Failed to create or update ETW session [{0}]", (object) config.Name);
        })))
          return source;
        source = this.GetExistingEtlFiles();
        this.currentEtlSessionFile = this.GenerateNextSessionFileName();
        stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "-mode Sequential -o \"{0}\"", new object[1]
        {
          (object) this.currentEtlSessionFile
        });
        if (config.SessionType == SessionType.FileAndRealtime || config.SessionType == SessionType.FileAndRealtime)
          stringBuilder1.Append(" -rt");
      }
      int num2 = ActiveCollector.RunCommand("logman", stringBuilder1.ToString());
      this.lastRotationTime = DateTime.UtcNow;
      if (num2 != 0)
      {
        Logger.Log(LoggerLevel.Error, ActiveCollector.LogId, nameof (StartCollector), "Logman failed to create or update ETW session [{0}].", (object) config.Name);
        if (flag)
        {
          if (config.SessionType != SessionType.Realtime)
          {
            if (!EtwSessionManager.TryGetCurrentFileOfSession(config.Name, out this.currentEtlSessionFile))
              Logger.Log(LoggerLevel.Error, ActiveCollector.LogId, nameof (StartCollector), "Failed to retrieve name of the ETL being used by ETW session [{0}].", (object) config.Name);
            if (source.Count > 0 && string.Compare(source.Last<string>(), this.currentEtlSessionFile, StringComparison.OrdinalIgnoreCase) == 0)
            {
              source.RemoveAt(source.Count - 1);
              Logger.Log(LoggerLevel.Warning, ActiveCollector.LogId, nameof (StartCollector), "Current ETL file removed from backlog list since it is still in use. ETL File [{0}]", (object) this.currentEtlSessionFile);
            }
          }
          Logger.Log(LoggerLevel.Info, ActiveCollector.LogId, nameof (StartCollector), "Attempting to mitigate with pre-existing session...");
          if (!ActiveCollector.ExistingSessionSatisfiesProviders(config))
          {
            Logger.Log(LoggerLevel.Error, ActiveCollector.LogId, nameof (StartCollector), "Pre-existing session cannot be used since it does not satisfy the config.");
          }
          else
          {
            Logger.Log(LoggerLevel.Error, ActiveCollector.LogId, nameof (StartCollector), "Using pre-existing ETW session since it satisfied the configuration.");
            num2 = 0;
          }
        }
      }
      if (num2 == 0 | flag)
        Logger.Log(LoggerLevel.Info, ActiveCollector.LogId, nameof (StartCollector), "ETW session is in place call UpdateProviders to enable them.");
      source.Sort((IComparer<string>) StringComparer.OrdinalIgnoreCase);
      return source;
    }

    public bool RotateSessionFile(DateTime referenceTime, out string closedSessionFile)
    {
      bool flag = false;
      closedSessionFile = (string) null;
      if (this.isFileCollector && (referenceTime - this.lastRotationTime > this.maxFileTimeSpan ? 1 : ((long) this.GetCurrentFileSize() > (long) this.maxFileSizeKB ? 1 : 0)) != 0)
      {
        string nextSessionFileName = this.GenerateNextSessionFileName();
        if (ActiveCollector.RunCommand("logman", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "update \"{0}\" -o \"{1}\" -ets", new object[2]
        {
          (object) this.Name,
          (object) nextSessionFileName
        })) == 0)
        {
          flag = true;
          closedSessionFile = this.currentEtlSessionFile;
          this.currentEtlSessionFile = nextSessionFileName;
          this.lastRotationTime = DateTime.UtcNow;
          this.DeleteOlderSessionFiles();
        }
      }
      return flag;
    }

    internal static bool ExistingSessionSatisfiesProviders(CollectorConfiguration config)
    {
      bool flag = false;
      NativeMethods.EventTraceProperties traceProperties;
      if (EtwSessionManager.TryGetSessionProperties(config.Name, out traceProperties))
      {
        flag = true;
        ulong historicalContext = traceProperties.Wnode.HistoricalContext;
        foreach (KeyValuePair<Guid, ProviderConfiguration> provider in config.Providers)
        {
          NativeMethods.TraceEnableInfo enableInfo;
          flag = EtwSessionManager.GetProviderInfo(historicalContext, provider.Value.Id, out enableInfo) && enableInfo.IsEnabled != 0U && provider.Value.Level <= (EtwTraceLevel) enableInfo.Level && (provider.Value.KeywordsAll | enableInfo.MatchAllKeyword) == provider.Value.KeywordsAll && (provider.Value.KeywordsAny & enableInfo.MatchAnyKeyword) == provider.Value.KeywordsAny;
          if (!flag)
          {
            ProviderConfiguration providerConfiguration = new ProviderConfiguration(provider.Value.Id, (EtwTraceLevel) enableInfo.Level, enableInfo.MatchAnyKeyword, enableInfo.MatchAllKeyword);
            Logger.Log(LoggerLevel.Error, ActiveCollector.LogId, nameof (ExistingSessionSatisfiesProviders), "Provider configuration [{0}] is not satisfied in ETW session [{1}]. Actual provider settings in session: {2}", (object) provider.Value, (object) config.Name, enableInfo.IsEnabled == 0U ? (object) "provider not enabled" : (object) providerConfiguration.ToString());
          }
        }
      }
      return flag;
    }

    private static void DisableProvider(string collectorName, Guid providerId) => ActiveCollector.RunCommand("logman", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "update \"{0}\" --p {1} -ets", new object[2]
    {
      (object) collectorName,
      (object) providerId.ToString("B")
    }));

    private static int RunCommand(string fileName, string arguments)
    {
      Logger.Log(LoggerLevel.Info, ActiveCollector.LogId, nameof (RunCommand), "[{0} {1}]", (object) fileName, (object) arguments);
      using (Process process = new Process())
      {
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.FileName = fileName;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        while (!process.StandardOutput.EndOfStream)
        {
          string str = process.StandardOutput.ReadLine();
          Logger.Log(LoggerLevel.Info, ActiveCollector.LogId, nameof (RunCommand), "\t\t{0}", (object) str);
        }
        Win32Exception win32Exception = new Win32Exception(process.ExitCode);
        Logger.Log(LoggerLevel.Info, ActiveCollector.LogId, nameof (RunCommand), "exitCode=0x{0}: {1}", (object) process.ExitCode.ToString("X8"), (object) win32Exception.Message);
        return process.ExitCode;
      }
    }

    private static bool ProtectedIO(Action ioOperation, Action<Exception> failureAction)
    {
      Exception exception = (Exception) null;
      try
      {
        ioOperation();
      }
      catch (IOException ex)
      {
        exception = (Exception) ex;
      }
      catch (SecurityException ex)
      {
        exception = (Exception) ex;
      }
      catch (UnauthorizedAccessException ex)
      {
        exception = (Exception) ex;
      }
      if (exception != null)
        failureAction(exception);
      return exception == null;
    }

    private string GenerateNextSessionFileName()
    {
      DateTime utcNow = DateTime.UtcNow;
      return Path.Combine(this.EtlLogsDirectory, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}_{1:0000}-{2:00}-{3:00}_{4:00}-{5:00}-{6:00}_utc.etl", (object) this.etlBaseName, (object) utcNow.Year, (object) utcNow.Month, (object) utcNow.Day, (object) utcNow.Hour, (object) utcNow.Minute, (object) utcNow.Second));
    }

    private void DeleteOlderSessionFiles()
    {
      if (!Directory.Exists(this.EtlLogsDirectory))
        return;
      List<string> existingEtlFiles = this.GetExistingEtlFiles();
      int num = existingEtlFiles.Count - this.maxFileCount;
      if (num <= 0)
        return;
      existingEtlFiles.Sort((IComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < num; ++index)
      {
        string existingFile = existingEtlFiles[index];
        ActiveCollector.ProtectedIO((Action) (() => File.Delete(existingFile)), (Action<Exception>) (e => Logger.Log(LoggerLevel.Warning, ActiveCollector.LogId, nameof (DeleteOlderSessionFiles), "Failed to delete ETL file [{0}]. Exception: {1}", (object) existingFile, (object) e.Message)));
      }
    }

    private List<string> GetExistingEtlFiles()
    {
      List<string> existingFiles = (List<string>) null;
      string searchPattern = this.etlBaseName + "_*.etl";
      return !ActiveCollector.ProtectedIO((Action) (() => existingFiles = ((IEnumerable<string>) Directory.GetFiles(this.EtlLogsDirectory, searchPattern)).ToList<string>()), (Action<Exception>) (e => Logger.Log(LoggerLevel.Warning, ActiveCollector.LogId, nameof (GetExistingEtlFiles), "Failed to enumerate files at [{0}] with mask [{1}] for collector [{2}]. Exception: {3}", (object) this.EtlLogsDirectory, (object) searchPattern, (object) this.Name, (object) e.Message))) ? new List<string>() : existingFiles;
    }

    private uint GetCurrentFileSize()
    {
      if (!this.isFileCollector)
        throw new InvalidOperationException("Tried to obtain file size for the non-file collector [" + this.Name + "]");
      throw new NotImplementedException();
    }
  }
}
