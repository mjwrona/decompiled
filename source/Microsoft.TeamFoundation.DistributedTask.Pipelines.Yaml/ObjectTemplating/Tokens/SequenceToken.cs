// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.SequenceToken
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  [DataContract]
  [JsonObject]
  internal sealed class SequenceToken : TemplateToken, IEnumerable<TemplateToken>, IEnumerable
  {
    [DataMember(Name = "seq", EmitDefaultValue = false)]
    private List<TemplateToken> m_items;

    public SequenceToken(int? fileId, int? line, int? column)
      : base(1, fileId, line, column)
    {
    }

    public TemplateToken this[int index]
    {
      get => this.m_items[index];
      set => this.m_items[index] = value;
    }

    internal int Count
    {
      get
      {
        List<TemplateToken> items = this.m_items;
        return items == null ? 0 : __nonvirtual (items.Count);
      }
    }

    public void Add(TemplateToken value)
    {
      if (this.m_items == null)
        this.m_items = new List<TemplateToken>();
      this.m_items.Add(value);
    }

    public IEnumerator<TemplateToken> GetEnumerator()
    {
      List<TemplateToken> items = this.m_items;
      // ISSUE: explicit non-virtual call
      return (items != null ? (__nonvirtual (items.Count) > 0 ? 1 : 0) : 0) != 0 ? (IEnumerator<TemplateToken>) this.m_items.GetEnumerator() : ((IEnumerable<TemplateToken>) Array.Empty<TemplateToken>()).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      List<TemplateToken> items = this.m_items;
      // ISSUE: explicit non-virtual call
      return (items != null ? (__nonvirtual (items.Count) > 0 ? 1 : 0) : 0) != 0 ? (IEnumerator) this.m_items.GetEnumerator() : (IEnumerator) ((IEnumerable<TemplateToken>) Array.Empty<TemplateToken>()).GetEnumerator();
    }

    public void Insert(int index, TemplateToken item)
    {
      if (this.m_items == null)
        this.m_items = new List<TemplateToken>();
      this.m_items.Insert(index, item);
    }

    public void RemoveAt(int index) => this.m_items.RemoveAt(index);

    internal override IReadOnlyTemplateToken ToReadOnly() => (IReadOnlyTemplateToken) new ReadOnlySequenceToken(this);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<TemplateToken> items = this.m_items;
      // ISSUE: explicit non-virtual call
      if ((items != null ? (__nonvirtual (items.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_items = (List<TemplateToken>) null;
    }
  }
}
