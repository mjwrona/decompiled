// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.CodeElementIdentityV3
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Common;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class CodeElementIdentityV3
  {
    public CodeElementIdentityV3(
      CodeElementIdentity codeElementIdentity,
      CodeElementKind codeElementKind)
    {
      this.Id = codeElementIdentity;
      this.ElementKind = codeElementKind;
    }

    [JsonConstructor]
    private CodeElementIdentityV3()
    {
    }

    [JsonProperty]
    public CodeElementIdentity Id { get; private set; }

    [JsonProperty]
    public CodeElementKind ElementKind { get; private set; }

    public override bool Equals(object obj) => obj is CodeElementIdentityV3 elementIdentityV3 && this.Id.Equals((IDictionary<string, string>) elementIdentityV3.Id) && this.ElementKind == elementIdentityV3.ElementKind;

    public override int GetHashCode() => this.Id.GetHashCode() ^ this.ElementKind.GetHashCode() * 37;
  }
}
