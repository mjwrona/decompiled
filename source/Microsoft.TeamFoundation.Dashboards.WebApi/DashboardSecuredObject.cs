// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.DashboardSecuredObject
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  public abstract class DashboardSecuredObject : ISecuredObject
  {
    private ISecuredObject SecuredObject;

    public void SetSecuredObject(ISecuredObject securedObject)
    {
      this.SecuredObject = securedObject;
      this.SetSecuredChildren(securedObject);
    }

    protected virtual void SetSecuredChildren(ISecuredObject securedObject)
    {
    }

    Guid ISecuredObject.NamespaceId => this.SecuredObject.NamespaceId;

    int ISecuredObject.RequiredPermissions => this.SecuredObject.RequiredPermissions;

    string ISecuredObject.GetToken() => this.SecuredObject.GetToken();
  }
}
