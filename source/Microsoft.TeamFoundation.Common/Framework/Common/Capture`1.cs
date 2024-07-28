// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.Capture`1
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public class Capture<T> where T : struct
  {
    private object m_boxedValue;
    private readonly Predicate<T> m_valueCheck;

    public Capture()
      : this(default (T))
    {
    }

    public Capture(T value, Predicate<T> valueCheck = null)
    {
      this.m_valueCheck = valueCheck;
      this.m_boxedValue = (object) value;
      this.Validate(value);
    }

    public static implicit operator T(Capture<T> value) => value.Value;

    public T Value
    {
      get => (T) this.m_boxedValue;
      set
      {
        this.Validate(value);
        Interlocked.Exchange(ref this.m_boxedValue, (object) value);
      }
    }

    public override string ToString() => this.Value.ToString();

    private void Validate(T value)
    {
      if (this.m_valueCheck != null && !this.m_valueCheck(value))
        throw new ArgumentException();
    }
  }
}
