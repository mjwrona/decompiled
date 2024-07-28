// Decompiled with JetBrains decompiler
// Type: Nest.SuggestBucket
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class SuggestBucket : ISuggestBucket
  {
    [DataMember(Name = "completion")]
    public ICompletionSuggester Completion { get; set; }

    [DataMember(Name = "phrase")]
    public IPhraseSuggester Phrase { get; set; }

    [DataMember(Name = "prefix")]
    public string Prefix { get; set; }

    [DataMember(Name = "regex")]
    public string Regex { get; set; }

    [DataMember(Name = "term")]
    public ITermSuggester Term { get; set; }

    [DataMember(Name = "text")]
    public string Text { get; set; }
  }
}
