// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedSqlResourceComponentBase
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using System;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedSqlResourceComponentBase : TeamFoundationSqlResourceComponent
  {
    public const int NormalizedPackageNameMaxLength = 255;
    public const int ProtocolTypeMaxLength = 20;

    internal int GetFeedDataspaceId(Guid? projectId, bool createIfNotExists = false)
    {
      int feedDataspaceId = !projectId.HasValue || projectId.Equals((object) Guid.Empty) ? this.GetDataspaceId(Guid.Empty, createIfNotExists) : this.GetDataspaceId(projectId.Value, createIfNotExists);
      if (!(projectId.HasValue & createIfNotExists))
        return feedDataspaceId;
      this.GetDataspaceId(projectId.Value, "Feed", true);
      return feedDataspaceId;
    }

    protected virtual void BindFeedIdentity(
      FeedIdentity feedId,
      bool createMissingDataspace = false,
      string dataspaceName = "@dataspaceId",
      string feedIdName = "@feedId")
    {
      this.BindInt(dataspaceName, this.GetDataspaceId(Guid.Empty));
      this.BindGuid(feedIdName, feedId.Id);
    }
  }
}
