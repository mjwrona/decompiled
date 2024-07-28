// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingConstantsCache
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemTrackingConstantsCache
  {
    private WorkItemTrackingConstantsCache.ConstantsCache<string> m_nonIdentityConstantsByDisplayText;
    private WorkItemTrackingConstantsCache.ConstantsCache<string> m_activeAndInactiveConstantsByDisplayText;
    private WorkItemTrackingConstantsCache.ConstantsCache<string> m_activeConstantsByDisplayText;
    private WorkItemTrackingConstantsCache.ConstantsCache<int> m_activeAndInactiveConstantsById;
    private WorkItemTrackingConstantsCache.ConstantsCache<int> m_activeConstantsById;
    private WorkItemTrackingConstantsCache.ConstantsCache<Guid> m_activeAndInactiveConstantsByTFID;
    private WorkItemTrackingConstantsCache.ConstantsCache<Guid> m_activeConstantsByTFID;
    private long m_currentConstantsStamp;
    private const string c_maxElementRegistryPath = "/Service/WorkItemTracking/Settings/MaxConstantsCacheItems";
    private const int c_defaultMaxElements = 1024;

    public WorkItemTrackingConstantsCache(IVssRequestContext requestContext)
    {
      this.m_nonIdentityConstantsByDisplayText = new WorkItemTrackingConstantsCache.ConstantsCache<string>(requestContext, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_activeAndInactiveConstantsByDisplayText = new WorkItemTrackingConstantsCache.ConstantsCache<string>(requestContext, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_activeConstantsByDisplayText = new WorkItemTrackingConstantsCache.ConstantsCache<string>(requestContext, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_activeAndInactiveConstantsById = new WorkItemTrackingConstantsCache.ConstantsCache<int>(requestContext);
      this.m_activeConstantsById = new WorkItemTrackingConstantsCache.ConstantsCache<int>(requestContext);
      this.m_activeAndInactiveConstantsByTFID = new WorkItemTrackingConstantsCache.ConstantsCache<Guid>(requestContext);
      this.m_activeConstantsByTFID = new WorkItemTrackingConstantsCache.ConstantsCache<Guid>(requestContext);
      this.EnsureCacheFreshness(requestContext);
    }

    public void AddConstants(
      IVssRequestContext requestContext,
      IEnumerable<ConstantRecord> constants,
      bool includesInactiveConstants,
      bool areNonIdentityConstants = false)
    {
      foreach (ConstantRecord constant in constants)
      {
        bool flag1 = constant.TeamFoundationId != Guid.Empty;
        bool flag2 = !string.IsNullOrEmpty(constant.DisplayText);
        this.m_activeAndInactiveConstantsById.Set(requestContext, constant.Id, constant);
        if (flag2)
        {
          if (areNonIdentityConstants)
            this.m_nonIdentityConstantsByDisplayText.Set(requestContext, constant.DisplayText, constant);
          this.m_activeAndInactiveConstantsByDisplayText.Set(requestContext, constant.DisplayText, constant);
        }
        if (flag1)
          this.m_activeAndInactiveConstantsByTFID.Set(requestContext, constant.TeamFoundationId, constant);
        if (!includesInactiveConstants)
        {
          this.m_activeConstantsById.Set(requestContext, constant.Id, constant);
          if (flag2)
            this.m_activeConstantsByDisplayText.Set(requestContext, constant.DisplayText, constant);
          if (flag1)
            this.m_activeConstantsByTFID.Set(requestContext, constant.TeamFoundationId, constant);
        }
      }
    }

    public bool TryGetConstants(
      IVssRequestContext requestContext,
      IEnumerable<string> constants,
      bool includeInactive,
      out IEnumerable<ConstantRecord> constantRecords)
    {
      this.EnsureCacheFreshness(requestContext);
      if (!includeInactive)
        return this.m_activeConstantsByDisplayText.TryGetConstants(requestContext, constants, out constantRecords);
      return this.m_activeAndInactiveConstantsByDisplayText.TryGetConstants(requestContext, constants, out constantRecords) || this.m_nonIdentityConstantsByDisplayText.TryGetConstants(requestContext, constants, out constantRecords);
    }

    public bool TryGetConstants(
      IVssRequestContext requestContext,
      IEnumerable<int> constantIds,
      bool includeInactive,
      out IEnumerable<ConstantRecord> constantRecords)
    {
      this.EnsureCacheFreshness(requestContext);
      return includeInactive ? this.m_activeAndInactiveConstantsById.TryGetConstants(requestContext, constantIds, out constantRecords) : this.m_activeConstantsById.TryGetConstants(requestContext, constantIds, out constantRecords);
    }

    public bool TryGetConstants(
      IVssRequestContext requestContext,
      IEnumerable<Guid> teamFoundationIds,
      bool includeInactive,
      out IEnumerable<ConstantRecord> constantRecords)
    {
      this.EnsureCacheFreshness(requestContext);
      return includeInactive ? this.m_activeAndInactiveConstantsByTFID.TryGetConstants(requestContext, teamFoundationIds, out constantRecords) : this.m_activeConstantsByTFID.TryGetConstants(requestContext, teamFoundationIds, out constantRecords);
    }

    private void EnsureCacheFreshness(IVssRequestContext requestContext)
    {
      long num = requestContext.MetadataDbStamps((IEnumerable<MetadataTable>) new MetadataTable[1]
      {
        MetadataTable.Constants
      }).First<KeyValuePair<MetadataTable, long>>().Value;
      if (this.m_currentConstantsStamp == num)
        return;
      this.m_currentConstantsStamp = num;
      this.m_activeConstantsByDisplayText.Clear(requestContext);
      this.m_activeConstantsById.Clear(requestContext);
      this.m_activeConstantsByTFID.Clear(requestContext);
    }

    private class ConstantsCache<TKey> : VssMemoryCacheService<TKey, ConstantRecord>
    {
      public ConstantsCache(IVssRequestContext requestContext)
        : this(requestContext, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default)
      {
      }

      public ConstantsCache(IVssRequestContext requestContext, IEqualityComparer<TKey> comparer)
        : base(comparer, WorkItemTrackingConstantsCache.ConstantsCache<TKey>.GetConstantsCacheConfiguration<TKey>(requestContext))
      {
      }

      public bool TryGetConstants(
        IVssRequestContext requestContext,
        IEnumerable<TKey> constantKeys,
        out IEnumerable<ConstantRecord> constantRecords)
      {
        constantRecords = Enumerable.Empty<ConstantRecord>();
        List<ConstantRecord> constantRecordList = new List<ConstantRecord>();
        foreach (TKey constantKey in constantKeys)
        {
          ConstantRecord constantRecord;
          if (!this.TryGetValue(requestContext, constantKey, out constantRecord) || constantRecord == null)
            return false;
          constantRecordList.Add(constantRecord);
        }
        constantRecords = (IEnumerable<ConstantRecord>) constantRecordList;
        return true;
      }

      private static MemoryCacheConfiguration<T, ConstantRecord> GetConstantsCacheConfiguration<T>(
        IVssRequestContext requestContext)
      {
        return new MemoryCacheConfiguration<T, ConstantRecord>().WithMaxElements(requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/MaxConstantsCacheItems", 1024));
      }
    }
  }
}
