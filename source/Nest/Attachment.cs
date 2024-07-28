// Decompiled with JetBrains decompiler
// Type: Nest.Attachment
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [JsonFormatter(typeof (AttachmentFormatter))]
  public class Attachment
  {
    [DataMember(Name = "author")]
    public string Author { get; set; }

    [IgnoreDataMember]
    [Ignore]
    public bool ContainsMetadata
    {
      get
      {
        if (this.Author.IsNullOrEmpty())
        {
          long? nullable = this.ContentLength;
          if (!nullable.HasValue && this.ContentType.IsNullOrEmpty() && !this.Date.HasValue && !this.DetectLanguage.HasValue)
          {
            nullable = this.IndexedCharacters;
            if (!nullable.HasValue && this.Keywords.IsNullOrEmpty() && this.Language.IsNullOrEmpty() && this.Name.IsNullOrEmpty())
              return !this.Title.IsNullOrEmpty();
          }
        }
        return true;
      }
    }

    [DataMember(Name = "content")]
    public string Content { get; set; }

    [DataMember(Name = "content_length")]
    public long? ContentLength { get; set; }

    [DataMember(Name = "content_type")]
    public string ContentType { get; set; }

    [DataMember(Name = "date")]
    public DateTime? Date { get; set; }

    [DataMember(Name = "detect_language")]
    public bool? DetectLanguage { get; set; }

    [DataMember(Name = "indexed_chars")]
    public long? IndexedCharacters { get; set; }

    [DataMember(Name = "keywords")]
    public string Keywords { get; set; }

    [DataMember(Name = "language")]
    public string Language { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "title")]
    public string Title { get; set; }
  }
}
