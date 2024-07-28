// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.DataImport.FeedDataImportSqlResourceComponent2
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Common.DataImport
{
  public class FeedDataImportSqlResourceComponent2 : FeedDataImportSqlResourceComponent
  {
    public override IList<FeedCore> GetFeeds()
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedForDataImport");
      this.BindNullValue("@feedId", SqlDbType.UniqueIdentifier);
      return this.ReadFeeds();
    }

    public override FeedCore GetFeed(Guid feedId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedForDataImport");
      this.BindGuid("@feedId", feedId);
      return this.ReadFeed();
    }
  }
}
