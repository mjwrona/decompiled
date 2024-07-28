// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingOutOfBoxStatesCache
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingOutOfBoxStatesCache : 
    WorkItemTrackingOutOfBoxCacheVersioning<ReadOnlyDictionary<string, WorkItemOobState>>
  {
    protected internal virtual ReadOnlyDictionary<string, WorkItemOobState> GetOutOfBoxStates(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.ClearCacheOnStaleCacheVersion(requestContext);
      return requestContext.TraceBlock<ReadOnlyDictionary<string, WorkItemOobState>>(911319, 911321, nameof (WorkItemTrackingOutOfBoxStatesCache), nameof (WorkItemTrackingOutOfBoxStatesCache), nameof (GetOutOfBoxStates), (Func<ReadOnlyDictionary<string, WorkItemOobState>>) (() =>
      {
        ReadOnlyDictionary<string, WorkItemOobState> outOfBoxStates = (ReadOnlyDictionary<string, WorkItemOobState>) null;
        Guid systemProcessId = ProcessConstants.SystemProcessId;
        if (!this.TryGetValue(requestContext, systemProcessId, out outOfBoxStates))
        {
          WorkItemOobState[] source = TeamFoundationSerializationUtility.Deserialize<WorkItemOobState[]>(MetadataResourceManager.GetSystemProcessMetadataResource(requestContext, ProcessMetadataResourceType.OutOfBoxStateIdsAndCategoriesMetadata, "OutOfBoxStateIdsAndCategoriesMetadata"), new XmlRootAttribute("states"));
          IDictionary<string, WorkItemOobState> oobStatesDict = (IDictionary<string, WorkItemOobState>) new Dictionary<string, WorkItemOobState>();
          if (source != null)
            ((IEnumerable<WorkItemOobState>) source).ToList<WorkItemOobState>()?.ForEach((Action<WorkItemOobState>) (oobState =>
            {
              if (!oobStatesDict.ContainsKey(oobState.Name))
                oobStatesDict.Add(oobState.Name, oobState);
              else
                requestContext.Trace(911319, TraceLevel.Warning, "Services", "MetadataService", "GetOutOfBoxStates : Duplicate State name :  {0} for state Id: {1} and state category )", (object) oobState.Name, (object) oobState.Id, (object) oobState.StateCategory);
            }));
          outOfBoxStates = new ReadOnlyDictionary<string, WorkItemOobState>(oobStatesDict);
          this.Set(requestContext, systemProcessId, outOfBoxStates);
        }
        return outOfBoxStates;
      }));
    }
  }
}
