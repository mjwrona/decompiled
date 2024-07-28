// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.AtomicBoolean
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  [Serializable]
  public class AtomicBoolean : IFormattable, IEquatable<AtomicBoolean>
  {
    private volatile int booleanValue;

    public bool Value
    {
      get => this.booleanValue != 0;
      set => this.booleanValue = value ? 1 : 0;
    }

    public AtomicBoolean()
      : this(false)
    {
    }

    public AtomicBoolean(bool initialValue) => this.Value = initialValue;

    public bool CompareAndSet(bool expect, bool update)
    {
      int comparand = expect ? 1 : 0;
      return Interlocked.CompareExchange(ref this.booleanValue, update ? 1 : 0, comparand) == comparand;
    }

    public bool Exchange(bool newValue) => Interlocked.Exchange(ref this.booleanValue, newValue ? 1 : 0) != 0;

    public bool Equals(AtomicBoolean other) => !(other == (AtomicBoolean) null) && this.Value == other.Value;

    public override bool Equals(object obj) => obj != null && this.Equals(obj as AtomicBoolean);

    public override int GetHashCode() => this.Value.GetHashCode();

    public override string ToString() => this.ToString((IFormatProvider) CultureInfo.CurrentCulture);

    public string ToString(IFormatProvider formatProvider) => this.Value.ToString(formatProvider);

    public string ToString(string format) => this.ToString(format, (IFormatProvider) CultureInfo.CurrentCulture);

    public string ToString(string format, IFormatProvider formatProvider) => this.Value.ToString(formatProvider);

    public static bool operator ==(AtomicBoolean left, AtomicBoolean right) => (object) left != null && (object) right != null && left.Value == right.Value;

    public static bool operator !=(AtomicBoolean left, AtomicBoolean right) => !(left == right);

    public static implicit operator bool(AtomicBoolean atomic) => !(atomic == (AtomicBoolean) null) && atomic.Value;
  }
}
