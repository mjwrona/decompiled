// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.BaseItemSet`1
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class BaseItemSet<T> : ICacheable where T : Item
  {
    private string m_queryPath;
    private string m_pattern;
    private StreamingCollection<T> m_items;

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string QueryPath
    {
      get => this.m_queryPath;
      set => this.m_queryPath = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Pattern
    {
      get => this.m_pattern;
      set => this.m_pattern = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public StreamingCollection<T> Items
    {
      get => this.m_items;
      set => this.m_items = value;
    }

    public int GetCachedSize()
    {
      int cachedSize = 200;
      if (this.QueryPath != null)
        cachedSize += this.QueryPath.Length;
      if (this.Pattern != null)
        cachedSize += this.Pattern.Length;
      return cachedSize;
    }
  }
}
