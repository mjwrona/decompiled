// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.MetadataTableHaveEntry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  public struct MetadataTableHaveEntry
  {
    private string m_tableName;
    private long m_rowVersion;

    public string TableName
    {
      get => this.m_tableName;
      set => this.m_tableName = value;
    }

    public long RowVersion
    {
      get => this.m_rowVersion;
      set => this.m_rowVersion = value;
    }
  }
}
