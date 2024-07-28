// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ConcurrentIteratorSelect`2
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class ConcurrentIteratorSelect<T1, T2> : ConcurrentIteratorWrapper<T1, T2>
  {
    private readonly Func<T1, T2> selector;

    public ConcurrentIteratorSelect(IConcurrentIterator<T1> baseEnumerator, Func<T1, T2> selector)
      : base(baseEnumerator)
    {
      this.selector = selector;
    }

    protected override bool OnBaseValueEnumerated(T1 value)
    {
      this.Current = this.selector(value);
      return true;
    }
  }
}
