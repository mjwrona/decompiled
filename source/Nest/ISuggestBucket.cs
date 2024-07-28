// Decompiled with JetBrains decompiler
// Type: Nest.ISuggestBucket
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (SuggestBucket))]
  public interface ISuggestBucket
  {
    [DataMember(Name = "completion")]
    ICompletionSuggester Completion { get; set; }

    [DataMember(Name = "phrase")]
    IPhraseSuggester Phrase { get; set; }

    [DataMember(Name = "prefix")]
    string Prefix { get; set; }

    [DataMember(Name = "regex")]
    string Regex { get; set; }

    [DataMember(Name = "term")]
    ITermSuggester Term { get; set; }

    [DataMember(Name = "text")]
    string Text { get; set; }
  }
}
