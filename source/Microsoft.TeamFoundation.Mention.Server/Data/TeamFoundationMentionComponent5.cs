// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.Data.TeamFoundationMentionComponent5
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
  internal class TeamFoundationMentionComponent5 : TeamFoundationMentionComponent4
  {
    private static SqlMetaData[] typ_MentionTable3 = new SqlMetaData[10]
    {
      new SqlMetaData("SourceId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("SourceDataspaceId", SqlDbType.Int),
      new SqlMetaData("SourceType", SqlDbType.NVarChar, 200L),
      new SqlMetaData("RawText", SqlDbType.NVarChar, 200L),
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("ArtifactType", SqlDbType.NVarChar, 200L),
      new SqlMetaData("MentionAction", SqlDbType.NVarChar, 200L),
      new SqlMetaData("TargetId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("TargetDataspaceId", SqlDbType.Int),
      new SqlMetaData("NormalizedSourceId", SqlDbType.NVarChar, 200L)
    };
    private static SqlMetaData[] typ_TargetTable = new SqlMetaData[2]
    {
      new SqlMetaData("TargetId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("TargetType", SqlDbType.NVarChar, 200L)
    };

    public override void SaveMentions(IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionObjects)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>>(mentionObjects, nameof (mentionObjects));
      this.TraceEnter(101201, nameof (SaveMentions));
      try
      {
        this.PrepareStoredProcedure("prc_SaveMention");
        this.BindTable("@mentionTable", "typ_MentionTable3", this.GetDataRecordFromMentionTable3(mentionObjects));
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

    public override IEnumerable<LastMentionedInfo> GetRecentMentionsForTargets(
      Guid? sourceProjectId,
      string sourceType,
      int limit,
      DateTime dateLimit,
      IEnumerable<MentionTarget> targets)
    {
      ArgumentUtility.CheckForNull<string>(sourceType, nameof (sourceType));
      ArgumentUtility.CheckForNull<IEnumerable<MentionTarget>>(targets, nameof (targets));
      ArgumentUtility.CheckForNonPositiveInt(limit, nameof (limit));
      try
      {
        this.TraceEnter(101226, nameof (GetRecentMentionsForTargets));
        this.PrepareStoredProcedure("prc_GetLatestMentionsByTarget");
        this.BindNullableInt("@sourceDataSpaceId", sourceProjectId.HasValue ? new int?(this.GetDataspaceIdFromProjectGuid(sourceProjectId.Value)) : new int?());
        this.BindString("@sourceType", sourceType, 200, false, SqlDbType.NVarChar);
        this.BindInt("@limit", limit);
        this.BindDateTime2("@dateLimit", dateLimit);
        this.BindTable("@targets", "typ_TargetTable", this.GetDataRecordFromTargetTable(targets));
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<LastMentionedInfo>((ObjectBinder<LastMentionedInfo>) new TeamFoundationMentionComponent5.LastMentionedInfoBinder());
        return (IEnumerable<LastMentionedInfo>) resultCollection.GetCurrent<LastMentionedInfo>().Items ?? Enumerable.Empty<LastMentionedInfo>();
      }
      catch (Exception ex)
      {
        this.TraceException(101928, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101227, nameof (GetRecentMentionsForTargets));
      }
    }

    public override int CleanupMentions(DateTime cleanupDate)
    {
      try
      {
        this.TraceEnter(101229, nameof (CleanupMentions));
        this.PrepareStoredProcedure("prc_CleanupMentions");
        this.BindDateTime2("@cleanupDate", cleanupDate);
        return (int) this.ExecuteScalar();
      }
      catch (Exception ex)
      {
        this.TraceException(101230, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101229, nameof (CleanupMentions));
      }
    }

    protected IEnumerable<SqlDataRecord> GetDataRecordFromMentionTable3(
      IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionObjects)
    {
      TeamFoundationMentionComponent5 mentionComponent5 = this;
      foreach (Microsoft.TeamFoundation.Mention.Server.Mention mentionObject in mentionObjects)
      {
        mentionObject.TargetDataspaceId = mentionComponent5.GetDataspaceIdFromProjectGuid(mentionObject.ProjectGuid);
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(TeamFoundationMentionComponent5.typ_MentionTable3);
        int num1 = 0;
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        string identifier = mentionObject.Source.Identifier;
        sqlDataRecord2.SetString(ordinal1, identifier);
        SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        int idFromProjectGuid = mentionComponent5.GetDataspaceIdFromProjectGuid(mentionObject.Source.ProjectGuid);
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
        int num9 = ordinal8 + 1;
        string targetId = mentionObject.TargetId;
        sqlDataRecord9.SetString(ordinal8, targetId);
        SqlDataRecord sqlDataRecord10 = sqlDataRecord1;
        int ordinal9 = num9;
        int ordinal10 = ordinal9 + 1;
        int targetDataspaceId = mentionObject.TargetDataspaceId;
        sqlDataRecord10.SetInt32(ordinal9, targetDataspaceId);
        sqlDataRecord1.SetString(ordinal10, mentionObject.Source.NormalizedId ?? string.Empty);
        yield return sqlDataRecord1;
      }
    }

    protected IEnumerable<SqlDataRecord> GetDataRecordFromTargetTable(
      IEnumerable<MentionTarget> targets)
    {
      foreach (MentionTarget target in targets)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TeamFoundationMentionComponent5.typ_TargetTable);
        sqlDataRecord.SetString(0, target.TargetId);
        sqlDataRecord.SetString(1, target.ArtifactType);
        yield return sqlDataRecord;
      }
    }

    internal class LastMentionedInfoBinder : ObjectBinder<LastMentionedInfo>
    {
      internal SqlColumnBinder normalizedSourceId = new SqlColumnBinder("NormalizedSourceId");
      internal SqlColumnBinder lastMentionedDate = new SqlColumnBinder("LastMentionedDate");
      internal SqlColumnBinder targetId = new SqlColumnBinder("TargetId");
      internal SqlColumnBinder artifactType = new SqlColumnBinder("ArtifactType");

      protected override LastMentionedInfo Bind() => new LastMentionedInfo()
      {
        NormalizedSourceId = this.normalizedSourceId.GetString((IDataReader) this.Reader, false),
        LastMentionedDate = this.lastMentionedDate.GetDateTime((IDataReader) this.Reader),
        TargetId = this.targetId.GetString((IDataReader) this.Reader, false),
        ArtifactType = this.artifactType.GetString((IDataReader) this.Reader, false)
      };
    }
  }
}
