// Decompiled with JetBrains decompiler
// Type: Nest.SlackMessageDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class SlackMessageDescriptor : 
    DescriptorBase<SlackMessageDescriptor, ISlackMessage>,
    ISlackMessage
  {
    IEnumerable<ISlackAttachment> ISlackMessage.Attachments { get; set; }

    ISlackDynamicAttachment ISlackMessage.DynamicAttachments { get; set; }

    string ISlackMessage.From { get; set; }

    string ISlackMessage.Icon { get; set; }

    string ISlackMessage.Text { get; set; }

    IEnumerable<string> ISlackMessage.To { get; set; }

    public SlackMessageDescriptor From(string from) => this.Assign<string>(from, (Action<ISlackMessage, string>) ((a, v) => a.From = v));

    public SlackMessageDescriptor To(IEnumerable<string> to) => this.Assign<IEnumerable<string>>(to, (Action<ISlackMessage, IEnumerable<string>>) ((a, v) => a.To = v));

    public SlackMessageDescriptor To(params string[] to) => this.Assign<string[]>(to, (Action<ISlackMessage, string[]>) ((a, v) => a.To = (IEnumerable<string>) v));

    public SlackMessageDescriptor Icon(string icon) => this.Assign<string>(icon, (Action<ISlackMessage, string>) ((a, v) => a.Icon = v));

    public SlackMessageDescriptor Text(string text) => this.Assign<string>(text, (Action<ISlackMessage, string>) ((a, v) => a.Text = v));

    public SlackMessageDescriptor Attachments(
      Func<SlackAttachmentsDescriptor, IPromise<IList<ISlackAttachment>>> selector)
    {
      return this.Assign<Func<SlackAttachmentsDescriptor, IPromise<IList<ISlackAttachment>>>>(selector, (Action<ISlackMessage, Func<SlackAttachmentsDescriptor, IPromise<IList<ISlackAttachment>>>>) ((a, v) => a.Attachments = v != null ? (IEnumerable<ISlackAttachment>) v(new SlackAttachmentsDescriptor())?.Value : (IEnumerable<ISlackAttachment>) null));
    }

    public SlackMessageDescriptor DynamicAttachments(
      Func<SlackDynamicAttachmentDescriptor, ISlackDynamicAttachment> selector)
    {
      return this.Assign<Func<SlackDynamicAttachmentDescriptor, ISlackDynamicAttachment>>(selector, (Action<ISlackMessage, Func<SlackDynamicAttachmentDescriptor, ISlackDynamicAttachment>>) ((a, v) => a.DynamicAttachments = v != null ? v(new SlackDynamicAttachmentDescriptor()) : (ISlackDynamicAttachment) null));
    }
  }
}
