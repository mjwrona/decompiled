// Decompiled with JetBrains decompiler
// Type: Nest.ISlackAttachment
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (SlackAttachment))]
  public interface ISlackAttachment
  {
    [DataMember(Name = "author_icon")]
    string AuthorIcon { get; set; }

    [DataMember(Name = "author_link")]
    string AuthorLink { get; set; }

    [DataMember(Name = "author_name")]
    string AuthorName { get; set; }

    [DataMember(Name = "color")]
    string Color { get; set; }

    [DataMember(Name = "fallback")]
    string Fallback { get; set; }

    [DataMember(Name = "fields")]
    IEnumerable<ISlackAttachmentField> Fields { get; set; }

    [DataMember(Name = "footer")]
    string Footer { get; set; }

    [DataMember(Name = "footer_icon")]
    string FooterIcon { get; set; }

    [DataMember(Name = "image_url")]
    string ImageUrl { get; set; }

    [DataMember(Name = "pretext")]
    string Pretext { get; set; }

    [DataMember(Name = "text")]
    string Text { get; set; }

    [DataMember(Name = "thumb_url")]
    string ThumbUrl { get; set; }

    [DataMember(Name = "title")]
    string Title { get; set; }

    [DataMember(Name = "title_link")]
    string TitleLink { get; set; }

    [DataMember(Name = "ts")]
    [JsonFormatter(typeof (NullableDateTimeOffsetEpochSecondsFormatter))]
    DateTimeOffset? Ts { get; set; }
  }
}
