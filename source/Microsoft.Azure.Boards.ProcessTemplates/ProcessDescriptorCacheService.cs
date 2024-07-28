// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessDescriptorCacheService
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class ProcessDescriptorCacheService : VssMemoryCacheService<Guid, ProcessDescriptor>
  {
    private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromMinutes(2.0);
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromMinutes(15.0);
    private const int c_maxCacheSize = 200;
    private Guid? m_defaultProcessTypeId;
    private ILockName m_gateDefaultProcess;
    private ISet<Guid> m_disabledProcessTypeIdSet;
    private ILockName m_gateDisabledProcessIdSet;
    private IReadOnlyCollection<ProcessDescriptor> m_tipList;
    private ILockName m_gateTipList;

    public ProcessDescriptorCacheService()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, new MemoryCacheConfiguration<Guid, ProcessDescriptor>().WithMaxElements(200).WithCleanupInterval(ProcessDescriptorCacheService.s_cacheCleanupInterval))
    {
      this.InactivityInterval.Value = ProcessDescriptorCacheService.s_maxCacheInactivityAge;
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      this.m_gateDefaultProcess = requestContext.ServiceHost.CreateUniqueLockName(this.GetType().Name + "/DefaultProcessId");
      this.m_gateDisabledProcessIdSet = requestContext.ServiceHost.CreateUniqueLockName(this.GetType().Name + "/DisabledProcessesIdSet");
      this.m_gateTipList = requestContext.ServiceHost.CreateUniqueLockName(this.GetType().Name + "/TipList");
    }

    public void ClearDisabledProcessIdSet(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.AcquireWriterLock(this.m_gateDisabledProcessIdSet))
        this.m_disabledProcessTypeIdSet = (ISet<Guid>) null;
    }

    public bool TryGetDisabledProcessIdSet(
      IVssRequestContext requestContext,
      out ISet<Guid> disabledProcessTypeIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.AcquireReaderLock(this.m_gateDisabledProcessIdSet))
      {
        disabledProcessTypeIds = this.m_disabledProcessTypeIdSet;
        return this.m_disabledProcessTypeIdSet != null;
      }
    }

    public void SetDisabledProcessIdSet(
      IVssRequestContext requestContext,
      ISet<Guid> disabledProcessTypeIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ISet<Guid>>(disabledProcessTypeIds, nameof (disabledProcessTypeIds));
      using (requestContext.AcquireWriterLock(this.m_gateDisabledProcessIdSet))
        this.m_disabledProcessTypeIdSet = disabledProcessTypeIds;
    }

    public void SetDisabledProcessId(
      IVssRequestContext requestContext,
      Guid processTypeId,
      bool isEnabled)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.AcquireWriterLock(this.m_gateDisabledProcessIdSet))
      {
        if (this.m_disabledProcessTypeIdSet == null)
          return;
        if (isEnabled)
          this.m_disabledProcessTypeIdSet.Remove(processTypeId);
        else
          this.m_disabledProcessTypeIdSet.Add(processTypeId);
      }
    }

    public void ClearDefaultProcessId(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.AcquireWriterLock(this.m_gateDefaultProcess))
        this.m_disabledProcessTypeIdSet = (ISet<Guid>) null;
    }

    public bool TryGetDefaultProcessId(
      IVssRequestContext requestContext,
      out Guid defaultProcessTypeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.AcquireReaderLock(this.m_gateDefaultProcess))
      {
        if (this.m_defaultProcessTypeId.HasValue)
        {
          defaultProcessTypeId = this.m_defaultProcessTypeId.Value;
          return true;
        }
        defaultProcessTypeId = Guid.Empty;
        return false;
      }
    }

    public void SetDefaultProcessTypeId(
      IVssRequestContext requestContext,
      Guid defaultProcessTypeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.AcquireWriterLock(this.m_gateDefaultProcess))
        this.m_defaultProcessTypeId = new Guid?(defaultProcessTypeId);
    }

    public virtual bool TryGetDescriptor(
      IVssRequestContext requestContext,
      Guid id,
      out ProcessDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return this.TryGetValue(requestContext, id, out descriptor);
    }

    public void Set(IVssRequestContext requestContext, ProcessDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessDescriptor>(descriptor, nameof (descriptor));
      this.Set(requestContext, descriptor.RowId, descriptor);
      if (!descriptor.IsLatest)
        return;
      this.Set(requestContext, descriptor.TypeId, descriptor);
      this.AddToTipList(requestContext, descriptor);
    }

    public void Set(IVssRequestContext requestContext, IEnumerable<ProcessDescriptor> descriptors)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<ProcessDescriptor>>(descriptors, nameof (descriptors));
      foreach (ProcessDescriptor descriptor in descriptors)
        this.Set(requestContext, descriptor);
    }

    public void Remove(IVssRequestContext requestContext, ProcessDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessDescriptor>(descriptor, nameof (descriptor));
      this.RemoveByIds(requestContext, (IEnumerable<Guid>) new Guid[2]
      {
        descriptor.RowId,
        descriptor.TypeId
      });
    }

    public void RemoveByIds(IVssRequestContext requestContext, IEnumerable<Guid> ids)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(ids, nameof (ids));
      foreach (Guid id in ids)
        this.Remove(requestContext, id);
      this.RemoveFromTipList(requestContext, ids);
    }

    public void InvalidateByIds(IVssRequestContext requestContext, IEnumerable<Guid> ids)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(ids, nameof (ids));
      foreach (Guid id in ids)
        this.Remove(requestContext, id);
      this.ClearTipList(requestContext);
    }

    public override void Clear(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      base.Clear(requestContext);
      this.ClearDefaultProcessId(requestContext);
      this.ClearDisabledProcessIdSet(requestContext);
      this.ClearTipList(requestContext);
    }

    public bool TryGetTipList(
      IVssRequestContext requestContext,
      out IReadOnlyCollection<ProcessDescriptor> descriptors)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.AcquireReaderLock(this.m_gateTipList))
        descriptors = this.m_tipList;
      return descriptors != null;
    }

    private void ClearTipList(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.AcquireWriterLock(this.m_gateTipList))
        this.m_tipList = (IReadOnlyCollection<ProcessDescriptor>) null;
    }

    public void SetTipList(
      IVssRequestContext requestContext,
      IReadOnlyCollection<ProcessDescriptor> descriptors)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IReadOnlyCollection<ProcessDescriptor>>(descriptors, nameof (descriptors));
      this.Set(requestContext, (IEnumerable<ProcessDescriptor>) descriptors);
      using (requestContext.AcquireWriterLock(this.m_gateTipList))
        this.m_tipList = descriptors;
    }

    private void AddToTipList(IVssRequestContext requestContext, ProcessDescriptor descriptor)
    {
      using (requestContext.AcquireWriterLock(this.m_gateTipList))
      {
        if (this.m_tipList == null)
          return;
        Dictionary<Guid, ProcessDescriptor> dictionary = this.m_tipList.ToDictionary<ProcessDescriptor, Guid>((Func<ProcessDescriptor, Guid>) (d => d.TypeId));
        dictionary[descriptor.TypeId] = descriptor;
        this.m_tipList = (IReadOnlyCollection<ProcessDescriptor>) dictionary.Values.ToArray<ProcessDescriptor>();
      }
    }

    private void RemoveFromTipList(IVssRequestContext requestContext, IEnumerable<Guid> ids)
    {
      using (requestContext.AcquireWriterLock(this.m_gateTipList))
      {
        if (this.m_tipList == null)
          return;
        HashSet<Guid> idSet = new HashSet<Guid>(ids);
        this.m_tipList = (IReadOnlyCollection<ProcessDescriptor>) this.m_tipList.Where<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (d => !idSet.Contains(d.TypeId))).ToArray<ProcessDescriptor>();
      }
    }
  }
}
