// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationTraceListener
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationTraceListener : DefaultTraceListener
  {
    internal static TeamFoundationTraceListener Instance = new TeamFoundationTraceListener();
    private bool m_launchDebugger = true;
    private bool m_useTraceWriter;
    private bool m_useAspNetTracing;
    private static bool s_assertUiEnabled;
    private static string s_teamFoundationComponent;
    private TextWriterTraceListener m_traceWriter;
    private static string s_traceDirectoryName;
    private const string s_defaultTraceDirectoryName = "%TEMP%\\TFLogfiles";
    private const string s_isTracingConfigKey = "traceWriter";
    private const string s_traceDirectoryConfigKey = "traceDirectoryName";

    internal static void SetDefaultListener(string teamFoundationComponent) => TeamFoundationTraceListener.InitializeTraceListener(teamFoundationComponent);

    public override void Fail(string message) => this.Fail(message, (string) null);

    public override void Fail(string message, string detailedMessage)
    {
    }

    public override void Write(string message)
    {
      if (!string.IsNullOrEmpty(TeamFoundationTraceListener.TeamFoundationComponent))
        this.OutputMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}] {1}", (object) TeamFoundationTraceListener.TeamFoundationComponent, (object) message), false);
      else
        this.OutputMessage(message, false);
    }

    public override void WriteLine(string message)
    {
      if (!string.IsNullOrEmpty(TeamFoundationTraceListener.TeamFoundationComponent))
        this.OutputMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}] {1}", (object) TeamFoundationTraceListener.TeamFoundationComponent, (object) message), true);
      else
        this.OutputMessage(message, true);
    }

    private static void InitializeTraceListener(string component)
    {
      for (int index = Trace.Listeners.Count - 1; index >= 0; --index)
      {
        if (Trace.Listeners[index] is TeamFoundationTraceListener)
          Trace.Listeners.RemoveAt(index);
        else if (Trace.Listeners[index] is DefaultTraceListener)
        {
          TeamFoundationTraceListener.s_assertUiEnabled = ((DefaultTraceListener) Trace.Listeners[index]).AssertUiEnabled;
          Trace.Listeners.RemoveAt(index);
        }
      }
      Trace.Listeners.Insert(0, (TraceListener) TeamFoundationTraceListener.Instance);
      TeamFoundationTraceListener.TeamFoundationComponent = component;
      bool useTraceWriter = false;
      string appSetting = ConfigurationManager.AppSettings["traceWriter"];
      if (!string.IsNullOrEmpty(appSetting))
        useTraceWriter = bool.Parse(appSetting);
      TeamFoundationTraceListener.SetTraceWriterState(useTraceWriter);
    }

    internal static void SetTraceWriterState(bool useTraceWriter) => TeamFoundationTraceListener.Instance.SetUseTraceWriter(useTraceWriter);

    private void SetUseTraceWriter(bool useTraceWriter)
    {
      if (useTraceWriter != this.m_useTraceWriter)
      {
        if (useTraceWriter)
          this.EnableTraceWriter();
        else
          this.DisableTraceWriter();
      }
      this.m_useTraceWriter = useTraceWriter;
    }

    [Conditional("DEBUG")]
    private void OutputFailureMessage(string message, string detailedMessage) => this.OutputMessage(message, detailedMessage, true, true);

    private void DisableTraceWriter()
    {
      if (TeamFoundationTraceListener.Instance.TraceWriter == null)
        return;
      TeamFoundationTraceListener.Instance.OutputMessage(TFCommonResources.TraceStopMessage((object) TeamFoundationTraceListener.TeamFoundationComponent, (object) DateTime.UtcNow.ToString((IFormatProvider) DateTimeFormatInfo.InvariantInfo)), true);
      TeamFoundationTraceListener.Instance.TraceWriter.Dispose();
      TeamFoundationTraceListener.Instance.TraceWriter = (TextWriterTraceListener) null;
    }

    private void EnableTraceWriter()
    {
      TeamFoundationTextWriterTraceListener.TraceFileDirectoryName = TeamFoundationTraceListener.TraceDirectoryName;
      TeamFoundationTraceListener.Instance.TraceWriter = (TextWriterTraceListener) new TeamFoundationTextWriterTraceListener();
      TeamFoundationTraceListener.Instance.UseAspNetTracing = true;
      TeamFoundationTraceListener.Instance.OutputMessage(TFCommonResources.TraceStartMessage((object) TeamFoundationTraceListener.TeamFoundationComponent, (object) DateTime.UtcNow.ToString((IFormatProvider) DateTimeFormatInfo.InvariantInfo)), true);
      TeamFoundationTraceListener.Instance.OutputMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "# Trace listener: {0}", (object) TeamFoundationTraceListener.Instance.TraceWriter.ToString()), true);
      TeamFoundationTraceListener instance = TeamFoundationTraceListener.Instance;
      CultureInfo invariantCulture = CultureInfo.InvariantCulture;
      string str1 = TeamFoundationTraceListener.s_assertUiEnabled.ToString();
      bool flag = TeamFoundationTraceListener.Instance.LaunchDebugger;
      string str2 = flag.ToString();
      flag = TeamFoundationTraceListener.Instance.UseAspNetTracing;
      string str3 = flag.ToString();
      string message = string.Format((IFormatProvider) invariantCulture, "# Config settings: Assert UI Enabled={0} Launch Debugger={1} ASP.NET={2}", (object) str1, (object) str2, (object) str3);
      instance.OutputMessage(message, true);
    }

    private TeamFoundationTraceListener.FrameInformation FrameDetails(StackFrame frame)
    {
      MethodBase method = frame.GetMethod();
      if (method == (MethodBase) null)
        return (TeamFoundationTraceListener.FrameInformation) null;
      string str = string.Empty;
      if (method.ReflectedType != (Type) null)
        str = method.ReflectedType.Name;
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.Append(TFCommonResources.StackFrameHeader((object) str, (object) method.Name));
      stringBuilder1.Append("(");
      ParameterInfo[] parameters = method.GetParameters();
      for (int index = 0; index < parameters.Length; ++index)
      {
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) parameters[index].ParameterType.Name, (object) parameters[index].Name));
        if (parameters.Length - 1 != index)
          stringBuilder1.Append(", ");
      }
      stringBuilder1.Append(") ");
      StringBuilder stringBuilder2 = new StringBuilder();
      if (frame.GetFileName() != null)
      {
        if (frame.GetFileLineNumber() == 0)
          stringBuilder2.Append(frame.GetFileName());
        else
          stringBuilder2.Append(TFCommonResources.StackFrameLineFormat((object) frame.GetFileName(), (object) frame.GetFileLineNumber().ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      }
      stringBuilder2.Append(Environment.NewLine);
      return new TeamFoundationTraceListener.FrameInformation(stringBuilder2.ToString(), stringBuilder1.ToString());
    }

    private void OutputMessage(string message, bool includeNewline) => this.OutputMessage(message, includeNewline, (string) null);

    private void OutputMessage(string message, bool includeNewline, string detailedMessage) => this.OutputMessage(message, detailedMessage, includeNewline, false);

    private bool IsLocalRequest
    {
      get
      {
        if (HttpContext.Current == null)
          return false;
        HttpRequest request = HttpContext.Current.Request;
        return request.UserHostAddress.Equals("127.0.0.1") || request.UserHostAddress.Equals(request.ServerVariables.Get("LOCAL_ADDR"));
      }
    }

    private List<TeamFoundationTraceListener.FrameInformation> StackInformation
    {
      get
      {
        List<TeamFoundationTraceListener.FrameInformation> stackInformation = new List<TeamFoundationTraceListener.FrameInformation>();
        StackTrace stackTrace = new StackTrace(2, true);
        for (int index = 0; index < stackTrace.FrameCount; ++index)
        {
          TeamFoundationTraceListener.FrameInformation frameInformation = this.FrameDetails(stackTrace.GetFrame(index));
          if (frameInformation != null)
            stackInformation.Add(frameInformation);
        }
        return stackInformation;
      }
    }

    private void OutputMessage(
      string message,
      string detailedMessage,
      bool includeNewLine,
      bool isAssert)
    {
      StringBuilder stringBuilder = new StringBuilder(message);
      if (includeNewLine)
        stringBuilder.Append(Environment.NewLine);
      if (!string.IsNullOrEmpty(detailedMessage))
        stringBuilder.AppendLine(detailedMessage);
      if (isAssert)
      {
        stringBuilder.AppendLine("-----------------------");
        stringBuilder.AppendLine(TFCommonResources.AssertionFailureHeader());
        stringBuilder.AppendLine("-----------------------");
        foreach (TeamFoundationTraceListener.FrameInformation frameInformation in this.StackInformation)
        {
          if (!string.IsNullOrEmpty(frameInformation.Method))
            stringBuilder.Append(frameInformation.Method);
          if (!string.IsNullOrEmpty(frameInformation.FrameDetails))
            stringBuilder.Append(frameInformation.FrameDetails);
        }
      }
      if (Debugger.IsLogging())
        Debugger.Log(0, (string) null, stringBuilder.ToString());
      TeamFoundationTraceListener.OutputDebugString(stringBuilder.ToString());
      if (this.m_traceWriter != null)
      {
        if (includeNewLine)
          this.m_traceWriter.WriteLine(stringBuilder.ToString());
        else
          this.m_traceWriter.Write(stringBuilder.ToString());
        this.m_traceWriter.Flush();
      }
      foreach (TraceListener listener in Trace.Listeners)
      {
        if (!(listener is TeamFoundationTraceListener))
        {
          try
          {
            if (includeNewLine)
              listener.WriteLine(stringBuilder.ToString());
            else
              listener.Write(stringBuilder.ToString());
          }
          catch
          {
          }
        }
      }
      if (this.m_useAspNetTracing && HttpContext.Current != null)
        HttpContext.Current.Trace.Write(stringBuilder.ToString());
      if (!isAssert)
        return;
      bool flag = false;
      if (this.m_launchDebugger)
      {
        try
        {
          if (Debugger.IsAttached)
          {
            Debugger.Break();
            flag = true;
          }
          else
          {
            if (!Environment.UserInteractive)
            {
              if (TeamFoundationTraceListener.s_assertUiEnabled)
              {
                if (!this.IsLocalRequest)
                  goto label_44;
              }
              else
                goto label_44;
            }
            flag = Debugger.Launch();
          }
        }
        catch (SecurityException ex)
        {
        }
      }
label_44:
      if (!flag)
        throw new TeamFoundationServerException(stringBuilder.ToString());
    }

    internal bool EnableUiAssert
    {
      get => TeamFoundationTraceListener.s_assertUiEnabled;
      set => TeamFoundationTraceListener.s_assertUiEnabled = value;
    }

    internal static string TeamFoundationComponent
    {
      get => string.IsNullOrEmpty(TeamFoundationTraceListener.s_teamFoundationComponent) ? string.Empty : TeamFoundationTraceListener.s_teamFoundationComponent;
      set => TeamFoundationTraceListener.s_teamFoundationComponent = value;
    }

    internal TextWriterTraceListener TraceWriter
    {
      get => this.m_traceWriter;
      set => this.m_traceWriter = value;
    }

    internal bool LaunchDebugger
    {
      get => this.m_launchDebugger;
      set => this.m_launchDebugger = value;
    }

    internal bool UseAspNetTracing
    {
      get => this.m_useAspNetTracing;
      set => this.m_useAspNetTracing = value;
    }

    private static string TraceDirectoryName
    {
      get
      {
        if (string.IsNullOrEmpty(TeamFoundationTraceListener.s_traceDirectoryName))
        {
          string appSetting = ConfigurationManager.AppSettings["traceDirectoryName"];
          TeamFoundationTraceListener.s_traceDirectoryName = string.IsNullOrEmpty(appSetting) ? "%TEMP%\\TFLogfiles" : appSetting;
        }
        return TeamFoundationTraceListener.s_traceDirectoryName;
      }
    }

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    public static extern void OutputDebugString(string s);

    internal class FrameInformation
    {
      internal string Method;
      internal string FrameDetails;

      internal FrameInformation(string method, string frameDetails)
      {
        this.Method = method;
        this.FrameDetails = frameDetails;
      }
    }
  }
}
