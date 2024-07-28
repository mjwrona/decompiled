// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.ObjectGraphVisitors.DefaultExclusiveObjectGraphVisitor
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
  public sealed class DefaultExclusiveObjectGraphVisitor : ChainedObjectGraphVisitor
  {
    private static readonly IEqualityComparer<object> _objectComparer = (IEqualityComparer<object>) EqualityComparer<object>.Default;

    public DefaultExclusiveObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
      : base(nextVisitor)
    {
    }

    private static object GetDefault(Type type) => !type.IsValueType() ? (object) null : Activator.CreateInstance(type);

    public override bool EnterMapping(
      IObjectDescriptor key,
      IObjectDescriptor value,
      IEmitter context)
    {
      return !DefaultExclusiveObjectGraphVisitor._objectComparer.Equals((object) value, DefaultExclusiveObjectGraphVisitor.GetDefault(value.Type)) && base.EnterMapping(key, value, context);
    }

    public override bool EnterMapping(
      IPropertyDescriptor key,
      IObjectDescriptor value,
      IEmitter context)
    {
      DefaultValueAttribute customAttribute = key.GetCustomAttribute<DefaultValueAttribute>();
      object y = customAttribute != null ? customAttribute.Value : DefaultExclusiveObjectGraphVisitor.GetDefault(key.Type);
      return !DefaultExclusiveObjectGraphVisitor._objectComparer.Equals(value.Value, y) && base.EnterMapping(key, value, context);
    }
  }
}
