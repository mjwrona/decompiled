// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalPopulateSecurityInfoElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalPopulateSecurityInfoElement : DalSqlElement
  {
    public void JoinBatch(
      IVssRequestContext requestContext,
      IVssIdentity userIdentity,
      IEnumerable<Guid> itemIds,
      IEnumerable<int> projectIds,
      bool isAnonymousOrPublicUser = false)
    {
      string empty = string.Empty;
      this.m_outputs = 1;
      this.m_index = this.SqlBatch.AddExpectedReturnedDataTables(this.m_outputs);
      this.SqlBatch.AppendSql("exec dbo.[");
      this.SqlBatch.AppendSql("GetQueryItemSecurityInfo");
      this.SqlBatch.AppendSql("] ");
      this.AppendPartitionIdVariable();
      if (this.Version < 42)
      {
        this.SqlBatch.AppendSql(this.SqlBatch.AddParameterNVarChar(userIdentity.Descriptor.Identifier));
        this.SqlBatch.AppendSql(", ");
      }
      else
      {
        this.SqlBatch.AppendSql(this.SqlBatch.AddParameterUniqueIdentifier(isAnonymousOrPublicUser ? Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA") : userIdentity.Id));
        this.SqlBatch.AppendSql(", ");
      }
      this.SqlBatch.AppendSql(this.SqlBatch.AddParameterTable<Guid>((WorkItemTrackingTableValueParameter<Guid>) new GuidTable(itemIds)));
      this.SqlBatch.AppendSql(", ");
      if (this.Version >= 30)
      {
        ITreeDictionary treeService = requestContext.WitContext().TreeService;
        this.SqlBatch.AppendSql(this.SqlBatch.AddParameterTable<int>((WorkItemTrackingTableValueParameter<int>) new Int32Table((IEnumerable<int>) projectIds.Select<int, Guid>((Func<int, Guid>) (pid => treeService.LegacyGetTreeNode(pid).CssNodeId)).ToList<Guid>().Select<Guid, int>((Func<Guid, int>) (projectGuid => this.GetDataspaceId(requestContext, projectGuid))).ToList<int>())));
      }
      else
        this.SqlBatch.AppendSql(this.SqlBatch.AddParameterTable<int>((WorkItemTrackingTableValueParameter<int>) new Int32Table(projectIds)));
      this.SqlBatch.AppendSql(Environment.NewLine);
    }

    public PayloadTable GetResults()
    {
      PayloadTable table = this.SqlBatch.ResultPayload.Tables[this.m_index];
      this.SqlBatch.ResultPayload.Tables.Remove(table);
      return table;
    }
  }
}
