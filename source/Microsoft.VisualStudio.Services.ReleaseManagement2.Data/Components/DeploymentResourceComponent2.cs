// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentResourceComponent2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class DeploymentResourceComponent2 : DeploymentResourceComponent
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public override IList<DeploymentResource> GetDeploymentResources(
      Guid projectId,
      int id = 0,
      string resourceIdentifier = null,
      int releaseDefinitionId = 0,
      int maxDeploymentResourcesCount = 0,
      int continuationToken = 0)
    {
      this.PrepareStoredProcedure("Release.prc_GetDeploymentResources", projectId);
      this.BindInt(nameof (id), id);
      this.BindString(nameof (resourceIdentifier), resourceIdentifier, 450, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt("@maxDeploymentResourcesCount", maxDeploymentResourcesCount);
      this.BindInt("@continuationToken", continuationToken);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeploymentResource>((ObjectBinder<DeploymentResource>) this.GetDeploymentResourceBinder());
        return (IList<DeploymentResource>) resultCollection.GetCurrent<DeploymentResource>().Items;
      }
    }
  }
}
