// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiComponent
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.Server.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  internal class WikiComponent : TeamFoundationSqlResourceComponent
  {
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory((IComponentCreator[]) new ComponentCreator<WikiComponent>[7]
    {
      new ComponentCreator<WikiComponent>(1),
      new ComponentCreator<WikiComponent>(1430),
      new ComponentCreator<WikiComponent>(1440),
      new ComponentCreator<WikiComponent>(1450),
      new ComponentCreator<WikiComponent>(1480),
      new ComponentCreator<WikiComponent>(1590),
      new ComponentCreator<WikiComponent>(1620)
    }, "Wiki");
    private static readonly SqlMetaData[] Wiki_typ_PageViewItem = new SqlMetaData[6]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("WikiId", SqlDbType.NVarChar, 40L),
      new SqlMetaData("Version", SqlDbType.NVarChar, 100L),
      new SqlMetaData("PagePath", SqlDbType.NVarChar, 300L),
      new SqlMetaData("ViewCount", SqlDbType.Int),
      new SqlMetaData("LastViewed", SqlDbType.DateTime)
    };
    private static readonly SqlMetaData[] Wiki_typ_Page = new SqlMetaData[3]
    {
      new SqlMetaData("PageId", SqlDbType.Int),
      new SqlMetaData("PagePath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("PagePathHash", SqlDbType.VarBinary, 32L)
    };
    private static readonly SqlMetaData[] Wiki_typ_RenamedPage = new SqlMetaData[4]
    {
      new SqlMetaData("OldPagePath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("OldPagePathHash", SqlDbType.VarBinary, 32L),
      new SqlMetaData("NewPagePath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("NewPagePathHash", SqlDbType.VarBinary, 32L)
    };

    static WikiComponent() => WikiComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1900001,
        new SqlExceptionFactory(typeof (PushNotLatestException))
      }
    };

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) WikiComponent.s_sqlExceptionFactories;

    public WikiComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    private void BindDataspaceForProject(Guid projectId) => this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));

    private void EnforceMinimalVersion(int version)
    {
      if (this.Version < version)
        throw new ServiceVersionNotSupportedException(WikiComponent.ComponentFactory.ServiceName, this.Version, version);
    }

    public void DeleteAllPageIds(
      Guid projectId,
      Guid wikiId,
      string wikiVersion,
      int associatedPushId)
    {
      this.EnforceMinimalVersion(1430);
      this.PrepareStoredProcedure("Wiki.prc_DeletePagesForWikiVersion");
      this.BindDataspaceForProject(projectId);
      this.BindGuid("@wikiId", wikiId);
      this.BindString("@wikiVersion", wikiVersion, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      this.BindBinary("@wikiVersionHash", this.GetSHA256Hash(wikiVersion), SqlDbType.VarBinary);
      this.BindInt("@associatedPushId", associatedPushId);
      this.ExecuteNonQuery();
    }

    public List<WikiPageIdInfo> GetAllPagesByPageId(Guid projectId, int pageId)
    {
      this.EnforceMinimalVersion(1450);
      this.PrepareStoredProcedure("Wiki.prc_GetPagesByPageId");
      this.BindDataspaceForProject(projectId);
      this.BindInt("@pageId", pageId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<WikiPageIdInfo>((ObjectBinder<WikiPageIdInfo>) new WikiComponent.WikiPageInfoBinder());
        return resultCollection.GetCurrent<WikiPageIdInfo>().Items;
      }
    }

    public virtual List<WikiPageIdInfo> GetAllPagesByPageIds(
      Guid projectId,
      IEnumerable<int> pageIds)
    {
      this.EnforceMinimalVersion(1590);
      this.PrepareStoredProcedure("Wiki.prc_GetPagesByPageIds");
      this.BindDataspaceForProject(projectId);
      this.BindInt32Table("@pageIds", pageIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<WikiPageIdInfo>((ObjectBinder<WikiPageIdInfo>) new WikiComponent.WikiPageInfoBinder());
        return resultCollection.GetCurrent<WikiPageIdInfo>().Items;
      }
    }

    public int? GetPageIdByPath(Guid projectId, Guid wikiId, string wikiVersion, string pagePath)
    {
      if (this.Version < 1450)
        return new int?();
      this.PrepareStoredProcedure("Wiki.prc_GetPageIdByPagePath");
      this.BindDataspaceForProject(projectId);
      this.BindGuid("@wikiId", wikiId);
      this.BindBinary("@wikiVersionHash", this.GetSHA256Hash(wikiVersion), SqlDbType.VarBinary);
      this.BindBinary("@pagePathHash", this.GetSHA256Hash(pagePath), SqlDbType.VarBinary);
      return (int?) this.ExecuteScalar();
    }

    public PageView GetPageView(PageView pageView)
    {
      if (this.Version < 1)
        return (PageView) null;
      this.PrepareStoredProcedure("Wiki.prc_GetPageView");
      this.BindPageViewTable("@pageView", (IEnumerable<PageView>) new PageView[1]
      {
        pageView
      });
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PageView>((ObjectBinder<PageView>) new WikiComponent.PageViewBinder());
        return resultCollection.GetCurrent<PageView>().Items.FirstOrDefault<PageView>();
      }
    }

    public IEnumerable<WikiPageWithId> GetPages(Guid projectId, Guid wikiId, string wikiVersion)
    {
      this.EnforceMinimalVersion(1430);
      this.PrepareStoredProcedure("Wiki.prc_GetPagesForWikiVersion");
      this.BindDataspaceForProject(projectId);
      this.BindGuid("@wikiId", wikiId);
      this.BindBinary("@wikiVersionHash", this.GetSHA256Hash(wikiVersion), SqlDbType.VarBinary);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<WikiPageWithId>((ObjectBinder<WikiPageWithId>) new WikiComponent.WikiPageWithIdBinder());
        return (IEnumerable<WikiPageWithId>) resultCollection.GetCurrent<WikiPageWithId>().Items;
      }
    }

    public virtual IList<WikiPageWithId> GetPages(
      Guid projectId,
      Guid wikiId,
      string wikiVersion,
      int afterPageId,
      int batchSize)
    {
      this.EnforceMinimalVersion(1590);
      this.PrepareStoredProcedure("Wiki.prc_GetPagesByWikiIdAndVersionAfterGivenPageIdInBatch");
      this.BindDataspaceForProject(projectId);
      this.BindGuid("@wikiId", wikiId);
      this.BindBinary("@wikiVersionHash", this.GetSHA256Hash(wikiVersion), SqlDbType.VarBinary);
      this.BindInt("@afterPageId", afterPageId);
      this.BindInt("@batchSize", batchSize);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<WikiPageWithId>((ObjectBinder<WikiPageWithId>) new WikiComponent.WikiPageWithIdBinder());
        return (IList<WikiPageWithId>) resultCollection.GetCurrent<WikiPageWithId>().Items;
      }
    }

    public virtual WikiPageIdInfo GetLatestPageById(Guid projectId, int pageId)
    {
      List<WikiPageIdInfo> allPagesByPageId = this.GetAllPagesByPageId(projectId, pageId);
      return allPagesByPageId == null ? (WikiPageIdInfo) null : allPagesByPageId.FirstOrDefault<WikiPageIdInfo>();
    }

    public int? GetWikiPushWaterMark(Guid projectId, Guid wikiId, string wikiVersion)
    {
      this.EnforceMinimalVersion(1430);
      this.PrepareStoredProcedure("Wiki.prc_GetWikiPushWaterMark");
      this.BindDataspaceForProject(projectId);
      this.BindGuid("@wikiId", wikiId);
      this.BindString("@wikiVersion", wikiVersion, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      return (int?) this.ExecuteScalar();
    }

    public IList<WikiVersionWaterMark> GetAllWikiVersionWaterMark(Guid projectId)
    {
      this.EnforceMinimalVersion(1440);
      this.PrepareStoredProcedure("Wiki.prc_GetAllWikiVersionWaterMarks");
      this.BindDataspaceForProject(projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<WikiVersionWaterMark>((ObjectBinder<WikiVersionWaterMark>) new WikiComponent.WikiVersionWaterMarkBinder());
        return (IList<WikiVersionWaterMark>) resultCollection.GetCurrent<WikiVersionWaterMark>().Items;
      }
    }

    public void UnpublishWikiVersion(
      Guid projectId,
      Guid wikiId,
      string wikiVersion,
      out List<WikiPageWithId> deletedPagesInDb)
    {
      this.PrepareStoredProcedure("Wiki.prc_UnpublishWikiVersion");
      this.BindDataspaceForProject(projectId);
      this.BindGuid("@wikiId", wikiId);
      this.BindString("@wikiVersion", wikiVersion, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      this.BindBinary("@wikiVersionHash", this.GetSHA256Hash(wikiVersion), SqlDbType.VarBinary);
      if (this.Version < 1620)
      {
        this.ExecuteNonQuery();
        deletedPagesInDb = new List<WikiPageWithId>();
      }
      else
      {
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<WikiPageWithId>((ObjectBinder<WikiPageWithId>) new WikiComponent.WikiPageWithIdOnlyBinder());
          deletedPagesInDb = resultCollection.GetCurrent<WikiPageWithId>().Items;
        }
      }
    }

    public void UpdatePageId(
      Guid projectId,
      Guid wikiId,
      string wikiVersion,
      IList<WikiPageWithId> addedPages,
      IList<WikiRenamedPage> renamedPages,
      IList<WikiPageWithId> deletedPages,
      int associatedPushId)
    {
      this.EnforceMinimalVersion(1430);
      this.PrepareStoredProcedure("Wiki.prc_UpdatePageId");
      this.BindDataspaceForProject(projectId);
      this.BindGuid("@wikiId", wikiId);
      this.BindString("@wikiVersion", wikiVersion, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      this.BindBinary("@wikiVersionHash", this.GetSHA256Hash(wikiVersion), SqlDbType.VarBinary);
      this.BindWikiPageTable("@addedPages", (IEnumerable<WikiPageWithId>) addedPages);
      this.BindWikiRenamePageTable("@renamedPages", (IEnumerable<WikiRenamedPage>) renamedPages);
      this.BindWikiPageTable("@deletedPages", (IEnumerable<WikiPageWithId>) deletedPages);
      this.BindInt("@associatedPushId", associatedPushId);
      this.ExecuteNonQuery();
    }

    public void UpdatePageIdIfNeeded(
      Guid projectId,
      Guid wikiId,
      string wikiVersion,
      IList<WikiPageWithId> addedPages,
      IList<WikiRenamedPage> renamedPages,
      IList<WikiPageWithId> deletedPages,
      int associatedPushId,
      out List<WikiPageWithId> addedPagesInDb,
      out List<WikiPageWithId> renamedPagesInDb,
      out List<WikiPageWithId> deletedPagesInDb)
    {
      if (this.Version < 1440)
      {
        this.UpdatePageId(projectId, wikiId, wikiVersion, addedPages, renamedPages, deletedPages, associatedPushId);
        addedPagesInDb = new List<WikiPageWithId>();
        renamedPagesInDb = new List<WikiPageWithId>();
        deletedPagesInDb = new List<WikiPageWithId>();
      }
      else
      {
        this.PrepareStoredProcedure("Wiki.prc_UpdatePageIdIfNeeded");
        this.BindDataspaceForProject(projectId);
        this.BindGuid("@wikiId", wikiId);
        this.BindString("@wikiVersion", wikiVersion, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
        this.BindBinary("@wikiVersionHash", this.GetSHA256Hash(wikiVersion), SqlDbType.VarBinary);
        this.BindWikiPageTable("@addedPages", (IEnumerable<WikiPageWithId>) addedPages);
        this.BindWikiRenamePageTable("@renamedPages", (IEnumerable<WikiRenamedPage>) renamedPages);
        this.BindWikiPageTable("@deletedPages", (IEnumerable<WikiPageWithId>) deletedPages);
        this.BindInt("@associatedPushId", associatedPushId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<WikiPageWithId>((ObjectBinder<WikiPageWithId>) new WikiComponent.WikiPageWithIdBinder(new int?(associatedPushId)));
          addedPagesInDb = resultCollection.GetCurrent<WikiPageWithId>().Items;
          resultCollection.NextResult();
          resultCollection.AddBinder<WikiPageWithId>((ObjectBinder<WikiPageWithId>) new WikiComponent.WikiPageWithIdBinder(new int?(associatedPushId)));
          renamedPagesInDb = resultCollection.GetCurrent<WikiPageWithId>().Items;
          if (this.Version < 1480)
          {
            deletedPagesInDb = new List<WikiPageWithId>();
          }
          else
          {
            resultCollection.NextResult();
            resultCollection.AddBinder<WikiPageWithId>((ObjectBinder<WikiPageWithId>) new WikiComponent.WikiPageWithIdBinder(new int?(associatedPushId)));
            deletedPagesInDb = resultCollection.GetCurrent<WikiPageWithId>().Items;
          }
        }
      }
    }

    public void UpdatePageViewBatch(List<PageView> pageViewsList)
    {
      if (this.Version < 1)
        return;
      this.PrepareStoredProcedure("Wiki.prc_UpdatePageViewBatch");
      this.BindPageViewTable("@pageViews", (IEnumerable<PageView>) pageViewsList);
      this.ExecuteNonQuery();
    }

    private SqlParameter BindPageViewTable(string parameterName, IEnumerable<PageView> rows)
    {
      rows = rows ?? Enumerable.Empty<PageView>();
      System.Func<PageView, SqlDataRecord> selector = (System.Func<PageView, SqlDataRecord>) (pageView =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(WikiComponent.Wiki_typ_PageViewItem);
        sqlDataRecord.SetInt32(0, pageView.DataspaceId);
        sqlDataRecord.SetString(1, pageView.WikiId);
        sqlDataRecord.SetString(2, pageView.Version);
        sqlDataRecord.SetString(3, pageView.PagePath);
        sqlDataRecord.SetInt32(4, pageView.ViewCount);
        sqlDataRecord.SetDateTime(5, pageView.LastViewed);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Wiki.typ_PageViewItem", rows.Select<PageView, SqlDataRecord>(selector));
    }

    private SqlParameter BindWikiPageTable(string parameterName, IEnumerable<WikiPageWithId> rows)
    {
      rows = rows ?? Enumerable.Empty<WikiPageWithId>();
      System.Func<WikiPageWithId, SqlDataRecord> selector = (System.Func<WikiPageWithId, SqlDataRecord>) (pageWithId =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(WikiComponent.Wiki_typ_Page);
        sqlDataRecord.SetInt32(0, pageWithId.PageId);
        sqlDataRecord.SetString(1, pageWithId.GitFriendlyPagePath);
        sqlDataRecord.SetBytes(2, 0L, this.GetSHA256Hash(pageWithId.GitFriendlyPagePath), 0, 32);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Wiki.typ_Page", rows.Select<WikiPageWithId, SqlDataRecord>(selector));
    }

    private SqlParameter BindWikiRenamePageTable(
      string parameterName,
      IEnumerable<WikiRenamedPage> rows)
    {
      rows = rows ?? Enumerable.Empty<WikiRenamedPage>();
      System.Func<WikiRenamedPage, SqlDataRecord> selector = (System.Func<WikiRenamedPage, SqlDataRecord>) (renamedPage =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(WikiComponent.Wiki_typ_RenamedPage);
        sqlDataRecord.SetString(0, renamedPage.OldGitFriendlyPagePath);
        sqlDataRecord.SetBytes(1, 0L, this.GetSHA256Hash(renamedPage.OldGitFriendlyPagePath), 0, 32);
        sqlDataRecord.SetString(2, renamedPage.NewGitFriendlyPagePath);
        sqlDataRecord.SetBytes(3, 0L, this.GetSHA256Hash(renamedPage.NewGitFriendlyPagePath), 0, 32);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Wiki.typ_RenamedPage", rows.Select<WikiRenamedPage, SqlDataRecord>(selector));
    }

    private byte[] GetSHA256Hash(string name)
    {
      name = name ?? string.Empty;
      byte[] bytes = Encoding.UTF8.GetBytes(name);
      using (SHA256Managed shA256Managed = new SHA256Managed())
        return shA256Managed.ComputeHash(bytes);
    }

    private class PageViewBinder : ObjectBinder<PageView>
    {
      private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
      private SqlColumnBinder m_wikiId = new SqlColumnBinder("WikiId");
      private SqlColumnBinder m_version = new SqlColumnBinder("Version");
      private SqlColumnBinder m_pagePath = new SqlColumnBinder("PagePath");
      private SqlColumnBinder m_viewCount = new SqlColumnBinder("ViewCount");
      private SqlColumnBinder m_lastViewed = new SqlColumnBinder("LastViewed");

      protected override PageView Bind() => new PageView(this.m_dataspaceId.GetInt32((IDataReader) this.Reader), this.m_wikiId.GetString((IDataReader) this.Reader, false), this.m_version.GetString((IDataReader) this.Reader, false), this.m_pagePath.GetString((IDataReader) this.Reader, false), this.m_viewCount.GetInt32((IDataReader) this.Reader), this.m_lastViewed.GetDateTime((IDataReader) this.Reader));
    }

    private class WikiPageWithIdBinder : ObjectBinder<WikiPageWithId>
    {
      private int? associatedPushId;
      private SqlColumnBinder m_pageId = new SqlColumnBinder("PageId");
      private SqlColumnBinder m_pagePath = new SqlColumnBinder("PagePath");
      private SqlColumnBinder m_associatedPushId = new SqlColumnBinder("AssociatedPushId");

      public WikiPageWithIdBinder(int? associatedPushId = null) => this.associatedPushId = associatedPushId;

      protected override WikiPageWithId Bind()
      {
        int associatedPushId = this.associatedPushId ?? this.m_associatedPushId.GetInt32((IDataReader) this.Reader);
        return new WikiPageWithId(this.m_pageId.GetInt32((IDataReader) this.Reader), this.m_pagePath.GetString((IDataReader) this.Reader, false), associatedPushId);
      }
    }

    private class WikiPageWithIdOnlyBinder : ObjectBinder<WikiPageWithId>
    {
      private SqlColumnBinder m_pageId = new SqlColumnBinder("Val");

      protected override WikiPageWithId Bind() => new WikiPageWithId(this.m_pageId.GetInt32((IDataReader) this.Reader));
    }

    private class WikiPageInfoBinder : ObjectBinder<WikiPageIdInfo>
    {
      private SqlColumnBinder m_pageId = new SqlColumnBinder("PageId");
      private SqlColumnBinder m_wikiId = new SqlColumnBinder("WikiId");
      private SqlColumnBinder m_wikiVersion = new SqlColumnBinder("WikiVersion");
      private SqlColumnBinder m_pagePath = new SqlColumnBinder("PagePath");
      private SqlColumnBinder m_associatedPushId = new SqlColumnBinder("AssociatedPushId");

      protected override WikiPageIdInfo Bind() => new WikiPageIdInfo(this.m_pageId.GetInt32((IDataReader) this.Reader), this.m_wikiId.GetGuid((IDataReader) this.Reader), this.m_wikiVersion.GetString((IDataReader) this.Reader, false), this.m_pagePath.GetString((IDataReader) this.Reader, false), this.m_associatedPushId.GetInt32((IDataReader) this.Reader));
    }

    private class WikiVersionWaterMarkBinder : ObjectBinder<WikiVersionWaterMark>
    {
      private SqlColumnBinder m_wikiId = new SqlColumnBinder("WikiId");
      private SqlColumnBinder m_wikiVersion = new SqlColumnBinder("WikiVersion");
      private SqlColumnBinder m_lastProcessedPushId = new SqlColumnBinder("LastProcessedPushId");

      protected override WikiVersionWaterMark Bind() => new WikiVersionWaterMark(this.m_wikiId.GetGuid((IDataReader) this.Reader), this.m_wikiVersion.GetString((IDataReader) this.Reader, false), this.m_lastProcessedPushId.GetInt32((IDataReader) this.Reader));
    }
  }
}
