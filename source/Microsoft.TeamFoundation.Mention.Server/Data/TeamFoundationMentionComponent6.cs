// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.Data.TeamFoundationMentionComponent6
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Mention.Server.Data
{
  internal class TeamFoundationMentionComponent6 : TeamFoundationMentionComponent5
  {
    private static SqlMetaData[] typ_MentionTable4 = new SqlMetaData[11]
    {
      new SqlMetaData("SourceId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("SourceDataspaceId", SqlDbType.Int),
      new SqlMetaData("SourceType", SqlDbType.NVarChar, 200L),
      new SqlMetaData("RawText", SqlDbType.NVarChar, 200L),
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("ArtifactType", SqlDbType.NVarChar, 200L),
      new SqlMetaData("MentionAction", SqlDbType.NVarChar, 200L),
      new SqlMetaData("TargetId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("StorageKey", SqlDbType.UniqueIdentifier),
      new SqlMetaData("TargetDataspaceId", SqlDbType.Int),
      new SqlMetaData("NormalizedSourceId", SqlDbType.NVarChar, 200L)
    };
    private static SqlMetaData[] typ_MentionTable5 = new SqlMetaData[12]
    {
      new SqlMetaData("SourceId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("SourceDataspaceId", SqlDbType.Int),
      new SqlMetaData("SourceType", SqlDbType.NVarChar, 200L),
      new SqlMetaData("RawText", SqlDbType.NVarChar, 200L),
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("ArtifactType", SqlDbType.NVarChar, 200L),
      new SqlMetaData("CommentId", SqlDbType.Int),
      new SqlMetaData("MentionAction", SqlDbType.NVarChar, 200L),
      new SqlMetaData("TargetId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("StorageKey", SqlDbType.UniqueIdentifier),
      new SqlMetaData("TargetDataspaceId", SqlDbType.Int),
      new SqlMetaData("NormalizedSourceId", SqlDbType.NVarChar, 200L)
    };
    private static SqlMetaData[] typ_MentionActionTable = new SqlMetaData[1]
    {
      new SqlMetaData("MentionAction", SqlDbType.NVarChar, 200L)
    };

    public override void SaveMentions(IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionObjects)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>>(mentionObjects, nameof (mentionObjects));
      this.TraceEnter(101201, nameof (SaveMentions));
      try
      {
        this.PrepareStoredProcedure("prc_SaveMention");
        this.BindTable("@mentionTable", "typ_MentionTable4", this.GetDataRecordFromMentionTable4(mentionObjects));
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

    public override IEnumerable<MentionActionResult> GetPersonMentions(int batchSize)
    {
      this.TraceEnter(101229, nameof (GetPersonMentions));
      try
      {
        this.PrepareStoredProcedure("prc_GetPersonMentions");
        this.BindInt("@batchSize", batchSize);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<MentionActionResult>((ObjectBinder<MentionActionResult>) new TeamFoundationMentionComponent6.MentionActionResultBinder());
        return (IEnumerable<MentionActionResult>) resultCollection.GetCurrent<MentionActionResult>().Items ?? Enumerable.Empty<MentionActionResult>();
      }
      catch (Exception ex)
      {
        this.TraceException(101231, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101230, nameof (GetPersonMentions));
      }
    }

    public override void UpdateStorageKeys(
      IEnumerable<MentionStorageKey> mentionStorageKeys,
      int batchSize)
    {
      ArgumentUtility.CheckForNull<IEnumerable<MentionStorageKey>>(mentionStorageKeys, nameof (mentionStorageKeys));
      this.TraceEnter(101232, nameof (UpdateStorageKeys));
      try
      {
        this.PrepareStoredProcedure("prc_UpdateStorageKeys");
        this.BindInt("@batchSize", batchSize);
        List<KeyValuePair<Guid, string>> rows = new List<KeyValuePair<Guid, string>>();
        foreach (MentionStorageKey mentionStorageKey in mentionStorageKeys)
          rows.Add(new KeyValuePair<Guid, string>(mentionStorageKey.StorageKey ?? Guid.Empty, mentionStorageKey.TargetId));
        this.BindKeyValuePairGuidStringTable("@mentionStorageKeys", (IEnumerable<KeyValuePair<Guid, string>>) rows);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(101234, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101233, nameof (UpdateStorageKeys));
      }
    }

    public override int ExecuteMentionUpdateScript(string sproc, int batchSize)
    {
      this.TraceEnter(101235, nameof (ExecuteMentionUpdateScript));
      try
      {
        this.PrepareStoredProcedure(sproc, 3600);
        this.BindInt("@batchSize", batchSize);
        SqlParameter sqlParameter = this.Command.Parameters.Add("@processedCount", SqlDbType.Int);
        sqlParameter.Direction = ParameterDirection.Output;
        this.ExecuteNonQuery();
        return (int) sqlParameter.Value;
      }
      catch (Exception ex)
      {
        this.TraceException(101237, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101236, nameof (ExecuteMentionUpdateScript));
      }
    }

    public override int ExecuteMentionScriptToUpdateTargetIdAndContent(
      string sproc,
      int batchSize,
      IDictionary<int, string> mentionIdTargetIdValues)
    {
      this.TraceEnter(101238, nameof (ExecuteMentionScriptToUpdateTargetIdAndContent));
      try
      {
        this.PrepareStoredProcedure(sproc, 3600);
        this.BindInt("@batchSize", batchSize);
        this.BindKeyValuePairInt32StringTable("@mentionIdTargetIdValues", (IEnumerable<KeyValuePair<int, string>>) mentionIdTargetIdValues);
        SqlParameter sqlParameter = this.Command.Parameters.Add("@processedCount", SqlDbType.Int);
        sqlParameter.Direction = ParameterDirection.Output;
        this.ExecuteNonQuery();
        return (int) sqlParameter.Value;
      }
      catch (Exception ex)
      {
        this.TraceException(101240, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101239, nameof (ExecuteMentionScriptToUpdateTargetIdAndContent));
      }
    }

    protected IEnumerable<SqlDataRecord> GetDataRecordFromMentionActionTable(
      IEnumerable<string> mentionActions)
    {
      foreach (string mentionAction in mentionActions)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TeamFoundationMentionComponent6.typ_MentionActionTable);
        int ordinal = 0;
        sqlDataRecord.SetString(ordinal, mentionAction);
        yield return sqlDataRecord;
      }
    }

    protected IEnumerable<SqlDataRecord> GetDataRecordFromMentionTable4(
      IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionObjects)
    {
      TeamFoundationMentionComponent6 mentionComponent6 = this;
      foreach (Microsoft.TeamFoundation.Mention.Server.Mention mentionObject in mentionObjects)
      {
        mentionObject.TargetDataspaceId = mentionComponent6.GetDataspaceIdFromProjectGuid(mentionObject.ProjectGuid);
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(TeamFoundationMentionComponent6.typ_MentionTable4);
        int num1 = 0;
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        string identifier = mentionObject.Source.Identifier;
        sqlDataRecord2.SetString(ordinal1, identifier);
        SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        int idFromProjectGuid = mentionComponent6.GetDataspaceIdFromProjectGuid(mentionObject.Source.ProjectGuid);
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
        SqlDataRecord record = sqlDataRecord1;
        int ordinal9 = num9;
        int num10 = ordinal9 + 1;
        Guid? storageKey = mentionObject.StorageKey;
        record.SetNullableGuid(ordinal9, storageKey);
        SqlDataRecord sqlDataRecord10 = sqlDataRecord1;
        int ordinal10 = num10;
        int ordinal11 = ordinal10 + 1;
        int targetDataspaceId = mentionObject.TargetDataspaceId;
        sqlDataRecord10.SetInt32(ordinal10, targetDataspaceId);
        sqlDataRecord1.SetString(ordinal11, mentionObject.Source.NormalizedId ?? string.Empty);
        yield return sqlDataRecord1;
      }
    }

    protected IEnumerable<SqlDataRecord> GetDataRecordFromMentionTable5(
      IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionObjects)
    {
      TeamFoundationMentionComponent6 mentionComponent6 = this;
      foreach (Microsoft.TeamFoundation.Mention.Server.Mention mentionObject in mentionObjects)
      {
        mentionObject.TargetDataspaceId = mentionComponent6.GetDataspaceIdFromProjectGuid(mentionObject.ProjectGuid);
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(TeamFoundationMentionComponent6.typ_MentionTable5);
        int num1 = 0;
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        string identifier = mentionObject.Source.Identifier;
        sqlDataRecord2.SetString(ordinal1, identifier);
        SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        int idFromProjectGuid = mentionComponent6.GetDataspaceIdFromProjectGuid(mentionObject.Source.ProjectGuid);
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
        SqlDataRecord record1 = sqlDataRecord1;
        int ordinal7 = num7;
        int num8 = ordinal7 + 1;
        int? commentId = mentionObject.CommentId;
        record1.SetNullableInt32(ordinal7, commentId);
        SqlDataRecord sqlDataRecord8 = sqlDataRecord1;
        int ordinal8 = num8;
        int num9 = ordinal8 + 1;
        string mentionAction = mentionObject.MentionAction;
        sqlDataRecord8.SetString(ordinal8, mentionAction);
        SqlDataRecord sqlDataRecord9 = sqlDataRecord1;
        int ordinal9 = num9;
        int num10 = ordinal9 + 1;
        string targetId = mentionObject.TargetId;
        sqlDataRecord9.SetString(ordinal9, targetId);
        SqlDataRecord record2 = sqlDataRecord1;
        int ordinal10 = num10;
        int num11 = ordinal10 + 1;
        Guid? storageKey = mentionObject.StorageKey;
        record2.SetNullableGuid(ordinal10, storageKey);
        SqlDataRecord sqlDataRecord10 = sqlDataRecord1;
        int ordinal11 = num11;
        int ordinal12 = ordinal11 + 1;
        int targetDataspaceId = mentionObject.TargetDataspaceId;
        sqlDataRecord10.SetInt32(ordinal11, targetDataspaceId);
        sqlDataRecord1.SetString(ordinal12, mentionObject.Source.NormalizedId ?? string.Empty);
        yield return sqlDataRecord1;
      }
    }

    internal class MentionActionResultBinder : ObjectBinder<MentionActionResult>
    {
      internal SqlColumnBinder targetId = new SqlColumnBinder("TargetId");
      internal SqlColumnBinder storageKey = new SqlColumnBinder("StorageKey");

      protected override MentionActionResult Bind() => new MentionActionResult()
      {
        TargetId = this.targetId.GetString((IDataReader) this.Reader, false),
        StorageKey = new Guid?(this.storageKey.GetGuid((IDataReader) this.Reader, true))
      };
    }
  }
}
