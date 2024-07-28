// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.FileLogger
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Microsoft.TeamFoundation.Common
{
  public class FileLogger : TFLogger, IDisposable
  {
    public static readonly string DefaultLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\DevOps\\Server Configuration\\Logs");
    private readonly object m_lock = new object();
    private readonly bool m_onlyLogToFile;
    private readonly bool m_includeFormatting;
    private Timer m_timer;
    private StreamWriter m_writer;
    private static readonly char[] s_lineSeparators = new char[2]
    {
      '\n',
      '\r'
    };

    private FileLogger()
    {
      this.m_onlyLogToFile = false;
      this.m_includeFormatting = true;
    }

    public FileLogger(Stream stream)
      : this()
    {
      this.m_writer = new StreamWriter(stream);
      this.m_timer = new Timer(new TimerCallback(this.OnTimer), (object) null, 100, 100);
    }

    public FileLogger(string filePath)
      : this(filePath, FileMode.CreateNew)
    {
    }

    public FileLogger(string filePath, FileMode createMode)
      : this(filePath, createMode, false, true)
    {
    }

    public FileLogger(
      string filePath,
      FileMode createMode,
      bool onlyLogToFile,
      bool includeFormatting)
      : this()
    {
      this.FilePath = filePath;
      Directory.CreateDirectory(Path.GetDirectoryName(filePath));
      this.m_writer = new StreamWriter((Stream) new FileStream(this.FilePath, createMode, FileAccess.Write, FileShare.ReadWrite, 10240));
      this.m_timer = new Timer(new TimerCallback(this.OnTimer), (object) null, 100, 100);
      this.m_onlyLogToFile = onlyLogToFile;
      this.m_includeFormatting = includeFormatting;
    }

    public void Dispose() => this.Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      lock (this.m_lock)
      {
        this.m_writer?.Dispose();
        this.m_writer = (StreamWriter) null;
        this.m_timer?.Dispose();
        this.m_timer = (Timer) null;
      }
    }

    public string FilePath { get; private set; }

    public static string GenerateLogFileName(string logFolder, string commandName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(logFolder, nameof (logFolder));
      ArgumentUtility.CheckStringForNullOrEmpty(commandName, nameof (commandName));
      DateTime now = DateTime.Now;
      string path = Path.Combine(logFolder, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1:yyyy-MM-dd_HH-mm-ss}.log", (object) commandName, (object) now));
      if (File.Exists(path))
      {
        for (int index = 1; index < 100; ++index)
        {
          string path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1:yyyy-MM-dd_HH-mm-ss}-{2}.log", (object) commandName, (object) now, (object) index);
          path = Path.Combine(logFolder, path2);
          if (!File.Exists(path))
            break;
        }
      }
      return path;
    }

    public override void Info(string message) => this.WriteLine(FileLogger.LogEntryLevel.Info, message);

    public override void Warning(string message) => this.WriteLine(FileLogger.LogEntryLevel.Warning, message);

    public override void Error(string message) => this.WriteLine(FileLogger.LogEntryLevel.Error, message);

    public void Flush()
    {
      lock (this.m_lock)
        this.m_writer?.Flush();
    }

    private void WriteLine(FileLogger.LogEntryLevel entryType, string message)
    {
      lock (this.m_lock)
      {
        if (this.m_writer == null)
          throw new ObjectDisposedException(nameof (FileLogger));
        try
        {
          if (this.m_includeFormatting)
          {
            if (string.IsNullOrEmpty(message) || message.IndexOfAny(FileLogger.s_lineSeparators) < 0)
            {
              string message1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "[{0,-7}@{1:HH}:{1:mm}:{1:ss}.{1:fff}] {2}", (object) entryType.ToString(), (object) DateTime.Now, (object) message);
              this.m_writer.WriteLine(message1);
              if (!this.m_onlyLogToFile)
                Trace.WriteLine(message1);
            }
            else
            {
              string[] strArray = message.Split(FileLogger.s_lineSeparators, StringSplitOptions.RemoveEmptyEntries);
              string message2 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "[{0,-7}@{1:HH}:{1:mm}:{1:ss}.{1:fff}] {2}", (object) entryType.ToString(), (object) DateTime.Now, (object) strArray[0]);
              this.m_writer.WriteLine(message2);
              if (!this.m_onlyLogToFile)
                Trace.WriteLine(message2);
              for (int index = 1; index < strArray.Length; ++index)
              {
                string message3 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "                       {0}", (object) strArray[index]);
                this.m_writer.WriteLine(message3);
                if (!this.m_onlyLogToFile)
                  Trace.WriteLine(message3);
              }
            }
          }
          else
          {
            this.m_writer.WriteLine(message);
            if (!this.m_onlyLogToFile)
              Trace.WriteLine(message);
          }
          if (entryType != FileLogger.LogEntryLevel.Error && entryType != FileLogger.LogEntryLevel.Warning)
            return;
          this.m_writer.Flush();
        }
        catch (Exception ex)
        {
          Trace.WriteLine(TeamFoundationExceptionFormatter.FormatException(ex, false));
        }
      }
    }

    private void OnTimer(object state) => this.Flush();

    private enum LogEntryLevel
    {
      Verbose,
      Info,
      Warning,
      Error,
    }
  }
}
