// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models.TraceabilityChanges
// Assembly: Microsoft.Azure.Pipelines.Deployment.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2CF55160-AB9F-45A3-BD33-54D24F269988
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Sdk.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models
{
  public class TraceabilityChanges
  {
    public string ChangesUniqueIdentifier { get; private set; }

    public IList<Change> Changes { get; private set; }

    public TraceabilityContinuationToken ContinuationToken { get; private set; }

    public TraceabilityChanges(
      string changesUniqueIdentifier,
      IList<Change> changes,
      TraceabilityContinuationToken continuationToken)
    {
      this.ChangesUniqueIdentifier = changesUniqueIdentifier ?? string.Empty;
      this.Changes = (IList<Change>) new List<Change>();
      if (!string.IsNullOrWhiteSpace(this.ChangesUniqueIdentifier) && changes != null && changes.Count > 0)
        this.Changes.AddRange<Change, IList<Change>>((IEnumerable<Change>) changes);
      if (!string.IsNullOrWhiteSpace(continuationToken?.UniqueIdentifier))
        this.ContinuationToken = new TraceabilityContinuationToken()
        {
          UniqueIdentifier = continuationToken?.UniqueIdentifier ?? string.Empty,
          Version = continuationToken?.Version ?? string.Empty
        };
      else
        this.ContinuationToken = (TraceabilityContinuationToken) null;
    }
  }
}
