// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.AtomicLong
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  [Serializable]
  public class AtomicLong : IFormattable
  {
    private long longValue;

    public long Value
    {
      get => Interlocked.Read(ref this.longValue);
      set => Interlocked.Exchange(ref this.longValue, value);
    }

    public AtomicLong()
      : this(0L)
    {
    }

    public AtomicLong(long initialValue) => this.longValue = initialValue;

    public long AddAndGet(long delta) => Interlocked.Add(ref this.longValue, delta);

    public bool CompareAndSet(long expect, long update) => Interlocked.CompareExchange(ref this.longValue, update, expect) == expect;

    public long DecrementAndGet() => Interlocked.Decrement(ref this.longValue);

    public long GetAndDecrement() => Interlocked.Decrement(ref this.longValue) + 1L;

    public long GetAndIncrement() => Interlocked.Increment(ref this.longValue) - 1L;

    public long GetAndSet(long newValue) => Interlocked.Exchange(ref this.longValue, newValue);

    public long IncrementAndGet() => Interlocked.Increment(ref this.longValue);

    public bool WeakCompareAndSet(long expect, long update) => this.CompareAndSet(expect, update);

    public override bool Equals(object obj) => obj as AtomicLong == this;

    public override int GetHashCode() => this.Value.GetHashCode();

    public override string ToString() => this.ToString((IFormatProvider) CultureInfo.CurrentCulture);

    public string ToString(IFormatProvider formatProvider) => this.Value.ToString(formatProvider);

    public string ToString(string format) => this.ToString(format, (IFormatProvider) CultureInfo.CurrentCulture);

    public string ToString(string format, IFormatProvider formatProvider) => this.Value.ToString(formatProvider);

    public static bool operator ==(AtomicLong left, AtomicLong right) => (object) left != null && (object) right != null && left.Value == right.Value;

    public static bool operator !=(AtomicLong left, AtomicLong right) => !(left == right);

    public static implicit operator long(AtomicLong atomic) => atomic == (AtomicLong) null ? 0L : atomic.Value;
  }
}
