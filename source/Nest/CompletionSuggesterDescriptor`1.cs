// Decompiled with JetBrains decompiler
// Type: Nest.CompletionSuggesterDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class CompletionSuggesterDescriptor<T> : 
    SuggestDescriptorBase<CompletionSuggesterDescriptor<T>, ICompletionSuggester, T>,
    ICompletionSuggester,
    ISuggester
    where T : class
  {
    IDictionary<string, IList<ISuggestContextQuery>> ICompletionSuggester.Contexts { get; set; }

    ISuggestFuzziness ICompletionSuggester.Fuzzy { get; set; }

    string ICompletionSuggester.Prefix { get; set; }

    string ICompletionSuggester.Regex { get; set; }

    bool? ICompletionSuggester.SkipDuplicates { get; set; }

    public CompletionSuggesterDescriptor<T> Prefix(string prefix) => this.Assign<string>(prefix, (Action<ICompletionSuggester, string>) ((a, v) => a.Prefix = v));

    public CompletionSuggesterDescriptor<T> Regex(string regex) => this.Assign<string>(regex, (Action<ICompletionSuggester, string>) ((a, v) => a.Regex = v));

    public CompletionSuggesterDescriptor<T> Fuzzy(
      Func<SuggestFuzzinessDescriptor<T>, ISuggestFuzziness> selector = null)
    {
      return this.Assign<ISuggestFuzziness>(selector.InvokeOrDefault<SuggestFuzzinessDescriptor<T>, ISuggestFuzziness>(new SuggestFuzzinessDescriptor<T>()), (Action<ICompletionSuggester, ISuggestFuzziness>) ((a, v) => a.Fuzzy = v));
    }

    public CompletionSuggesterDescriptor<T> Contexts(
      Func<SuggestContextQueriesDescriptor<T>, IPromise<IDictionary<string, IList<ISuggestContextQuery>>>> contexts)
    {
      return this.Assign<Func<SuggestContextQueriesDescriptor<T>, IPromise<IDictionary<string, IList<ISuggestContextQuery>>>>>(contexts, (Action<ICompletionSuggester, Func<SuggestContextQueriesDescriptor<T>, IPromise<IDictionary<string, IList<ISuggestContextQuery>>>>>) ((a, v) => a.Contexts = v != null ? v(new SuggestContextQueriesDescriptor<T>()).Value : (IDictionary<string, IList<ISuggestContextQuery>>) null));
    }

    public CompletionSuggesterDescriptor<T> SkipDuplicates(bool? skipDuplicates = true) => this.Assign<bool?>(skipDuplicates, (Action<ICompletionSuggester, bool?>) ((a, v) => a.SkipDuplicates = v));
  }
}
