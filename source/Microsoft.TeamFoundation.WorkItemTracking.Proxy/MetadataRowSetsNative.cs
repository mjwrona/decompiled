// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.MetadataRowSetsNative
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class MetadataRowSetsNative : IRowSetsNative
  {
    private IMetadataRowSets m_rs;

    public MetadataRowSetsNative(IMetadataRowSets rs) => this.m_rs = rs;

    object IRowSetsNative.GetRowSet(MetadataRowSetNames name) => (object) new RowSetNative(this.m_rs[name]);
  }
}
