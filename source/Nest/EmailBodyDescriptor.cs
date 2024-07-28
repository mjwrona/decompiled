// Decompiled with JetBrains decompiler
// Type: Nest.EmailBodyDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class EmailBodyDescriptor : DescriptorBase<EmailBodyDescriptor, IEmailBody>, IEmailBody
  {
    string IEmailBody.Html { get; set; }

    string IEmailBody.Text { get; set; }

    public EmailBodyDescriptor Text(string text) => this.Assign<string>(text, (Action<IEmailBody, string>) ((a, v) => a.Text = v));

    public EmailBodyDescriptor Html(string html) => this.Assign<string>(html, (Action<IEmailBody, string>) ((a, v) => a.Html = v));
  }
}
