// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.RetentionSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class RetentionSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<RetentionSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<RetentionSqlComponent2>(2)
    }, "ReleaseManagementRetention", "ReleaseManagement");

    public IEnumerable<string> GetBuildsToStopRetaining(
      Guid projectId,
      int definitionId,
      IEnumerable<int> releaseIds,
      bool isDeleted)
    {
      this.PrepareStoredProcedure("Release.prc_GetBuildsToStopRetaining", projectId);
      this.BindInt("releaseDefinitionId", definitionId);
      this.BindInt32Table(nameof (releaseIds), releaseIds);
      this.BindString("artifactTypeId", "Build", 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.DataspaceRlsEnabled = false;
      this.BindBoolean(nameof (isDeleted), isDeleted);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<string>((ObjectBinder<string>) new BuildIdListBinder());
        return (IEnumerable<string>) resultCollection.GetCurrent<string>().Items;
      }
    }

    public void UpdateRetainBuildForReleaseArtifactSources(
      Guid projectId,
      IEnumerable<int> releaseIds,
      bool retainBuildStatus,
      string artifactType)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateRetainBuildForReleaseArtifactSource", projectId);
      this.BindInt32Table(nameof (releaseIds), releaseIds);
      this.BindString("artifactTypeId", artifactType, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean(nameof (retainBuildStatus), retainBuildStatus);
      this.ExecuteNonQuery();
    }

    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "Necessary to convey usage")]
    public IEnumerable<Release> GetReleasesToUpdateRetention(
      Guid projectId,
      int definitionId,
      IEnumerable<int> definitionEnvironments,
      bool stopRetainBuild,
      int minReleaseId,
      int maxReleases,
      string artifactTypeId,
      bool isRdDeleted)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleasesToUpdateRetention", projectId);
      this.BindInt("releaseDefinitionId", definitionId);
      this.BindInt32Table("definitionEnvironmentsOnWhichRetainBuildIsUpdated", definitionEnvironments);
      this.BindInt(nameof (minReleaseId), minReleaseId);
      this.BindInt(nameof (maxReleases), maxReleases);
      this.BindString(nameof (artifactTypeId), artifactTypeId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean(nameof (stopRetainBuild), stopRetainBuild);
      this.BindBoolean(nameof (isRdDeleted), isRdDeleted);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        return (IEnumerable<Release>) resultCollection.GetCurrent<Release>().Items;
      }
    }

    public virtual IEnumerable<string> GetBuildsRetainedByReleases(
      Guid projectId,
      IEnumerable<int> releaseIds)
    {
      return (IEnumerable<string>) new List<string>();
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Override of base class method.")]
    protected virtual ReleaseBinder GetReleaseBinder(Guid projectId) => new ReleaseBinder(this.RequestContext, (ReleaseManagementSqlResourceComponentBase) this);
  }
}
