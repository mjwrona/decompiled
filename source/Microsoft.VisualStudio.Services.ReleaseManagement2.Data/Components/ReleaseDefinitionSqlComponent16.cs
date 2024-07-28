// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent16
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle onprem host servicing")]
  public class ReleaseDefinitionSqlComponent16 : ReleaseDefinitionSqlComponent15
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
      this.BindIncludeTriggers(includeTriggers);
      this.BindMaxModifiedTime(maxModifiedTime);
      this.BindByte(nameof (queryOrder), (byte) queryOrder);
      this.BindContinuationToken(continuationToken);
      this.BindMaxReleaseDefinitionsCount(nameof (maxReleaseDefinitionsCount), maxReleaseDefinitionsCount);
      this.BindSearchTextContainsFolderName(searchTextContainsFolderName);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        List<DefinitionEnvironment> environments = new List<DefinitionEnvironment>();
        List<ReleaseDefinitionArtifactSourceMap> releaseDefinitionArtifactSourceData = new List<ReleaseDefinitionArtifactSourceMap>();
        List<ReleaseTriggerBase> triggers = new List<ReleaseTriggerBase>();
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
        this.StitchReleaseDefinitions((IList<ReleaseDefinition>) items, (IList<DefinitionEnvironment>) environments, (IList<ReleaseTriggerBase>) triggers, (IList<ReleaseReference>) null, (IList<ReleaseDefinitionArtifactSourceMap>) releaseDefinitionArtifactSourceData, (IList<DefinitionTagData>) null, isDefaultToLatestArtifactVersionEnabled);
        return (IEnumerable<ReleaseDefinition>) items;
      }
    }

    protected void BindContinuationToken(string continuationToken)
    {
      if (string.IsNullOrWhiteSpace(continuationToken))
        return;
      this.BindString(nameof (continuationToken), continuationToken, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
    }
  }
}
