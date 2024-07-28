// Decompiled with JetBrains decompiler
// Type: Nest.SlackActionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SlackActionDescriptor : 
    ActionsDescriptorBase<SlackActionDescriptor, ISlackAction>,
    ISlackAction,
    IAction
  {
    public SlackActionDescriptor(string name)
      : base(name)
    {
    }

    protected override ActionType ActionType => ActionType.Slack;

    string ISlackAction.Account { get; set; }

    ISlackMessage ISlackAction.Message { get; set; }

    public SlackActionDescriptor Account(string account) => this.Assign<string>(account, (Action<ISlackAction, string>) ((a, v) => a.Account = v));

    public SlackActionDescriptor Message(
      Func<SlackMessageDescriptor, ISlackMessage> selector)
    {
      return this.Assign<ISlackMessage>(selector.InvokeOrDefault<SlackMessageDescriptor, ISlackMessage>(new SlackMessageDescriptor()), (Action<ISlackAction, ISlackMessage>) ((a, v) => a.Message = v));
    }
  }
}
