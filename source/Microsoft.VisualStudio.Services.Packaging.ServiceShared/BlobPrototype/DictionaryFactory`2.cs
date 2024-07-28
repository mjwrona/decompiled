// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.DictionaryFactory`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class DictionaryFactory<TIn, TOut> : 
    IFactory<TIn, TOut>,
    IEnumerable<KeyValuePair<TIn, TOut>>,
    IEnumerable
    where TIn : class
    where TOut : class
  {
    private readonly IDictionary<TIn, TOut> dict;

    public DictionaryFactory() => this.dict = (IDictionary<TIn, TOut>) new Dictionary<TIn, TOut>();

    public DictionaryFactory(IDictionary<TIn, TOut> dict) => this.dict = dict;

    public TOut Get(TIn input)
    {
      TOut @out;
      this.dict.TryGetValue(input, out @out);
      return @out;
    }

    public void Add(TIn key, TOut val) => this.dict.Add(key, val);

    public IEnumerator<KeyValuePair<TIn, TOut>> GetEnumerator() => this.dict.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
