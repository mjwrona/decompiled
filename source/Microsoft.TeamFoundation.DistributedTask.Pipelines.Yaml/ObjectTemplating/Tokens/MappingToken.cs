// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.MappingToken
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  [DataContract]
  [JsonObject]
  internal sealed class MappingToken : 
    TemplateToken,
    IEnumerable<KeyValuePair<ScalarToken, TemplateToken>>,
    IEnumerable
  {
    [DataMember(Name = "map", EmitDefaultValue = false)]
    private List<KeyValuePair<ScalarToken, TemplateToken>> m_items;

    public MappingToken(int? fileId, int? line, int? column)
      : base(2, fileId, line, column)
    {
    }

    internal int Count
    {
      get
      {
        List<KeyValuePair<ScalarToken, TemplateToken>> items = this.m_items;
        return items == null ? 0 : __nonvirtual (items.Count);
      }
    }

    public KeyValuePair<ScalarToken, TemplateToken> this[int index]
    {
      get => this.m_items[index];
      set => this.m_items[index] = value;
    }

    public void Add(
      IEnumerable<KeyValuePair<ScalarToken, TemplateToken>> items)
    {
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in items)
        this.Add(keyValuePair);
    }

    public void Add(KeyValuePair<ScalarToken, TemplateToken> item)
    {
      if (this.m_items == null)
        this.m_items = new List<KeyValuePair<ScalarToken, TemplateToken>>();
      this.m_items.Add(item);
    }

    public void Add(ScalarToken key, TemplateToken value) => this.Add(new KeyValuePair<ScalarToken, TemplateToken>(key, value));

    public void Insert(KeyValuePair<ScalarToken, TemplateToken> item)
    {
      if (this.m_items == null)
        this.m_items = new List<KeyValuePair<ScalarToken, TemplateToken>>();
      this.m_items.Insert(0, item);
    }

    public IEnumerator<KeyValuePair<ScalarToken, TemplateToken>> GetEnumerator()
    {
      List<KeyValuePair<ScalarToken, TemplateToken>> items = this.m_items;
      // ISSUE: explicit non-virtual call
      return (items != null ? (__nonvirtual (items.Count) > 0 ? 1 : 0) : 0) != 0 ? (IEnumerator<KeyValuePair<ScalarToken, TemplateToken>>) this.m_items.GetEnumerator() : (IEnumerator<KeyValuePair<ScalarToken, TemplateToken>>) new List<KeyValuePair<ScalarToken, TemplateToken>>(0).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      List<KeyValuePair<ScalarToken, TemplateToken>> items = this.m_items;
      // ISSUE: explicit non-virtual call
      return (items != null ? (__nonvirtual (items.Count) > 0 ? 1 : 0) : 0) != 0 ? (IEnumerator) this.m_items.GetEnumerator() : new KeyValuePair<ScalarToken, TemplateToken>[0].GetEnumerator();
    }

    public void Insert(int index, KeyValuePair<ScalarToken, TemplateToken> item)
    {
      if (this.m_items == null)
        this.m_items = new List<KeyValuePair<ScalarToken, TemplateToken>>();
      this.m_items.Insert(index, item);
    }

    public void Insert(int index, ScalarToken key, TemplateToken value) => this.Insert(index, new KeyValuePair<ScalarToken, TemplateToken>(key, value));

    public void RemoveAt(int index) => this.m_items.RemoveAt(index);

    internal override IReadOnlyTemplateToken ToReadOnly() => (IReadOnlyTemplateToken) new ReadOnlyMappingToken(this);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<KeyValuePair<ScalarToken, TemplateToken>> items = this.m_items;
      // ISSUE: explicit non-virtual call
      if ((items != null ? (__nonvirtual (items.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_items = (List<KeyValuePair<ScalarToken, TemplateToken>>) null;
    }
  }
}
