// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.Data.TeamFoundationMentionComponent9
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
  internal class TeamFoundationMentionComponent9 : TeamFoundationMentionComponent8
  {
    private static readonly SqlMetaData[] typ_StringTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.NText)
    };

    public override void DeleteMentionsBySourceIds(ISet<string> sourceIds)
    {
      ArgumentUtility.CheckForNull<ISet<string>>(sourceIds, nameof (sourceIds));
      this.TraceEnter(101220, nameof (DeleteMentionsBySourceIds));
      try
      {
        this.PrepareStoredProcedure("prc_DeleteMentionsBySourceId");
        this.BindTable("@sourceIds", "typ_StringTable", sourceIds.Select<string, SqlDataRecord>((System.Func<string, SqlDataRecord>) (sourceId =>
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(TeamFoundationMentionComponent9.typ_StringTable);
          sqlDataRecord.SetString(0, sourceId);
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
        this.TraceLeave(101221, nameof (DeleteMentionsBySourceIds));
      }
    }

    public override IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentionsBySourceIds(
      ISet<string> sourceIds)
    {
      try
      {
        this.TraceEnter(101261, nameof (GetMentionsBySourceIds));
        this.PrepareStoredProcedure("prc_GetMentionsBySourceIds");
        this.BindTable("@sourceIds", "typ_StringTable", sourceIds.Select<string, SqlDataRecord>((System.Func<string, SqlDataRecord>) (sourceId =>
        {
          SqlDataRecord mentionsBySourceIds = new SqlDataRecord(TeamFoundationMentionComponent9.typ_StringTable);
          mentionsBySourceIds.SetString(0, sourceId);
          return mentionsBySourceIds;
        })));
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.TeamFoundation.Mention.Server.Mention>((ObjectBinder<Microsoft.TeamFoundation.Mention.Server.Mention>) new TeamFoundationMentionComponent8.MentionByCommentId2Binder((TeamFoundationMentionComponent8) this));
        IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentionsBySourceIds1 = (IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>) resultCollection.GetCurrent<Microsoft.TeamFoundation.Mention.Server.Mention>().Items ?? Enumerable.Empty<Microsoft.TeamFoundation.Mention.Server.Mention>();
        foreach (Microsoft.TeamFoundation.Mention.Server.Mention mention in mentionsBySourceIds1)
          mention.ProjectGuid = this.GetProjectGuidFromDataspaceId(mention.TargetDataspaceId);
        return mentionsBySourceIds1;
      }
      catch (Exception ex)
      {
        this.TraceException(101270, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101269, nameof (GetMentionsBySourceIds));
      }
    }
  }
}
