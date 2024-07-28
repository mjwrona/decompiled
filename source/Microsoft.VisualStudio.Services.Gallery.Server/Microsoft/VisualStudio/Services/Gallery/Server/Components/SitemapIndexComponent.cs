// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.SitemapIndexComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Sitemap;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class SitemapIndexComponent : TeamFoundationSqlResourceComponent
  {
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> SqlExceptionFactories;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<SitemapIndexComponent>(1, true)
    }, "Sitemap");

    static SitemapIndexComponent() => SitemapIndexComponent.SqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        270010,
        new SqlExceptionFactory(typeof (FileIdNullException))
      }
    };

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) SitemapIndexComponent.SqlExceptionFactories;

    protected override string TraceArea => "Gallery";

    public void CreateSitemapIndex(int fileId)
    {
      this.PrepareStoredProcedure("Gallery.prc_CreateSitemapIndex");
      this.BindInt("FileId", fileId);
      this.ExecuteNonQuery();
    }

    public SitemapIndex QuerySitemapIndex()
    {
      this.PrepareStoredProcedure("Gallery.prc_QuerySitemapIndex");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QuerySitemapIndex", this.RequestContext))
      {
        resultCollection.AddBinder<SitemapIndex>((ObjectBinder<SitemapIndex>) new SitemapIndexComponent.SitemapIndexBinder());
        return resultCollection.GetCurrent<SitemapIndex>().Items.Capacity > 0 ? resultCollection.GetCurrent<SitemapIndex>().Items[0] : (SitemapIndex) null;
      }
    }

    internal class SitemapIndexBinder : ObjectBinder<SitemapIndex>
    {
      protected SqlColumnBinder FileIdColumn = new SqlColumnBinder("FileId");
      protected SqlColumnBinder LastUpdatedColumn = new SqlColumnBinder("LastUpdated");
      protected SqlColumnBinder IsLatestColumn = new SqlColumnBinder("IsLatest");

      protected override SitemapIndex Bind() => new SitemapIndex()
      {
        FileId = this.FileIdColumn.GetInt32((IDataReader) this.Reader),
        LastUpdated = this.LastUpdatedColumn.GetDateTime((IDataReader) this.Reader),
        IsLatest = this.IsLatestColumn.GetBoolean((IDataReader) this.Reader)
      };
    }
  }
}
