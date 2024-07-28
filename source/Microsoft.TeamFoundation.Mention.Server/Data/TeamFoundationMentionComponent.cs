// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.Data.TeamFoundationMentionComponent
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
  public class TeamFoundationMentionComponent : TeamFoundationSqlResourceComponent
  {
    private static SqlMetaData[] typ_MentionTable = new SqlMetaData[7]
    {
      new SqlMetaData("SourceId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("SourceType", SqlDbType.NVarChar, 200L),
      new SqlMetaData("RawText", SqlDbType.NVarChar, 200L),
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("ArtifactType", SqlDbType.NVarChar, 200L),
      new SqlMetaData("MentionAction", SqlDbType.NVarChar, 200L),
      new SqlMetaData("TargetId", SqlDbType.NVarChar, 200L)
    };
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[9]
    {
      (IComponentCreator) new ComponentCreator<TeamFoundationMentionComponent>(1),
      (IComponentCreator) new ComponentCreator<TeamFoundationMentionComponent2>(2),
      (IComponentCreator) new ComponentCreator<TeamFoundationMentionComponent3>(3),
      (IComponentCreator) new ComponentCreator<TeamFoundationMentionComponent4>(4),
      (IComponentCreator) new ComponentCreator<TeamFoundationMentionComponent5>(5),
      (IComponentCreator) new ComponentCreator<TeamFoundationMentionComponent6>(6),
      (IComponentCreator) new ComponentCreator<TeamFoundationMentionComponent7>(7),
      (IComponentCreator) new ComponentCreator<TeamFoundationMentionComponent8>(8),
      (IComponentCreator) new ComponentCreator<TeamFoundationMentionComponent9>(9)
    }, "Mention");

    public TeamFoundationMentionComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    public virtual void SaveMentions(IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionObjects)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>>(mentionObjects, nameof (mentionObjects));
      this.TraceEnter(101201, nameof (SaveMentions));
      try
      {
        this.PrepareStoredProcedure("prc_SaveMention");
        this.BindMentionTable(mentionObjects);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(101209, nameof (SaveMentions));
      }
    }

    private SqlParameter BindMentionTable(IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentions) => this.BindTable("@mentionTable", "typ_MentionTable", mentions.Select<Microsoft.TeamFoundation.Mention.Server.Mention, SqlDataRecord>((System.Func<Microsoft.TeamFoundation.Mention.Server.Mention, SqlDataRecord>) (mention =>
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(TeamFoundationMentionComponent.typ_MentionTable);
      int ordinal1 = 0;
      int num1 = ordinal1 + 1;
      sqlDataRecord.SetString(ordinal1, mention.Source.Identifier);
      int ordinal2 = num1;
      int num2 = ordinal2 + 1;
      sqlDataRecord.SetString(ordinal2, mention.Source.Type);
      int ordinal3 = num2;
      int num3 = ordinal3 + 1;
      sqlDataRecord.SetString(ordinal3, mention.RawText);
      int ordinal4 = num3;
      int num4 = ordinal4 + 1;
      sqlDataRecord.SetString(ordinal4, mention.ArtifactId);
      int ordinal5 = num4;
      int num5 = ordinal5 + 1;
      sqlDataRecord.SetString(ordinal5, mention.ArtifactType);
      int ordinal6 = num5;
      int ordinal7 = ordinal6 + 1;
      sqlDataRecord.SetString(ordinal6, mention.MentionAction);
      sqlDataRecord.SetString(ordinal7, mention.TargetId);
      return sqlDataRecord;
    })));

    public virtual IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentions(
      IEnumerable<MentionSource> sources)
    {
      return Enumerable.Empty<Microsoft.TeamFoundation.Mention.Server.Mention>();
    }

    public virtual IEnumerable<LastMentionedInfo> GetRecentMentionsForTargets(
      Guid? sourceProjectId,
      string sourceType,
      int limit,
      DateTime dateLimit,
      IEnumerable<MentionTarget> targets)
    {
      return Enumerable.Empty<LastMentionedInfo>();
    }

    public virtual void DeleteMentions(IEnumerable<MentionSourceExtended> sources)
    {
    }

    public virtual int CleanupMentions(DateTime cleanupDate) => 0;

    public virtual IEnumerable<MentionActionResult> GetPersonMentions(int batchSize) => Enumerable.Empty<MentionActionResult>();

    public virtual void UpdateStorageKeys(
      IEnumerable<MentionStorageKey> mentionStorageKeys,
      int batchSize)
    {
    }

    public virtual int ExecuteMentionUpdateScript(string sproc, int batchSize) => -1;

    public virtual int ExecuteMentionScriptToUpdateTargetIdAndContent(
      string sproc,
      int batchSize,
      IDictionary<int, string> mentionIdTargetIdValues)
    {
      return -1;
    }

    public virtual IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentionsByCommentId(
      ISet<int> commentIds)
    {
      return Enumerable.Empty<Microsoft.TeamFoundation.Mention.Server.Mention>();
    }

    public virtual IEnumerable<MentionRecord> UpdateMentions(IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionObjects) => Enumerable.Empty<MentionRecord>();

    public virtual void DeleteMentionsByCommentId(ISet<int> commentIds)
    {
    }

    public virtual void DeleteMentionsBySourceIds(ISet<string> sourceIds)
    {
    }

    public virtual IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentionsBySourceIds(
      ISet<string> sourceIds)
    {
      return Enumerable.Empty<Microsoft.TeamFoundation.Mention.Server.Mention>();
    }
  }
}
