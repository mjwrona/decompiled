// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Events.OnMappingKeyEventArgs
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Events
{
  internal sealed class OnMappingKeyEventArgs : TemplateEventArgs
  {
    private readonly MappingToken m_mapping;
    private readonly LiteralToken m_key;
    private ReadOnlyMappingToken m_readOnlyMapping;
    private ReadOnlyLiteralToken m_readOnlyKey;

    internal OnMappingKeyEventArgs(TemplateContext context, MappingToken mapping, LiteralToken key)
      : base(context)
    {
      this.m_mapping = mapping;
      this.m_key = key;
    }

    public ReadOnlyMappingToken Mapping
    {
      get
      {
        if (this.m_readOnlyMapping == null)
          this.m_readOnlyMapping = new ReadOnlyMappingToken(this.m_mapping);
        return this.m_readOnlyMapping;
      }
    }

    public ReadOnlyLiteralToken Key
    {
      get
      {
        if (this.m_readOnlyKey == null && this.m_key != null)
          this.m_readOnlyKey = new ReadOnlyLiteralToken(this.m_key);
        return this.m_readOnlyKey;
      }
    }
  }
}
