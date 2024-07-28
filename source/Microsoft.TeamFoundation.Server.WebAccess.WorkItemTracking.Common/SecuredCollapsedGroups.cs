// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SecuredCollapsedGroups
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DataContract]
  public class SecuredCollapsedGroups : HashSet<string>, ISecuredObject
  {
    private string m_token;
    private int m_requiredPermission;

    public SecuredCollapsedGroups(StringComparer comparer)
      : base((IEqualityComparer<string>) comparer)
    {
    }

    int ISecuredObject.RequiredPermissions => this.m_requiredPermission;

    Guid ISecuredObject.NamespaceId => WorkItemTrackingNamespaceSecurityConstants.NamespaceId;

    string ISecuredObject.GetToken() => this.m_token;

    public string GetToken() => this.m_token;

    public void SetTokenAndPermission(string token, int requiredPermission)
    {
      this.m_token = token;
      this.m_requiredPermission = requiredPermission;
    }
  }
}
