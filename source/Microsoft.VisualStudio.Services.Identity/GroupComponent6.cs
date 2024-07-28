// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent6
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupComponent6 : GroupComponent5
  {
    protected override SqlParameter BindVersionedGroupTable(
      string parameterName,
      IEnumerable<GroupDescription> descriptions)
    {
      return this.BindGroupTable3(parameterName, descriptions);
    }

    protected override GroupComponent.GroupIdentitiesColumns GetGroupIdentitiesColumns() => (GroupComponent.GroupIdentitiesColumns) new GroupComponent.GroupIdentitiesColumns2();

    protected override GroupComponent.GroupIdentityColumns GetGroupIdentityColumns() => (GroupComponent.GroupIdentityColumns) new GroupComponent.GroupIdentityColumns2();

    public override int InitGroupScopeVisibility()
    {
      int num = -1;
      try
      {
        this.TraceEnter(4703700, nameof (InitGroupScopeVisibility));
        this.PrepareStoredProcedure("prc_InitGroupScopeVisibility");
        SqlDataReader reader = this.ExecuteReader();
        if (reader.Read())
          num = !reader.IsDBNull(0) ? new SqlColumnBinder("PartitionId").GetInt32((IDataReader) reader) : throw new Exception(string.Format("call to prc_InitGroupScopeVisibility should return partitionId. something went wrong for partitionId = {0}, hostName = {1}", (object) this.PartitionId, (object) this.RequestContext.ServiceHost.Name));
      }
      finally
      {
        this.TraceLeave(4703709, nameof (InitGroupScopeVisibility));
      }
      return num;
    }
  }
}
