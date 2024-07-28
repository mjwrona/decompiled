// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Events.OnMappingValueEventArgs
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Events
{
  internal sealed class OnMappingValueEventArgs : TemplateEventArgs
  {
    private readonly MappingToken m_mapping;
    private readonly LiteralToken m_key;
    private readonly TemplateToken m_value;
    private ReadOnlyMappingToken m_readOnlyMapping;
    private ReadOnlyLiteralToken m_readOnlyKey;
    private IReadOnlyTemplateToken m_readOnlyValue;

    internal OnMappingValueEventArgs(
      TemplateContext context,
      MappingToken mapping,
      LiteralToken key,
      TemplateToken value)
      : base(context)
    {
      this.m_mapping = mapping;
      this.m_key = key;
      this.m_value = value;
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

    public IReadOnlyTemplateToken Value
    {
      get
      {
        if (this.m_readOnlyValue == null && this.m_value != null)
          this.m_readOnlyValue = this.m_value.ToReadOnly();
        return this.m_readOnlyValue;
      }
    }
  }
}
