// Decompiled with JetBrains decompiler
// Type: WebGrease.DelayedLogManager
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using WebGrease.Activities;
using WebGrease.Css.Extensions;
using WebGrease.Extensions;

namespace WebGrease
{
  internal class DelayedLogManager
  {
    private const string MessageFormat = "{0} {1:HH:mm:ss.ff} {2}";
    private readonly string messagePrefix;
    private readonly IList<Tuple<string, Action<string>>> actions = (IList<Tuple<string, Action<string>>>) new List<Tuple<string, Action<string>>>();
    private readonly object flushLock = new object();
    private bool isFlushed;

    public DelayedLogManager(LogManager syncLogManager, string messagePrefix = null)
    {
      DelayedLogManager delayedLogManager = this;
      this.messagePrefix = messagePrefix;
      this.LogManager = new LogManager((Action<string, MessageImportance>) ((m, importance) => delayedLogManager.AddTimedAction(m, (Action<string>) (message => syncLogManager.Information(message, importance)))), (Action<string>) (m => delayedLogManager.AddTimedAction(m, new Action<string>(syncLogManager.Warning))), (LogExtendedError) ((subcategory, code, keyword, file, number, columnNumber, lineNumber, endColumnNumber, m) => delayedLogManager.AddTimedAction(m, (Action<string>) (message => syncLogManager.Warning(subcategory, code, keyword, file, number, columnNumber, lineNumber, endColumnNumber, message)))), (Action<string>) (m => delayedLogManager.AddTimedAction(m, new Action<string>(syncLogManager.Error))), (LogError) ((exception, m, name) => delayedLogManager.AddTimedAction(m, (Action<string>) (message => syncLogManager.Error(exception, message, name)))), (LogExtendedError) ((subcategory, code, keyword, file, number, columnNumber, lineNumber, endColumnNumber, m) => delayedLogManager.AddTimedAction(m, (Action<string>) (message => syncLogManager.Error(subcategory, code, keyword, file, number, columnNumber, lineNumber, endColumnNumber, message)))));
    }

    public LogManager LogManager { get; private set; }

    public void Flush()
    {
      if (this.isFlushed)
        return;
      Safe.Lock(this.flushLock, (Action) (() =>
      {
        if (this.isFlushed)
          return;
        this.isFlushed = true;
        this.actions.ForEach<Tuple<string, Action<string>>>((Action<Tuple<string, Action<string>>>) (a => a.Item2(a.Item1)));
        this.actions.Clear();
      }));
    }

    private void AddTimedAction(string message, Action<string> action)
    {
      string formattedMessage = "{0} {1:HH:mm:ss.ff} {2}".InvariantFormat((object) this.messagePrefix, (object) DateTime.Now, (object) message);
      Safe.Lock(this.flushLock, (Action) (() =>
      {
        if (this.isFlushed)
          action(formattedMessage);
        else
          this.actions.Add(Tuple.Create<string, Action<string>>(formattedMessage, action));
      }));
    }
  }
}
