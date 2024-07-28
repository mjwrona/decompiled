// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.MetaTaskDefinitionComponent6
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class MetaTaskDefinitionComponent6 : MetaTaskDefinitionComponent5
  {
    public override void DeleteTeamProject(Guid projectId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteTeamProject)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteProjectMetaTasks", projectId);
        this.BindGuid("@projectId", projectId);
        this.BindGuid("@namespaceGuid", DefaultSecurityProvider.MetaTaskNamespaceId);
        this.DataspaceRlsEnabled = false;
        this.ExecuteNonQuery();
      }
    }
  }
}
