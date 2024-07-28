// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.PriorityManager
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  internal class PriorityManager : IPriorityManager
  {
    [StaticSafe]
    private static readonly Dictionary<string, JobPriorityLevel> s_priorityMap = PriorityManager.InitializePriorityMap();
    private static readonly RegistryQuery s_registryAccountPriority = new RegistryQuery("/AccountIndexingPriority/PriorityValue");

    public Dictionary<string, JobPriorityLevel> GetPriorityMap() => PriorityManager.s_priorityMap;

    public JobPriorityLevel GetPriority(
      IVssRequestContext requestContext,
      IndexingUnitChangeEvent changeEvent)
    {
      JobPriorityLevel accountBasedPriority = this.GetAccountBasedPriority(requestContext);
      if (accountBasedPriority != JobPriorityLevel.None)
        return accountBasedPriority;
      if (changeEvent == null)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", FormattableString.Invariant(FormattableStringFactory.Create("Parameter {0} is null.", (object) nameof (changeEvent))));
        return JobPriorityLevel.Lowest;
      }
      PriorityManager.s_priorityMap.TryGetValue(changeEvent.ChangeType, out accountBasedPriority);
      return accountBasedPriority;
    }

    public JobPriorityLevel GetAccountBasedPriority(IVssRequestContext requestContext)
    {
      string str = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, in PriorityManager.s_registryAccountPriority, true);
      if (!string.IsNullOrEmpty(str))
      {
        try
        {
          return PriorityManager.GetJobPriorityLevel((JobQueuePriority) Enum.Parse(typeof (JobQueuePriority), str));
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080473, "Indexing Pipeline", "CommonJobUtils", FormattableString.Invariant(FormattableStringFactory.Create("Parse failed for retrieved JobQueuePriority enum. Returning None priority. Exception: {0}", (object) ex)));
        }
      }
      return JobPriorityLevel.None;
    }

    private static JobPriorityLevel GetJobPriorityLevel(JobQueuePriority jobQueuePriority)
    {
      switch (jobQueuePriority)
      {
        case JobQueuePriority.Low:
          return JobPriorityLevel.Lowest;
        case JobQueuePriority.Normal:
          return JobPriorityLevel.BelowNormal;
        case JobQueuePriority.High:
          return JobPriorityLevel.Normal;
        default:
          return JobPriorityLevel.Normal;
      }
    }

    private static Dictionary<string, JobPriorityLevel> InitializePriorityMap() => new Dictionary<string, JobPriorityLevel>()
    {
      {
        "CrawlMetadata",
        JobPriorityLevel.Lowest
      },
      {
        "UpdateMetadata",
        JobPriorityLevel.Lowest
      },
      {
        "BeginBulkIndex",
        JobPriorityLevel.Lowest
      },
      {
        "BeginProjectRename",
        JobPriorityLevel.Lowest
      },
      {
        "BeginEntityRename",
        JobPriorityLevel.Lowest
      },
      {
        "CompleteEntityRename",
        JobPriorityLevel.Lowest
      },
      {
        "Add",
        JobPriorityLevel.Lowest
      },
      {
        "Delete",
        JobPriorityLevel.Lowest
      },
      {
        "DeleteOrphanFiles",
        JobPriorityLevel.Lowest
      },
      {
        "BranchDelete",
        JobPriorityLevel.Lowest
      },
      {
        "CompleteBranchDelete",
        JobPriorityLevel.Lowest
      },
      {
        "CustomGitRepositoryBulkIndex",
        JobPriorityLevel.Lowest
      },
      {
        "GitRepositorySecurityAcesSync",
        JobPriorityLevel.Lowest
      },
      {
        "AreaNodeSecurityAcesSync",
        JobPriorityLevel.Lowest
      },
      {
        "FeedSecurityAcesSync",
        JobPriorityLevel.Lowest
      },
      {
        "ReIndex",
        JobPriorityLevel.Lowest
      },
      {
        "UpdateField",
        JobPriorityLevel.Lowest
      },
      {
        "AddClassificationNode",
        JobPriorityLevel.Lowest
      },
      {
        "SyncAllClassificationNode",
        JobPriorityLevel.Lowest
      },
      {
        "CleanUpFeeds",
        JobPriorityLevel.Lowest
      },
      {
        "FeedUpdates",
        JobPriorityLevel.Lowest
      },
      {
        "Patch",
        JobPriorityLevel.BelowNormal
      },
      {
        "UpdateIndex",
        JobPriorityLevel.BelowNormal
      },
      {
        "UpdateIndexingUnitProperties",
        JobPriorityLevel.BelowNormal
      },
      {
        "UpdateClassificationNode",
        JobPriorityLevel.BelowNormal
      },
      {
        "ResetBranchesInGitRepoAttributes",
        JobPriorityLevel.BelowNormal
      },
      {
        "ProjectDeleteFromCollection",
        JobPriorityLevel.BelowNormal
      },
      {
        "RepositoryDeleteFromCollection",
        JobPriorityLevel.BelowNormal
      },
      {
        "ProjectUpdateInCollection",
        JobPriorityLevel.BelowNormal
      },
      {
        "RepositoryRenameInCollection",
        JobPriorityLevel.BelowNormal
      },
      {
        "UpdateWorkItemFieldValues",
        JobPriorityLevel.BelowNormal
      },
      {
        "AssignRouting",
        JobPriorityLevel.BelowNormal
      },
      {
        "Destroy",
        JobPriorityLevel.BelowNormal
      },
      {
        "CompleteBulkIndex",
        JobPriorityLevel.Normal
      },
      {
        "BeginMigrateIndex",
        JobPriorityLevel.Normal
      },
      {
        "CompleteMigrateIndex",
        JobPriorityLevel.Normal
      },
      {
        "ExtensionBeginUninstall",
        JobPriorityLevel.Normal
      },
      {
        "ExtensionFinalizeUninstall",
        JobPriorityLevel.Normal
      },
      {
        "UpdateSearchUrlInIndexingUnitProperties",
        JobPriorityLevel.Normal
      },
      {
        "DeleteDuplicateDocuments",
        JobPriorityLevel.Highest
      }
    };
  }
}
