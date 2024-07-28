// Decompiled with JetBrains decompiler
// Type: Nest.EmailResult
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class EmailResult
  {
    [DataMember(Name = "bcc")]
    public IEnumerable<string> Bcc { get; set; }

    [DataMember(Name = "body")]
    public EmailBody Body { get; set; }

    [DataMember(Name = "cc")]
    public IEnumerable<string> Cc { get; set; }

    [DataMember(Name = "from")]
    public string From { get; set; }

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "priority")]
    public EmailPriority? Priority { get; set; }

    [DataMember(Name = "reply_to")]
    public IEnumerable<string> ReplyTo { get; set; }

    [DataMember(Name = "sent_date")]
    public DateTime? SentDate { get; set; }

    [DataMember(Name = "subject")]
    public string Subject { get; set; }

    [DataMember(Name = "to")]
    public IEnumerable<string> To { get; set; }
  }
}
