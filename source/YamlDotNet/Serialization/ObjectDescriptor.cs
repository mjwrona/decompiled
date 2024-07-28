// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.ObjectDescriptor
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
  public sealed class ObjectDescriptor : IObjectDescriptor
  {
    public object Value { get; private set; }

    public Type Type { get; private set; }

    public Type StaticType { get; private set; }

    public ScalarStyle ScalarStyle { get; private set; }

    public ObjectDescriptor(object value, Type type, Type staticType)
      : this(value, type, staticType, ScalarStyle.Any)
    {
    }

    public ObjectDescriptor(object value, Type type, Type staticType, ScalarStyle scalarStyle)
    {
      this.Value = value;
      this.Type = (object) type != null ? type : throw new ArgumentNullException(nameof (type));
      this.StaticType = (object) staticType != null ? staticType : throw new ArgumentNullException(nameof (staticType));
      this.ScalarStyle = scalarStyle;
    }
  }
}
