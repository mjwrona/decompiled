// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestResultSummary
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestResultSummary : ISecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public AggregatedResultsAnalysis AggregatedResultsAnalysis { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestFailuresAnalysis TestFailures { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestResultsContext TestResultsContext { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TeamProjectReference TeamProject { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TotalRunsCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int NoConfigRunsCount { get; set; }

    Guid ISecuredObject.NamespaceId => TeamProjectSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => TeamProjectSecurityConstants.GenericRead;

    string ISecuredObject.GetToken() => TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(this.TeamProject.Id));
  }
}
