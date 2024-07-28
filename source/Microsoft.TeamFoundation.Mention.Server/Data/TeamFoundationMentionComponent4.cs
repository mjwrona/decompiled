// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.Data.TeamFoundationMentionComponent4
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
  internal class TeamFoundationMentionComponent4 : TeamFoundationMentionComponent3
  {
    private Dictionary<int, Guid> m_dataspaceIdToProjectGuidMap = new Dictionary<int, Guid>();
    private Dictionary<Guid, int> m_projectGuidToDataspaceIdMap = new Dictionary<Guid, int>();
    private static SqlMetaData[] typ_MentionTable2 = new SqlMetaData[9]
    {
      new SqlMetaData("SourceId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("SourceDataspaceId", SqlDbType.Int),
      new SqlMetaData("SourceType", SqlDbType.NVarChar, 200L),
      new SqlMetaData("RawText", SqlDbType.NVarChar, 200L),
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("ArtifactType", SqlDbType.NVarChar, 200L),
      new SqlMetaData("MentionAction", SqlDbType.NVarChar, 200L),
      new SqlMetaData("TargetId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("TargetDataspaceId", SqlDbType.Int)
    };
    private static SqlMetaData[] typ_SourceTable = new SqlMetaData[3]
    {
      new SqlMetaData("SourceId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("SourceDataspaceId", SqlDbType.Int),
      new SqlMetaData("SourceType", SqlDbType.NVarChar, 200L)
    };

    public override void DeleteMentions(IEnumerable<MentionSourceExtended> sources)
    {
      try
      {
        this.TraceEnter(101223, nameof (DeleteMentions));
        this.PrepareStoredProcedure("prc_DeleteMentions");
        this.BindTable("@sourcesToDelete", "typ_SourceTable", this.GetDataRecordFromSourceTable(sources.Select<MentionSourceExtended, MentionSource>((System.Func<MentionSourceExtended, MentionSource>) (s => new MentionSource()
        {
          SourceId = s.SourceId,
          ProjectGuid = s.ProjectGuid,
          SourceType = s.SourceType
        }))));
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

    public override void SaveMentions(IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionObjects)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>>(mentionObjects, nameof (mentionObjects));
      this.TraceEnter(101201, nameof (SaveMentions));
      try
      {
        this.PrepareStoredProcedure("prc_SaveMention");
        this.BindTable("@mentionTable", "typ_MentionTable2", this.GetDataRecordFromMentionTable2(mentionObjects));
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

    public override IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentions(
      IEnumerable<MentionSource> sources)
    {
      try
      {
        this.TraceEnter(101211, nameof (GetMentions));
        this.PrepareStoredProcedure("prc_GetMentions");
        this.BindTable("@sources", "typ_SourceTable", this.GetDataRecordFromSourceTable(sources));
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.TeamFoundation.Mention.Server.Mention>((ObjectBinder<Microsoft.TeamFoundation.Mention.Server.Mention>) new TeamFoundationMentionComponent4.MentionBinder2());
        IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentions = (IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>) resultCollection.GetCurrent<Microsoft.TeamFoundation.Mention.Server.Mention>().Items ?? Enumerable.Empty<Microsoft.TeamFoundation.Mention.Server.Mention>();
        foreach (Microsoft.TeamFoundation.Mention.Server.Mention mention in mentions)
          mention.ProjectGuid = this.GetProjectGuidFromDataspaceId(mention.TargetDataspaceId);
        return mentions;
      }
      catch (Exception ex)
      {
        this.TraceException(101910, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101219, nameof (GetMentions));
      }
    }

    protected IEnumerable<SqlDataRecord> GetDataRecordFromMentionTable2(
      IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionObjects)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>>(mentionObjects, nameof (mentionObjects));
      foreach (Microsoft.TeamFoundation.Mention.Server.Mention mentionObject in mentionObjects)
      {
        ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Mention.Server.Mention>(mentionObject, "mentionObject");
        mentionObject.TargetDataspaceId = this.GetDataspaceIdFromProjectGuid(mentionObject.ProjectGuid);
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(TeamFoundationMentionComponent4.typ_MentionTable2);
        int num1 = 0;
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        string identifier = mentionObject.Source.Identifier;
        sqlDataRecord2.SetString(ordinal1, identifier);
        SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        int idFromProjectGuid = this.GetDataspaceIdFromProjectGuid(mentionObject.Source.ProjectGuid);
        sqlDataRecord3.SetInt32(ordinal2, idFromProjectGuid);
        SqlDataRecord sqlDataRecord4 = sqlDataRecord1;
        int ordinal3 = num3;
        int num4 = ordinal3 + 1;
        string type = mentionObject.Source.Type;
        sqlDataRecord4.SetString(ordinal3, type);
        SqlDataRecord sqlDataRecord5 = sqlDataRecord1;
        int ordinal4 = num4;
        int num5 = ordinal4 + 1;
        string rawText = mentionObject.RawText;
        sqlDataRecord5.SetString(ordinal4, rawText);
        SqlDataRecord sqlDataRecord6 = sqlDataRecord1;
        int ordinal5 = num5;
        int num6 = ordinal5 + 1;
        string artifactId = mentionObject.ArtifactId;
        sqlDataRecord6.SetString(ordinal5, artifactId);
        SqlDataRecord sqlDataRecord7 = sqlDataRecord1;
        int ordinal6 = num6;
        int num7 = ordinal6 + 1;
        string artifactType = mentionObject.ArtifactType;
        sqlDataRecord7.SetString(ordinal6, artifactType);
        SqlDataRecord sqlDataRecord8 = sqlDataRecord1;
        int ordinal7 = num7;
        int num8 = ordinal7 + 1;
        string mentionAction = mentionObject.MentionAction;
        sqlDataRecord8.SetString(ordinal7, mentionAction);
        SqlDataRecord sqlDataRecord9 = sqlDataRecord1;
        int ordinal8 = num8;
        int ordinal9 = ordinal8 + 1;
        string targetId = mentionObject.TargetId;
        sqlDataRecord9.SetString(ordinal8, targetId);
        sqlDataRecord1.SetInt32(ordinal9, mentionObject.TargetDataspaceId);
        yield return sqlDataRecord1;
      }
    }

    protected IEnumerable<SqlDataRecord> GetDataRecordFromSourceTable(
      IEnumerable<MentionSource> sources)
    {
      foreach (MentionSource source in sources)
      {
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(TeamFoundationMentionComponent4.typ_SourceTable);
        int num1 = 0;
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        string sourceId = source.SourceId;
        sqlDataRecord2.SetString(ordinal1, sourceId);
        SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
        int ordinal2 = num2;
        int ordinal3 = ordinal2 + 1;
        int idFromProjectGuid = this.GetDataspaceIdFromProjectGuid(source.ProjectGuid);
        sqlDataRecord3.SetInt32(ordinal2, idFromProjectGuid);
        sqlDataRecord1.SetString(ordinal3, source.SourceType);
        yield return sqlDataRecord1;
      }
    }

    protected int GetDataspaceIdFromProjectGuid(Guid projectGuid)
    {
      if (!this.m_projectGuidToDataspaceIdMap.ContainsKey(projectGuid))
        this.m_projectGuidToDataspaceIdMap.Add(projectGuid, this.GetDataspaceId(projectGuid));
      return this.m_projectGuidToDataspaceIdMap[projectGuid];
    }

    protected Guid GetProjectGuidFromDataspaceId(int dataspaceId)
    {
      if (!this.m_dataspaceIdToProjectGuidMap.ContainsKey(dataspaceId))
        this.m_dataspaceIdToProjectGuidMap.Add(dataspaceId, this.GetDataspaceIdentifier(dataspaceId));
      return this.m_dataspaceIdToProjectGuidMap[dataspaceId];
    }

    internal class MentionBinder2 : TeamFoundationMentionComponent2.MentionBinder
    {
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
          RawText = this.rawText.GetString((IDataReader) this.Reader, false),
          MentionAction = this.mentionAction.GetString((IDataReader) this.Reader, false),
          ArtifactType = this.artifactType.GetString((IDataReader) this.Reader, false),
          TargetId = this.targetId.GetString((IDataReader) this.Reader, false),
          TargetDataspaceId = this.targetDataspaceId.GetInt32((IDataReader) this.Reader)
        };
      }
    }

    internal class MentionBinder3 : TeamFoundationMentionComponent4.MentionBinder2
    {
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
          RawText = this.rawText.GetString((IDataReader) this.Reader, false),
          MentionAction = this.mentionAction.GetString((IDataReader) this.Reader, false),
          ArtifactType = this.artifactType.GetString((IDataReader) this.Reader, false),
          TargetId = this.targetId.GetString((IDataReader) this.Reader, false),
          TargetDataspaceId = this.targetDataspaceId.GetInt32((IDataReader) this.Reader)
        };
      }
    }

    internal class MentionBinder4 : TeamFoundationMentionComponent4.MentionBinder3
    {
      internal SqlColumnBinder commentId = new SqlColumnBinder("CommentId");

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
          RawText = this.rawText.GetString((IDataReader) this.Reader, false),
          MentionAction = this.mentionAction.GetString((IDataReader) this.Reader, false),
          ArtifactType = this.artifactType.GetString((IDataReader) this.Reader, false),
          CommentId = this.commentId.GetNullableInt32((IDataReader) this.Reader),
          TargetId = this.targetId.GetString((IDataReader) this.Reader, false),
          TargetDataspaceId = this.targetDataspaceId.GetInt32((IDataReader) this.Reader)
        };
      }
    }
  }
}
