// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Sharpen.AtomicReference`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Threading;

namespace Antlr4.Runtime.Sharpen
{
  internal class AtomicReference<T> where T : class
  {
    private volatile T _value;

    public AtomicReference()
    {
    }

    public AtomicReference(T value) => this._value = value;

    public T Get() => this._value;

    public void Set(T value) => this._value = value;

    public bool CompareAndSet(T expect, T update) => (object) Interlocked.CompareExchange<T>(ref this._value, update, expect) == (object) expect;

    public T GetAndSet(T value) => Interlocked.Exchange<T>(ref this._value, value);
  }
}
