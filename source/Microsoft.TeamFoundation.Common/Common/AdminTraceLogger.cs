// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.AdminTraceLogger
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Common
{
  public class AdminTraceLogger : MarshalByRefObject, ITFLogger
  {
    private readonly TraceLevel m_traceLevel;
    private static AdminTraceLogger s_default;
    private const string c_traceFormat = "[{0,-7}@{1:HH}:{1:mm}:{1:ss}.{1:fff}] {2}";

    public AdminTraceLogger()
      : this(AdminTraceLogger.ReadTraceLevel())
    {
    }

    public AdminTraceLogger(TraceLevel traceLevel) => this.m_traceLevel = traceLevel;

    public static AdminTraceLogger Default => AdminTraceLogger.s_default ?? (AdminTraceLogger.s_default = new AdminTraceLogger());

    public TraceLevel TraceLevel => this.m_traceLevel;

    public void Verbose(string message) => this.WriteLine(TraceLevel.Verbose, message);

    public void Verbose(string format, params object[] args) => this.WriteLine(TraceLevel.Verbose, format, args);

    public void Info(string message) => this.WriteLine(TraceLevel.Info, message);

    public void Info(string format, params object[] args) => this.WriteLine(TraceLevel.Info, format, args);

    public void Warning(string message) => this.WriteLine(TraceLevel.Warning, message);

    public void Warning(string format, params object[] args) => this.WriteLine(TraceLevel.Warning, format, args);

    public void Warning(Exception exception)
    {
      if (exception == null)
        return;
      this.Warning("---------------------------------------------------------------------------");
      this.WriteMultipleLines(TraceLevel.Warning, TeamFoundationExceptionFormatter.FormatException(exception, false));
      this.Warning("---------------------------------------------------------------------------");
    }

    public void Error(string message) => this.WriteLine(TraceLevel.Error, message);

    public void Error(string format, params object[] args) => this.WriteLine(TraceLevel.Error, format, args);

    public void Error(Exception exception)
    {
      if (exception == null)
        return;
      this.Error("---------------------------------------------------------------------------");
      this.WriteMultipleLines(TraceLevel.Error, TeamFoundationExceptionFormatter.FormatException(exception, false));
      this.Error("---------------------------------------------------------------------------");
    }

    [Conditional("DEBUG")]
    public void Debug(string message) => Trace.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0,-7}@{1:HH}:{1:mm}:{1:ss}.{1:fff}] {2}", (object) nameof (Debug), (object) DateTime.UtcNow, (object) message));

    [Conditional("DEBUG")]
    public void Debug(string format, params object[] args)
    {
    }

    [Conditional("DEBUG")]
    public void RegisterPassword(string password)
    {
    }

    protected virtual void WriteLine(TraceLevel level, string message, params object[] args)
    {
      if (level > this.m_traceLevel)
        return;
      this.WriteLine(level, string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));
    }

    public virtual void WriteLine(TraceLevel traceLevel, string message)
    {
      try
      {
        string message1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0,-7}@{1:HH}:{1:mm}:{1:ss}.{1:fff}] {2}", (object) EnumUtility.ToString<TraceLevel>(traceLevel), (object) DateTime.UtcNow, (object) message);
        if (traceLevel > this.m_traceLevel)
          return;
        Trace.WriteLine(message1);
      }
      catch
      {
      }
    }

    public void Heading(string message)
    {
      this.Info(string.Empty);
      this.Info("-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
      this.Info(message);
      this.Info("-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
    }

    public void Heading2(string message) => this.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-+-+-+-+-+ {0} -+-+-+-+-+", (object) message));

    public void Enter(string message) => this.WriteLine(TraceLevel.Verbose, "--> {0}", (object) message);

    public void Exit(string message) => this.WriteLine(TraceLevel.Verbose, "<-- {0}", (object) message);

    private void WriteMultipleLines(TraceLevel level, string message)
    {
      if (string.IsNullOrEmpty(message))
        return;
      string str = message;
      char[] chArray = new char[1]{ '\n' };
      foreach (string message1 in str.Split(chArray))
        this.WriteLine(level, message1);
    }

    private static TraceLevel ReadTraceLevel()
    {
      TraceLevel traceLevel = TraceLevel.Off;
      try
      {
        using (SafeHandle registryKey = RegistryHelper.OpenSubKey(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\TeamFoundationServer", RegistryAccessMask.Execute | RegistryAccessMask.Wow6464Key))
        {
          if (registryKey != null)
            traceLevel = (TraceLevel) RegistryHelper.GetValue(registryKey, "AdminTraceLevel", (object) 0);
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine("[Error]Cannot read trace level from registry. {0}", TeamFoundationExceptionFormatter.FormatException(ex, false));
      }
      return traceLevel;
    }
  }
}
