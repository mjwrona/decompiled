// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalInsertQueryItemElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalInsertQueryItemElement : DalSqlElement
  {
    public void JoinBatch(
      ElementGroup group,
      ServerQueryItem item,
      IVssRequestContext requestContext)
    {
      string empty = string.Empty;
      this.SetOutputs(0);
      this.SetGroup(group);
      this.AppendSql("exec dbo.[");
      this.AppendSql("AddQueryItem");
      this.AppendSql("] ");
      this.AppendPartitionIdVariable();
      this.AppendSql(DalSqlElement.InlineGuid(item.Id));
      this.AppendSql(", ");
      if (item.New.ParentId != Guid.Empty)
      {
        this.AppendSql(DalSqlElement.InlineGuid(item.New.ParentId));
        this.AppendSql(", default, default");
      }
      else
      {
        bool? isPublic = item.New.IsPublic;
        if (isPublic.HasValue)
        {
          this.AppendSql("default, ");
          if (this.Version >= 30)
          {
            Guid cssNodeId = requestContext.WitContext().TreeService.LegacyGetTreeNode(item.New.ProjectId).CssNodeId;
            this.AppendSql(DalSqlElement.InlineInt(this.GetDataspaceId(requestContext, cssNodeId)));
          }
          else
            this.AppendSql(DalSqlElement.InlineInt(item.New.ProjectId));
          this.AppendSql(", ");
          isPublic = item.New.IsPublic;
          this.AppendSql(DalSqlElement.InlineBit(isPublic.Value));
        }
      }
      this.AppendSql(", ");
      this.AppendSql("@PersonId");
      this.AppendSql(", ");
      this.AppendSql(this.SqlBatch.AddParameterNVarChar(item.New.QueryName));
      this.AppendSql(", ");
      if (!string.IsNullOrEmpty(item.New.QueryText))
        this.AppendSql(this.SqlBatch.AddParameterNVarChar(WiqlTransformUtils.TransformNamesToIds(requestContext, item.New.QueryText, false)));
      else
        this.AppendSql("default");
      this.AppendSql(", ");
      if (this.Version >= 26)
      {
        this.AppendSql(this.SqlBatch.AddParameterInt(requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).MaxQueryItemChildrenUnderParent));
        this.AppendSql(", ");
      }
      if (!string.IsNullOrEmpty(item.New.Description))
        this.AppendSql(this.SqlBatch.AddParameterNVarChar(item.New.Description));
      else
        this.AppendSql("default");
      this.AppendSql(", ");
      this.AppendSql("@NowUtc");
      this.AppendSql(", ");
      this.AppendSql(DalSqlElement.InlineBit(item.IsBackCompat));
      this.AppendSql(", ");
      this.AppendSql(this.ParamOrDefault(item.New.OwnerTeamFoundationId == Guid.Empty ? (object) (string) null : (object) item.New.OwnerTeamFoundationId.ToString()));
      if (this.Version >= 45)
      {
        this.AppendSql(", ");
        if (!string.IsNullOrEmpty(item.New.QueryText))
          this.AppendSql(this.SqlBatch.AddParameterInt((int) requestContext.GetService<WorkItemQueryService>().GetQueryType(requestContext, item.New.QueryText)));
        else
          this.AppendSql("default");
      }
      this.AppendSql(" if @@trancount = 0 return");
      this.AppendSql(Environment.NewLine);
    }
  }
}
