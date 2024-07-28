// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Ingestion.LazySerDesValue`2
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Ingestion
{
  public sealed class LazySerDesValue<TSerialized, TValue>
    where TSerialized : class
    where TValue : class
  {
    private readonly Func<TSerialized, TValue> deserializer;
    private readonly Func<TValue, TSerialized> serializer;
    private TSerialized? serialized;
    private TValue? value;

    public LazySerDesValue(TSerialized serialized, Func<TSerialized, TValue> deserializer)
    {
      TValue obj = default (TValue);
      TSerialized serialized1 = serialized;
      if ((object) serialized1 == null)
        throw new ArgumentNullException(nameof (serialized));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      Func<TValue, TSerialized> serializer = LazySerDesValue<TSerialized, TValue>.\u003C\u003EO.\u003C0\u003E__AlreadySerialized ?? (LazySerDesValue<TSerialized, TValue>.\u003C\u003EO.\u003C0\u003E__AlreadySerialized = new Func<TValue, TSerialized>(LazySerDesValue<TSerialized, TValue>.AlreadySerialized));
      Func<TSerialized, TValue> deserializer1 = deserializer;
      // ISSUE: explicit constructor call
      this.\u002Ector(obj, serialized1, serializer, deserializer1);
    }

    public LazySerDesValue(TValue value, Func<TValue, TSerialized> serializer)
      : this(value ?? throw new ArgumentNullException(nameof (value)), default (TSerialized), serializer, LazySerDesValue<TSerialized, TValue>.\u003C\u003EO.\u003C1\u003E__AlreadyDeserialized ?? (LazySerDesValue<TSerialized, TValue>.\u003C\u003EO.\u003C1\u003E__AlreadyDeserialized = new Func<TSerialized, TValue>(LazySerDesValue<TSerialized, TValue>.AlreadyDeserialized)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public LazySerDesValue(TValue value, TSerialized serialized)
      : this(value ?? throw new ArgumentNullException(nameof (value)), serialized ?? throw new ArgumentNullException(nameof (serialized)), LazySerDesValue<TSerialized, TValue>.\u003C\u003EO.\u003C0\u003E__AlreadySerialized ?? (LazySerDesValue<TSerialized, TValue>.\u003C\u003EO.\u003C0\u003E__AlreadySerialized = new Func<TValue, TSerialized>(LazySerDesValue<TSerialized, TValue>.AlreadySerialized)), LazySerDesValue<TSerialized, TValue>.\u003C\u003EO.\u003C1\u003E__AlreadyDeserialized ?? (LazySerDesValue<TSerialized, TValue>.\u003C\u003EO.\u003C1\u003E__AlreadyDeserialized = new Func<TSerialized, TValue>(LazySerDesValue<TSerialized, TValue>.AlreadyDeserialized)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    [ExcludeFromCodeCoverage]
    private static TValue AlreadyDeserialized(TSerialized _) => throw new NotSupportedException();

    [ExcludeFromCodeCoverage]
    private static TSerialized AlreadySerialized(TValue _) => throw new NotSupportedException();

    private LazySerDesValue(
      TValue? value,
      TSerialized? serialized,
      Func<TValue, TSerialized> serializer,
      Func<TSerialized, TValue> deserializer)
    {
      this.value = value;
      this.serialized = serialized;
      this.deserializer = deserializer ?? throw new ArgumentNullException(nameof (deserializer));
      this.serializer = serializer ?? throw new ArgumentNullException(nameof (serializer));
    }

    public TSerialized Serialized => LazyInitializer.EnsureInitialized<TSerialized>(ref this.serialized, (Func<TSerialized>) (() => this.serializer(this.value)));

    public TValue Value => LazyInitializer.EnsureInitialized<TValue>(ref this.value, (Func<TValue>) (() => this.deserializer(this.serialized)));
  }
}
