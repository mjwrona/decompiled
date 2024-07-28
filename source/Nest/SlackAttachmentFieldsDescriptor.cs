// Decompiled with JetBrains decompiler
// Type: Nest.SlackAttachmentFieldsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class SlackAttachmentFieldsDescriptor : 
    DescriptorPromiseBase<SlackAttachmentFieldsDescriptor, IList<ISlackAttachmentField>>
  {
    public SlackAttachmentFieldsDescriptor()
      : base((IList<ISlackAttachmentField>) new List<ISlackAttachmentField>())
    {
    }

    public SlackAttachmentFieldsDescriptor Field(
      Func<SlackAttachmentFieldDescriptor, ISlackAttachmentField> selector)
    {
      return this.Assign<Func<SlackAttachmentFieldDescriptor, ISlackAttachmentField>>(selector, (Action<IList<ISlackAttachmentField>, Func<SlackAttachmentFieldDescriptor, ISlackAttachmentField>>) ((a, v) => a.AddIfNotNull<ISlackAttachmentField>(v != null ? v(new SlackAttachmentFieldDescriptor()) : (ISlackAttachmentField) null)));
    }
  }
}
