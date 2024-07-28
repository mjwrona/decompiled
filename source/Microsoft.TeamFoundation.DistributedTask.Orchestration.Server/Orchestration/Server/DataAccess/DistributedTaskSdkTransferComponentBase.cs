// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.DistributedTaskSdkTransferComponentBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  public abstract class DistributedTaskSdkTransferComponentBase : FilterTransferComponentBase
  {
    public static Type[] TransferComponentTypes = new Type[15]
    {
      typeof (DistributedTaskAttachmentTransferComponent),
      typeof (DistributedTaskJobTransferComponent),
      typeof (DistributedTaskLogTransferComponent),
      typeof (DistributedTaskLogPageTransferComponent),
      typeof (DistributedTaskPlanTransferComponent),
      typeof (DistributedTaskPlanDefinitionReferenceTransferComponent),
      typeof (DistributedTaskPlanOwnerReferenceTransferComponent),
      typeof (DistributedTaskPlanQueueTransferComponent),
      typeof (DistributedTaskPlanTaskReferenceTransferComponent),
      typeof (DistributedTaskTaskDefinitionReferenceTransferComponent),
      typeof (DistributedTaskTimelineTransferComponent),
      typeof (DistributedTaskTimelineRecordTransferComponent),
      typeof (DistributedTaskTimelineRecordIssueTransferComponent),
      typeof (DistributedTaskTimelineRecordVariableTransferComponent),
      typeof (DistributedTaskTimelineStringTransferComponent)
    };
    private HashSet<string> m_dataspaces;

    public override string SchemaName => "Task";

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
      return "\r\n            FROM    tbl_Dataspace\r\n            INNER JOIN " + this.SchemaName + "." + this.TableName + "\r\n            ON      " + this.SchemaName + "." + this.TableName + ".PartitionId = @partitionId\r\n                    AND " + this.SchemaName + "." + this.TableName + ".DataspaceId = tbl_Dataspace.DataspaceId\r\n            WHERE   tbl_Dataspace.PartitionId = @partitionId\r\n                    AND tbl_Dataspace.DataspaceCategory IN ( " + this.m_dataspaces.ToQuotedStringList<string>() + " )";
    }
  }
}
