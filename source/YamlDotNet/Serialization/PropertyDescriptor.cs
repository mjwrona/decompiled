// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.PropertyDescriptor
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
  public sealed class PropertyDescriptor : IPropertyDescriptor
  {
    private readonly IPropertyDescriptor baseDescriptor;

    public PropertyDescriptor(IPropertyDescriptor baseDescriptor)
    {
      this.baseDescriptor = baseDescriptor;
      this.Name = baseDescriptor.Name;
    }

    public string Name { get; set; }

    public Type Type => this.baseDescriptor.Type;

    public Type TypeOverride
    {
      get => this.baseDescriptor.TypeOverride;
      set => this.baseDescriptor.TypeOverride = value;
    }

    public int Order { get; set; }

    public ScalarStyle ScalarStyle
    {
      get => this.baseDescriptor.ScalarStyle;
      set => this.baseDescriptor.ScalarStyle = value;
    }

    public bool CanWrite => this.baseDescriptor.CanWrite;

    public void Write(object target, object value) => this.baseDescriptor.Write(target, value);

    public T GetCustomAttribute<T>() where T : Attribute => this.baseDescriptor.GetCustomAttribute<T>();

    public IObjectDescriptor Read(object target) => this.baseDescriptor.Read(target);
  }
}
