// Decompiled with JetBrains decompiler
// Type: Nest.ActionsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ActionsDescriptor : 
    IsADictionaryDescriptorBase<ActionsDescriptor, Actions, string, IAction>
  {
    public ActionsDescriptor()
      : base(new Actions())
    {
    }

    public ActionsDescriptor Email(string name, Func<EmailActionDescriptor, IEmailAction> selector) => this.Assign(name, (IAction) selector.InvokeOrDefault<EmailActionDescriptor, IEmailAction>(new EmailActionDescriptor(name)));

    public ActionsDescriptor Index(string name, Func<IndexActionDescriptor, IIndexAction> selector) => this.Assign(name, (IAction) selector.InvokeOrDefault<IndexActionDescriptor, IIndexAction>(new IndexActionDescriptor(name)));

    public ActionsDescriptor Logging(
      string name,
      Func<LoggingActionDescriptor, ILoggingAction> selector)
    {
      return this.Assign(name, (IAction) selector.InvokeOrDefault<LoggingActionDescriptor, ILoggingAction>(new LoggingActionDescriptor(name)));
    }

    public ActionsDescriptor PagerDuty(
      string name,
      Func<PagerDutyActionDescriptor, IPagerDutyAction> selector)
    {
      return this.Assign(name, (IAction) selector.InvokeOrDefault<PagerDutyActionDescriptor, IPagerDutyAction>(new PagerDutyActionDescriptor(name)));
    }

    public ActionsDescriptor Slack(string name, Func<SlackActionDescriptor, ISlackAction> selector) => this.Assign(name, (IAction) selector.InvokeOrDefault<SlackActionDescriptor, ISlackAction>(new SlackActionDescriptor(name)));

    public ActionsDescriptor Webhook(
      string name,
      Func<WebhookActionDescriptor, IWebhookAction> selector)
    {
      return this.Assign(name, (IAction) selector.InvokeOrDefault<WebhookActionDescriptor, IWebhookAction>(new WebhookActionDescriptor(name)));
    }
  }
}
