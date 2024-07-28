// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.AnalyzerFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1A24B0F-DA40-425D-8B54-1865D1FC90B8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers
{
  public class AnalyzerFactory : AbstractAnalyzerFactory
  {
    private readonly Dictionary<Scenario, List<IAnalyzer>> m_scenarioAnalyzersDictionary = new Dictionary<Scenario, List<IAnalyzer>>()
    {
      {
        Scenario.CodeExtensionInstalledNoResults,
        new List<IAnalyzer>()
        {
          (IAnalyzer) new MissedExtensionInstallNotificationAnalyzer(),
          (IAnalyzer) new ESConnectionStringAnalyzer(),
          (IAnalyzer) new PendingProcessAnalyzer(),
          (IAnalyzer) new FailedFilesAnalyzer(),
          (IAnalyzer) new ESCollectionAnyResultsExistAnalyzer()
        }
      },
      {
        Scenario.Default,
        new List<IAnalyzer>()
        {
          (IAnalyzer) new ESConnectionStringAnalyzer(),
          (IAnalyzer) new PendingProcessAnalyzer(),
          (IAnalyzer) new RoutingInfoAnalyzer(),
          (IAnalyzer) new FailedFilesAnalyzer(),
          (IAnalyzer) new ChangeEventsStateAnalyzer()
        }
      },
      {
        Scenario.DummyForTest,
        new List<IAnalyzer>()
      },
      {
        Scenario.StaleSearchResults,
        new List<IAnalyzer>()
        {
          (IAnalyzer) new ESConnectionStringAnalyzer(),
          (IAnalyzer) new PendingProcessAnalyzer(),
          (IAnalyzer) new RoutingInfoAnalyzer(),
          (IAnalyzer) new FailedFilesAnalyzer(),
          (IAnalyzer) new ChangeEventsStateAnalyzer()
        }
      },
      {
        Scenario.DuplicateFiles,
        new List<IAnalyzer>()
        {
          (IAnalyzer) new RoutingInfoAnalyzer(),
          (IAnalyzer) new ESDuplicateFilesDetectionAnalyzer()
        }
      },
      {
        Scenario.OptimizeIndex,
        new List<IAnalyzer>()
        {
          (IAnalyzer) new EsDeletedDocumentCountAnalyzer()
        }
      },
      {
        Scenario.ShardSizeReduction,
        new List<IAnalyzer>()
        {
          (IAnalyzer) new CodeLargeShardsAnalyzer()
        }
      },
      {
        Scenario.BranchLevelDocumentCount,
        new List<IAnalyzer>()
        {
          (IAnalyzer) new GitRepoBranchLevelDocumentCountAnalyzer()
        }
      }
    };

    public override HashSet<IAnalyzer> GetAnalyzers(Scenario scenario) => this.m_scenarioAnalyzersDictionary[scenario].ToHashSet<IAnalyzer>();
  }
}
