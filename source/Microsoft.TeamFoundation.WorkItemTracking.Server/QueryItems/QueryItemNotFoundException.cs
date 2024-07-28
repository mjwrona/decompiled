// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItemNotFoundException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  [Serializable]
  public class QueryItemNotFoundException : QueryItemException
  {
    public QueryItemNotFoundException(Guid id)
      : base(ServerResources.QueryNotFound((object) id))
    {
      this.QueryItemId = id;
      this.ErrorCode = 600288;
    }

    public QueryItemNotFoundException(Guid id, Guid projectId)
      : base(ServerResources.QueryNotFoundUnderProject((object) id))
    {
      this.QueryItemId = id;
      this.ErrorCode = 600288;
    }

    public QueryItemNotFoundException(string path)
      : base(ServerResources.QueryNotFound((object) path))
    {
      this.QueryItemPath = path;
      this.ErrorCode = 600288;
    }

    public Guid QueryItemId { get; private set; }

    public string QueryItemPath { get; private set; }
  }
}
