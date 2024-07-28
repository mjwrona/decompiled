// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Events.OnSequenceItemEventArgs
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Events
{
  internal sealed class OnSequenceItemEventArgs : TemplateEventArgs
  {
    private readonly SequenceToken m_sequence;
    private readonly TemplateToken m_item;
    private ReadOnlySequenceToken m_readOnlySequence;
    private IReadOnlyTemplateToken m_readOnlyItem;

    internal OnSequenceItemEventArgs(
      TemplateContext context,
      SequenceToken sequence,
      TemplateToken item)
      : base(context)
    {
      this.m_sequence = sequence;
      this.m_item = item;
    }

    public ReadOnlySequenceToken Sequence
    {
      get
      {
        if (this.m_readOnlySequence == null)
          this.m_readOnlySequence = new ReadOnlySequenceToken(this.m_sequence);
        return this.m_readOnlySequence;
      }
    }

    public IReadOnlyTemplateToken Item
    {
      get
      {
        if (this.m_readOnlyItem == null && this.m_item != null)
          this.m_readOnlyItem = this.m_item.ToReadOnly();
        return this.m_readOnlyItem;
      }
    }
  }
}
