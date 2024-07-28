// Decompiled with JetBrains decompiler
// Type: Nest.SlackAttachmentFieldDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SlackAttachmentFieldDescriptor : 
    DescriptorBase<SlackAttachmentFieldDescriptor, ISlackAttachmentField>,
    ISlackAttachmentField
  {
    bool? ISlackAttachmentField.Short { get; set; }

    string ISlackAttachmentField.Title { get; set; }

    string ISlackAttachmentField.Value { get; set; }

    public SlackAttachmentFieldDescriptor Title(string title) => this.Assign<string>(title, (Action<ISlackAttachmentField, string>) ((a, v) => a.Title = v));

    public SlackAttachmentFieldDescriptor Value(string value) => this.Assign<string>(value, (Action<ISlackAttachmentField, string>) ((a, v) => a.Value = v));

    public SlackAttachmentFieldDescriptor Short(bool? @short = true) => this.Assign<bool?>(@short, (Action<ISlackAttachmentField, bool?>) ((a, v) => a.Short = v));
  }
}
