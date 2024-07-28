// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Telemetry.AnalyticsOnPremTelemetryService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Analytics.Telemetry
{
  public class AnalyticsOnPremTelemetryService : 
    IAnalyticsOnPremTelemetryService,
    IVssFrameworkService
  {
    private bool m_telemetryEnabled;

    public void IncrementCounter(
      IVssRequestContext requestContext,
      string counterArea,
      string counterName,
      int value = 1)
    {
      if (!this.m_telemetryEnabled)
        return;
      try
      {
        string str = AnalyticsTelemetryConstants.AnalyticsTelemetryCountersRoot + "/" + counterArea + "/" + counterName;
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
        List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
        using (requestContext.AcquireWriterLock(requestContext.ServiceHost.CreateLockName(str)))
        {
          int num = service.GetValue<int>(vssRequestContext, (RegistryQuery) str, 0) + value;
          registryEntryList.Add(new RegistryEntry(str, num.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
        }
      }
      catch (Exception ex)
      {
      }
    }

    public void SetDataQualityResult(IVssRequestContext requestContext, DataQualityResult result)
    {
      if (!this.m_telemetryEnabled)
        return;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      try
      {
        string str = string.Format("{0}/{1}/{2}/{3}", (object) AnalyticsTelemetryConstants.AnalyticsTelemetryDataQualityRoot, (object) requestContext.ServiceHost.InstanceId, (object) result.Name, (object) result.Scope);
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
        CachedRegistryService service = context.GetService<CachedRegistryService>();
        List<RegistryEntry> registryEntryList1 = new List<RegistryEntry>();
        registryEntryList1.Add(new RegistryEntry(str + "/KPI", result.KpiValue.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        registryEntryList1.Add(new RegistryEntry(str + "/Actual", result.ActualValue.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        registryEntryList1.Add(new RegistryEntry(str + "/Expected", result.ExpectedValue.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        IVssRequestContext requestContext1 = context;
        List<RegistryEntry> registryEntryList2 = registryEntryList1;
        service.WriteEntries(requestContext1, (IEnumerable<RegistryEntry>) registryEntryList2);
      }
      catch (Exception ex)
      {
      }
    }

    public void SetDatabaseSegmentFragmentationResult(
      IVssRequestContext requestContext,
      int databaseId,
      ColumnStoreFragmentationStatistics columnStoreIndexStatRowAggregated)
    {
      if (!this.m_telemetryEnabled)
        return;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      try
      {
        string str = string.Format("{0}/{1}/{2}/{3}", (object) AnalyticsTelemetryConstants.AnalyticsTelemetryDatabaseFragmentationRoot, (object) databaseId, (object) columnStoreIndexStatRowAggregated.IndexName, (object) columnStoreIndexStatRowAggregated.Action);
        CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
        List<RegistryEntry> registryEntryList1 = new List<RegistryEntry>();
        List<RegistryEntry> registryEntryList2 = registryEntryList1;
        string registryPath1 = str + "/TotalCount";
        long num = columnStoreIndexStatRowAggregated.TotalCount;
        string registryValue1 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        RegistryEntry registryEntry1 = new RegistryEntry(registryPath1, registryValue1);
        registryEntryList2.Add(registryEntry1);
        List<RegistryEntry> registryEntryList3 = registryEntryList1;
        string registryPath2 = str + "/DictionarySizeRowGroupCount";
        num = columnStoreIndexStatRowAggregated.DictonarySizeRowGroupCount;
        string registryValue2 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        RegistryEntry registryEntry2 = new RegistryEntry(registryPath2, registryValue2);
        registryEntryList3.Add(registryEntry2);
        List<RegistryEntry> registryEntryList4 = registryEntryList1;
        string registryPath3 = str + "/FragmentedRowGroupCount";
        num = columnStoreIndexStatRowAggregated.FragmentedRowGroupCount;
        string registryValue3 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        RegistryEntry registryEntry3 = new RegistryEntry(registryPath3, registryValue3);
        registryEntryList4.Add(registryEntry3);
        registryEntryList1.Add(new RegistryEntry(str + "/MaxFragmentation", columnStoreIndexStatRowAggregated.MaxFragmentation.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        registryEntryList1.Add(new RegistryEntry(str + "/OpenRowGroupsCount", columnStoreIndexStatRowAggregated.OpenRowGroupsCount.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        List<RegistryEntry> registryEntryList5 = registryEntryList1;
        string registryPath4 = str + "/OpenRowGroupMaxSizeInBytes";
        num = columnStoreIndexStatRowAggregated.OpenRowGroupMaxSizeInBytes;
        string registryValue4 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        RegistryEntry registryEntry4 = new RegistryEntry(registryPath4, registryValue4);
        registryEntryList5.Add(registryEntry4);
        List<RegistryEntry> registryEntryList6 = registryEntryList1;
        string registryPath5 = str + "/OpenRowGroupMaxRowCount";
        num = columnStoreIndexStatRowAggregated.OpenRowGroupMaxRowCount;
        string registryValue5 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        RegistryEntry registryEntry5 = new RegistryEntry(registryPath5, registryValue5);
        registryEntryList6.Add(registryEntry5);
        IVssRequestContext requestContext1 = requestContext;
        List<RegistryEntry> registryEntryList7 = registryEntryList1;
        service.WriteEntries(requestContext1, (IEnumerable<RegistryEntry>) registryEntryList7);
      }
      catch (Exception ex)
      {
      }
    }

    public void SetDatabaseSegmentOverlapsResult(
      IVssRequestContext requestContext,
      int databaseId,
      string tableName,
      int overlaps,
      long segmentsInPartition)
    {
      if (!this.m_telemetryEnabled)
        return;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      try
      {
        string str = string.Format("{0}/{1}/{2}", (object) AnalyticsTelemetryConstants.AnalyticsTelemetryDatabaseOverlapsRoot, (object) databaseId, (object) tableName);
        CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
        List<RegistryEntry> registryEntryList1 = new List<RegistryEntry>();
        registryEntryList1.Add(new RegistryEntry(str + "/Overlaps", overlaps.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        registryEntryList1.Add(new RegistryEntry(str + "/SegmentsInPartition", segmentsInPartition.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        IVssRequestContext requestContext1 = requestContext;
        List<RegistryEntry> registryEntryList2 = registryEntryList1;
        service.WriteEntries(requestContext1, (IEnumerable<RegistryEntry>) registryEntryList2);
      }
      catch (Exception ex)
      {
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      this.m_telemetryEnabled = vssRequestContext.GetService<CachedRegistryService>().GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData, false);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
