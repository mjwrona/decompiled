// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.EvaluatedRelationship
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  [DataContract]
  public class EvaluatedRelationship
  {
    [DataMember(Name = "parentContributionId")]
    public string ParentContributionId;
    [DataMember(Name = "childContributionId")]
    public string ChildContributionId;
    [DataMember(Name = "inputOptions")]
    public ContributionQueryOptions InputOptions;
    [DataMember(Name = "outputOptions")]
    public ContributionQueryOptions OutputOptions;
    [DataMember(Name = "conditions", EmitDefaultValue = false)]
    public List<EvaluatedCondition> Conditions;
  }
}
