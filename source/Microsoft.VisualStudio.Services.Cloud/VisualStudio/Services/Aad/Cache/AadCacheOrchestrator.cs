// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Cache.AadCacheOrchestrator
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad.Cache
{
  internal class AadCacheOrchestrator : IAadCacheOrchestrator, IVssFrameworkService
  {
    private readonly Func<IVssRequestContext, DateTimeOffset> currentTime;
    private readonly Func<IVssRequestContext, DateTimeOffset> expirationTime;
    private readonly Type[] types;
    private readonly IDictionary<Type, int> chunkSizes;
    private readonly IDictionary<Type, TimeSpan> localStalenessThresholds;
    private readonly IDictionary<Type, TimeSpan> remoteStalenessThresholds;
    private readonly IDictionary<Type, TimeSpan> maximumStalenessThresholds;
    private readonly IDictionary<Type, bool> isRemoteCacheEnabled;

    internal AadCacheOrchestrator()
      : this(typeof (AadCacheAncestorIds), typeof (AadCacheTenant), typeof (AadCacheDescendantIds), typeof (AadCacheDirectoryRoles), typeof (AadCacheDirectoryRoleMembers), typeof (AadCacheUserRolesAndGroups))
    {
    }

    internal AadCacheOrchestrator(params Type[] types)
      : this(AadCacheOrchestrator.\u003C\u003EO.\u003C0\u003E__GetCurrentTime ?? (AadCacheOrchestrator.\u003C\u003EO.\u003C0\u003E__GetCurrentTime = new Func<IVssRequestContext, DateTimeOffset>(AadCacheOrchestrator.GetCurrentTime)), AadCacheOrchestrator.\u003C\u003EO.\u003C1\u003E__GetExpirationTime ?? (AadCacheOrchestrator.\u003C\u003EO.\u003C1\u003E__GetExpirationTime = new Func<IVssRequestContext, DateTimeOffset>(AadCacheOrchestrator.GetExpirationTime)), types)
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    internal AadCacheOrchestrator(
      Func<IVssRequestContext, DateTimeOffset> currentTime,
      Func<IVssRequestContext, DateTimeOffset> expirationTime,
      params Type[] types)
    {
      ArgumentUtility.CheckForNull<Func<IVssRequestContext, DateTimeOffset>>(currentTime, nameof (currentTime));
      ArgumentUtility.CheckForNull<Func<IVssRequestContext, DateTimeOffset>>(expirationTime, nameof (expirationTime));
      AadCacheUtils.ValidateTypes((IEnumerable<Type>) types);
      this.currentTime = currentTime;
      this.expirationTime = expirationTime;
      this.types = types;
      this.chunkSizes = (IDictionary<Type, int>) new ConcurrentDictionary<Type, int>();
      this.localStalenessThresholds = (IDictionary<Type, TimeSpan>) new ConcurrentDictionary<Type, TimeSpan>();
      this.remoteStalenessThresholds = (IDictionary<Type, TimeSpan>) new ConcurrentDictionary<Type, TimeSpan>();
      this.maximumStalenessThresholds = (IDictionary<Type, TimeSpan>) new ConcurrentDictionary<Type, TimeSpan>();
      this.isRemoteCacheEnabled = (IDictionary<Type, bool>) new ConcurrentDictionary<Type, bool>();
    }

    public void ServiceStart(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (!context.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      this.UpdateSettings(context);
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/Aad/Cache/Orchestrator/...");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      vssRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(vssRequestContext, "Default", SqlNotificationEventClasses.AadUserMembershipChanged, AadCacheOrchestrator.\u003C\u003EO.\u003C2\u003E__OnUserMembershipChangedNotification ?? (AadCacheOrchestrator.\u003C\u003EO.\u003C2\u003E__OnUserMembershipChangedNotification = new SqlNotificationCallback(AadCacheOrchestrator.OnUserMembershipChangedNotification)), false);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().UnregisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      vssRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(context, "Default", SqlNotificationEventClasses.AadUserMembershipChanged, AadCacheOrchestrator.\u003C\u003EO.\u003C2\u003E__OnUserMembershipChangedNotification ?? (AadCacheOrchestrator.\u003C\u003EO.\u003C2\u003E__OnUserMembershipChangedNotification = new SqlNotificationCallback(AadCacheOrchestrator.OnUserMembershipChangedNotification)), false);
    }

    private void OnRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      this.UpdateSettings(context);
    }

    private void UpdateSettings(IVssRequestContext context)
    {
      foreach (Type type in this.types)
      {
        this.chunkSizes[type] = AadCacheUtils.GetRegistryValue<int>(context, type, AadCacheConstants.DefaultSettings.Orchestrator.DefaultChunkSize, AadCacheConstants.DefaultSettings.Orchestrator.ChunkSize, "/Service/Aad/Cache/Orchestrator/ChunkSize", AadCacheConstants.RegistryKeys.Orchestrator.ChunkSize);
        this.localStalenessThresholds[type] = AadCacheUtils.GetRegistryValue<TimeSpan>(context, type, AadCacheConstants.DefaultSettings.Orchestrator.DefaultLocalStalenessThreshold, AadCacheConstants.DefaultSettings.Orchestrator.LocalStalenessThreshold, "/Service/Aad/Cache/Orchestrator/LocalStalenessThreshold", AadCacheConstants.RegistryKeys.Orchestrator.LocalStalenessThreshold);
        this.remoteStalenessThresholds[type] = AadCacheUtils.GetRegistryValue<TimeSpan>(context, type, AadCacheConstants.DefaultSettings.Orchestrator.DefaultRemoteStalenessThreshold, AadCacheConstants.DefaultSettings.Orchestrator.RemoteStalenessThreshold, "/Service/Aad/Cache/Orchestrator/RemoteStalenessThreshold", AadCacheConstants.RegistryKeys.Orchestrator.RemoteStalenessThreshold);
        this.maximumStalenessThresholds[type] = AadCacheUtils.GetRegistryValue<TimeSpan>(context, type, AadCacheConstants.DefaultSettings.Orchestrator.DefaultMaximumStalenessThreshold, AadCacheConstants.DefaultSettings.Orchestrator.MaximumStalenessThreshold, "/Service/Aad/Cache/Orchestrator/MaximumStalenessThreshold", AadCacheConstants.RegistryKeys.Orchestrator.MaximumStalenessThreshold);
        this.isRemoteCacheEnabled[type] = AadCacheUtils.GetRegistryValue<bool>(context, type, AadCacheConstants.DefaultSettings.Orchestrator.DefaultIsRemoteCacheEnabled, AadCacheConstants.DefaultSettings.Orchestrator.IsRemoteCacheEnabled, "/Service/Aad/Cache/Orchestrator/IsRemoteCacheEnabled", AadCacheConstants.RegistryKeys.Orchestrator.IsRemoteCacheEnabled);
      }
    }

    private static void OnUserMembershipChangedNotification(
      IVssRequestContext context,
      Guid eventClass,
      string eventData)
    {
      context.TraceEnter(867530971, "VisualStudio.Services.Aad", "Cache", nameof (OnUserMembershipChangedNotification));
      try
      {
        AadCacheKey aadCacheKey = JsonUtilities.Deserialize<AadCacheKey>(eventData);
        if (aadCacheKey == null)
          return;
        List<AadCacheKey> keys = new List<AadCacheKey>()
        {
          aadCacheKey
        };
        AadCacheOrchestrator.RemoveObjectsFromLocalCache<AadCacheObject>(context, (IEnumerable<AadCacheKey>) keys);
      }
      catch (Exception ex)
      {
        context.TraceException(867530978, TraceLevel.Error, "VisualStudio.Services.Aad", "Cache", ex);
      }
      finally
      {
        context.TraceLeave(867530979, "VisualStudio.Services.Aad", "Cache", nameof (OnUserMembershipChangedNotification));
      }
    }

    public IEnumerable<AadCacheLookup<T>> GetObjects<T>(
      IVssRequestContext context,
      IEnumerable<AadCacheKey> keys,
      Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<T>>> onCacheMiss = null)
      where T : AadCacheObject
    {
      try
      {
        context.TraceEnter(867530901, "VisualStudio.Services.Aad", "Cache", nameof (GetObjects));
        ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
        ArgumentUtility.CheckForNull<IEnumerable<AadCacheKey>>(keys, nameof (keys));
        DateTimeOffset expirationTime = this.expirationTime(context);
        DateTimeOffset currentTime = this.currentTime(context);
        int chunkSiz = this.chunkSizes[typeof (T)];
        TimeSpan localStalenessThreshold = this.localStalenessThresholds[typeof (T)];
        TimeSpan remoteStalenessThreshold = this.remoteStalenessThresholds[typeof (T)];
        TimeSpan stalenessThreshold = this.maximumStalenessThresholds[typeof (T)];
        bool flag = this.isRemoteCacheEnabled[typeof (T)];
        IDictionary<AadCacheKey, AadCacheLookup<T>> localLookups = AadCacheOrchestrator.GetObjectsFromLocalCache<T>(context, keys);
        context.TraceConditionally(867530903, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Cache", (Func<string>) (() => AadCacheOrchestrator.GetTraceMessage<T>("GetObjectsFromLocalCache", localLookups)));
        IEnumerable<AadCacheKey> aadCacheKeys = ((IEnumerable<KeyValuePair<AadCacheKey, AadCacheLookup<T>>>) localLookups).Where<KeyValuePair<AadCacheKey, AadCacheLookup<T>>>((Func<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, bool>) (kvp => AadCacheObject.IsNullOrOlderThan((AadCacheObject) kvp.Value.Result, localStalenessThreshold, expirationTime))).Select<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, AadCacheKey>((Func<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, AadCacheKey>) (kvp => kvp.Key));
        if (!aadCacheKeys.Any<AadCacheKey>())
          return AadCacheOrchestrator.MergeLookups<T>(keys, localLookups, (IDictionary<AadCacheKey, AadCacheLookup<T>>) null, (IDictionary<AadCacheKey, AadCacheLookup<T>>) null, stalenessThreshold, currentTime);
        IDictionary<AadCacheKey, AadCacheLookup<T>> dictionary1 = (IDictionary<AadCacheKey, AadCacheLookup<T>>) null;
        IList<AadCacheKey> sourceKeys;
        if (flag)
        {
          dictionary1 = AadCacheOrchestrator.GetObjectsFromRemoteCache<T>(context, aadCacheKeys);
          context.TraceConditionally(867530905, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Cache", (Func<string>) (() => AadCacheOrchestrator.GetTraceMessage<T>("GetObjectsFromRemoteCache", localLookups)));
          AadCacheOrchestrator.AddObjectsToLocalCache<T>(context, AadCacheOrchestrator.GetMoreRecentObjectsFromLeftSet<T>(dictionary1, localLookups).Select<T, T>((Func<T, T>) (o => (T) o.WithTime(currentTime))));
          sourceKeys = (IList<AadCacheKey>) dictionary1.Where<KeyValuePair<AadCacheKey, AadCacheLookup<T>>>((Func<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, bool>) (kvp => AadCacheObject.IsNullOrOlderThan((AadCacheObject) kvp.Value.Result, remoteStalenessThreshold, expirationTime))).Select<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, AadCacheKey>((Func<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, AadCacheKey>) (kvp => kvp.Key)).ToList<AadCacheKey>();
          context.TraceConditionally(867530907, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Cache", (Func<string>) (() => AadCacheOrchestrator.GetTraceMessage<T>("KeysToFetchFromSource", sourceKeys)));
          if (!sourceKeys.Any<AadCacheKey>())
            return AadCacheOrchestrator.MergeLookups<T>(keys, localLookups, dictionary1, (IDictionary<AadCacheKey, AadCacheLookup<T>>) null, stalenessThreshold, currentTime);
        }
        else
          sourceKeys = (IList<AadCacheKey>) aadCacheKeys.ToList<AadCacheKey>();
        Dictionary<AadCacheKey, AadCacheLookup<T>> sourceLookups = new Dictionary<AadCacheKey, AadCacheLookup<T>>(sourceKeys.Count);
        if (onCacheMiss == null)
          return AadCacheOrchestrator.MergeLookups<T>(keys, localLookups, dictionary1, (IDictionary<AadCacheKey, AadCacheLookup<T>>) sourceLookups, stalenessThreshold, currentTime);
        try
        {
          List<AadCacheKey> list1;
          for (int count = 0; count < sourceKeys.Count; count += list1.Count)
          {
            list1 = sourceKeys.Skip<AadCacheKey>(count).Take<AadCacheKey>(chunkSiz).ToList<AadCacheKey>();
            Dictionary<AadCacheKey, AadCacheLookup<T>> dictionary2 = onCacheMiss((IEnumerable<AadCacheKey>) list1).ToDictionary<AadCacheLookup<T>, AadCacheKey, AadCacheLookup<T>>((Func<AadCacheLookup<T>, AadCacheKey>) (lookup => lookup.Key), (Func<AadCacheLookup<T>, AadCacheLookup<T>>) (lookup => lookup));
            T[] array = dictionary2.Where<KeyValuePair<AadCacheKey, AadCacheLookup<T>>>((Func<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, bool>) (kvp => (object) kvp.Value.Result != null)).Select<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, T>((Func<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, T>) (kvp => kvp.Value.Result)).ToArray<T>();
            AadCacheOrchestrator.AddObjectsToLocalCache<T>(context, (IEnumerable<T>) array);
            if (flag)
              AadCacheOrchestrator.AddObjectsToRemoteCache<T>(context, (IEnumerable<T>) array);
            List<AadCacheKey> list2 = dictionary2.Where<KeyValuePair<AadCacheKey, AadCacheLookup<T>>>((Func<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, bool>) (kvp => kvp.Value.Status == AadCacheLookupStatus.Purge)).Select<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, AadCacheKey>((Func<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, AadCacheKey>) (kvp => kvp.Key)).ToList<AadCacheKey>();
            AadCacheOrchestrator.RemoveObjectsFromLocalCache<T>(context, (IEnumerable<AadCacheKey>) list2);
            if (flag)
              AadCacheOrchestrator.RemoveObjectsFromRemoteCache<T>(context, (IEnumerable<AadCacheKey>) list2);
            foreach (KeyValuePair<AadCacheKey, AadCacheLookup<T>> keyValuePair in dictionary2)
              sourceLookups[keyValuePair.Key] = keyValuePair.Value;
          }
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          if (!(ex is AadCredentialsNotFoundException))
            context.TraceException(867530958, TraceLevel.Error, "VisualStudio.Services.Aad", "Cache", ex);
          IEnumerable<AadCacheLookup<T>> lookups = AadCacheOrchestrator.MergeLookups<T>(keys, localLookups, dictionary1, (IDictionary<AadCacheKey, AadCacheLookup<T>>) sourceLookups, stalenessThreshold, currentTime);
          if (AadCacheOrchestrator.IsCompleteResult<T>(keys, lookups))
            return lookups;
          throw;
        }
        IEnumerable<AadCacheLookup<T>> objects = AadCacheOrchestrator.MergeLookups<T>(keys, localLookups, dictionary1, (IDictionary<AadCacheKey, AadCacheLookup<T>>) sourceLookups, stalenessThreshold, currentTime);
        if (AadCacheOrchestrator.IsCompleteResult<T>(keys, objects))
          return objects;
        if (keys.Count<AadCacheKey>() == 1 && (objects != null ? objects.Single<AadCacheLookup<T>>()?.Exception : (Exception) null) != null)
          throw objects.Single<AadCacheLookup<T>>().Exception;
        throw new AadException("Incomplete result.");
      }
      finally
      {
        context.TraceLeave(867530909, "VisualStudio.Services.Aad", "Cache", nameof (GetObjects));
      }
    }

    private static bool IsCompleteResult<T>(
      IEnumerable<AadCacheKey> keys,
      IEnumerable<AadCacheLookup<T>> lookups)
      where T : AadCacheObject
    {
      double num1 = (double) keys.Count<AadCacheKey>();
      double num2 = (double) lookups.Count<AadCacheLookup<T>>((Func<AadCacheLookup<T>, bool>) (kvp => (object) kvp.Result != null));
      return num1 < 200.0 ? num2 == num1 : num2 / num1 >= 0.99;
    }

    private static IEnumerable<T> GetMoreRecentObjectsFromLeftSet<T>(
      IDictionary<AadCacheKey, AadCacheLookup<T>> leftSet,
      IDictionary<AadCacheKey, AadCacheLookup<T>> rightSet)
      where T : AadCacheObject
    {
      AadCacheLookup<T> lookup;
      return leftSet.Where<KeyValuePair<AadCacheKey, AadCacheLookup<T>>>((Func<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, bool>) (kvp => !rightSet.TryGetValue(kvp.Key, out lookup) || AadCacheObject.IsMoreRecent((AadCacheObject) kvp.Value.Result, (AadCacheObject) lookup.Result))).Where<KeyValuePair<AadCacheKey, AadCacheLookup<T>>>((Func<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, bool>) (kvp => (object) kvp.Value.Result != null)).Select<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, T>((Func<KeyValuePair<AadCacheKey, AadCacheLookup<T>>, T>) (kvp => kvp.Value.Result));
    }

    private static IEnumerable<AadCacheLookup<T>> MergeLookups<T>(
      IEnumerable<AadCacheKey> keys,
      IDictionary<AadCacheKey, AadCacheLookup<T>> localLookups,
      IDictionary<AadCacheKey, AadCacheLookup<T>> remoteLookups,
      IDictionary<AadCacheKey, AadCacheLookup<T>> sourceLookups,
      TimeSpan maximumStalenessThreshold,
      DateTimeOffset now)
      where T : AadCacheObject
    {
      AadCacheLookup<T> localLookup;
      T localObject;
      AadCacheLookup<T> remoteLookup;
      T remoteObject;
      AadCacheLookup<T> sourceLookup;
      T sourceObject;
      T mostRecentObject;
      return keys.Select<AadCacheKey, AadCacheLookup<T>>((Func<AadCacheKey, AadCacheLookup<T>>) (key =>
      {
        localLookup = (AadCacheLookup<T>) null;
        localLookups?.TryGetValue(key, out localLookup);
        localObject = default (T);
        if (localLookup != null)
          localObject = localLookup.Result;
        remoteLookup = (AadCacheLookup<T>) null;
        remoteLookups?.TryGetValue(key, out remoteLookup);
        remoteObject = default (T);
        if (remoteLookup != null)
          remoteObject = remoteLookup.Result;
        sourceLookup = (AadCacheLookup<T>) null;
        sourceLookups?.TryGetValue(key, out sourceLookup);
        sourceObject = default (T);
        if (sourceLookup != null)
        {
          if (sourceLookup.Status == AadCacheLookupStatus.Purge)
            return sourceLookup;
          sourceObject = sourceLookup.Result;
        }
        mostRecentObject = AadCacheObject.GetMostRecent<T>(localObject, remoteObject, sourceObject);
        if (!AadCacheObject.IsNullOrOlderThan((AadCacheObject) mostRecentObject, maximumStalenessThreshold, now))
          return new AadCacheLookup<T>(key, mostRecentObject);
        mostRecentObject = default (T);
        return new AadCacheLookup<T>(key, AadCacheLookupStatus.Miss, mostRecentObject, sourceLookup?.Exception);
      }));
    }

    private static IDictionary<AadCacheKey, AadCacheLookup<T>> GetObjectsFromLocalCache<T>(
      IVssRequestContext context,
      IEnumerable<AadCacheKey> keys)
      where T : AadCacheObject
    {
      try
      {
        context.TraceEnter(867530911, "VisualStudio.Services.Aad", "Cache", nameof (GetObjectsFromLocalCache));
        return (IDictionary<AadCacheKey, AadCacheLookup<T>>) context.To(TeamFoundationHostType.Deployment).GetService<IAadLocalCache>().GetObjects<T>(context, keys).ToDictionary<AadCacheLookup<T>, AadCacheKey, AadCacheLookup<T>>((Func<AadCacheLookup<T>, AadCacheKey>) (lookup => lookup.Key), (Func<AadCacheLookup<T>, AadCacheLookup<T>>) (lookup => lookup));
      }
      catch (RequestCanceledException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        context.TraceException(867530918, TraceLevel.Error, "VisualStudio.Services.Aad", "Cache", ex);
        return (IDictionary<AadCacheKey, AadCacheLookup<T>>) keys.ToDictionary<AadCacheKey, AadCacheKey, AadCacheLookup<T>>((Func<AadCacheKey, AadCacheKey>) (key => key), (Func<AadCacheKey, AadCacheLookup<T>>) (key => new AadCacheLookup<T>(key, AadCacheLookupStatus.Failure, ex)));
      }
      finally
      {
        context.TraceLeave(867530919, "VisualStudio.Services.Aad", "Cache", nameof (GetObjectsFromLocalCache));
      }
    }

    private static void AddObjectsToLocalCache<T>(
      IVssRequestContext context,
      IEnumerable<T> objects)
      where T : AadCacheObject
    {
      try
      {
        context.TraceEnter(867530921, "VisualStudio.Services.Aad", "Cache", nameof (AddObjectsToLocalCache));
        context.To(TeamFoundationHostType.Deployment).GetService<IAadLocalCache>().AddObjects<T>(context, objects);
      }
      catch (RequestCanceledException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        context.TraceException(867530928, TraceLevel.Error, "VisualStudio.Services.Aad", "Cache", ex);
      }
      finally
      {
        context.TraceLeave(867530929, "VisualStudio.Services.Aad", "Cache", nameof (AddObjectsToLocalCache));
      }
    }

    private static void RemoveObjectsFromLocalCache<T>(
      IVssRequestContext context,
      IEnumerable<AadCacheKey> keys)
      where T : AadCacheObject
    {
      try
      {
        context.TraceEnter(867530931, "VisualStudio.Services.Aad", "Cache", nameof (RemoveObjectsFromLocalCache));
        context.To(TeamFoundationHostType.Deployment).GetService<IAadLocalCache>().RemoveObjects<T>(context, keys);
      }
      catch (RequestCanceledException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        context.TraceException(867530938, TraceLevel.Error, "VisualStudio.Services.Aad", "Cache", ex);
      }
      finally
      {
        context.TraceLeave(867530939, "VisualStudio.Services.Aad", "Cache", nameof (RemoveObjectsFromLocalCache));
      }
    }

    private static IDictionary<AadCacheKey, AadCacheLookup<T>> GetObjectsFromRemoteCache<T>(
      IVssRequestContext context,
      IEnumerable<AadCacheKey> keys)
      where T : AadCacheObject
    {
      try
      {
        context.TraceEnter(867530941, "VisualStudio.Services.Aad", "Cache", nameof (GetObjectsFromRemoteCache));
        return (IDictionary<AadCacheKey, AadCacheLookup<T>>) context.To(TeamFoundationHostType.Deployment).GetService<IAadRemoteCache>().GetObjects<T>(context, keys).ToDictionary<AadCacheLookup<T>, AadCacheKey, AadCacheLookup<T>>((Func<AadCacheLookup<T>, AadCacheKey>) (lookup => lookup.Key), (Func<AadCacheLookup<T>, AadCacheLookup<T>>) (lookup => lookup));
      }
      catch (RequestCanceledException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        context.TraceException(867530948, TraceLevel.Error, "VisualStudio.Services.Aad", "Cache", ex);
        return (IDictionary<AadCacheKey, AadCacheLookup<T>>) keys.ToDictionary<AadCacheKey, AadCacheKey, AadCacheLookup<T>>((Func<AadCacheKey, AadCacheKey>) (key => key), (Func<AadCacheKey, AadCacheLookup<T>>) (key => new AadCacheLookup<T>(key, AadCacheLookupStatus.Failure, ex)));
      }
      finally
      {
        context.TraceLeave(867530949, "VisualStudio.Services.Aad", "Cache", nameof (GetObjectsFromRemoteCache));
      }
    }

    private static void AddObjectsToRemoteCache<T>(
      IVssRequestContext context,
      IEnumerable<T> objects)
      where T : AadCacheObject
    {
      try
      {
        context.TraceEnter(867530951, "VisualStudio.Services.Aad", "Cache", nameof (AddObjectsToRemoteCache));
        context.To(TeamFoundationHostType.Deployment).GetService<IAadRemoteCache>().AddObjects<T>(context, objects);
      }
      catch (RequestCanceledException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        context.TraceException(867530958, TraceLevel.Error, "VisualStudio.Services.Aad", "Cache", ex);
      }
      finally
      {
        context.TraceLeave(867530959, "VisualStudio.Services.Aad", "Cache", nameof (AddObjectsToRemoteCache));
      }
    }

    private static void RemoveObjectsFromRemoteCache<T>(
      IVssRequestContext context,
      IEnumerable<AadCacheKey> keys)
      where T : AadCacheObject
    {
      try
      {
        context.TraceEnter(867530961, "VisualStudio.Services.Aad", "Cache", nameof (RemoveObjectsFromRemoteCache));
        context.To(TeamFoundationHostType.Deployment).GetService<IAadRemoteCache>().RemoveObjects<T>(context, keys);
      }
      catch (RequestCanceledException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        context.TraceException(867530968, TraceLevel.Error, "VisualStudio.Services.Aad", "Cache", ex);
      }
      finally
      {
        context.TraceLeave(867530969, "VisualStudio.Services.Aad", "Cache", nameof (RemoveObjectsFromRemoteCache));
      }
    }

    private static DateTimeOffset GetExpirationTime(IVssRequestContext context)
    {
      DateTimeOffset dateTimeOffset;
      return !context.Items.TryGetValue<DateTimeOffset>("AadCacheConstants.ExpirationTime", out dateTimeOffset) ? AadCacheOrchestrator.GetCurrentTime(context) : dateTimeOffset;
    }

    private static DateTimeOffset GetCurrentTime(IVssRequestContext context) => DateTimeOffset.UtcNow;

    private static string GetTraceMessage<T>(
      string operationName,
      IDictionary<AadCacheKey, AadCacheLookup<T>> aadCacheKeyToLookup)
      where T : AadCacheObject
    {
      return string.Format("{0} for {1}, No.of keys:{2}, No.of hits:{3}", (object) operationName, (object) typeof (T), (object) aadCacheKeyToLookup.Keys.Count, (object) aadCacheKeyToLookup.Values.Where<AadCacheLookup<T>>((Func<AadCacheLookup<T>, bool>) (x => x != null && x.Status == AadCacheLookupStatus.Hit)).Count<AadCacheLookup<T>>());
    }

    private static string GetTraceMessage<T>(string operationName, IList<AadCacheKey> aadCacheKeys) where T : AadCacheObject => string.Format("{0} for {1}, No.of keys:{2}", (object) operationName, (object) typeof (T), (object) aadCacheKeys.Count);
  }
}
