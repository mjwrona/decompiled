// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionDiagnostics
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  [DataContract]
  public class ContributionDiagnostics
  {
    [DataMember(Name = "contributions")]
    public Dictionary<string, Contribution> Contributions = new Dictionary<string, Contribution>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    [DataMember(Name = "relationships")]
    public Dictionary<string, List<EvaluatedRelationship>> Relationships = new Dictionary<string, List<EvaluatedRelationship>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  }
}
