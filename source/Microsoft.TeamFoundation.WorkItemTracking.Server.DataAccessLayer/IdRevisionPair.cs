// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IdRevisionPair
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  public struct IdRevisionPair
  {
    private int m_id;
    private int m_revision;

    public int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }
  }
}
