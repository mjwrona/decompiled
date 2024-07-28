// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent40
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent40 : ReleaseSqlComponent39
  {
    public override IEnumerable<Release> QueryActiveReleases(
      Guid projectId,
      int releaseDefinitionId,
      int maxReleasesCount,
      int continuationToken,
      bool includeEnvironments,
      bool includeArtifacts,
      bool includeApprovals,
      bool includeManualInterventions,
      bool includeVariables,
      bool includeTags)
    {
      return this.ListReleases(projectId, string.Empty, releaseDefinitionId, 0, ReleaseStatus.Undefined, ReleaseEnvironmentStatus.Undefined, (IEnumerable<Guid>) new List<Guid>(), new DateTime?(), new DateTime?(), new DateTime?(), maxReleasesCount, ReleaseQueryOrder.IdDescending, continuationToken, includeEnvironments, includeArtifacts, includeApprovals, includeManualInterventions, string.Empty, string.Empty, string.Empty, string.Empty, false, false, includeVariables, includeTags, (IEnumerable<string>) null, (IEnumerable<int>) null, (string) null);
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Will be overridden in derived classes.")]
    protected override DeploymentGateBinder GetDeploymentGateBinder() => (DeploymentGateBinder) new DeploymentGateBinder2((ReleaseManagementSqlResourceComponentBase) this);
  }
}
