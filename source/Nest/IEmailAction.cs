// Decompiled with JetBrains decompiler
// Type: Nest.IEmailAction
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IEmailAction : IAction
  {
    [DataMember(Name = "account")]
    string Account { get; set; }

    [DataMember(Name = "attachments")]
    IEmailAttachments Attachments { get; set; }

    [DataMember(Name = "bcc")]
    IEnumerable<string> Bcc { get; set; }

    [DataMember(Name = "body")]
    IEmailBody Body { get; set; }

    [DataMember(Name = "cc")]
    IEnumerable<string> Cc { get; set; }

    [DataMember(Name = "from")]
    string From { get; set; }

    [DataMember(Name = "priority")]
    EmailPriority? Priority { get; set; }

    [DataMember(Name = "reply_to")]
    IEnumerable<string> ReplyTo { get; set; }

    [DataMember(Name = "subject")]
    string Subject { get; set; }

    [DataMember(Name = "to")]
    IEnumerable<string> To { get; set; }
  }
}
