// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalQueryItemElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalQueryItemElement : DalSqlElement
  {
    private const int c_rootTokenLength = 2;
    private const int c_projectIdLength = 36;

    protected int GetDataspaceIdForSecurityToken(SqlAccess component, string token)
    {
      Guid result;
      return token.Length > 2 && Guid.TryParse(token.Substring(2, 36), out result) ? component.GetDataspaceId(result) : component.GetDataspaceId(Guid.Empty, "WorkItem");
    }
  }
}
