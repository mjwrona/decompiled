// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.ArtifactServicesTraceSource
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  public abstract class ArtifactServicesTraceSource : TraceSource, IAppTraceSource
  {
    private const int DefaultId = 0;
    private readonly ErrorDetectionListener DetectionListener = new ErrorDetectionListener();

    protected ArtifactServicesTraceSource(string name)
      : base(name)
    {
    }

    public abstract IAppTraceSource Instance { get; }

    protected virtual string AppName
    {
      get
      {
        Assembly assembly = Assembly.GetEntryAssembly();
        if ((object) assembly == null)
          assembly = Assembly.GetExecutingAssembly();
        return Path.GetFileName(assembly.Location);
      }
    }

    public bool HasError => this.DetectionListener.HasError;

    public void ResetErrorDetection()
    {
      if (!this.Instance.Listeners.Contains((TraceListener) this.DetectionListener))
        this.Instance.Listeners.Add((TraceListener) this.DetectionListener);
      this.DetectionListener.Reset();
    }

    public IAppTraceSource SetSourceLevel(SourceLevels level)
    {
      this.Switch.Level = level;
      return (IAppTraceSource) this;
    }

    public SourceLevels SwitchLevel => this.Switch.Level;

    public void AddConsoleTraceListener() => this.AddTraceListener<ConsoleTraceListener>(new ConsoleTraceListener());

    public void AddFileTraceListener(string fullFileName) => this.AddTraceListener<FileTraceListener>(new FileTraceListener(fullFileName));

    public void AddTraceListener<T>(T listener) where T : AppTraceListener
    {
      if ((object) listener == null)
        throw new ArgumentNullException(nameof (listener), "Cannot add a null trace listener.");
      try
      {
        listener.AppName = this.AppName;
        this.Instance.Listeners.Add((TraceListener) listener);
      }
      catch (Exception ex)
      {
        object[] objArray = new object[1]
        {
          (object) typeof (T)
        };
        ConsoleMessageUtil.PrintErrorMessage(ArtifactServicesTraceSource.GetMessageAndExceptionText(ex, "Failed to create {0} type trace listener.", objArray));
      }
    }

    public virtual void Info(string format, params object[] args) => this.Info(0, format, args);

    public virtual void Info(int id, string format, params object[] args) => this.TraceEvent(TraceEventType.Information, id, format, args);

    public virtual void Warn(string format, params object[] args) => this.Warn(0, format, args);

    public virtual void Warn(int id, string format, params object[] args) => this.TraceEvent(TraceEventType.Warning, id, format, args);

    public virtual void Warn(Exception ex) => this.Warn(0, ex);

    public virtual void Warn(int id, Exception ex) => this.TraceEvent(TraceEventType.Warning, id, ArtifactServicesTraceSource.GetExceptionText(ex));

    public virtual void Warn(Exception ex, string format, params object[] args) => this.Warn(0, ex, format, args);

    public virtual void Warn(int id, Exception ex, string format, params object[] args)
    {
      this.TraceEvent(TraceEventType.Warning, id, format, args);
      this.Warn(id, ex);
    }

    public virtual void Error(string format, params object[] args) => this.Error(0, format, args);

    public virtual void Error(int id, string format, params object[] args) => this.TraceEvent(TraceEventType.Error, id, format, args);

    public virtual void Error(Exception ex) => this.Error(0, ex);

    public virtual void Error(int id, Exception ex) => this.TraceEvent(TraceEventType.Error, id, ArtifactServicesTraceSource.GetExceptionText(ex));

    public virtual void Error(Exception ex, string format, params object[] args) => this.Error(0, ex, format, args);

    public virtual void Error(int id, Exception ex, string format, params object[] args) => this.TraceEvent(TraceEventType.Error, id, ArtifactServicesTraceSource.GetMessageAndExceptionText(ex, format, args));

    public virtual void Critical(string format, params object[] args) => this.Critical(0, format, args);

    public virtual void Critical(int id, string format, params object[] args) => this.TraceEvent(TraceEventType.Critical, id, format, args);

    public virtual void Critical(Exception ex) => this.Critical(0, ex);

    public virtual void Critical(int id, Exception ex) => this.TraceEvent(TraceEventType.Critical, id, ArtifactServicesTraceSource.GetExceptionText(ex));

    public virtual void Critical(Exception ex, string format, params object[] args) => this.Critical(0, ex, format, args);

    public virtual void Critical(int id, Exception ex, string format, params object[] args) => this.TraceEvent(TraceEventType.Critical, id, ArtifactServicesTraceSource.GetMessageAndExceptionText(ex, format, args));

    public virtual void Verbose(string format, params object[] args) => this.Verbose(0, format, args);

    public virtual void Verbose(int id, string format, params object[] args) => this.TraceEvent(TraceEventType.Verbose, id, format, args);

    public virtual void Verbose(Exception ex) => this.Verbose(0, ex);

    public virtual void Verbose(int id, Exception ex) => this.TraceEvent(TraceEventType.Verbose, id, ex.ToString());

    public virtual void Verbose(Exception ex, string format, params object[] args) => this.Verbose(0, ex, format, args);

    public virtual void Verbose(int id, Exception ex, string format, params object[] args)
    {
      this.TraceEvent(TraceEventType.Verbose, id, format, args);
      this.Verbose(id, ex);
    }

    public new void TraceEvent(TraceEventType eventType, int id) => base.TraceEvent(eventType, id);

    public new void TraceEvent(TraceEventType eventType, int id, string message) => base.TraceEvent(eventType, id, message);

    public new void TraceEvent(
      TraceEventType eventType,
      int id,
      string format,
      params object[] args)
    {
      base.TraceEvent(eventType, id, format, args);
    }

    private static string GetMessageAndExceptionText(
      Exception ex,
      string format,
      params object[] args)
    {
      StringBuilder message = new StringBuilder();
      message.AppendFormatSafe(format, args);
      message.AppendLine();
      message.Append(ArtifactServicesTraceSource.GetExceptionText(ex));
      return message.ToString();
    }

    private static string GetExceptionText(Exception ex) => ex.ToString();

    TraceListenerCollection IAppTraceSource.get_Listeners() => this.Listeners;
  }
}
