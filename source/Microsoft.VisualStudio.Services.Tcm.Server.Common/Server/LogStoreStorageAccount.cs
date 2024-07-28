// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreStorageAccount
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class LogStoreStorageAccount
  {
    public LogStoreStorageAccount(int storageAccountIndex) => this.StorageAccountConnectionIndex = storageAccountIndex;

    public int StorageAccountConnectionIndex { get; }

    public TcmStorageAccountStatus StorageAccountStatus { get; set; }

    public int AllocatedTenantCount { get; set; }

    public int MaxTenantCount { get; set; }

    public static void CreateTcmLogStoreStorageAccounts(
      IVssRequestContext requestContext,
      int storageAccountCount)
    {
      using (PerfManager.Measure(requestContext, "LogStorage", TraceUtils.GetActionName("LogStoreStorageAccount.CreateTcmLogStoreStorageAccounts", "Tcm")))
      {
        requestContext.TraceVerbose("LogStorage", "CreateTcmLogStoreStorageAccounts - {0}", (object) storageAccountCount);
        List<LogStoreStorageAccount> logStoreStorageAccounts = new List<LogStoreStorageAccount>(storageAccountCount);
        for (int storageAccountIndex = 0; storageAccountIndex < storageAccountCount; ++storageAccountIndex)
          logStoreStorageAccounts.Add(new LogStoreStorageAccount(storageAccountIndex)
          {
            StorageAccountStatus = TcmStorageAccountStatus.Free
          });
        try
        {
          using (TestManagementConfigDatabase component = requestContext.CreateComponent<TestManagementConfigDatabase>())
            component.CreateLogStoreStorageAccounts((IEnumerable<LogStoreStorageAccount>) logStoreStorageAccounts);
        }
        catch (TestManagementInvalidOperationException ex)
        {
          requestContext.TraceException("LogStorage", (Exception) ex);
        }
      }
    }
  }
}
