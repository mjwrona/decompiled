// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.TemplateTokenReadOnlyList
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  internal sealed class TemplateTokenReadOnlyList : 
    IReadOnlyArray,
    IReadOnlyList<object>,
    IReadOnlyCollection<object>,
    IEnumerable<object>,
    IEnumerable
  {
    private readonly SequenceToken m_sequence;
    private List<object> m_list;

    internal TemplateTokenReadOnlyList(SequenceToken sequence) => this.m_sequence = sequence;

    public int Count
    {
      get
      {
        if (this.m_list == null)
          this.Initialize();
        return this.m_list.Count;
      }
    }

    public object this[int index]
    {
      get
      {
        if (this.m_list == null)
          this.Initialize();
        return this.m_list[index];
      }
    }

    public IEnumerator<object> GetEnumerator()
    {
      if (this.m_list == null)
        this.Initialize();
      return (IEnumerator<object>) this.m_list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      if (this.m_list == null)
        this.Initialize();
      return (IEnumerator) this.m_list.GetEnumerator();
    }

    private void Initialize() => this.m_list = new List<object>((IEnumerable<object>) this.m_sequence);
  }
}
