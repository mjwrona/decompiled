// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DictionaryWrapper`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DictionaryWrapper<TKey, TValue> : SerializationWrapper
  {
    private IDictionary<TKey, TValue> m_value;

    public DictionaryWrapper() => this.m_value = (IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>();

    public DictionaryWrapper(IDictionary<TKey, TValue> value) => this.m_value = value;

    public override object Value => (object) this.m_value;

    public KeyValue<TKey, TValue>[] SerializedValue
    {
      get => KeyValue<TKey, TValue>.ConvertToArray((ICollection<KeyValuePair<TKey, TValue>>) this.m_value);
      set => this.m_value = (IDictionary<TKey, TValue>) KeyValue<TKey, TValue>.ConvertToDictionary((IEnumerable<KeyValue<TKey, TValue>>) value);
    }
  }
}
