// Decompiled with JetBrains decompiler
// Type: Nest.LoggingActionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class LoggingActionDescriptor : 
    ActionsDescriptorBase<LoggingActionDescriptor, ILoggingAction>,
    ILoggingAction,
    IAction
  {
    public LoggingActionDescriptor(string name)
      : base(name)
    {
    }

    protected override ActionType ActionType => ActionType.Logging;

    string ILoggingAction.Category { get; set; }

    LogLevel? ILoggingAction.Level { get; set; }

    string ILoggingAction.Text { get; set; }

    public LoggingActionDescriptor Level(LogLevel? level) => this.Assign<LogLevel?>(level, (Action<ILoggingAction, LogLevel?>) ((a, v) => a.Level = v));

    public LoggingActionDescriptor Text(string text) => this.Assign<string>(text, (Action<ILoggingAction, string>) ((a, v) => a.Text = v));

    public LoggingActionDescriptor Category(string category) => this.Assign<string>(category, (Action<ILoggingAction, string>) ((a, v) => a.Category = v));
  }
}
