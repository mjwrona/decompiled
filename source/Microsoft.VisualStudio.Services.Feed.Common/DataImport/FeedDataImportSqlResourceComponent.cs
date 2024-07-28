// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.DataImport.FeedDataImportSqlResourceComponent
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Common.DataImport
{
  public class FeedDataImportSqlResourceComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<FeedDataImportSqlResourceComponent>(1, true),
      (IComponentCreator) new ComponentCreator<FeedDataImportSqlResourceComponent2>(2)
    }, "FeedDataImport");

    public FeedDataImportSqlResourceComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual IList<FeedCore> GetFeeds()
    {
      this.PrepareCustomSqlStatement();
      this.BindNullValue("@feedId", SqlDbType.UniqueIdentifier);
      return this.ReadFeeds();
    }

    public virtual FeedCore GetFeed(Guid feedId)
    {
      this.PrepareCustomSqlStatement();
      this.BindGuid("@feedId", feedId);
      return this.ReadFeed();
    }

    protected FeedCore ReadFeed() => this.ReadFeeds().FirstOrDefault<FeedCore>();

    protected IList<FeedCore> ReadFeeds()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<FeedCore>((ObjectBinder<FeedCore>) new DataImportFeedBinder());
        return (IList<FeedCore>) resultCollection.GetCurrent<FeedCore>().Items;
      }
    }

    private void PrepareCustomSqlStatement()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetFeedForDataImport.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
    }
  }
}
