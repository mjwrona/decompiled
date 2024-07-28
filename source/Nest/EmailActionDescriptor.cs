// Decompiled with JetBrains decompiler
// Type: Nest.EmailActionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class EmailActionDescriptor : 
    ActionsDescriptorBase<EmailActionDescriptor, IEmailAction>,
    IEmailAction,
    IAction
  {
    public EmailActionDescriptor(string name)
      : base(name)
    {
    }

    protected override ActionType ActionType => ActionType.Email;

    string IEmailAction.Account { get; set; }

    IEmailAttachments IEmailAction.Attachments { get; set; }

    IEnumerable<string> IEmailAction.Bcc { get; set; }

    IEmailBody IEmailAction.Body { get; set; }

    IEnumerable<string> IEmailAction.Cc { get; set; }

    string IEmailAction.From { get; set; }

    EmailPriority? IEmailAction.Priority { get; set; }

    IEnumerable<string> IEmailAction.ReplyTo { get; set; }

    string IEmailAction.Subject { get; set; }

    IEnumerable<string> IEmailAction.To { get; set; }

    public EmailActionDescriptor Account(string account) => this.Assign<string>(account, (Action<IEmailAction, string>) ((a, v) => a.Account = v));

    public EmailActionDescriptor From(string from) => this.Assign<string>(from, (Action<IEmailAction, string>) ((a, v) => a.From = v));

    public EmailActionDescriptor To(IEnumerable<string> to) => this.Assign<IEnumerable<string>>(to, (Action<IEmailAction, IEnumerable<string>>) ((a, v) => a.To = v));

    public EmailActionDescriptor To(params string[] to) => this.Assign<string[]>(to, (Action<IEmailAction, string[]>) ((a, v) => a.To = (IEnumerable<string>) v));

    public EmailActionDescriptor Cc(IEnumerable<string> cc) => this.Assign<IEnumerable<string>>(cc, (Action<IEmailAction, IEnumerable<string>>) ((a, v) => a.Cc = v));

    public EmailActionDescriptor Cc(params string[] cc) => this.Assign<string[]>(cc, (Action<IEmailAction, string[]>) ((a, v) => a.Cc = (IEnumerable<string>) v));

    public EmailActionDescriptor Bcc(IEnumerable<string> bcc) => this.Assign<IEnumerable<string>>(bcc, (Action<IEmailAction, IEnumerable<string>>) ((a, v) => a.Bcc = v));

    public EmailActionDescriptor Bcc(params string[] bcc) => this.Assign<string[]>(bcc, (Action<IEmailAction, string[]>) ((a, v) => a.Bcc = (IEnumerable<string>) v));

    public EmailActionDescriptor ReplyTo(IEnumerable<string> replyTo) => this.Assign<IEnumerable<string>>(replyTo, (Action<IEmailAction, IEnumerable<string>>) ((a, v) => a.ReplyTo = v));

    public EmailActionDescriptor ReplyTo(params string[] replyTo) => this.Assign<string[]>(replyTo, (Action<IEmailAction, string[]>) ((a, v) => a.ReplyTo = (IEnumerable<string>) v));

    public EmailActionDescriptor Subject(string subject) => this.Assign<string>(subject, (Action<IEmailAction, string>) ((a, v) => a.Subject = v));

    public EmailActionDescriptor Body(Func<EmailBodyDescriptor, IEmailBody> selector) => this.Assign<IEmailBody>(selector.InvokeOrDefault<EmailBodyDescriptor, IEmailBody>(new EmailBodyDescriptor()), (Action<IEmailAction, IEmailBody>) ((a, v) => a.Body = v));

    public EmailActionDescriptor Priority(EmailPriority? priority) => this.Assign<EmailPriority?>(priority, (Action<IEmailAction, EmailPriority?>) ((a, v) => a.Priority = v));

    public EmailActionDescriptor Attachments(
      Func<EmailAttachmentsDescriptor, IPromise<IEmailAttachments>> selector)
    {
      return this.Assign<Func<EmailAttachmentsDescriptor, IPromise<IEmailAttachments>>>(selector, (Action<IEmailAction, Func<EmailAttachmentsDescriptor, IPromise<IEmailAttachments>>>) ((a, v) => a.Attachments = v != null ? v(new EmailAttachmentsDescriptor())?.Value : (IEmailAttachments) null));
    }
  }
}
