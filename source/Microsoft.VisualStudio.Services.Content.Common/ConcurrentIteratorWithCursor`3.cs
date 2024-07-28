// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ConcurrentIteratorWithCursor`3
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  internal class ConcurrentIteratorWithCursor<T1, T2, TCursor> : 
    ConcurrentIteratorWrapper<T1, T2>,
    IConcurrentIteratorWithCursor<T2, TCursor>,
    IConcurrentIterator<T2>,
    IDisposable
  {
    private Func<T1, TCursor> mkCursor;
    private Func<T1, T2> select;
    private TCursor initial;
    private T1 Current1;

    public ConcurrentIteratorWithCursor(
      IConcurrentIterator<T1> baseEnumerator,
      Func<T1, TCursor> mkCursor,
      Func<T1, T2> select,
      TCursor initial = null)
      : base(baseEnumerator)
    {
      this.mkCursor = mkCursor;
      this.initial = initial;
      this.select = select;
    }

    public TCursor Cursor => !this.EnumerationStarted ? this.initial : this.mkCursor(this.Current1);

    protected override bool OnBaseValueEnumerated(T1 value)
    {
      this.Current1 = value;
      this.Current = this.select(value);
      return true;
    }
  }
}
