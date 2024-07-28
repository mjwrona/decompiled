// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.CodeElementResult
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Common;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public abstract class CodeElementResult
  {
    public CodeElementResult(
      CodeElementResult element,
      IEnumerable<CollectorResult> collectorResults)
      : this(element.Id, element.Kind, collectorResults)
    {
    }

    public CodeElementResult(
      CodeElementIdentity identity,
      CodeElementKind elementKind,
      params CollectorResult[] collectorResults)
      : this(identity, elementKind, ((IEnumerable<CollectorResult>) collectorResults).AsEnumerable<CollectorResult>())
    {
    }

    public CodeElementResult(
      CodeElementIdentity identity,
      CodeElementKind elementKind,
      IEnumerable<CollectorResult> collectorResults)
    {
      ArgumentUtility.CheckForNull<CodeElementIdentity>(identity, nameof (identity));
      ArgumentUtility.CheckForNull<IEnumerable<CollectorResult>>(collectorResults, nameof (collectorResults));
      this.Id = identity;
      this.Kind = elementKind;
      this.ElementData = (IEnumerable<CollectorResult>) collectorResults.ToArray<CollectorResult>();
    }

    protected CodeElementResult()
    {
    }

    [JsonProperty(Order = 1)]
    public CodeElementKind Kind { get; private set; }

    [JsonProperty(Order = 2)]
    public CodeElementIdentity Id { get; private set; }

    [JsonIgnore]
    protected IEnumerable<CollectorResult> ElementData { get; set; }
  }
}
