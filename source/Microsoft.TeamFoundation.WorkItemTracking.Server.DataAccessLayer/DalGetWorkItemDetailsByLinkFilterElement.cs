// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalGetWorkItemDetailsByLinkFilterElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalGetWorkItemDetailsByLinkFilterElement : DalGetWorkItemDetailsElementBase
  {
    public void JoinBatch(
      string uri,
      int artifactType,
      string artifactLinkTypeName,
      IVssIdentity user)
    {
      this.m_outputs = 1;
      this.m_index = this.SqlBatch.AddExpectedReturnedDataTables(this.m_outputs);
      this.SqlBatch.AppendSql("exec dbo.");
      this.SqlBatch.AppendSql("GetWorkItemDetailsByLinkFilter");
      this.SqlBatch.AppendSql(" ");
      this.AppendPartitionIdVariable();
      this.SqlBatch.AppendSql(this.SqlBatch.AddParameterNVarChar(uri));
      this.SqlBatch.AppendSql(",");
      this.SqlBatch.AppendSql(DalSqlElement.InlineInt(artifactType));
      this.SqlBatch.AppendSql(",");
      this.SqlBatch.AppendSql(this.SqlBatch.AddParameterNVarChar(artifactLinkTypeName));
      if (this.Version < 8)
      {
        this.SqlBatch.AppendSql(",");
        this.SqlBatch.AppendSql(this.SqlBatch.AddParameterNVarChar(user.Descriptor.Identifier));
      }
      this.SqlBatch.AppendSql(Environment.NewLine);
    }
  }
}
