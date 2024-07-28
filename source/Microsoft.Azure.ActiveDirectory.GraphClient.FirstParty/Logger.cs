// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.Logger
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using System;
using System.Diagnostics;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class Logger : ILogger
  {
    private static ILogger loggerInstance;

    public static bool WriteOnConsole { get; set; }

    public static ILogger Instance
    {
      get
      {
        if (Logger.loggerInstance == null)
          Logger.loggerInstance = (ILogger) new Logger();
        return Logger.loggerInstance;
      }
      set => Logger.loggerInstance = value;
    }

    public void Error(string message) => this.Error("{0}", new object[1]
    {
      (object) message
    });

    public void Error(string message, params object[] args)
    {
      Trace.TraceError(message, args);
      if (!Logger.WriteOnConsole)
        return;
      Console.WriteLine(message, args);
    }

    public void Warning(string message) => this.Warning("{0}", new object[1]
    {
      (object) message
    });

    public void Warning(string message, params object[] args)
    {
      Trace.TraceWarning(message, args);
      if (!Logger.WriteOnConsole)
        return;
      Console.WriteLine(message, args);
    }

    public void Info(string message) => this.Info("{0}", new object[1]
    {
      (object) message
    });

    public void Info(string message, params object[] args)
    {
      Trace.TraceInformation(message, args);
      if (!Logger.WriteOnConsole)
        return;
      Console.WriteLine(message, args);
    }
  }
}
