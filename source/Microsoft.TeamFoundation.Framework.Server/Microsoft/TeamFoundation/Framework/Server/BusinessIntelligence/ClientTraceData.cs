// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence.ClientTraceData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence
{
  public class ClientTraceData
  {
    private IDictionary<string, object> m_data;

    public ClientTraceData() => this.m_data = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);

    public ClientTraceData(IDictionary<string, object> data)
      : this()
    {
      if (data == null)
        return;
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) data)
        this.AddProperty(keyValuePair.Key, keyValuePair.Value);
    }

    public void Add(string name, object value) => this.AddProperty(name, value);

    private void AddProperty(string name, object value)
    {
      if (this.m_data.ContainsKey(name))
        return;
      this.m_data[name] = value;
    }

    public IDictionary<string, object> GetData() => this.m_data;
  }
}
