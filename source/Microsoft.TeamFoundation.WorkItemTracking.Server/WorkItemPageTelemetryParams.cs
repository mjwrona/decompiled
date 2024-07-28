// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemPageTelemetryParams
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemPageTelemetryParams
  {
    internal IEnumerable<FieldEntry> Fields;
    internal IEnumerable<WorkItemIdRevisionPair> WorkItemIdRevPairs;
    internal int WideTableProjectionLevel;
    internal IEnumerable<int> WideFields;
    internal IEnumerable<int> LongFields;
    internal IEnumerable<int> TextFields;
    internal bool ByRevision;
    internal DateTime? AsOfDateTime;
    internal int MaxLongTextLength;
    internal IdentityDisplayType IdentityDisplayType;
    internal WorkItemRetrievalMode WorkItemRetrievalMode;
    internal int FetchWorkItemsCount;
    internal int ReturnWorkItemsCount;

    internal WorkItemPageTelemetryParams(
      IEnumerable<FieldEntry> fields,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      int wideTableProjectionLevel,
      IEnumerable<int> wideFields,
      IEnumerable<int> longFields,
      IEnumerable<int> textFields,
      bool byRevision,
      DateTime? asOfDateTime,
      int maxLongTextLength,
      IdentityDisplayType identityDisplayType,
      WorkItemRetrievalMode workItemRetrievalMode,
      int fetchWorkItemsCount,
      int returnWorkItemsCount)
    {
      this.Fields = fields;
      this.WorkItemIdRevPairs = workItemIdRevPairs;
      this.WideTableProjectionLevel = wideTableProjectionLevel;
      this.WideFields = wideFields;
      this.LongFields = longFields;
      this.TextFields = textFields;
      this.ByRevision = byRevision;
      this.AsOfDateTime = asOfDateTime;
      this.MaxLongTextLength = maxLongTextLength;
      this.IdentityDisplayType = identityDisplayType;
      this.WorkItemRetrievalMode = workItemRetrievalMode;
      this.FetchWorkItemsCount = fetchWorkItemsCount;
      this.ReturnWorkItemsCount = returnWorkItemsCount;
    }
  }
}
