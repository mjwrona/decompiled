// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.GitRepoBranchLevelDocumentCountDataProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F07E19E9-6199-4A9C-8D41-E26991BA8812
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders
{
  public class GitRepoBranchLevelDocumentCountDataProvider : IDataProvider
  {
    public GitRepoBranchLevelDocumentCountDataProvider()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.SearchPlatformFactory.GetInstance())
    {
    }

    internal GitRepoBranchLevelDocumentCountDataProvider(
      IDataAccessFactory dataAccessFactory,
      ISearchPlatformFactory searchPlatformFactory)
    {
      this.DataAccessFactory = dataAccessFactory;
      this.SearchPlatformFactory = searchPlatformFactory;
    }

    internal IDataAccessFactory DataAccessFactory { get; }

    internal ISearchPlatformFactory SearchPlatformFactory { get; }

    public List<HealthData> GetData(ProviderContext providerContext) => new List<HealthData>()
    {
      (HealthData) new GitRepoBranchLevelDocumentCountData(this.GetBranchNameToDocumentCountMap((ESContext) providerContext), DataType.GitRepoBranchLevelDocCount)
    };

    internal virtual IDictionary<string, long> GetBranchNameToDocumentCountMap(ESContext esContext)
    {
      string input = (string) null;
      List<string> stringList;
      if (esContext.SearchFilters.TryGetValue("repositoryId", out stringList) && stringList != null && stringList.Count == 1)
        input = stringList[0];
      Guid result;
      if (!string.IsNullOrWhiteSpace(input) && Guid.TryParse(input, out result) && result != Guid.Empty)
      {
        IndexingUnit indexingUnit = this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(esContext.RequestContext, result, "Git_Repository", (IEntityType) CodeEntityType.GetInstance());
        List<string> branchesToIndex = ((GitCodeRepoTFSAttributes) indexingUnit.TFSEntityAttributes).BranchesToIndex;
        List<IndexInfo> indexIndices = indexingUnit.Properties.IndexIndices;
        IDocumentContractTypeService service = esContext.RequestContext.GetService<IDocumentContractTypeService>();
        ISearchPlatform searchPlatform = this.GetSearchPlatform(esContext);
        IDictionary<string, long> documentCountMap = (IDictionary<string, long>) new FriendlyDictionary<string, long>();
        CodeFileContract codeContract = CodeFileContract.CreateCodeContract(service.GetSupportedQueryDocumentContractType(esContext.RequestContext, (IEntityType) CodeEntityType.GetInstance()), searchPlatform);
        foreach (string str in branchesToIndex)
        {
          long branchDocCount = codeContract.GetBranchDocCount(result, str, (IEnumerable<IndexInfo>) indexIndices);
          documentCountMap[str] = branchDocCount;
        }
        return documentCountMap;
      }
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("repositoryId {0} is not a valid Guid. SearchFilters must have repositoryId as a filter.", (object) input)));
    }

    internal virtual ISearchPlatform GetSearchPlatform(ESContext esContext) => this.SearchPlatformFactory.Create(esContext.QueryConnectionString, esContext.SearchPlatformSettings, esContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment);
  }
}
