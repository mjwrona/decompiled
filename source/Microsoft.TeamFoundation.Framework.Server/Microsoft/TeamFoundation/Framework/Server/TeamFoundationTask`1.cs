// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationTask`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationTask<T> : IEquatable<TeamFoundationTask<T>>
  {
    public TeamFoundationTask(
      TeamFoundationTaskCallback callback,
      object taskArgs,
      DateTime startTime,
      int interval)
    {
      ArgumentUtility.CheckForNull<TeamFoundationTaskCallback>(callback, nameof (callback));
      ArgumentUtility.CheckForOutOfRange(interval, nameof (interval), 0);
      this.Callback = callback;
      this.TaskArgs = taskArgs;
      this.StartTime = startTime.ToUniversalTime();
      this.Interval = interval;
    }

    public TeamFoundationTaskCallback Callback { get; private set; }

    public object TaskArgs { get; private set; }

    public DateTime StartTime { get; private set; }

    public T Identifier { get; internal set; }

    public int Interval { get; private set; }

    public bool ServicingContext { get; internal set; }

    internal string CreatorStackTrace { get; set; }

    internal bool KeepStartTimeOnRequeue { get; set; }

    internal DateTime GetNextRunTime(bool requeue = false)
    {
      if (!(this.StartTime <= DateTime.UtcNow))
        return this.StartTime;
      return requeue ? DateTime.UtcNow.Add(TimeSpan.FromMilliseconds((double) this.Interval)) : DateTime.UtcNow;
    }

    public override bool Equals(object obj) => obj is TeamFoundationTask<T> task && this.Equals(task);

    public bool Equals(TeamFoundationTask<T> task) => object.Equals((object) this.Identifier, (object) task.Identifier) && (task.Callback == this.Callback || task.Callback.Method == this.Callback.Method && task.Callback.Target == this.Callback.Target) && task.TaskArgs == this.TaskArgs;

    public virtual bool NeedsTargetRequestContext => false;

    public virtual bool IsHighPriority => false;

    public virtual IVssRequestContext GetRequestContext(
      IVssRequestContext deploymentContext,
      T identifier)
    {
      throw new NotImplementedException();
    }

    public override int GetHashCode() => ((object) this.Identifier == null ? 0 : this.Identifier.GetHashCode()) + this.Callback.Method.GetHashCode() + (this.Callback.Target == null ? 0 : this.Callback.Target.GetHashCode()) + (this.TaskArgs == null ? 0 : this.TaskArgs.GetHashCode());

    public override string ToString()
    {
      if (this.Identifier is string connectionString)
        connectionString = ConnectionStringUtility.MaskPassword(connectionString);
      else if ((object) this.Identifier != null)
        connectionString = this.Identifier.ToString();
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Task:\r\nIdentifier: {0}\r\nCallback: {1}\r\nStart Time: {2}\r\nInterval: {3}\r\nCreator Stack Trace: {4}\r\n", (object) connectionString, (object) this.Callback.Method, (object) this.StartTime, (object) this.Interval, (object) (this.CreatorStackTrace ?? string.Empty));
    }
  }
}
