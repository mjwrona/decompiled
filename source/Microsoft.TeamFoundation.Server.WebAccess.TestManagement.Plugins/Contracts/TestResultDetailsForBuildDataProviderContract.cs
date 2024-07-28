// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts.TestResultDetailsForBuildDataProviderContract
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts
{
  [DataContract]
  internal class TestResultDetailsForBuildDataProviderContract : ISecuredObject
  {
    private Guid projectId;

    public TestResultDetailsForBuildDataProviderContract(Guid projectId) => this.projectId = projectId;

    [DataMember(EmitDefaultValue = false)]
    public TestResultsDetails TestResultsDetails { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int BuildId { get; set; }

    public Guid NamespaceId => TeamProjectSecurityConstants.NamespaceId;

    public int RequiredPermissions => TeamProjectSecurityConstants.GenericRead;

    public string GetToken() => TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(this.projectId));
  }
}
