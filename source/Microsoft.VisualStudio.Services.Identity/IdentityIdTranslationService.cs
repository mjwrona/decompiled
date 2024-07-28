// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityIdTranslationService
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityIdTranslationService : IIdentityIdTranslationService, IVssFrameworkService
  {
    private Dictionary<Guid, IdentityIdTranslation> masterIdToIdentityTranslationMap;
    private Dictionary<Guid, IdentityIdTranslation> idToIdentityTranslationMap;
    private IList<IdentityIdTranslation> identityIdTranslations;
    private readonly object translationsLock = new object();
    private readonly object mapLock = new object();
    private const string s_area = "IdentityIdTranslationService";
    private const string s_layer = "IdentityIdTranslationService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ExecutionEnvironment.IsHostedDeployment || !systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      this.EnsureIdTranslationMapsExists<bool>(systemRequestContext, (Func<bool>) (() => false));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ExecutionEnvironment.IsHostedDeployment || !systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      this.ClearCaches(systemRequestContext);
    }

    public void CheckForLeakedMasterIds(IVssRequestContext requestContext, IEnumerable<Guid> ids)
    {
      if (!this.IsRequestContextAtCorrectLevel(requestContext))
        return;
      IdentityIdTranslation identityIdTranslation;
      IdentityIdTranslationService.CheckForLeakedMasterIds(requestContext, ids, (Func<Guid, IdentityIdTranslation>) (id => this.EnsureIdTranslationMapsExists<IdentityIdTranslation>(requestContext, (Func<IdentityIdTranslation>) (() => this.masterIdToIdentityTranslationMap.TryGetValue(id, out identityIdTranslation) ? identityIdTranslation : (IdentityIdTranslation) null))));
    }

    public Guid TranslateToMasterId(IVssRequestContext requestContext, Guid id)
    {
      if (!this.IsRequestContextAtCorrectLevel(requestContext))
        return id;
      IdentityIdTranslatorPerfCounters.TranslateToMasterId.CallsPerSecond.Increment();
      return this.EnsureIdTranslationMapsExists<Guid>(requestContext, (Func<Guid>) (() =>
      {
        IdentityIdTranslation identityIdTranslation;
        if (!this.idToIdentityTranslationMap.TryGetValue(id, out identityIdTranslation))
        {
          requestContext.Trace(80300, TraceLevel.Info, nameof (IdentityIdTranslationService), nameof (IdentityIdTranslationService), "Translation miss (TranslateToMasterId) - given id: {0}", (object) id);
          IdentityIdTranslatorPerfCounters.TranslateToMasterId.MissesPerSecond.Increment();
          return id;
        }
        requestContext.Trace(80301, TraceLevel.Info, nameof (IdentityIdTranslationService), nameof (IdentityIdTranslationService), "Translation hit (TranslateToMasterId) - given id: {0}, mapped id: {1}", (object) id, (object) identityIdTranslation.MasterId);
        IdentityIdTranslatorPerfCounters.TranslateToMasterId.HitsPerSecond.Increment();
        return identityIdTranslation.MasterId;
      }));
    }

    public Guid TranslateFromMasterId(IVssRequestContext requestContext, Guid masterId)
    {
      if (!this.IsRequestContextAtCorrectLevel(requestContext))
        return masterId;
      IdentityIdTranslatorPerfCounters.TranslateFromMasterId.CallsPerSecond.Increment();
      return this.EnsureIdTranslationMapsExists<Guid>(requestContext, (Func<Guid>) (() =>
      {
        IdentityIdTranslation identityIdTranslation;
        if (!this.masterIdToIdentityTranslationMap.TryGetValue(masterId, out identityIdTranslation))
        {
          requestContext.Trace(80302, TraceLevel.Info, nameof (IdentityIdTranslationService), nameof (IdentityIdTranslationService), "Translation miss (TranslateFromMasterId) - given id: {0}", (object) masterId);
          IdentityIdTranslatorPerfCounters.TranslateFromMasterId.MissesPerSecond.Increment();
          return masterId;
        }
        requestContext.Trace(80303, TraceLevel.Info, nameof (IdentityIdTranslationService), nameof (IdentityIdTranslationService), "Translation hit (TranslateFromMasterId) - given id: {0}, mapped id: {1}", (object) masterId, (object) identityIdTranslation.Id);
        IdentityIdTranslatorPerfCounters.TranslateFromMasterId.HitsPerSecond.Increment();
        return identityIdTranslation.Id;
      }));
    }

    private bool IsRequestContextAtCorrectLevel(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        requestContext.Trace(80320, TraceLevel.Warning, nameof (IdentityIdTranslationService), nameof (IdentityIdTranslationService), "Identity id translation service is called at deployment level.");
        return false;
      }
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        return true;
      requestContext.Trace(80321, TraceLevel.Info, nameof (IdentityIdTranslationService), nameof (IdentityIdTranslationService), "Identity id translation service is called at organization level.");
      return false;
    }

    private static void CheckForLeakedMasterIds(
      IVssRequestContext requestContext,
      IEnumerable<Guid> ids,
      Func<Guid, IdentityIdTranslation> getTranslationForMasterId)
    {
      foreach (Guid id in ids)
      {
        IdentityIdTranslation identityIdTranslation = getTranslationForMasterId(id);
        if (identityIdTranslation != null && identityIdTranslation.Id != identityIdTranslation.MasterId)
          requestContext.TraceSerializedConditionally(80309, TraceLevel.Error, nameof (IdentityIdTranslationService), nameof (IdentityIdTranslationService), "Detected master ID leak for ID '{0}': {1}", (object) id, (object) identityIdTranslation);
      }
    }

    private T EnsureIdTranslationMapsExists<T>(IVssRequestContext requestContext, Func<T> function)
    {
      IdentityIdTranslatorPerfCounters.WithReadLockOnIdTranslationMaps.CallsPerSecond.Increment();
      lock (this.mapLock)
      {
        if (this.masterIdToIdentityTranslationMap == null)
        {
          IdentityIdTranslatorPerfCounters.InitializeIdTranslationMaps.CallsPerSecond.Increment();
          this.masterIdToIdentityTranslationMap = new Dictionary<Guid, IdentityIdTranslation>();
          this.idToIdentityTranslationMap = new Dictionary<Guid, IdentityIdTranslation>();
          this.EnsureIdTranslationsExists(requestContext, (Action) (() =>
          {
            requestContext.Trace(80305, TraceLevel.Info, nameof (IdentityIdTranslationService), nameof (IdentityIdTranslationService), "Loading identity id translation maps.");
            foreach (IdentityIdTranslation identityIdTranslation in (IEnumerable<IdentityIdTranslation>) this.identityIdTranslations)
            {
              this.masterIdToIdentityTranslationMap.TryAdd<Guid, IdentityIdTranslation>(identityIdTranslation.MasterId, identityIdTranslation);
              this.idToIdentityTranslationMap.TryAdd<Guid, IdentityIdTranslation>(identityIdTranslation.Id, identityIdTranslation);
            }
          }));
        }
        return function != null ? function() : default (T);
      }
    }

    private void EnsureIdTranslationsExists(IVssRequestContext requestContext, Action action = null)
    {
      if (this.identityIdTranslations == null)
      {
        lock (this.translationsLock)
        {
          if (this.identityIdTranslations == null)
          {
            VssPerformanceCounter performanceCounter = IdentityIdTranslatorPerfCounters.InitializeIdTranslationMaps.CallsAccepted;
            performanceCounter.Increment();
            this.identityIdTranslations = this.GetIdentityIdTranslations(requestContext) ?? (IList<IdentityIdTranslation>) new List<IdentityIdTranslation>();
            if (action != null)
              action();
            performanceCounter = IdentityIdTranslatorPerfCounters.InitializeIdTranslationMaps.CallsCompleted;
            performanceCounter.Increment();
          }
          else
          {
            IdentityIdTranslatorPerfCounters.InitializeIdTranslationMaps.CallsDroppedAfterTakingLock.Increment();
            if (action == null)
              return;
            action();
          }
        }
      }
      else
        IdentityIdTranslatorPerfCounters.InitializeIdTranslationMaps.CallsDroppedBeforeTakingLock.Increment();
    }

    internal IList<IdentityIdTranslation> GetIdentityIdTranslations(
      IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || !IdentityTranslationHelper.IsEnabled(requestContext))
        return (IList<IdentityIdTranslation>) null;
      requestContext.Trace(80304, TraceLevel.Info, nameof (IdentityIdTranslationService), nameof (IdentityIdTranslationService), "Loading identity id translations from database.");
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        requestContext.TraceSerializedConditionally(80321, TraceLevel.Error, nameof (IdentityIdTranslationService), nameof (IdentityIdTranslationService), "Identity translations are initialized with non-collection level data.");
      return this.ReadTranslationsFromDatabase(requestContext);
    }

    private IList<IdentityIdTranslation> ReadTranslationsFromDatabase(
      IVssRequestContext requestContext)
    {
      using (IdentityIdTranslationComponent component = requestContext.CreateComponent<IdentityIdTranslationComponent>())
        return component.GetTranslations();
    }

    public void ClearCaches(IVssRequestContext requestContext)
    {
      if (!this.IsRequestContextAtCorrectLevel(requestContext))
        return;
      IdentityIdTranslatorPerfCounters.ClearCaches.CallsPerSecond.Increment();
      lock (this.mapLock)
      {
        if (this.masterIdToIdentityTranslationMap != null)
        {
          IdentityIdTranslatorPerfCounters.ClearCaches.CallsAccepted.Increment();
          this.masterIdToIdentityTranslationMap.Clear();
          this.masterIdToIdentityTranslationMap = (Dictionary<Guid, IdentityIdTranslation>) null;
          this.idToIdentityTranslationMap.Clear();
          this.idToIdentityTranslationMap = (Dictionary<Guid, IdentityIdTranslation>) null;
          IdentityIdTranslatorPerfCounters.ClearCaches.CallsCompleted.Increment();
        }
        else
          IdentityIdTranslatorPerfCounters.ClearCaches.CallsDroppedAfterTakingLock.Increment();
        lock (this.translationsLock)
        {
          if (this.identityIdTranslations == null)
            return;
          this.identityIdTranslations.Clear();
          this.identityIdTranslations = (IList<IdentityIdTranslation>) null;
        }
      }
    }

    public void InvalidateIdTranslationCache(
      IVssRequestContext requestContext,
      IdentityIdTranslationChangeData changeData)
    {
      if (!this.IsRequestContextAtCorrectLevel(requestContext) || changeData == null)
        return;
      if (changeData.TranslationChangeType == IdentityIdTranslationChangeType.Added)
      {
        if (changeData.TranslationChanges == null)
          return;
        IdentityIdTranslatorPerfCounters.InvalidateIdTranslationCache.ChangeTypeAdded.CallsPerSecond.Increment();
        requestContext.Trace(80306, TraceLevel.Info, nameof (IdentityIdTranslationService), nameof (IdentityIdTranslationService), "InvalidateIdTranslationCache - new entries are being added.");
        lock (this.mapLock)
        {
          if (this.masterIdToIdentityTranslationMap != null)
          {
            IdentityIdTranslatorPerfCounters.InvalidateIdTranslationCache.ChangeTypeAdded.CallsAccepted.Increment();
            foreach (IdentityIdTranslation translationChange in changeData.TranslationChanges)
            {
              IdentityIdTranslation identityIdTranslation;
              this.masterIdToIdentityTranslationMap.TryRemove<Guid, IdentityIdTranslation>(translationChange.MasterId, out identityIdTranslation);
              this.idToIdentityTranslationMap.TryRemove<Guid, IdentityIdTranslation>(translationChange.Id, out identityIdTranslation);
              this.masterIdToIdentityTranslationMap.TryAdd<Guid, IdentityIdTranslation>(translationChange.MasterId, translationChange);
              this.idToIdentityTranslationMap.TryAdd<Guid, IdentityIdTranslation>(translationChange.Id, translationChange);
            }
            IdentityIdTranslatorPerfCounters.InvalidateIdTranslationCache.ChangeTypeAdded.CallsCompleted.Increment();
          }
          else
            IdentityIdTranslatorPerfCounters.InvalidateIdTranslationCache.ChangeTypeAdded.CallsDroppedAfterTakingLock.Increment();
        }
      }
      else if (changeData.TranslationChangeType == IdentityIdTranslationChangeType.Removed)
      {
        if (changeData.TranslationChanges == null)
          return;
        IdentityIdTranslatorPerfCounters.InvalidateIdTranslationCache.ChangeTypeRemoved.CallsPerSecond.Increment();
        requestContext.Trace(80307, TraceLevel.Info, nameof (IdentityIdTranslationService), nameof (IdentityIdTranslationService), "InvalidateIdTranslationCache - entries are being removed.");
        lock (this.mapLock)
        {
          if (this.masterIdToIdentityTranslationMap != null)
          {
            IdentityIdTranslatorPerfCounters.InvalidateIdTranslationCache.ChangeTypeRemoved.CallsAccpted.Increment();
            foreach (IdentityIdTranslation translationChange in changeData.TranslationChanges)
            {
              IdentityIdTranslation identityIdTranslation;
              this.masterIdToIdentityTranslationMap.TryRemove<Guid, IdentityIdTranslation>(translationChange.MasterId, out identityIdTranslation);
              this.idToIdentityTranslationMap.TryRemove<Guid, IdentityIdTranslation>(translationChange.Id, out identityIdTranslation);
            }
            IdentityIdTranslatorPerfCounters.InvalidateIdTranslationCache.ChangeTypeRemoved.CallsCompleted.Increment();
          }
          else
            IdentityIdTranslatorPerfCounters.InvalidateIdTranslationCache.ChangeTypeRemoved.CallsDroppedAfterTakingLock.Increment();
        }
      }
      else
      {
        if (changeData.TranslationChangeType != IdentityIdTranslationChangeType.BulkChange)
          return;
        IdentityIdTranslatorPerfCounters.InvalidateIdTranslationCache.ChangeTypeBulkChange.CallsPerSecond.Increment();
        requestContext.Trace(80308, TraceLevel.Info, nameof (IdentityIdTranslationService), nameof (IdentityIdTranslationService), "InvalidateIdTranslationCache - bulk change, clearing cache.");
        this.ClearCaches(requestContext);
      }
    }
  }
}
