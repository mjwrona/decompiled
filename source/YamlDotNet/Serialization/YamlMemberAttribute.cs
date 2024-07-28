// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.YamlMemberAttribute
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public sealed class YamlMemberAttribute : Attribute
  {
    public Type SerializeAs { get; set; }

    public int Order { get; set; }

    public string Alias { get; set; }

    public bool ApplyNamingConventions { get; set; }

    public ScalarStyle ScalarStyle { get; set; }

    public YamlMemberAttribute()
    {
      this.ScalarStyle = ScalarStyle.Any;
      this.ApplyNamingConventions = true;
    }

    public YamlMemberAttribute(Type serializeAs)
      : this()
    {
      this.SerializeAs = serializeAs;
    }
  }
}
