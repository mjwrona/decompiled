// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.Utilities.SerializerState
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace YamlDotNet.Serialization.Utilities
{
  public sealed class SerializerState : IDisposable
  {
    private readonly IDictionary<Type, object> items = (IDictionary<Type, object>) new Dictionary<Type, object>();

    public T Get<T>() where T : class, new()
    {
      object obj;
      if (!this.items.TryGetValue(typeof (T), out obj))
      {
        obj = (object) new T();
        this.items.Add(typeof (T), obj);
      }
      return (T) obj;
    }

    public void OnDeserialization()
    {
      foreach (IPostDeserializationCallback deserializationCallback in this.items.Values.OfType<IPostDeserializationCallback>())
        deserializationCallback.OnDeserialization();
    }

    public void Dispose()
    {
      foreach (IDisposable disposable in this.items.Values.OfType<IDisposable>())
        disposable.Dispose();
    }
  }
}
