// Decompiled with JetBrains decompiler
// Type: Nest.EmailAttachmentsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class EmailAttachmentsDescriptor : 
    DescriptorPromiseBase<EmailAttachmentsDescriptor, IEmailAttachments>
  {
    public EmailAttachmentsDescriptor()
      : base((IEmailAttachments) new EmailAttachments())
    {
    }

    public EmailAttachmentsDescriptor HttpAttachment(
      string name,
      Func<HttpAttachmentDescriptor, IHttpAttachment> selector)
    {
      this.PromisedValue.Add(name, selector != null ? (IEmailAttachment) selector(new HttpAttachmentDescriptor()) : (IEmailAttachment) null);
      return this;
    }

    public EmailAttachmentsDescriptor DataAttachment(
      string name,
      Func<DataAttachmentDescriptor, IDataAttachment> selector)
    {
      this.PromisedValue.Add(name, selector != null ? (IEmailAttachment) selector(new DataAttachmentDescriptor()) : (IEmailAttachment) null);
      return this;
    }
  }
}
