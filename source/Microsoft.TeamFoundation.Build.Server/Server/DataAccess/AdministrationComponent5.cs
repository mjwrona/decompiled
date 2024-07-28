// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.AdministrationComponent5
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class AdministrationComponent5 : AdministrationComponent4
  {
    public AdministrationComponent5()
    {
      this.ServiceVersion = ServiceVersion.V5;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override ResultCollection UpdateBuildServiceHostStatus(
      string uri,
      ServiceHostStatus status,
      Guid? ownerId,
      string ownerName,
      int? sequenceId,
      bool recursive,
      bool clearOwner = false,
      Uri queueAddress = null)
    {
      this.TraceEnter(0, nameof (UpdateBuildServiceHostStatus));
      this.PrepareStoredProcedure("prc_UpdateBuildServiceHostStatus");
      this.BindItemUriToInt32("@serviceHostId", uri);
      if (ownerId.HasValue)
        this.BindGuid("@ownerId", ownerId.Value);
      else
        this.BindNullValue("@ownerId", SqlDbType.UniqueIdentifier);
      this.BindString("@ownerName", ownerName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      if (sequenceId.HasValue)
        this.BindInt("@sequenceId", sequenceId.Value);
      else
        this.BindNullValue("@sequenceId", SqlDbType.Int);
      this.BindByte("@newStatus", (byte) status);
      this.BindBoolean("@recursive", recursive);
      this.BindBoolean("@clearOwner", clearOwner);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<KeyValuePair<Guid, Guid>>((ObjectBinder<KeyValuePair<Guid, Guid>>) new ServiceHostOwnershipBinder());
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      resultCollection.AddBinder<AgentReservationData>((ObjectBinder<AgentReservationData>) new AgentReservationDataBinder());
      resultCollection.AddBinder<WorkflowCancellationData>((ObjectBinder<WorkflowCancellationData>) new WorkflowCancellationDataBinder());
      resultCollection.AddBinder<WorkflowCancellationData>((ObjectBinder<WorkflowCancellationData>) new WorkflowCancellationDataBinder());
      this.TraceLeave(0, nameof (UpdateBuildServiceHostStatus));
      return resultCollection;
    }
  }
}
