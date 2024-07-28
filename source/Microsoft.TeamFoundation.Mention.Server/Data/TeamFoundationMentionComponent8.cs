// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.Data.TeamFoundationMentionComponent8
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Mention.Server.Data
{
  internal class TeamFoundationMentionComponent8 : TeamFoundationMentionComponent7
  {
    private static SqlMetaData[] typ_SourceTargetTable = new SqlMetaData[4]
    {
      new SqlMetaData("SourceId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("SourceDataspaceId", SqlDbType.Int),
      new SqlMetaData("SourceType", SqlDbType.NVarChar, 200L),
      new SqlMetaData("TargetId", SqlDbType.NVarChar, 200L)
    };
    private static readonly SqlMetaData[] typ_Int32Table = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.Int)
    };

    public override IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentionsByCommentId(
      ISet<int> commentIds)
    {
      try
      {
        this.TraceEnter(101251, nameof (GetMentionsByCommentId));
        this.PrepareStoredProcedure("prc_GetMentionsByCommentId");
        this.BindTable("@commentIds", "typ_Int32Table", commentIds.Select<int, SqlDataRecord>((System.Func<int, SqlDataRecord>) (commentId =>
        {
          SqlDataRecord mentionsByCommentId = new SqlDataRecord(TeamFoundationMentionComponent8.typ_Int32Table);
          mentionsByCommentId.SetInt32(0, commentId);
          return mentionsByCommentId;
        })));
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.TeamFoundation.Mention.Server.Mention>((ObjectBinder<Microsoft.TeamFoundation.Mention.Server.Mention>) new TeamFoundationMentionComponent8.MentionByCommentId2Binder(this));
        IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionsByCommentId1 = (IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>) resultCollection.GetCurrent<Microsoft.TeamFoundation.Mention.Server.Mention>().Items ?? Enumerable.Empty<Microsoft.TeamFoundation.Mention.Server.Mention>();
        foreach (Microsoft.TeamFoundation.Mention.Server.Mention mention in mentionsByCommentId1)
          mention.ProjectGuid = this.GetProjectGuidFromDataspaceId(mention.TargetDataspaceId);
        return mentionsByCommentId1;
      }
      catch (Exception ex)
      {
        this.TraceException(101260, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101259, nameof (GetMentionsByCommentId));
      }
    }

    public override void DeleteMentions(IEnumerable<MentionSourceExtended> sources)
    {
      try
      {
        this.TraceEnter(101223, nameof (DeleteMentions));
        this.PrepareStoredProcedure("prc_DeleteMentions");
        this.BindTable("@sourcesToDelete", "typ_SourceTargetTable", this.GetDataRecordFromSourceTable2(sources));
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(101225, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101224, nameof (DeleteMentions));
      }
    }

    protected IEnumerable<SqlDataRecord> GetDataRecordFromSourceTable2(
      IEnumerable<MentionSourceExtended> sources)
    {
      TeamFoundationMentionComponent8 mentionComponent8 = this;
      foreach (MentionSourceExtended source in sources)
      {
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(TeamFoundationMentionComponent8.typ_SourceTargetTable);
        int num1 = 0;
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        string sourceId = source.SourceId;
        sqlDataRecord2.SetString(ordinal1, sourceId);
        SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        int idFromProjectGuid = mentionComponent8.GetDataspaceIdFromProjectGuid(source.ProjectGuid);
        sqlDataRecord3.SetInt32(ordinal2, idFromProjectGuid);
        SqlDataRecord sqlDataRecord4 = sqlDataRecord1;
        int ordinal3 = num3;
        int ordinal4 = ordinal3 + 1;
        string sourceType = source.SourceType;
        sqlDataRecord4.SetString(ordinal3, sourceType);
        sqlDataRecord1.SetString(ordinal4, source.TargetId);
        yield return sqlDataRecord1;
      }
    }

    internal class MentionByCommentId2Binder : ObjectBinder<Microsoft.TeamFoundation.Mention.Server.Mention>
    {
      private TeamFoundationMentionComponent8 m_mentionComponentRef;
      internal SqlColumnBinder sourceId = new SqlColumnBinder("SourceId");
      internal SqlColumnBinder sourceType = new SqlColumnBinder("SourceType");
      internal SqlColumnBinder artifactId = new SqlColumnBinder("ArtifactId");
      internal SqlColumnBinder artifactType = new SqlColumnBinder("ArtifactType");
      internal SqlColumnBinder targetId = new SqlColumnBinder("TargetId");
      internal SqlColumnBinder commentId = new SqlColumnBinder("CommentId");
      internal SqlColumnBinder targetDataspaceId = new SqlColumnBinder("TargetDataspaceId");
      internal SqlColumnBinder sourceDataspaceId = new SqlColumnBinder("SourceDataspaceId");

      public MentionByCommentId2Binder(TeamFoundationMentionComponent8 mentionComponent) => this.m_mentionComponentRef = mentionComponent;

      protected override Microsoft.TeamFoundation.Mention.Server.Mention Bind()
      {
        MentionSourceContext mentionSourceContext = new MentionSourceContext()
        {
          Type = this.sourceType.GetString((IDataReader) this.Reader, false),
          Identifier = this.sourceId.GetString((IDataReader) this.Reader, false),
          ProjectGuid = this.m_mentionComponentRef.GetProjectGuidFromDataspaceId(this.sourceDataspaceId.GetInt32((IDataReader) this.Reader))
        };
        return new Microsoft.TeamFoundation.Mention.Server.Mention()
        {
          Source = (IMentionSourceContext) mentionSourceContext,
          ArtifactId = this.artifactId.GetString((IDataReader) this.Reader, false),
          ArtifactType = this.artifactType.GetString((IDataReader) this.Reader, false),
          CommentId = this.commentId.GetNullableInt32((IDataReader) this.Reader),
          TargetId = this.targetId.GetString((IDataReader) this.Reader, false),
          TargetDataspaceId = this.targetDataspaceId.GetInt32((IDataReader) this.Reader)
        };
      }
    }
  }
}
