// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalUpdateQueryItemElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalUpdateQueryItemElement : DalQueryItemElement
  {
    public void JoinBatch(
      ElementGroup group,
      ServerQueryItem item,
      IVssRequestContext requestContext)
    {
      this.SetOutputs(0);
      this.SetGroup(group);
      if (!this.IsSchemaPartitioned)
        this.AppendExecProc("UpdateQueryItem", DalSqlElement.InlineGuid(item.Id), DalSqlElement.InlineOrDefault((object) item.New.ParentId), "@PersonId", this.ParamOrDefault((object) item.New.QueryName), this.ParamOrDefault((object) WiqlTransformUtils.TransformNamesToIds(requestContext, item.New.QueryText, false)), this.ParamOrDefault((object) item.New.Description), "@NowUtc", DalSqlElement.InlineBit(item.IsBackCompat), this.ParamOrDefault(item.New.OwnerTeamFoundationId == Guid.Empty ? (object) (string) null : (object) item.New.OwnerTeamFoundationId.ToString()));
      else if (this.Version >= 26)
      {
        int childrenUnderParent = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).MaxQueryItemChildrenUnderParent;
        List<string> stringList = new List<string>((IEnumerable<string>) new string[11]
        {
          "@partitionId",
          DalSqlElement.InlineGuid(item.Id),
          DalSqlElement.InlineOrDefault((object) item.New.ParentId),
          "@PersonId",
          this.ParamOrDefault((object) item.New.QueryName),
          this.ParamOrDefault((object) WiqlTransformUtils.TransformNamesToIds(requestContext, item.New.QueryText, false)),
          this.ParamOrDefault((object) item.New.Description),
          "@NowUtc",
          DalSqlElement.InlineBit(item.IsBackCompat),
          this.ParamOrDefault(item.New.OwnerTeamFoundationId == Guid.Empty ? (object) (string) null : (object) item.New.OwnerTeamFoundationId.ToString()),
          this.ParamOrDefault((object) childrenUnderParent)
        });
        if (this.Version >= 45)
          stringList.Add(this.ParamOrDefault((object) (string.IsNullOrEmpty(item.New.QueryText) ? new int?() : new int?((int) requestContext.GetService<WorkItemQueryService>().GetQueryType(requestContext, item.New.QueryText)))));
        this.AppendExecProc("UpdateQueryItem", stringList.ToArray());
      }
      else
        this.AppendExecProc("UpdateQueryItem", "@partitionId", DalSqlElement.InlineGuid(item.Id), DalSqlElement.InlineOrDefault((object) item.New.ParentId), "@PersonId", this.ParamOrDefault((object) item.New.QueryName), this.ParamOrDefault((object) WiqlTransformUtils.TransformNamesToIds(requestContext, item.New.QueryText, false)), this.ParamOrDefault((object) item.New.Description), "@NowUtc", DalSqlElement.InlineBit(item.IsBackCompat), this.ParamOrDefault(item.New.OwnerTeamFoundationId == Guid.Empty ? (object) (string) null : (object) item.New.OwnerTeamFoundationId.ToString()));
      if (item.New.ParentId.Equals(Guid.Empty) || item.New.ParentId.Equals(item.Existing.ParentId))
        return;
      Func<string, int> dataspaceMapper = (Func<string, int>) (token => this.GetDataspaceIdForSecurityToken(this.SqlBatch.Component, token));
      bool? isPublic = item.Existing.IsPublic;
      if (isPublic.Value)
      {
        isPublic = item.New.IsPublic;
        if (isPublic.HasValue)
        {
          isPublic = item.New.IsPublic;
          if (isPublic.Value)
          {
            string newToken = item.New.Parent.SecurityToken + QueryItemSecurityConstants.PathSeparator.ToString() + item.Id.ToString().ToUpperInvariant();
            string str = this.SqlBatch.AddParameterTable<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP.TokenRename>((WorkItemTrackingTableValueParameter<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP.TokenRename>) new TokenRenameTable3(dataspaceMapper, (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP.TokenRename>) new List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP.TokenRename>()
            {
              new Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP.TokenRename(item.SecurityToken, newToken, false, true)
            }));
            this.AppendExecProc("prc_pRenameTokens3", DalSqlElement.InlineInt(this.SqlBatch.RequestContext.ServiceHost.PartitionId), DalSqlElement.InlineGuid(QueryItemSecurityConstants.NamespaceGuid), str, DalSqlElement.InlineChar(QueryItemSecurityConstants.PathSeparator));
            return;
          }
        }
      }
      isPublic = item.Existing.IsPublic;
      if (!isPublic.Value)
        return;
      isPublic = item.New.IsPublic;
      if (!isPublic.HasValue)
        return;
      isPublic = item.New.IsPublic;
      if (isPublic.Value)
        return;
      string str1 = this.SqlBatch.AddParameterTable<string>((WorkItemTrackingTableValueParameter<string>) new TokenTable2(dataspaceMapper, (IEnumerable<string>) new List<string>()
      {
        item.SecurityToken
      }, true, QueryItemSecurityConstants.PathSeparator));
      this.AppendExecProc("prc_pRemoveAccessControlEntries2", DalSqlElement.InlineInt(this.SqlBatch.RequestContext.ServiceHost.PartitionId), DalSqlElement.InlineGuid(QueryItemSecurityConstants.NamespaceGuid), str1, DalSqlElement.InlineChar(QueryItemSecurityConstants.PathSeparator));
    }
  }
}
