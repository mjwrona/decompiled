// Decompiled with JetBrains decompiler
// Type: Nest.SuggestContainerDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SuggestContainerDescriptor<T> : 
    IsADictionaryDescriptorBase<SuggestContainerDescriptor<T>, ISuggestContainer, string, ISuggestBucket>
    where T : class
  {
    public SuggestContainerDescriptor()
      : base((ISuggestContainer) new SuggestContainer())
    {
    }

    private SuggestContainerDescriptor<T> AssignToBucket<TSuggester>(
      string name,
      TSuggester suggester,
      Action<SuggestBucket, TSuggester> assign)
      where TSuggester : ISuggester
    {
      SuggestBucket suggestBucket = new SuggestBucket();
      assign(suggestBucket, suggester);
      return this.Assign(name, (ISuggestBucket) suggestBucket);
    }

    public SuggestContainerDescriptor<T> Term(
      string name,
      Func<TermSuggesterDescriptor<T>, ITermSuggester> suggest)
    {
      return this.AssignToBucket<ITermSuggester>(name, suggest != null ? suggest(new TermSuggesterDescriptor<T>()) : (ITermSuggester) null, (Action<SuggestBucket, ITermSuggester>) ((b, s) =>
      {
        b.Term = s;
        b.Text = s.Text;
      }));
    }

    public SuggestContainerDescriptor<T> Phrase(
      string name,
      Func<PhraseSuggesterDescriptor<T>, IPhraseSuggester> suggest)
    {
      return this.AssignToBucket<IPhraseSuggester>(name, suggest != null ? suggest(new PhraseSuggesterDescriptor<T>()) : (IPhraseSuggester) null, (Action<SuggestBucket, IPhraseSuggester>) ((b, s) =>
      {
        b.Phrase = s;
        b.Text = s.Text;
      }));
    }

    public SuggestContainerDescriptor<T> Completion(
      string name,
      Func<CompletionSuggesterDescriptor<T>, ICompletionSuggester> suggest)
    {
      return this.AssignToBucket<ICompletionSuggester>(name, suggest != null ? suggest(new CompletionSuggesterDescriptor<T>()) : (ICompletionSuggester) null, (Action<SuggestBucket, ICompletionSuggester>) ((b, s) =>
      {
        b.Completion = s;
        b.Prefix = s.Prefix;
        b.Regex = s.Regex;
      }));
    }
  }
}
