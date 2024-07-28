// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.WorkItemTrackingOutOfBoxPicklistCache
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata
{
  internal class WorkItemTrackingOutOfBoxPicklistCache : 
    WorkItemTrackingOutOfBoxCacheVersioning<OobPicklistValues[]>
  {
    protected OobPicklistValues[] GetOutOfBoxPicklistValues(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.ClearCacheOnStaleCacheVersion(requestContext);
      return requestContext.TraceBlock<OobPicklistValues[]>(911319, 911321, nameof (WorkItemTrackingOutOfBoxPicklistCache), nameof (WorkItemTrackingOutOfBoxPicklistCache), nameof (GetOutOfBoxPicklistValues), (Func<OobPicklistValues[]>) (() =>
      {
        Guid systemProcessId = ProcessConstants.SystemProcessId;
        OobPicklistValues[] boxPicklistValues;
        if (!this.TryGetValue(requestContext, systemProcessId, out boxPicklistValues))
        {
          boxPicklistValues = TeamFoundationSerializationUtility.Deserialize<OobPicklistValues[]>(MetadataResourceManager.GetSystemProcessMetadataResource(requestContext, ProcessMetadataResourceType.OutOfBoxPicklistValuesMetadata, "OutOfBoxPicklistValuesMetadata"), new XmlRootAttribute("picklist"));
          this.Set(requestContext, systemProcessId, boxPicklistValues);
        }
        return boxPicklistValues;
      }));
    }

    protected internal virtual IReadOnlyDictionary<string, IReadOnlyCollection<string>> GetFieldAllowedValuesMappings(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.ClearCacheOnStaleCacheVersion(requestContext);
      List<OobPicklistValues> oobPicklistValuesList1 = new List<OobPicklistValues>();
      OobPicklistValues[] boxPicklistValues = this.GetOutOfBoxPicklistValues(requestContext);
      List<OobPicklistValues> oobPicklistValuesList2;
      if (boxPicklistValues == null)
      {
        oobPicklistValuesList2 = (List<OobPicklistValues>) null;
      }
      else
      {
        IEnumerable<OobPicklistValues> source = ((IEnumerable<OobPicklistValues>) boxPicklistValues).Where<OobPicklistValues>((Func<OobPicklistValues, bool>) (v => v?.AllowedValues != null && v != null && v.AllowedValues.Count > 0));
        oobPicklistValuesList2 = source != null ? source.ToList<OobPicklistValues>() : (List<OobPicklistValues>) null;
      }
      Dictionary<string, IReadOnlyCollection<string>> dictionary = new Dictionary<string, IReadOnlyCollection<string>>();
      foreach (OobPicklistValues oobPicklistValues in oobPicklistValuesList2)
        dictionary[oobPicklistValues.Field] = (IReadOnlyCollection<string>) oobPicklistValues.AllowedValues;
      return (IReadOnlyDictionary<string, IReadOnlyCollection<string>>) new ReadOnlyDictionary<string, IReadOnlyCollection<string>>((IDictionary<string, IReadOnlyCollection<string>>) dictionary);
    }

    protected internal virtual IReadOnlyDictionary<string, IReadOnlyCollection<string>> GetFieldSuggestedValuesMappings(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.ClearCacheOnStaleCacheVersion(requestContext);
      List<OobPicklistValues> oobPicklistValuesList1 = new List<OobPicklistValues>();
      OobPicklistValues[] boxPicklistValues = this.GetOutOfBoxPicklistValues(requestContext);
      List<OobPicklistValues> oobPicklistValuesList2;
      if (boxPicklistValues == null)
      {
        oobPicklistValuesList2 = (List<OobPicklistValues>) null;
      }
      else
      {
        IEnumerable<OobPicklistValues> source = ((IEnumerable<OobPicklistValues>) boxPicklistValues).Where<OobPicklistValues>((Func<OobPicklistValues, bool>) (v => v?.SuggestedValues != null && v != null && v.SuggestedValues.Count > 0));
        oobPicklistValuesList2 = source != null ? source.ToList<OobPicklistValues>() : (List<OobPicklistValues>) null;
      }
      Dictionary<string, IReadOnlyCollection<string>> dictionary = new Dictionary<string, IReadOnlyCollection<string>>();
      foreach (OobPicklistValues oobPicklistValues in oobPicklistValuesList2)
        dictionary[oobPicklistValues.Field] = (IReadOnlyCollection<string>) oobPicklistValues.SuggestedValues;
      return (IReadOnlyDictionary<string, IReadOnlyCollection<string>>) new ReadOnlyDictionary<string, IReadOnlyCollection<string>>((IDictionary<string, IReadOnlyCollection<string>>) dictionary);
    }
  }
}
