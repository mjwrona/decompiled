// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.Logging.AggregateLogger
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement.Logging
{
  internal class AggregateLogger : ILogger
  {
    private readonly ReaderWriterLockSlim loggersLock = new ReaderWriterLockSlim();
    private readonly IList<ILogger> loggers = (IList<ILogger>) new List<ILogger>();

    ~AggregateLogger() => this.loggersLock.Dispose();

    public void LogEvent(string name, IDictionary<string, object> properties)
    {
      this.loggersLock.EnterReadLock();
      try
      {
        foreach (ILogger logger in (IEnumerable<ILogger>) this.loggers)
        {
          try
          {
            logger.LogEvent(name, properties);
          }
          catch
          {
          }
        }
      }
      catch
      {
      }
      finally
      {
        this.loggersLock.ExitReadLock();
      }
    }

    public void Add(ILogger logger) => this.ModifyLoggers(logger, (Action) (() =>
    {
      if (this.loggers.Contains(logger))
        return;
      this.loggers.Add(logger);
    }));

    public void Remove(ILogger logger) => this.ModifyLoggers(logger, (Action) (() => this.loggers.Remove(logger)));

    internal void ClearLoggers() => this.ModifyLoggers((Action) (() => this.loggers.Clear()));

    private void ModifyLoggers(ILogger logger, Action action)
    {
      if (logger == null)
        return;
      this.ModifyLoggers(action);
    }

    private void ModifyLoggers(Action action)
    {
      this.loggersLock.EnterWriteLock();
      try
      {
        action();
      }
      catch
      {
      }
      finally
      {
        this.loggersLock.ExitWriteLock();
      }
    }
  }
}
