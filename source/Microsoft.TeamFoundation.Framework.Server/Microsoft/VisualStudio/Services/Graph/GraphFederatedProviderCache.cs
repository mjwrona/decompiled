// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphFederatedProviderCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal class GraphFederatedProviderCache : 
    VssBaseService,
    IGraphFederatedProviderCache,
    IVssFrameworkService
  {
    private const string Area = "Graph";
    private const string Layer = "GraphFederatedProviderCache";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.MaxChangeVersionLock = this.CreateLockName(systemRequestContext, "MaxChangeVersionLock");
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      this.NotificationAuthoringId = service.Author;
      this.NotificationRegistration = service.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.IdentityFederatedProviderDataChanged, new SqlNotificationCallback(this.OnProviderDataChanged), false, false);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => this.NotificationRegistration.Unregister(systemRequestContext);

    GraphFederatedProviderData IGraphFederatedProviderCache.GetProviderData(
      IVssRequestContext context,
      SubjectDescriptor descriptor,
      string providerName)
    {
      context.TraceEnter(60550140, "Graph", nameof (GraphFederatedProviderCache), "GetProviderData");
      try
      {
        context.TraceDataConditionally(60550141, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Retrieving from cache", (Func<object>) (() => (object) new
        {
          descriptor = descriptor,
          providerName = providerName
        }), "GetProviderData");
        GraphFederatedProviderCache.ProviderDataCacheKey cacheKey = new GraphFederatedProviderCache.ProviderDataCacheKey(descriptor, providerName);
        GraphFederatedProviderData providerData;
        if (context.GetService<GraphFederatedProviderCache.ProviderDataMemoryCache>().TryGetValue(context, cacheKey, out providerData))
        {
          context.TraceDataConditionally(60550142, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Retrieved from cache", (Func<object>) (() => (object) new
          {
            descriptor = descriptor,
            providerName = providerName,
            providerData = providerData.Hashed(context),
            cacheKey = cacheKey
          }), "GetProviderData");
          return providerData;
        }
        context.TraceDataConditionally(60550143, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Not found in cache", (Func<object>) (() => (object) new
        {
          descriptor = descriptor,
          providerName = providerName,
          cacheKey = cacheKey
        }), "GetProviderData");
        return (GraphFederatedProviderData) null;
      }
      finally
      {
        context.TraceLeave(60550140, "Graph", nameof (GraphFederatedProviderCache), "GetProviderData");
      }
    }

    void IGraphFederatedProviderCache.SetProviderData(
      IVssRequestContext context,
      SubjectDescriptor descriptor,
      string providerName,
      GraphFederatedProviderData providerData)
    {
      context.TraceEnter(60550150, "Graph", nameof (GraphFederatedProviderCache), "SetProviderData");
      try
      {
        context.TraceDataConditionally(60550151, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Adding to cache", (Func<object>) (() => (object) new
        {
          descriptor = descriptor,
          providerName = providerName,
          providerData = providerData.Hashed(context)
        }), "SetProviderData");
        GraphFederatedProviderCache.ProviderDataCacheKey cacheKey = new GraphFederatedProviderCache.ProviderDataCacheKey(descriptor, providerName);
        context.GetService<GraphFederatedProviderCache.ProviderDataMemoryCache>().Set(context, cacheKey, providerData);
        context.TraceDataConditionally(60550153, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Added to cache", (Func<object>) (() => (object) new
        {
          descriptor = descriptor,
          providerName = providerName,
          providerData = providerData.Hashed(context),
          cacheKey = cacheKey
        }), "SetProviderData");
      }
      finally
      {
        context.TraceLeave(60550150, "Graph", nameof (GraphFederatedProviderCache), "SetProviderData");
      }
    }

    private void OnProviderDataChanged(
      IVssRequestContext context,
      Guid eventClass,
      string eventData)
    {
      context.TraceEnter(60550160, "Graph", nameof (GraphFederatedProviderCache), nameof (OnProviderDataChanged));
      try
      {
        context.TraceDataConditionally(60550161, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Processing changes", (Func<object>) (() => (object) new
        {
          eventClass = eventClass,
          eventData = eventData
        }), nameof (OnProviderDataChanged));
        GraphFederatedProviderChangeData changeData = TeamFoundationSerializationUtility.Deserialize<GraphFederatedProviderChangeData>(eventData);
        context.TraceDataConditionally(60550162, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Deserialized change data", (Func<object>) (() => (object) new
        {
          changeData = changeData
        }), nameof (OnProviderDataChanged));
        if (((IEnumerable<GraphFederatedProviderChange>) changeData?.Changes).IsNullOrEmpty<GraphFederatedProviderChange>())
        {
          context.TraceDataConditionally(60550163, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Skipping null or empty set of changes", (Func<object>) (() => (object) new
          {
            changeData = changeData
          }), nameof (OnProviderDataChanged));
        }
        else
        {
          GraphFederatedProviderCache.ProviderDataMemoryCache service1 = context.GetService<GraphFederatedProviderCache.ProviderDataMemoryCache>();
          IGraphIdentifierConversionService service2 = context.GetService<IGraphIdentifierConversionService>();
          List<GraphFederatedProviderChange> source = new List<GraphFederatedProviderChange>();
          foreach (GraphFederatedProviderChange change1 in changeData.Changes)
          {
            GraphFederatedProviderChange change = change1;
            context.TraceDataConditionally(60550164, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Processing change", (Func<object>) (() => (object) new
            {
              change = change
            }), nameof (OnProviderDataChanged));
            if (change.SubjectDescriptor != new SubjectDescriptor())
              context.TraceDataConditionally(60550165, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Change already contains a subject descriptor", (Func<object>) (() => (object) new
              {
                change = change
              }), nameof (OnProviderDataChanged));
            else if (change.StorageKey != new Guid())
            {
              context.TraceDataConditionally(60550166, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Change does not contain a subject descriptor but contains a storage key; populating subject descriptor", (Func<object>) (() => (object) new
              {
                change = change
              }), nameof (OnProviderDataChanged));
              change.SubjectDescriptor = service2.GetDescriptorByStorageKey(context, change.StorageKey);
              if (change.SubjectDescriptor != new SubjectDescriptor())
              {
                context.TraceDataConditionally(60550167, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Populated subject descriptor from storage key", (Func<object>) (() => (object) new
                {
                  change = change
                }), nameof (OnProviderDataChanged));
              }
              else
              {
                context.TraceDataConditionally(60550168, TraceLevel.Error, "Graph", nameof (GraphFederatedProviderCache), "Failed to populate subject descriptor from storage key; skipping change", (Func<object>) (() => (object) new
                {
                  change = change
                }), nameof (OnProviderDataChanged));
                continue;
              }
            }
            else
            {
              context.TraceDataConditionally(60550167, TraceLevel.Error, "Graph", nameof (GraphFederatedProviderCache), "Change contains neither subject descriptor nor storage key; skipping change", (Func<object>) (() => (object) new
              {
                change = change
              }), nameof (OnProviderDataChanged));
              continue;
            }
            context.TraceDataConditionally(60550169, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Checking cached data against change", (Func<object>) (() => (object) new
            {
              change = change
            }), nameof (OnProviderDataChanged));
            GraphFederatedProviderCache.ProviderDataCacheKey cacheKey = new GraphFederatedProviderCache.ProviderDataCacheKey(change.SubjectDescriptor, change.ProviderName);
            GraphFederatedProviderData cachedProviderData;
            if (service1.TryPeekValue(context, cacheKey, out cachedProviderData) && cachedProviderData != null && cachedProviderData.Version >= change.Version)
            {
              context.TraceDataConditionally(60550170, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Cached data is already up to date; no op", (Func<object>) (() => (object) new
              {
                change = change,
                cacheKey = cacheKey,
                cachedProviderData = cachedProviderData
              }), nameof (OnProviderDataChanged));
            }
            else
            {
              bool removed = service1.Remove(context, cacheKey);
              context.TraceDataConditionally(60550171, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Cached data is either missing or out of date; invalidated", (Func<object>) (() => (object) new
              {
                change = change,
                cacheKey = cacheKey,
                cachedProviderData = cachedProviderData,
                removed = removed
              }), nameof (OnProviderDataChanged));
            }
            source.Add(change);
          }
          long maxVersionOfProcessedChanges = source.Max<GraphFederatedProviderChange>((Func<GraphFederatedProviderChange, long>) (change => change.Version));
          bool flag;
          long previousMaxChangeVersion;
          using (context.Lock(this.MaxChangeVersionLock))
          {
            previousMaxChangeVersion = this.MaxChangeVersion;
            if (maxVersionOfProcessedChanges > previousMaxChangeVersion)
            {
              this.MaxChangeVersion = maxVersionOfProcessedChanges;
              flag = true;
            }
            else
              flag = false;
          }
          if (flag)
            context.TraceDataConditionally(60550172, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Increased max change version to max version of processed changes", (Func<object>) (() => (object) new
            {
              maxVersionOfProcessedChanges = maxVersionOfProcessedChanges,
              previousMaxChangeVersion = previousMaxChangeVersion
            }), nameof (OnProviderDataChanged));
          else
            context.TraceDataConditionally(60550173, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Kept previous max change version and ignored max version of processed changes", (Func<object>) (() => (object) new
            {
              maxVersionOfProcessedChanges = maxVersionOfProcessedChanges,
              previousMaxChangeVersion = previousMaxChangeVersion
            }), nameof (OnProviderDataChanged));
          if (changeData.Author == this.NotificationAuthoringId)
            GraphFederatedProviderCache.BroadcastProviderDataChange(context, changeData);
          context.TraceDataConditionally(60550174, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Processed changes", (Func<object>) (() => (object) new
          {
            eventClass = eventClass,
            eventData = eventData,
            changeData = changeData
          }), nameof (OnProviderDataChanged));
        }
      }
      finally
      {
        context.TraceLeave(60550160, "Graph", nameof (GraphFederatedProviderCache), nameof (OnProviderDataChanged));
      }
    }

    private static void BroadcastProviderDataChange(
      IVssRequestContext context,
      GraphFederatedProviderChangeData changeData)
    {
      context.TraceEnter(60550180, "Graph", nameof (GraphFederatedProviderCache), nameof (BroadcastProviderDataChange));
      try
      {
        GraphFederatedProviderChangeMessage message = GraphFederatedProviderCache.GetProviderDataChangeMessage(context, changeData);
        if (message == null)
        {
          context.TraceDataConditionally(60550181, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Skipping broadcast since we could not create change message", (Func<object>) (() => (object) new
          {
            changeData = changeData,
            message = message
          }), nameof (BroadcastProviderDataChange));
        }
        else
        {
          context.TraceDataConditionally(60550182, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Broadcasting change message", (Func<object>) (() => (object) new
          {
            changeData = changeData,
            message = message
          }), nameof (BroadcastProviderDataChange));
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          TeamFoundationTask task = new TeamFoundationTask(GraphFederatedProviderCache.\u003C\u003EO.\u003C0\u003E__PublishServiceBusNotification ?? (GraphFederatedProviderCache.\u003C\u003EO.\u003C0\u003E__PublishServiceBusNotification = new TeamFoundationTaskCallback(GraphFederatedProviderCache.PublishServiceBusNotification)), (object) message, 0);
          context.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(context, task);
          context.TraceDataConditionally(60550183, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Created task to broadcast change message", (Func<object>) (() => (object) new
          {
            Identifier = task.Identifier,
            TaskArgs = task.TaskArgs
          }), nameof (BroadcastProviderDataChange));
        }
      }
      finally
      {
        context.TraceLeave(60550180, "Graph", nameof (GraphFederatedProviderCache), nameof (BroadcastProviderDataChange));
      }
    }

    private static GraphFederatedProviderChangeMessage GetProviderDataChangeMessage(
      IVssRequestContext context,
      GraphFederatedProviderChangeData changeData)
    {
      GraphFederatedProviderChangeData providerChangeData = changeData;
      GraphFederatedProviderChange[] array1 = providerChangeData != null ? ((IEnumerable<GraphFederatedProviderChange>) providerChangeData.Changes).Where<GraphFederatedProviderChange>((Func<GraphFederatedProviderChange, bool>) (change => change != null)).ToArray<GraphFederatedProviderChange>() : (GraphFederatedProviderChange[]) null;
      if (((IEnumerable<GraphFederatedProviderChange>) array1).IsNullOrEmpty<GraphFederatedProviderChange>())
      {
        context.TraceDataConditionally(60550191, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Skipping creation of change message since input change data is null or empty", (Func<object>) (() => (object) new
        {
          changeData = changeData
        }), nameof (GetProviderDataChangeMessage));
        return (GraphFederatedProviderChangeMessage) null;
      }
      Guid guid = context.ServiceHost.Is(TeamFoundationHostType.Deployment) ? Guid.Empty : context.ServiceHost.InstanceId;
      GraphFederatedProviderChangeMessage.Change[] array2 = ((IEnumerable<GraphFederatedProviderChange>) array1).Select<GraphFederatedProviderChange, GraphFederatedProviderChangeMessage.Change>((Func<GraphFederatedProviderChange, GraphFederatedProviderChangeMessage.Change>) (change => new GraphFederatedProviderChangeMessage.Change()
      {
        SubjectDescriptor = change.SubjectDescriptor,
        ProviderName = change.ProviderName,
        Version = change.Version
      })).ToArray<GraphFederatedProviderChangeMessage.Change>();
      GraphFederatedProviderChangeMessage changeMessage = new GraphFederatedProviderChangeMessage()
      {
        InstanceId = guid,
        Changes = array2
      };
      context.TraceDataConditionally(60550192, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Converted change data to change message", (Func<object>) (() => (object) new
      {
        changeData = changeData,
        changeMessage = changeMessage
      }), nameof (GetProviderDataChangeMessage));
      return changeMessage;
    }

    private static void PublishServiceBusNotification(IVssRequestContext context, object taskArgs)
    {
      context.TraceEnter(60550200, "Graph", nameof (GraphFederatedProviderCache), nameof (PublishServiceBusNotification));
      try
      {
        GraphFederatedProviderChangeMessage changeMessage = taskArgs as GraphFederatedProviderChangeMessage;
        if (changeMessage == null)
        {
          string messageType = taskArgs.GetType().AssemblyQualifiedName;
          context.TraceDataConditionally(60550201, TraceLevel.Error, "Graph", nameof (GraphFederatedProviderCache), "Cannot publish message of invalid type", (Func<object>) (() => (object) new
          {
            messageType = messageType,
            message = taskArgs
          }), nameof (PublishServiceBusNotification));
        }
        else
        {
          context.TraceDataConditionally(60550202, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Publishing change message", (Func<object>) (() => (object) new
          {
            changeMessage = changeMessage
          }), nameof (PublishServiceBusNotification));
          context.GetService<IMessageBusPublisherService>().Publish(context, "Microsoft.VisualStudio.Services.Graph", (object[]) new GraphFederatedProviderChangeMessage[1]
          {
            changeMessage
          }, false);
          context.TraceDataConditionally(60550203, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderCache), "Published change message", (Func<object>) (() => (object) new
          {
            changeMessage = changeMessage
          }), nameof (PublishServiceBusNotification));
        }
      }
      catch (Exception ex)
      {
        context.TraceException(60550209, "Graph", nameof (GraphFederatedProviderCache), ex);
      }
      finally
      {
        context.TraceLeave(60550200, "Graph", nameof (GraphFederatedProviderCache), nameof (PublishServiceBusNotification));
      }
    }

    public long MaxChangeVersion { get; private set; }

    private ILockName MaxChangeVersionLock { get; set; }

    private Guid NotificationAuthoringId { get; set; }

    private INotificationRegistration NotificationRegistration { get; set; }

    private class ProviderDataMemoryCache : 
      VssMemoryCacheService<GraphFederatedProviderCache.ProviderDataCacheKey, GraphFederatedProviderData>
    {
    }

    private struct ProviderDataCacheKey : 
      IEquatable<GraphFederatedProviderCache.ProviderDataCacheKey>
    {
      private SubjectDescriptor Descriptor { get; }

      private string ProviderName { get; }

      public ProviderDataCacheKey(SubjectDescriptor descriptor, string providerName)
      {
        this.Descriptor = descriptor;
        this.ProviderName = providerName;
      }

      public static bool operator ==(
        GraphFederatedProviderCache.ProviderDataCacheKey left,
        GraphFederatedProviderCache.ProviderDataCacheKey right)
      {
        return left.Equals(right);
      }

      public static bool operator !=(
        GraphFederatedProviderCache.ProviderDataCacheKey left,
        GraphFederatedProviderCache.ProviderDataCacheKey right)
      {
        return !left.Equals(right);
      }

      public bool Equals(
        GraphFederatedProviderCache.ProviderDataCacheKey other)
      {
        return this.Descriptor == other.Descriptor && this.ProviderName == other.ProviderName;
      }

      public override bool Equals(object obj) => obj is GraphFederatedProviderCache.ProviderDataCacheKey other && this.Equals(other);

      public override int GetHashCode()
      {
        if (this == new GraphFederatedProviderCache.ProviderDataCacheKey())
          return 0;
        int num1 = 7443;
        int num2 = (num1 << 19) - num1 + this.Descriptor.GetHashCode();
        return (num2 << 19) - num2 + this.ProviderName.GetHashCode();
      }
    }
  }
}
