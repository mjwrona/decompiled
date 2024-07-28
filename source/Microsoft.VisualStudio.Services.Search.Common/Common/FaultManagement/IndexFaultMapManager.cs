// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.IndexFaultMapManager
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement
{
  public static class IndexFaultMapManager
  {
    private static volatile List<FaultMapper> s_faultMappers;
    private static readonly object s_lock = new object();

    public static List<FaultMapper> FaultMappers
    {
      get
      {
        if (IndexFaultMapManager.s_faultMappers == null)
        {
          lock (IndexFaultMapManager.s_lock)
          {
            if (IndexFaultMapManager.s_faultMappers == null)
            {
              IndexFaultMapManager.s_faultMappers = new List<FaultMapper>();
              IndexFaultMapManager.Initialize();
            }
          }
        }
        return IndexFaultMapManager.s_faultMappers;
      }
    }

    public static FaultMapper GetFaultMapper(Type mapperType)
    {
      foreach (FaultMapper faultMapper in IndexFaultMapManager.FaultMappers)
      {
        if (faultMapper.GetType() == mapperType)
          return faultMapper;
      }
      return (FaultMapper) null;
    }

    private static void Initialize()
    {
      List<FaultMapper> faultMappers1 = IndexFaultMapManager.s_faultMappers;
      ESRejectedExecutionExceptionFaultMapper exceptionFaultMapper1 = new ESRejectedExecutionExceptionFaultMapper();
      exceptionFaultMapper1.Enabled = true;
      exceptionFaultMapper1.Retriable = true;
      exceptionFaultMapper1.Severity = IndexerFaultSeverity.Medium;
      faultMappers1.Add((FaultMapper) exceptionFaultMapper1);
      List<FaultMapper> faultMappers2 = IndexFaultMapManager.s_faultMappers;
      ESSearchPlatformNonRetriableExceptionsFaultMapper exceptionsFaultMapper = new ESSearchPlatformNonRetriableExceptionsFaultMapper();
      exceptionsFaultMapper.Enabled = true;
      exceptionsFaultMapper.Retriable = false;
      exceptionsFaultMapper.Severity = IndexerFaultSeverity.Critical;
      faultMappers2.Add((FaultMapper) exceptionsFaultMapper);
      List<FaultMapper> faultMappers3 = IndexFaultMapManager.s_faultMappers;
      ESStateRedFaultMapper stateRedFaultMapper = new ESStateRedFaultMapper();
      stateRedFaultMapper.Enabled = true;
      stateRedFaultMapper.Retriable = false;
      stateRedFaultMapper.Severity = IndexerFaultSeverity.Critical;
      faultMappers3.Add((FaultMapper) stateRedFaultMapper);
      List<FaultMapper> faultMappers4 = IndexFaultMapManager.s_faultMappers;
      ESStateYellowFaultMapper yellowFaultMapper = new ESStateYellowFaultMapper();
      yellowFaultMapper.Enabled = true;
      yellowFaultMapper.Retriable = true;
      yellowFaultMapper.Severity = IndexerFaultSeverity.Medium;
      faultMappers4.Add((FaultMapper) yellowFaultMapper);
      List<FaultMapper> faultMappers5 = IndexFaultMapManager.s_faultMappers;
      ESTimeOutFaultMapper timeOutFaultMapper1 = new ESTimeOutFaultMapper();
      timeOutFaultMapper1.Enabled = true;
      timeOutFaultMapper1.Retriable = true;
      timeOutFaultMapper1.Severity = IndexerFaultSeverity.Medium;
      faultMappers5.Add((FaultMapper) timeOutFaultMapper1);
      List<FaultMapper> faultMappers6 = IndexFaultMapManager.s_faultMappers;
      ESNestNonGenericFaultMapper genericFaultMapper = new ESNestNonGenericFaultMapper();
      genericFaultMapper.Enabled = true;
      genericFaultMapper.Retriable = true;
      genericFaultMapper.Severity = IndexerFaultSeverity.Medium;
      faultMappers6.Add((FaultMapper) genericFaultMapper);
      List<FaultMapper> faultMappers7 = IndexFaultMapManager.s_faultMappers;
      VssThrottlingFaultMapper throttlingFaultMapper = new VssThrottlingFaultMapper();
      throttlingFaultMapper.Enabled = true;
      throttlingFaultMapper.Retriable = false;
      throttlingFaultMapper.Severity = IndexerFaultSeverity.Medium;
      faultMappers7.Add((FaultMapper) throttlingFaultMapper);
      List<FaultMapper> faultMappers8 = IndexFaultMapManager.s_faultMappers;
      VssConcurrencyLimitFaultMapper limitFaultMapper = new VssConcurrencyLimitFaultMapper();
      limitFaultMapper.Enabled = true;
      limitFaultMapper.Retriable = false;
      limitFaultMapper.Severity = IndexerFaultSeverity.Medium;
      faultMappers8.Add((FaultMapper) limitFaultMapper);
      List<FaultMapper> faultMappers9 = IndexFaultMapManager.s_faultMappers;
      ESEntityTooLargeFaultMapper largeFaultMapper = new ESEntityTooLargeFaultMapper();
      largeFaultMapper.Enabled = true;
      largeFaultMapper.Retriable = true;
      largeFaultMapper.Severity = IndexerFaultSeverity.Low;
      faultMappers9.Add((FaultMapper) largeFaultMapper);
      List<FaultMapper> faultMappers10 = IndexFaultMapManager.s_faultMappers;
      VssInsufficientPermissionsFaultMapper permissionsFaultMapper = new VssInsufficientPermissionsFaultMapper();
      permissionsFaultMapper.Enabled = true;
      permissionsFaultMapper.Retriable = false;
      permissionsFaultMapper.LogFault = false;
      permissionsFaultMapper.Severity = IndexerFaultSeverity.Low;
      faultMappers10.Add((FaultMapper) permissionsFaultMapper);
      List<FaultMapper> faultMappers11 = IndexFaultMapManager.s_faultMappers;
      VssTimeOutFaultMapper timeOutFaultMapper2 = new VssTimeOutFaultMapper();
      timeOutFaultMapper2.Enabled = true;
      timeOutFaultMapper2.Retriable = false;
      timeOutFaultMapper2.Severity = IndexerFaultSeverity.Medium;
      faultMappers11.Add((FaultMapper) timeOutFaultMapper2);
      List<FaultMapper> faultMappers12 = IndexFaultMapManager.s_faultMappers;
      TfsBranchNotFoundFaultMapper foundFaultMapper1 = new TfsBranchNotFoundFaultMapper();
      foundFaultMapper1.Enabled = true;
      foundFaultMapper1.Retriable = false;
      foundFaultMapper1.LogFault = false;
      foundFaultMapper1.Severity = IndexerFaultSeverity.Low;
      faultMappers12.Add((FaultMapper) foundFaultMapper1);
      List<FaultMapper> faultMappers13 = IndexFaultMapManager.s_faultMappers;
      TfvcScopePathConflictFaultMapper conflictFaultMapper = new TfvcScopePathConflictFaultMapper();
      conflictFaultMapper.Enabled = true;
      conflictFaultMapper.Retriable = false;
      conflictFaultMapper.LogFault = false;
      conflictFaultMapper.Severity = IndexerFaultSeverity.Low;
      faultMappers13.Add((FaultMapper) conflictFaultMapper);
      List<FaultMapper> faultMappers14 = IndexFaultMapManager.s_faultMappers;
      WorkItemDeletedFaultMapper deletedFaultMapper1 = new WorkItemDeletedFaultMapper();
      deletedFaultMapper1.Enabled = true;
      deletedFaultMapper1.Retriable = false;
      deletedFaultMapper1.LogFault = false;
      deletedFaultMapper1.Severity = IndexerFaultSeverity.Low;
      faultMappers14.Add((FaultMapper) deletedFaultMapper1);
      List<FaultMapper> faultMappers15 = IndexFaultMapManager.s_faultMappers;
      WorkItemPermanentlyDeletedFaultMapper deletedFaultMapper2 = new WorkItemPermanentlyDeletedFaultMapper();
      deletedFaultMapper2.Enabled = true;
      deletedFaultMapper2.Retriable = false;
      deletedFaultMapper2.LogFault = false;
      deletedFaultMapper2.Severity = IndexerFaultSeverity.Low;
      faultMappers15.Add((FaultMapper) deletedFaultMapper2);
      List<FaultMapper> faultMappers16 = IndexFaultMapManager.s_faultMappers;
      WorkItemAreaIterationIdNotRecognizedFaultMapper recognizedFaultMapper1 = new WorkItemAreaIterationIdNotRecognizedFaultMapper();
      recognizedFaultMapper1.Enabled = true;
      recognizedFaultMapper1.Retriable = false;
      recognizedFaultMapper1.LogFault = false;
      recognizedFaultMapper1.Severity = IndexerFaultSeverity.Low;
      faultMappers16.Add((FaultMapper) recognizedFaultMapper1);
      List<FaultMapper> faultMappers17 = IndexFaultMapManager.s_faultMappers;
      ClassificationNodeNotRecognizedFaultMapper recognizedFaultMapper2 = new ClassificationNodeNotRecognizedFaultMapper();
      recognizedFaultMapper2.Enabled = true;
      recognizedFaultMapper2.Retriable = true;
      recognizedFaultMapper2.LogFault = false;
      recognizedFaultMapper2.Severity = IndexerFaultSeverity.Low;
      faultMappers17.Add((FaultMapper) recognizedFaultMapper2);
      List<FaultMapper> faultMappers18 = IndexFaultMapManager.s_faultMappers;
      ProjectNotFoundFaultMapper foundFaultMapper2 = new ProjectNotFoundFaultMapper();
      foundFaultMapper2.Enabled = true;
      foundFaultMapper2.Retriable = false;
      foundFaultMapper2.LogFault = false;
      foundFaultMapper2.Severity = IndexerFaultSeverity.Low;
      faultMappers18.Add((FaultMapper) foundFaultMapper2);
      List<FaultMapper> faultMappers19 = IndexFaultMapManager.s_faultMappers;
      DefaultTeamNotFoundFaultMapper foundFaultMapper3 = new DefaultTeamNotFoundFaultMapper();
      foundFaultMapper3.Enabled = true;
      foundFaultMapper3.Retriable = false;
      foundFaultMapper3.LogFault = false;
      foundFaultMapper3.Severity = IndexerFaultSeverity.Low;
      faultMappers19.Add((FaultMapper) foundFaultMapper3);
      List<FaultMapper> faultMappers20 = IndexFaultMapManager.s_faultMappers;
      LocationHttpClientCircuitBreakerFaultMapper breakerFaultMapper = new LocationHttpClientCircuitBreakerFaultMapper();
      breakerFaultMapper.Enabled = true;
      breakerFaultMapper.Retriable = false;
      breakerFaultMapper.Severity = IndexerFaultSeverity.Low;
      faultMappers20.Add((FaultMapper) breakerFaultMapper);
      List<FaultMapper> faultMappers21 = IndexFaultMapManager.s_faultMappers;
      RepoDoesNotExistFaultMapper existFaultMapper1 = new RepoDoesNotExistFaultMapper();
      existFaultMapper1.Enabled = true;
      existFaultMapper1.Retriable = false;
      existFaultMapper1.LogFault = false;
      existFaultMapper1.Severity = IndexerFaultSeverity.Low;
      faultMappers21.Add((FaultMapper) existFaultMapper1);
      List<FaultMapper> faultMappers22 = IndexFaultMapManager.s_faultMappers;
      GitItemDoesNotExistFaultMapper existFaultMapper2 = new GitItemDoesNotExistFaultMapper();
      existFaultMapper2.Enabled = true;
      existFaultMapper2.Retriable = false;
      existFaultMapper2.LogFault = false;
      existFaultMapper2.Severity = IndexerFaultSeverity.Low;
      faultMappers22.Add((FaultMapper) existFaultMapper2);
      List<FaultMapper> faultMappers23 = IndexFaultMapManager.s_faultMappers;
      GitCommitNotFoundFaultMapper foundFaultMapper4 = new GitCommitNotFoundFaultMapper();
      foundFaultMapper4.Enabled = true;
      foundFaultMapper4.Retriable = false;
      foundFaultMapper4.LogFault = false;
      foundFaultMapper4.Severity = IndexerFaultSeverity.Low;
      faultMappers23.Add((FaultMapper) foundFaultMapper4);
      List<FaultMapper> faultMappers24 = IndexFaultMapManager.s_faultMappers;
      TfvcItemNotFoundFaultMapper foundFaultMapper5 = new TfvcItemNotFoundFaultMapper();
      foundFaultMapper5.Enabled = true;
      foundFaultMapper5.Retriable = false;
      foundFaultMapper5.LogFault = false;
      foundFaultMapper5.Severity = IndexerFaultSeverity.Low;
      faultMappers24.Add((FaultMapper) foundFaultMapper5);
      List<FaultMapper> faultMappers25 = IndexFaultMapManager.s_faultMappers;
      HostStoppedFaultMapper stoppedFaultMapper = new HostStoppedFaultMapper();
      stoppedFaultMapper.Enabled = true;
      stoppedFaultMapper.Retriable = false;
      stoppedFaultMapper.LogFault = false;
      stoppedFaultMapper.Severity = IndexerFaultSeverity.Low;
      faultMappers25.Add((FaultMapper) stoppedFaultMapper);
      List<FaultMapper> faultMappers26 = IndexFaultMapManager.s_faultMappers;
      ItemBatchNotFoundExceptionFaultMapper exceptionFaultMapper2 = new ItemBatchNotFoundExceptionFaultMapper();
      exceptionFaultMapper2.Enabled = true;
      exceptionFaultMapper2.Retriable = false;
      exceptionFaultMapper2.LogFault = false;
      exceptionFaultMapper2.Severity = IndexerFaultSeverity.Low;
      faultMappers26.Add((FaultMapper) exceptionFaultMapper2);
      List<FaultMapper> faultMappers27 = IndexFaultMapManager.s_faultMappers;
      ContinuationTokenInvalidFaultMapper invalidFaultMapper = new ContinuationTokenInvalidFaultMapper();
      invalidFaultMapper.Enabled = true;
      invalidFaultMapper.Retriable = false;
      invalidFaultMapper.LogFault = false;
      invalidFaultMapper.Severity = IndexerFaultSeverity.Low;
      faultMappers27.Add((FaultMapper) invalidFaultMapper);
    }
  }
}
