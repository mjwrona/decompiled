// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.ObjectFactories.LambdaObjectFactory
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;

namespace YamlDotNet.Serialization.ObjectFactories
{
  public sealed class LambdaObjectFactory : IObjectFactory
  {
    private readonly Func<Type, object> _factory;

    public LambdaObjectFactory(Func<Type, object> factory) => this._factory = factory != null ? factory : throw new ArgumentNullException(nameof (factory));

    public object Create(Type type) => this._factory(type);
  }
}
