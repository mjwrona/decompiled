// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.PolicySecuredObjectFactory
// Assembly: Microsoft.TeamFoundation.Policy.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E2CB80F-05BD-43A4-BD5A-A4654EDC6268
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Policy.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  public class PolicySecuredObjectFactory : ISecuredObject
  {
    private readonly Guid m_namespaceId;
    private readonly string m_projectUri;
    private readonly int m_requiredPermissions;

    private PolicySecuredObjectFactory(
      Guid namespaceId,
      int requiredPermissions,
      string projectUri)
    {
      this.m_namespaceId = namespaceId;
      this.m_requiredPermissions = requiredPermissions;
      this.m_projectUri = projectUri;
    }

    public static ISecuredObject CreateReadOnlyInstance(string projectUri)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
      return (ISecuredObject) new PolicySecuredObjectFactory(TeamProjectSecurityConstants.NamespaceId, TeamProjectSecurityConstants.GenericRead, projectUri);
    }

    public static ISecuredObject CreateReadOnlyInstance(Guid projectId) => PolicySecuredObjectFactory.CreateReadOnlyInstance(ProjectInfo.GetProjectUri(projectId));

    string ISecuredObject.GetToken() => TeamProjectSecurityConstants.GetToken(this.m_projectUri);

    Guid ISecuredObject.NamespaceId => this.m_namespaceId;

    int ISecuredObject.RequiredPermissions => this.m_requiredPermissions;
  }
}
