// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TeamFoundationTextWriterTraceListener
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation
{
  public class TeamFoundationTextWriterTraceListener : TextWriterTraceListener
  {
    private static readonly string s_fileDateFormat = "yyyyMMdd.HHmm";
    private static string s_traceFileDirectoryName;
    private FileStream m_stream;
    private string m_filename;
    private bool m_startingNewLine = true;

    public TeamFoundationTextWriterTraceListener(string fileName) => this.InitializeFile(fileName);

    public TeamFoundationTextWriterTraceListener()
      : this(Path.Combine(Environment.ExpandEnvironmentVariables(TeamFoundationTextWriterTraceListener.TraceFileDirectoryName), TeamFoundationTextWriterTraceListener.NewTraceLogFilename))
    {
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (this.m_stream == null)
        return;
      this.m_stream.Dispose();
      this.m_stream = (FileStream) null;
    }

    public override void Write(string message) => this.WriteInternal(message, false);

    public override void WriteLine(string message) => this.WriteInternal(message, true);

    private void InitializeFile(string filename)
    {
      try
      {
        string directoryName = Path.GetDirectoryName(filename);
        if (!Directory.Exists(directoryName))
          Directory.CreateDirectory(directoryName);
        if (this.m_stream != null)
        {
          this.m_stream.Dispose();
          this.m_stream = (FileStream) null;
        }
        this.m_filename = Environment.ExpandEnvironmentVariables(filename);
        this.m_stream = new FileStream(this.m_filename, FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
        this.Writer = TextWriter.Synchronized((TextWriter) new StreamWriter((Stream) this.m_stream));
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine(TFCommonResources.InvalidTraceFile((object) filename, (object) ex.Message));
        Console.Error.WriteLine();
      }
    }

    private void WriteInternal(string message, bool appendNewLine)
    {
      if (this.m_stream == null)
        return;
      this.m_stream.Seek(0L, SeekOrigin.End);
      if (this.m_startingNewLine && message.Length > 0 && message[0] != '[')
        message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[, PID {0}, TID {1}, {2:yyyy}/{2:MM}/{2:dd} {2:HH}:{2:mm}:{2:ss}.{2:fff}] {3}", (object) TFCommonUtil.CurrentProcess.Id, (object) TeamFoundationTextWriterTraceListener.GetCurrentThreadId(), (object) DateTime.UtcNow, (object) message);
      if (appendNewLine)
        base.WriteLine(message);
      else
        base.Write(message);
      this.Flush();
      this.m_startingNewLine = appendNewLine;
    }

    public static string TraceFileDirectoryName
    {
      get => string.IsNullOrEmpty(TeamFoundationTextWriterTraceListener.s_traceFileDirectoryName) ? "%TEMP%" : TeamFoundationTextWriterTraceListener.s_traceFileDirectoryName;
      set => TeamFoundationTextWriterTraceListener.s_traceFileDirectoryName = value;
    }

    public string TraceFileName => this.m_stream == null ? (string) null : this.m_filename;

    private static string NewTraceLogFilename => "TFS-" + DateTime.UtcNow.ToString(TeamFoundationTextWriterTraceListener.s_fileDateFormat, (IFormatProvider) CultureInfo.InvariantCulture) + ".log";

    public override void Fail(string message, string detailedMessage) => this.WriteLine("[FAIL] " + message + Environment.NewLine + detailedMessage + (object) new StackTrace(true));

    [DllImport("kernel32")]
    private static extern int GetCurrentThreadId();
  }
}
