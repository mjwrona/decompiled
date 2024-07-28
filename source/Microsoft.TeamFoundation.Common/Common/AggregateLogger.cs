// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.AggregateLogger
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;

namespace Microsoft.TeamFoundation.Common
{
  public class AggregateLogger : ITFLogger, IDisposable
  {
    private readonly ITFLogger[] m_loggers;

    public AggregateLogger(params ITFLogger[] loggers) => this.m_loggers = loggers;

    public void Info(string message)
    {
      foreach (ITFLogger logger in this.m_loggers)
        logger.Info(message);
    }

    public void Info(string message, params object[] args)
    {
      foreach (ITFLogger logger in this.m_loggers)
        logger.Info(message, args);
    }

    public void Warning(string message)
    {
      foreach (ITFLogger logger in this.m_loggers)
        logger.Warning(message);
    }

    public void Warning(string message, params object[] args)
    {
      foreach (ITFLogger logger in this.m_loggers)
        logger.Warning(message, args);
    }

    public void Warning(Exception exception)
    {
      foreach (ITFLogger logger in this.m_loggers)
        logger.Warning(exception);
    }

    public void Error(string message)
    {
      foreach (ITFLogger logger in this.m_loggers)
        logger.Error(message);
    }

    public void Error(string message, params object[] args)
    {
      foreach (ITFLogger logger in this.m_loggers)
        logger.Error(message, args);
    }

    public void Error(Exception exception)
    {
      foreach (ITFLogger logger in this.m_loggers)
        logger.Error(exception);
    }

    public void Dispose()
    {
      foreach (ITFLogger logger in this.m_loggers)
      {
        if (logger is IDisposable disposable)
          disposable.Dispose();
      }
    }
  }
}
