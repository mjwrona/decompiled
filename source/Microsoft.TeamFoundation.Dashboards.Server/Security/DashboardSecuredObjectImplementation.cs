// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Security.DashboardSecuredObjectImplementation
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Dashboards.Security
{
  internal class DashboardSecuredObjectImplementation : ISecuredObject
  {
    private string Token;

    internal DashboardSecuredObjectImplementation(string token) => this.Token = token;

    Guid ISecuredObject.NamespaceId => DashboardsPrivileges.NamespaceId;

    int ISecuredObject.RequiredPermissions => 1;

    string ISecuredObject.GetToken() => this.Token;
  }
}
