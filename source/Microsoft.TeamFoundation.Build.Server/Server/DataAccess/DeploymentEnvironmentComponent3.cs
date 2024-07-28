// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.DeploymentEnvironmentComponent3
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class DeploymentEnvironmentComponent3 : DeploymentEnvironmentComponent2
  {
    public DeploymentEnvironmentComponent3()
    {
      this.ServiceVersion = ServiceVersion.V4;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override List<DeploymentEnvironmentMetadata> GetDeploymentEnvironments(
      Guid projectId,
      string teamProject,
      string serviceName = "")
    {
      this.PrepareStoredProcedure("prc_GetDeploymentEnvironments");
      this.BindGuid("@projectId", projectId);
      this.BindString("@connectedServiceName", serviceName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeploymentEnvironmentMetadata>((ObjectBinder<DeploymentEnvironmentMetadata>) new DeploymentEnvironmentComponent.EnvironmentMetadataBinder(teamProject));
        List<DeploymentEnvironmentMetadata> items = resultCollection.GetCurrent<DeploymentEnvironmentMetadata>().Items;
        return items != null && items.Count > 0 ? items : new List<DeploymentEnvironmentMetadata>();
      }
    }
  }
}
