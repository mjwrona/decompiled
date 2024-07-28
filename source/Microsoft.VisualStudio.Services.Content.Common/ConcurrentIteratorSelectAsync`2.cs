// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ConcurrentIteratorSelectAsync`2
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class ConcurrentIteratorSelectAsync<T1, T2> : ConcurrentIteratorWrapperAsync<T1, T2>
  {
    private readonly Func<T1, Task<T2>> selectorAsync;

    public ConcurrentIteratorSelectAsync(
      IConcurrentIterator<T1> baseEnumerator,
      Func<T1, Task<T2>> selectorAsync)
      : base(baseEnumerator)
    {
      this.selectorAsync = selectorAsync;
    }

    protected override async Task<bool> OnBaseValueEnumeratedAsync(T1 value)
    {
      ConcurrentIteratorSelectAsync<T1, T2> iteratorSelectAsync = this;
      T2 obj = await iteratorSelectAsync.selectorAsync(value).ConfigureAwait(true);
      iteratorSelectAsync.Current = obj;
      return true;
    }
  }
}
