// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Charting.WebApi.SecuredChartObject
// Assembly: Microsoft.TeamFoundation.Charting.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D43D663A-A882-4856-B0B7-D0E666C5CC4A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Charting.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Charting.WebApi
{
  public class SecuredChartObject : ISecuredObject
  {
    protected ISecuredObject m_securedObject;

    public void SetSecuredObject(ISecuredObject securedObject)
    {
      this.m_securedObject = securedObject;
      this.UpdateSecuredObjectOfChildren(securedObject);
    }

    protected virtual void UpdateSecuredObjectOfChildren(ISecuredObject securedObject)
    {
    }

    Guid ISecuredObject.NamespaceId => this.m_securedObject.NamespaceId;

    int ISecuredObject.RequiredPermissions => this.m_securedObject.RequiredPermissions;

    string ISecuredObject.GetToken() => this.m_securedObject.GetToken();
  }
}
