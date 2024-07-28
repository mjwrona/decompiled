// Decompiled with JetBrains decompiler
// Type: Nest.ISlackMessage
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (SlackMessage))]
  public interface ISlackMessage
  {
    [DataMember(Name = "attachments")]
    IEnumerable<ISlackAttachment> Attachments { get; set; }

    [DataMember(Name = "dynamic_attachments")]
    ISlackDynamicAttachment DynamicAttachments { get; set; }

    [DataMember(Name = "from")]
    string From { get; set; }

    [DataMember(Name = "icon")]
    string Icon { get; set; }

    [DataMember(Name = "text")]
    string Text { get; set; }

    [DataMember(Name = "to")]
    IEnumerable<string> To { get; set; }
  }
}
