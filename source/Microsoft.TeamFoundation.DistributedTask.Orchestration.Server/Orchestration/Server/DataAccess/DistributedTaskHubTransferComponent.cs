// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.DistributedTaskHubTransferComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  public class DistributedTaskHubTransferComponent : FilterTransferComponentBase
  {
    private HashSet<string> m_dataspaces;

    public override string SchemaName => "Task";

    public override string TableName => "tbl_Hub";

    public override void SetFilters(ICopyFilterProvider provider)
    {
      ArgumentUtility.CheckForNull<ICopyFilterProvider>(provider, nameof (provider));
      this.m_dataspaces = provider.GetFilter<string>("DistributedTaskSdk.DataspaceCategories");
      if (this.HasDynamicFilters())
        return;
      this.Logger.Info("Filter Tokens not found, no filtering of Dataspace will take place.");
    }

    public override bool HasDynamicFilters() => this.m_dataspaces.Any<string>();

    public override string ReadStatementBase(PartitionTransferContext transferContext)
    {
      if (!this.HasDynamicFilters())
        return base.ReadStatementBase(transferContext);
      return "\r\n            FROM    " + this.SchemaName + "." + this.TableName + "\r\n            WHERE   " + this.SchemaName + "." + this.TableName + ".PartitionId = @partitionId\r\n                    AND " + this.SchemaName + "." + this.TableName + ".DataspaceCategory IN ( " + this.m_dataspaces.ToQuotedStringList<string>() + " )";
    }
  }
}
