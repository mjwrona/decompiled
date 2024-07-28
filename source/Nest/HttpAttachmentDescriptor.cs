// Decompiled with JetBrains decompiler
// Type: Nest.HttpAttachmentDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class HttpAttachmentDescriptor : 
    DescriptorBase<HttpAttachmentDescriptor, IHttpAttachment>,
    IHttpAttachment,
    IEmailAttachment
  {
    string IHttpAttachment.ContentType { get; set; }

    bool? IHttpAttachment.Inline { get; set; }

    IHttpInputRequest IHttpAttachment.Request { get; set; }

    public HttpAttachmentDescriptor Request(
      Func<HttpInputRequestDescriptor, IHttpInputRequest> selector)
    {
      return this.Assign<IHttpInputRequest>(selector.InvokeOrDefault<HttpInputRequestDescriptor, IHttpInputRequest>(new HttpInputRequestDescriptor()), (Action<IHttpAttachment, IHttpInputRequest>) ((a, v) => a.Request = v));
    }

    public HttpAttachmentDescriptor Inline(bool? inline = true) => this.Assign<bool?>(inline, (Action<IHttpAttachment, bool?>) ((a, v) => a.Inline = v));

    public HttpAttachmentDescriptor ContentType(string contentType) => this.Assign<string>(contentType, (Action<IHttpAttachment, string>) ((a, v) => a.ContentType = v));
  }
}
