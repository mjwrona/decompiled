// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.NodeTypeResolvers.TagNodeTypeResolver
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.NodeTypeResolvers
{
  public sealed class TagNodeTypeResolver : INodeTypeResolver
  {
    private readonly IDictionary<string, Type> tagMappings;

    public TagNodeTypeResolver(IDictionary<string, Type> tagMappings) => this.tagMappings = tagMappings != null ? tagMappings : throw new ArgumentNullException(nameof (tagMappings));

    bool INodeTypeResolver.Resolve(NodeEvent nodeEvent, ref Type currentType)
    {
      Type type;
      if (string.IsNullOrEmpty(nodeEvent.Tag) || !this.tagMappings.TryGetValue(nodeEvent.Tag, out type))
        return false;
      currentType = type;
      return true;
    }
  }
}
