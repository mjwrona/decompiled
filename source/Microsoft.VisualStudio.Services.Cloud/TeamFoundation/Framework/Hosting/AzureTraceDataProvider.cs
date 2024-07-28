// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Hosting.AzureTraceDataProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.TeamFoundation.Framework.Hosting
{
  public class AzureTraceDataProvider : ITraceDataProvider
  {
    private CloudStorageAccount m_cloudStorageAccount;
    private static readonly string DiagnosticsConnectionString = nameof (DiagnosticsConnectionString);

    public void ServiceStart(IVssRequestContext requestContext)
    {
      try
      {
        string connectionString = (string) null;
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Cloud.UseStrongBoxToGetDiagnosticsConnectionString"))
        {
          try
          {
            ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
            Guid drawerId = service.UnlockDrawer(requestContext, FrameworkServerConstants.ConfigurationSecretsDrawerName, true);
            connectionString = service.GetString(requestContext, drawerId, AzureTraceDataProvider.DiagnosticsConnectionString);
            requestContext.TraceAlways(2349959, TraceLevel.Info, nameof (AzureTraceDataProvider), nameof (AzureTraceDataProvider), "DiagnosticsConnectionString obtained from StrongBox.");
          }
          catch (Exception ex)
          {
            requestContext.TraceAlways(2349958, TraceLevel.Error, nameof (AzureTraceDataProvider), nameof (AzureTraceDataProvider), "Failed to obtain DiagnosticsConnectionString from StrongBox. Error: {0}", (object) ex.Message);
          }
        }
        else
          connectionString = AzureRoleUtil.Configuration.GetDecryptedString(AzureTraceDataProvider.DiagnosticsConnectionString);
        if (!string.IsNullOrEmpty(connectionString))
          this.m_cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
      if (this.m_cloudStorageAccount == null)
        throw new InvalidOperationException("The Azure Trace Data Provider is not configured properly");
    }

    public void ServiceEnd()
    {
    }

    public IEnumerable<TraceEvent> QueryTraceData(
      IVssRequestContext requestContext,
      Guid traceId,
      DateTime since,
      int pageSize)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      AzureTraceDataProvider.\u003C\u003Ec__DisplayClass2_0 cDisplayClass20 = new AzureTraceDataProvider.\u003C\u003Ec__DisplayClass2_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass20.since = since;
      AzureTableStorageContext tableStorageContext = new AzureTableStorageContext(this.m_cloudStorageAccount.TableEndpoint.ToString(), this.m_cloudStorageAccount);
      List<TraceEvent> traceEventList = new List<TraceEvent>(pageSize);
      int num1 = 0;
      int num2;
      do
      {
        ParameterExpression parameterExpression;
        // ISSUE: method reference
        // ISSUE: reference to a compiler-generated field
        // ISSUE: method reference
        // ISSUE: method reference
        // ISSUE: method reference
        // ISSUE: method reference
        // ISSUE: method reference
        IEnumerable<AzureEventLogEntry> azureEventLogEntries = (IEnumerable<AzureEventLogEntry>) tableStorageContext.EventLogTable.Where<AzureEventLogEntry>(Expression.Lambda<Func<AzureEventLogEntry, bool>>((Expression) Expression.AndAlso((Expression) Expression.AndAlso(entry.EventId == 0, (Expression) Expression.GreaterThan((Expression) Expression.Call((Expression) null, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Compare)), new Expression[2]
        {
          entry.PartitionKey,
          (Expression) Expression.Add("0", (Expression) Expression.Call((Expression) Expression.Property((Expression) Expression.Call(cDisplayClass20.since, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DateTime.ToUniversalTime)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DateTime.get_Ticks))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (long.ToString)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Concat)))
        }), (Expression) Expression.Constant((object) 0, typeof (int)))), (Expression) Expression.Equal((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (AzureEventLogEntry.get_ProviderGuid))), (Expression) Expression.Constant((object) "{80761876-6844-47D5-8106-F8ED2AA8687B}", typeof (string)))), parameterExpression)).Take<AzureEventLogEntry>(pageSize);
        TraceEvent traceEvent = new TraceEvent();
        num2 = 0;
        try
        {
          foreach (AzureEventLogEntry azureLogEntry in azureEventLogEntries)
          {
            this.ExtractTraceData(ref traceEvent, azureLogEntry);
            ++num2;
            if (traceId.Equals(Guid.Empty) || traceEvent.TraceId.Equals(traceId))
            {
              traceEventList.Add(traceEvent);
              ++num1;
            }
          }
          if (num2 == pageSize)
          {
            // ISSUE: reference to a compiler-generated field
            cDisplayClass20.since = traceEvent.TimeCreated;
          }
        }
        catch (Exception ex)
        {
          return (IEnumerable<TraceEvent>) traceEventList;
        }
      }
      while (num2 == pageSize && num1 < pageSize);
      return (IEnumerable<TraceEvent>) traceEventList;
    }

    private void ExtractTraceData(ref TraceEvent traceEvent, AzureEventLogEntry azureLogEntry)
    {
      WindowsEventLogRecord windowsEventLogRecord = TeamFoundationSerializationUtility.Deserialize<WindowsEventLogRecord>(azureLogEntry.RawXml, UnknownXmlNodeProcessing.Ignore);
      traceEvent = new TraceEvent(windowsEventLogRecord.UserData.Info.Message);
      traceEvent.TraceId = windowsEventLogRecord.UserData.Info.TraceId;
      traceEvent.Tracepoint = windowsEventLogRecord.UserData.Info.Tracepoint;
      traceEvent.ServiceHost = windowsEventLogRecord.UserData.Info.ServiceHost;
      traceEvent.ContextId = windowsEventLogRecord.UserData.Info.ContextId;
      traceEvent.ProcessName = windowsEventLogRecord.UserData.Info.ProcessName;
      traceEvent.UserLogin = windowsEventLogRecord.UserData.Info.Username;
      traceEvent.Service = windowsEventLogRecord.UserData.Info.Service;
      traceEvent.Method = windowsEventLogRecord.UserData.Info.Method;
      traceEvent.Area = windowsEventLogRecord.UserData.Info.Area;
      traceEvent.Level = (TraceLevel) (azureLogEntry.Level - 1);
      traceEvent.UserAgent = windowsEventLogRecord.UserData.Info.UserAgent;
      traceEvent.Layer = windowsEventLogRecord.UserData.Info.Layer;
      traceEvent.Uri = windowsEventLogRecord.UserData.Info.Uri;
      traceEvent.Path = windowsEventLogRecord.UserData.Info.Path;
      traceEvent.TimeCreated = windowsEventLogRecord.SystemData.TimeCreated.SystemTime;
      traceEvent.ActivityId = windowsEventLogRecord.SystemData.Correlation.ActivityId;
    }
  }
}
