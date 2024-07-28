// Decompiled with JetBrains decompiler
// Type: Nest.SlackAttachmentDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class SlackAttachmentDescriptor : 
    DescriptorBase<SlackAttachmentDescriptor, ISlackAttachment>,
    ISlackAttachment
  {
    string ISlackAttachment.AuthorIcon { get; set; }

    string ISlackAttachment.AuthorLink { get; set; }

    string ISlackAttachment.AuthorName { get; set; }

    string ISlackAttachment.Color { get; set; }

    string ISlackAttachment.Fallback { get; set; }

    IEnumerable<ISlackAttachmentField> ISlackAttachment.Fields { get; set; }

    string ISlackAttachment.Footer { get; set; }

    string ISlackAttachment.FooterIcon { get; set; }

    string ISlackAttachment.ImageUrl { get; set; }

    string ISlackAttachment.Pretext { get; set; }

    string ISlackAttachment.Text { get; set; }

    string ISlackAttachment.ThumbUrl { get; set; }

    string ISlackAttachment.Title { get; set; }

    string ISlackAttachment.TitleLink { get; set; }

    DateTimeOffset? ISlackAttachment.Ts { get; set; }

    public SlackAttachmentDescriptor Fallback(string fallback) => this.Assign<string>(fallback, (Action<ISlackAttachment, string>) ((a, v) => a.Fallback = v));

    public SlackAttachmentDescriptor Color(string color) => this.Assign<string>(color, (Action<ISlackAttachment, string>) ((a, v) => a.Color = v));

    public SlackAttachmentDescriptor Pretext(string pretext) => this.Assign<string>(pretext, (Action<ISlackAttachment, string>) ((a, v) => a.Pretext = v));

    public SlackAttachmentDescriptor AuthorName(string authorName) => this.Assign<string>(authorName, (Action<ISlackAttachment, string>) ((a, v) => a.AuthorName = v));

    public SlackAttachmentDescriptor AuthorIcon(string authorIcon) => this.Assign<string>(authorIcon, (Action<ISlackAttachment, string>) ((a, v) => a.AuthorIcon = v));

    public SlackAttachmentDescriptor Title(string title) => this.Assign<string>(title, (Action<ISlackAttachment, string>) ((a, v) => a.Title = v));

    public SlackAttachmentDescriptor TitleLink(string titleLink) => this.Assign<string>(titleLink, (Action<ISlackAttachment, string>) ((a, v) => a.TitleLink = v));

    public SlackAttachmentDescriptor Text(string text) => this.Assign<string>(text, (Action<ISlackAttachment, string>) ((a, v) => a.Text = v));

    public SlackAttachmentDescriptor Fields(
      Func<SlackAttachmentFieldsDescriptor, IPromise<IList<ISlackAttachmentField>>> selector)
    {
      return this.Assign<Func<SlackAttachmentFieldsDescriptor, IPromise<IList<ISlackAttachmentField>>>>(selector, (Action<ISlackAttachment, Func<SlackAttachmentFieldsDescriptor, IPromise<IList<ISlackAttachmentField>>>>) ((a, v) => a.Fields = v != null ? (IEnumerable<ISlackAttachmentField>) v(new SlackAttachmentFieldsDescriptor())?.Value : (IEnumerable<ISlackAttachmentField>) null));
    }

    public SlackAttachmentDescriptor ImageUrl(string url) => this.Assign<string>(url, (Action<ISlackAttachment, string>) ((a, v) => a.ImageUrl = v));

    public SlackAttachmentDescriptor ThumbUrl(string url) => this.Assign<string>(url, (Action<ISlackAttachment, string>) ((a, v) => a.ThumbUrl = v));

    public SlackAttachmentDescriptor Footer(string footer) => this.Assign<string>(footer, (Action<ISlackAttachment, string>) ((a, v) => a.Footer = v));

    public SlackAttachmentDescriptor FooterIcon(string footerIcon) => this.Assign<string>(footerIcon, (Action<ISlackAttachment, string>) ((a, v) => a.FooterIcon = v));

    public SlackAttachmentDescriptor Ts(DateTimeOffset? ts) => this.Assign<DateTimeOffset?>(ts, (Action<ISlackAttachment, DateTimeOffset?>) ((a, v) => a.Ts = v));
  }
}
