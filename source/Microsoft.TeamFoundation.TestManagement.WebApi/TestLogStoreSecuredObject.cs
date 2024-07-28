// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestLogStoreSecuredObject
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  public class TestLogStoreSecuredObject : ISecuredObject
  {
    private Guid m_ProjectId;

    public TestLogStoreSecuredObject(Guid projectId) => this.m_ProjectId = projectId;

    Guid ISecuredObject.NamespaceId => TeamProjectSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => TeamProjectSecurityConstants.ViewTestResults;

    string ISecuredObject.GetToken() => TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(this.m_ProjectId));
  }
}
