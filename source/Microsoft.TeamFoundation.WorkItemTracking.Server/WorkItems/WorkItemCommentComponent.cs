// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemCommentComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemCommentComponent : WorkItemTrackingResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<WorkItemCommentComponent>(1530)
    }, "WorkItemComments", "WorkItem");

    public virtual WorkItemComment GetWorkItemComment(int workItemId, int revision)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemComment2");
      this.BindInt("@workItemId", workItemId);
      this.BindInt("@revision", revision);
      IDataReader reader = this.ExecuteReader();
      return this.GetWorkItemCommentBinder().BindAll(reader).FirstOrDefault<WorkItemComment>();
    }

    public virtual WorkItemComments GetWorkItemComments(
      int workItemId,
      int fromRevision,
      int count,
      CommentSortOrder sort)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemComments2");
      this.BindInt("@workItemId", workItemId);
      this.BindInt("@fromRevision", fromRevision);
      this.BindInt("@count", count);
      this.BindBoolean("@sort", sort == CommentSortOrder.Asc);
      IDataReader reader = this.ExecuteReader();
      WorkItemComment[] array = this.GetWorkItemCommentBinder().BindAll(reader).ToArray<WorkItemComment>();
      reader.NextResult();
      reader.Read();
      int int32_1 = reader.GetInt32(0);
      int int32_2 = reader.GetInt32(1);
      int totalCount = int32_1;
      int fromRevisionCount = int32_2;
      return new WorkItemComments((IEnumerable<WorkItemComment>) array, totalCount, fromRevisionCount);
    }

    protected virtual WorkItemCommentComponent.WorkItemCommentBinder GetWorkItemCommentBinder() => new WorkItemCommentComponent.WorkItemCommentBinder();

    protected class WorkItemCommentBinder : WorkItemTrackingObjectBinder<WorkItemComment>
    {
      private SqlColumnBinder Id = new SqlColumnBinder("System.Id");
      private SqlColumnBinder Text = new SqlColumnBinder(nameof (Text));
      private SqlColumnBinder Rev = new SqlColumnBinder("System.Rev");
      private SqlColumnBinder IdentityDisplayName = new SqlColumnBinder("System.CreatedBy_IdentityDisplayName");
      private SqlColumnBinder IdentityDisplayPart = new SqlColumnBinder("System.CreatedBy_IdentityDisplayPart");
      private SqlColumnBinder IdentityTeamFoundationId = new SqlColumnBinder("System.CreatedBy_IdentityTeamFoundationId");
      private SqlColumnBinder ChangedDate = new SqlColumnBinder("System.ChangedDate");
      private SqlColumnBinder Format = new SqlColumnBinder(nameof (Format));
      private SqlColumnBinder RenderedText = new SqlColumnBinder(nameof (RenderedText));

      public override WorkItemComment Bind(IDataReader reader)
      {
        int int32_1 = this.Id.GetInt32(reader);
        string str1 = this.Text.GetString(reader, true);
        int int32_2 = this.Rev.GetInt32(reader);
        string str2 = this.IdentityDisplayName.GetString(reader, true);
        string str3 = this.IdentityDisplayPart.GetString(reader, true);
        Guid guid = this.IdentityTeamFoundationId.GetGuid(reader, true);
        DateTime dateTime = this.ChangedDate.GetDateTime(reader);
        string str4 = this.RenderedText.GetString(reader, (string) null);
        byte num = 1;
        CommentFormat commentFormat = (CommentFormat) this.Format.GetByte(reader, num, num);
        string text = str1;
        int revisionId = int32_2;
        string createdByDisplayName = str2;
        string createdByDisplayPart = str3;
        Guid createdByTeamFoundationId = guid;
        DateTime revisionDate = dateTime;
        return new WorkItemComment(int32_1, text, revisionId, createdByDisplayName, createdByDisplayPart, createdByTeamFoundationId, revisionDate)
        {
          RenderedText = str4,
          Format = commentFormat
        };
      }
    }
  }
}
