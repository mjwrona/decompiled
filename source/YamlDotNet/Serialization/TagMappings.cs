// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.TagMappings
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;

namespace YamlDotNet.Serialization
{
  public sealed class TagMappings
  {
    private readonly IDictionary<string, Type> mappings;

    public TagMappings() => this.mappings = (IDictionary<string, Type>) new Dictionary<string, Type>();

    public TagMappings(IDictionary<string, Type> mappings) => this.mappings = (IDictionary<string, Type>) new Dictionary<string, Type>(mappings);

    public void Add(string tag, Type mapping) => this.mappings.Add(tag, mapping);

    internal Type GetMapping(string tag)
    {
      Type type;
      return this.mappings.TryGetValue(tag, out type) ? type : (Type) null;
    }
  }
}
