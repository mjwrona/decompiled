// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.CodeElementSummaryResult
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Common;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public sealed class CodeElementSummaryResult : CodeElementResult
  {
    public CodeElementSummaryResult(
      CodeElementIdentity identity,
      CodeElementKind elementKind,
      IEnumerable<CollectorResult> collectorResults)
      : base(identity, elementKind, collectorResults)
    {
    }

    [JsonConstructor]
    private CodeElementSummaryResult()
    {
    }

    [JsonProperty(Order = 3)]
    public IEnumerable<CollectorResult> ElementSummaries
    {
      get => this.ElementData;
      set => this.ElementData = value;
    }
  }
}
