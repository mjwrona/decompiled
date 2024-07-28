// Decompiled with JetBrains decompiler
// Type: Nest.EmailAttachments
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class EmailAttachments : 
    IsADictionaryBase<string, IEmailAttachment>,
    IEmailAttachments,
    IIsADictionary<string, IEmailAttachment>,
    IDictionary<string, IEmailAttachment>,
    ICollection<KeyValuePair<string, IEmailAttachment>>,
    IEnumerable<KeyValuePair<string, IEmailAttachment>>,
    IEnumerable,
    IIsADictionary
  {
    public EmailAttachments()
    {
    }

    public EmailAttachments(IDictionary<string, IEmailAttachment> attachments)
      : base(attachments)
    {
    }

    public void Add(string name, IEmailAttachment attachment) => this.BackingDictionary.Add(name, attachment);
  }
}
