// Decompiled with JetBrains decompiler
// Type: Nest.SlackAttachmentsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class SlackAttachmentsDescriptor : 
    DescriptorPromiseBase<SlackAttachmentsDescriptor, IList<ISlackAttachment>>
  {
    public SlackAttachmentsDescriptor()
      : base((IList<ISlackAttachment>) new List<ISlackAttachment>())
    {
    }

    public SlackAttachmentsDescriptor Attachment(
      Func<SlackAttachmentDescriptor, ISlackAttachment> selector)
    {
      return this.Assign<Func<SlackAttachmentDescriptor, ISlackAttachment>>(selector, (Action<IList<ISlackAttachment>, Func<SlackAttachmentDescriptor, ISlackAttachment>>) ((a, v) => a.AddIfNotNull<ISlackAttachment>(v != null ? v(new SlackAttachmentDescriptor()) : (ISlackAttachment) null)));
    }
  }
}
