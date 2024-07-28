// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent23
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseDefinitionSqlComponent23 : ReleaseDefinitionSqlComponent22
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public override IEnumerable<ReleaseDefinition> ListReleaseDefinitions(
      Guid projectId,
      string nameFilter,
      IEnumerable<string> sourceIdFilter = null,
      string artifactTypeId = null,
      bool isDeleted = false,
      bool includeEnvironments = false,
      bool includeArtifacts = false,
      bool includeTriggers = false,
      bool includeLatestRelease = true,
      DateTime? maxModifiedTime = null,
      ReleaseDefinitionQueryOrder queryOrder = ReleaseDefinitionQueryOrder.IdAscending,
      string continuationToken = null,
      int maxReleaseDefinitionsCount = 0,
      string path = null,
      bool isExactNameMatch = false,
      bool includeTags = false,
      IEnumerable<string> tagFilter = null,
      bool includeVariables = false,
      IEnumerable<int> definitionIdFilter = null,
      bool isDefaultToLatestArtifactVersionEnabled = false,
      bool searchTextContainsFolderName = false)
    {
      if (projectId.Equals(Guid.Empty))
        this.PrepareStoredProcedure("Release.prc_QueryReleaseDefinitions");
      else
        this.PrepareStoredProcedure("Release.prc_QueryReleaseDefinitions", projectId);
      this.BindString(nameof (nameFilter), nameFilter, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindArtifactsFilter(sourceIdFilter, artifactTypeId);
      this.BindIsDeletedFilter(isDeleted);
      this.BindBoolean(nameof (includeEnvironments), includeEnvironments);
      this.BindBoolean(nameof (includeArtifacts), includeArtifacts);
      this.BindBoolean("includeLastRelease", includeLatestRelease);
      this.BindBoolean(nameof (includeTags), includeTags);
      this.BindIncludeVariables(includeVariables);
      this.BindIncludeTriggers(includeTriggers);
      this.BindMaxModifiedTime(maxModifiedTime);
      this.BindByte(nameof (queryOrder), (byte) queryOrder);
      this.BindContinuationToken(continuationToken);
      this.BindMaxReleaseDefinitionsCount(nameof (maxReleaseDefinitionsCount), maxReleaseDefinitionsCount);
      this.BindFolderPath(path);
      this.BindIsExactNameMatch(isExactNameMatch);
      this.BindStringTable(nameof (tagFilter), tagFilter);
      this.BindDefinitionIdFilter(definitionIdFilter);
      this.BindSearchTextContainsFolderName(searchTextContainsFolderName);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        List<DefinitionEnvironment> environments = new List<DefinitionEnvironment>();
        List<ReleaseDefinitionArtifactSourceMap> releaseDefinitionArtifactSourceData = new List<ReleaseDefinitionArtifactSourceMap>();
        List<ReleaseReference> releases = new List<ReleaseReference>();
        List<ReleaseTriggerBase> triggers = new List<ReleaseTriggerBase>();
        List<DefinitionTagData> tags = new List<DefinitionTagData>();
        resultCollection.AddBinder<ReleaseDefinition>((ObjectBinder<ReleaseDefinition>) this.GetReleaseDefinitionBinder());
        List<ReleaseDefinition> items = resultCollection.GetCurrent<ReleaseDefinition>().Items;
        if (includeEnvironments)
        {
          resultCollection.AddBinder<DefinitionEnvironment>((ObjectBinder<DefinitionEnvironment>) this.GetDefinitionEnvironmentListBinder());
          resultCollection.NextResult();
          environments = resultCollection.GetCurrent<DefinitionEnvironment>().Items;
        }
        if (includeArtifacts)
        {
          resultCollection.AddBinder<ReleaseDefinitionArtifactSourceMap>((ObjectBinder<ReleaseDefinitionArtifactSourceMap>) this.GetReleaseDefinitionArtifactSourceBinder);
          resultCollection.NextResult();
          releaseDefinitionArtifactSourceData = resultCollection.GetCurrent<ReleaseDefinitionArtifactSourceMap>().Items;
        }
        if (includeTriggers)
        {
          this.AddTriggersBinder(resultCollection);
          triggers = this.GetReleaseTriggers(resultCollection).ToList<ReleaseTriggerBase>();
        }
        if (includeLatestRelease)
        {
          resultCollection.AddBinder<ReleaseReference>((ObjectBinder<ReleaseReference>) this.GetReleaseReferenceBinder());
          resultCollection.NextResult();
          releases = resultCollection.GetCurrent<ReleaseReference>().Items;
        }
        if (includeTags)
        {
          resultCollection.AddBinder<DefinitionTagData>((ObjectBinder<DefinitionTagData>) this.GetDefinitionTagBinder());
          resultCollection.NextResult();
          tags = resultCollection.GetCurrent<DefinitionTagData>().Items;
        }
        this.StitchReleaseDefinitions((IList<ReleaseDefinition>) items, (IList<DefinitionEnvironment>) environments, (IList<ReleaseTriggerBase>) triggers, (IList<ReleaseReference>) releases, (IList<ReleaseDefinitionArtifactSourceMap>) releaseDefinitionArtifactSourceData, (IList<DefinitionTagData>) tags, isDefaultToLatestArtifactVersionEnabled);
        return ReleaseDefinitionSqlComponent21.SortDefinitionsOnQueryOrder((IList<ReleaseDefinition>) items, queryOrder);
      }
    }

    protected virtual void BindDefinitionIdFilter(IEnumerable<int> definitionIdFilter)
    {
    }

    public override void UpdateDefinitionEnvironmentSchedules(
      Guid projectId,
      int releaseDefinitionId,
      DefinitionEnvironment definitionEnvironment)
    {
      if (definitionEnvironment == null)
        throw new ArgumentNullException(nameof (definitionEnvironment));
      this.PrepareStoredProcedure("Release.prc_UpdateDefinitionEnvironmentSchedules", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt("definitionEnvironmentId", definitionEnvironment.Id);
      this.BindString("schedules", JsonConvert.SerializeObject((object) definitionEnvironment.Schedules), 4000, true, SqlDbType.NVarChar);
      this.ExecuteScalar();
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This input has already been validated at above layer")]
    protected override void BindToReleaseDefinitionTable(
      ReleaseDefinition releaseDefinition,
      bool bindIds)
    {
      if (bindIds)
      {
        this.BindInt("id", releaseDefinition.Id);
        this.BindInt("revision", releaseDefinition.Revision);
      }
      this.BindString("Name", string.IsNullOrEmpty(releaseDefinition.Name) ? string.Empty : releaseDefinition.Name.Trim(), 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("CreatedBy", releaseDefinition.CreatedBy);
      this.BindNullableDateTime("CreatedOn", releaseDefinition.CreatedOn);
      this.BindGuid("ModifiedBy", releaseDefinition.ModifiedBy);
      this.BindNullableDateTime("ModifiedOn", releaseDefinition.ModifiedOn);
      this.BindString("Variables", ServerModelUtility.ToString((object) VariablesUtility.ReplaceSecretVariablesWithNull(releaseDefinition.Variables)), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("VariableGroups", ServerModelUtility.ToString((object) releaseDefinition.VariableGroups), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("ReleaseNameFormat", string.IsNullOrEmpty(releaseDefinition.ReleaseNameFormat) ? string.Empty : releaseDefinition.ReleaseNameFormat, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindByte("Source", (byte) releaseDefinition.Source);
    }

    protected override ReleaseDefinitionBinder GetReleaseDefinitionBinder() => (ReleaseDefinitionBinder) new ReleaseDefinitionBinder4((ReleaseManagementSqlResourceComponentBase) this);

    protected virtual void BindIncludeVariables(bool includeVariables)
    {
    }
  }
}
