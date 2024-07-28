// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceHostHistoryJobExtension
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServiceHostHistoryJobExtension : ITeamFoundationJobExtension
  {
    private const string ResultMessageFormat = "Fired {0} service host history entries - watermark set to {1}";
    private const string TcpPrefix = "tcp:";
    internal static readonly string s_maxServiceHostHistoryTracesPerIterationRegistryPath = FrameworkServerConstants.HostManagementRoot + "/MaxServiceHostHistoryTracesPerIteration";

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int historyId = service.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.ServiceHostHistoryWatermark, 0);
      int num1 = service.GetValue<int>(requestContext, (RegistryQuery) ServiceHostHistoryJobExtension.s_maxServiceHostHistoryTracesPerIterationRegistryPath, 10000);
      int num2 = 0;
      do
      {
        List<ServiceHostHistoryEntry> hostHistoryEntryList = this.QueryServiceHostHistoryForTracing(requestContext, historyId);
        if (hostHistoryEntryList != null && hostHistoryEntryList.Count > 0)
        {
          foreach (ServiceHostHistoryEntry entry in hostHistoryEntryList)
          {
            historyId = entry.HistoryId;
            if (entry.DatabaseId != -2)
            {
              string serverName = string.Empty;
              string databaseName = string.Empty;
              if (entry.DatabaseId != DatabaseManagementConstants.InvalidDatabaseId)
                this.GetDatabaseProperties(requestContext, entry.DatabaseId, out serverName, out databaseName);
              this.TraceServiceHostHistory(entry, serverName, databaseName);
              ++num2;
              if (num2 >= num1)
                break;
            }
          }
          if (historyId != 0)
            service.SetValue<int>(requestContext, FrameworkServerConstants.ServiceHostHistoryWatermark, historyId);
        }
        else
          break;
      }
      while (num2 < num1);
      resultMessage = string.Format("Fired {0} service host history entries - watermark set to {1}", (object) num2, (object) historyId);
      return TeamFoundationJobExecutionResult.Succeeded;
    }

    internal virtual void GetDatabaseProperties(
      IVssRequestContext requestContext,
      int databaseId,
      out string serverName,
      out string databaseName)
    {
      serverName = string.Empty;
      databaseName = string.Empty;
      if (databaseId == DatabaseManagementConstants.InvalidDatabaseId)
        return;
      ITeamFoundationDatabaseManagementService service = requestContext.GetService<ITeamFoundationDatabaseManagementService>();
      try
      {
        ITeamFoundationDatabaseProperties database = service.GetDatabase(requestContext, databaseId);
        if (database == null)
          return;
        serverName = database.SqlConnectionInfo.DataSource;
        databaseName = database.SqlConnectionInfo.InitialCatalog;
        if (serverName == null)
          return;
        serverName = serverName.Trim();
        if (!serverName.StartsWith("tcp:", StringComparison.OrdinalIgnoreCase))
          return;
        serverName = serverName.Substring("tcp:".Length);
      }
      catch (DatabaseNotFoundException ex)
      {
        requestContext.TraceException(0, "HostManagement", "ServiceHostHistoryJob", (Exception) ex);
      }
    }

    internal virtual List<ServiceHostHistoryEntry> QueryServiceHostHistoryForTracing(
      IVssRequestContext requestContext,
      int watermark)
    {
      using (HostManagementComponent component = requestContext.CreateComponent<HostManagementComponent>())
        return component.QueryServiceHostHistoryForTracing(watermark)?.GetCurrent<ServiceHostHistoryEntry>().Items;
    }

    internal virtual void TraceServiceHostHistory(
      ServiceHostHistoryEntry entry,
      string serverName,
      string databaseName)
    {
      TeamFoundationTracingService.TraceServiceHostHistory(entry.HostId, entry.InsertedDate, (short) entry.ChangeType, entry.ParentHostId, serverName, databaseName, entry.DatabaseId, entry.StorageAccountId, entry.Name, (short) entry.Status, entry.StatusReason, (short) entry.HostType, entry.LastUserAccess, (int) entry.SubStatus);
    }
  }
}
