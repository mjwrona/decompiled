// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.Data.TeamFoundationMentionComponent7
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Mention.Server.Data
{
  internal class TeamFoundationMentionComponent7 : TeamFoundationMentionComponent6
  {
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
          SqlDataRecord mentionsByCommentId = new SqlDataRecord(TeamFoundationMentionComponent7.typ_Int32Table);
          mentionsByCommentId.SetInt32(0, commentId);
          return mentionsByCommentId;
        })));
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.TeamFoundation.Mention.Server.Mention>((ObjectBinder<Microsoft.TeamFoundation.Mention.Server.Mention>) new TeamFoundationMentionComponent7.MentionByCommentIdBinder());
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

    public override IEnumerable<MentionRecord> UpdateMentions(IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionObjects)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>>(mentionObjects, nameof (mentionObjects));
      this.TraceEnter(101241, nameof (UpdateMentions));
      try
      {
        this.PrepareStoredProcedure("prc_UpdateMentions");
        this.BindTable("@mentionTable", "typ_MentionTable5", this.GetDataRecordFromMentionTable5(mentionObjects));
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<MentionRecord>((ObjectBinder<MentionRecord>) new MentionRecordBinder());
        return (IEnumerable<MentionRecord>) resultCollection.GetCurrent<MentionRecord>().Items ?? Enumerable.Empty<MentionRecord>();
      }
      catch (Exception ex)
      {
        this.TraceException(101250, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101249, nameof (UpdateMentions));
      }
    }

    public override void DeleteMentionsByCommentId(ISet<int> commentIds)
    {
      ArgumentUtility.CheckForNull<ISet<int>>(commentIds, "mentionObjects");
      this.TraceEnter(101220, nameof (DeleteMentionsByCommentId));
      try
      {
        this.PrepareStoredProcedure("prc_DeleteMentionsByCommentId");
        this.BindTable("@commentIds", "typ_Int32Table", commentIds.Select<int, SqlDataRecord>((System.Func<int, SqlDataRecord>) (c =>
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(TeamFoundationMentionComponent7.typ_Int32Table);
          sqlDataRecord.SetInt32(0, c);
          return sqlDataRecord;
        })));
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(101222, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101221, nameof (DeleteMentionsByCommentId));
      }
    }

    public override void SaveMentions(IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionObjects)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>>(mentionObjects, nameof (mentionObjects));
      this.TraceEnter(101201, nameof (SaveMentions));
      try
      {
        this.PrepareStoredProcedure("prc_SaveMention");
        this.BindTable("@mentionTable", "typ_MentionTable5", this.GetDataRecordFromMentionTable5(mentionObjects));
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(101210, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101209, nameof (SaveMentions));
      }
    }

    internal class MentionByCommentIdBinder : ObjectBinder<Microsoft.TeamFoundation.Mention.Server.Mention>
    {
      internal SqlColumnBinder sourceId = new SqlColumnBinder("SourceId");
      internal SqlColumnBinder sourceType = new SqlColumnBinder("SourceType");
      internal SqlColumnBinder artifactId = new SqlColumnBinder("ArtifactId");
      internal SqlColumnBinder artifactType = new SqlColumnBinder("ArtifactType");
      internal SqlColumnBinder targetId = new SqlColumnBinder("TargetId");
      internal SqlColumnBinder commentId = new SqlColumnBinder("CommentId");
      internal SqlColumnBinder targetDataspaceId = new SqlColumnBinder("TargetDataspaceId");

      protected override Microsoft.TeamFoundation.Mention.Server.Mention Bind()
      {
        MentionSourceContext mentionSourceContext = new MentionSourceContext()
        {
          Type = this.sourceType.GetString((IDataReader) this.Reader, false),
          Identifier = this.sourceId.GetString((IDataReader) this.Reader, false)
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
