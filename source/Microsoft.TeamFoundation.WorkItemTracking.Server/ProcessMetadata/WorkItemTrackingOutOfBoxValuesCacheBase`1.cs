// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.WorkItemTrackingOutOfBoxValuesCacheBase`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.Rules;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata
{
  internal abstract class WorkItemTrackingOutOfBoxValuesCacheBase<T> : 
    WorkItemTrackingOutOfBoxCacheVersioning<IDictionary<string, IReadOnlyCollection<T>>>
  {
    protected bool TryGetOutOfBoxValues(
      IVssRequestContext requestContext,
      ProcessDescriptor systemDescriptor,
      string workItemTypeReferenceName,
      ProcessMetadataResourceType resourceType,
      string xmlRoot,
      Func<IVssRequestContext, IEnumerable<T>, IEnumerable<T>> processValues,
      out IReadOnlyCollection<T> values)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessDescriptor>(systemDescriptor, nameof (systemDescriptor));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(workItemTypeReferenceName, nameof (workItemTypeReferenceName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(xmlRoot, nameof (xmlRoot));
      ArgumentUtility.CheckForNull<Func<IVssRequestContext, IEnumerable<T>, IEnumerable<T>>>(processValues, nameof (processValues));
      this.ClearCacheOnStaleCacheVersion(requestContext);
      IReadOnlyCollection<T> valuesToReturn = (IReadOnlyCollection<T>) null;
      bool cacheMiss = false;
      int num = requestContext.TraceBlock<bool>(911319, 911321, "WorkItemRulesService", nameof (WorkItemTrackingOutOfBoxValuesCacheBase<T>), nameof (TryGetOutOfBoxValues), (Func<bool>) (() =>
      {
        if (!systemDescriptor.IsSystem)
          throw new InvalidOperationException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.CacheMustBeCalledWithSystemDescriptor((object) systemDescriptor.TypeId));
        IDictionary<string, IReadOnlyCollection<T>> dictionary;
        if (!this.TryGetValue(requestContext, systemDescriptor.TypeId, out dictionary))
          dictionary = (IDictionary<string, IReadOnlyCollection<T>>) new ConcurrentDictionary<string, IReadOnlyCollection<T>>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName);
        if (!dictionary.TryGetValue(workItemTypeReferenceName, out valuesToReturn))
        {
          cacheMiss = true;
          valuesToReturn = requestContext.To(TeamFoundationHostType.Deployment).GetService<IWorkItemTrackingRawOutOfBoxValuesCacheService>().GetOutOfBoxRawValues<T>(requestContext, systemDescriptor, workItemTypeReferenceName, resourceType, xmlRoot);
          valuesToReturn = (IReadOnlyCollection<T>) new ReadOnlyCollection<T>((IList<T>) processValues(requestContext, (IEnumerable<T>) valuesToReturn).ToList<T>());
          dictionary[workItemTypeReferenceName] = valuesToReturn;
          this.Set(requestContext, systemDescriptor.TypeId, dictionary);
        }
        return valuesToReturn != null;
      })) ? 1 : 0;
      if (cacheMiss)
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("Process", systemDescriptor.Name);
        properties.Add(nameof (workItemTypeReferenceName), workItemTypeReferenceName);
        properties.Add("ElapsedTime", requestContext.LastTracedBlockElapsedMilliseconds());
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (TryGetOutOfBoxValues), typeof (T).Name, properties);
      }
      values = valuesToReturn;
      return num != 0;
    }
  }
}
