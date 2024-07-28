// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.CodeElementDetailsResultV3
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class CodeElementDetailsResultV3
  {
    public CodeElementDetailsResultV3(int id, IEnumerable<CollectorResult> elementDetails)
    {
      this.Id = id;
      this.ElementDetails = elementDetails;
    }

    [JsonConstructor]
    public CodeElementDetailsResultV3()
    {
    }

    [JsonProperty]
    public int Id { get; set; }

    [JsonProperty]
    public IEnumerable<CollectorResult> ElementDetails { get; private set; }
  }
}
