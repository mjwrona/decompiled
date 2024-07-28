// Decompiled with JetBrains decompiler
// Type: Nest.SlackDynamicAttachmentDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SlackDynamicAttachmentDescriptor : 
    DescriptorBase<SlackDynamicAttachmentDescriptor, ISlackDynamicAttachment>,
    ISlackDynamicAttachment
  {
    ISlackAttachment ISlackDynamicAttachment.AttachmentTemplate { get; set; }

    string ISlackDynamicAttachment.ListPath { get; set; }

    public SlackDynamicAttachmentDescriptor ListPath(string listPath) => this.Assign<string>(listPath, (Action<ISlackDynamicAttachment, string>) ((a, v) => a.ListPath = v));

    public SlackDynamicAttachmentDescriptor AttachmentTemplate(
      Func<SlackAttachmentDescriptor, ISlackAttachment> selector)
    {
      return this.Assign<Func<SlackAttachmentDescriptor, ISlackAttachment>>(selector, (Action<ISlackDynamicAttachment, Func<SlackAttachmentDescriptor, ISlackAttachment>>) ((a, v) => a.AttachmentTemplate = v != null ? v(new SlackAttachmentDescriptor()) : (ISlackAttachment) null));
    }
  }
}
