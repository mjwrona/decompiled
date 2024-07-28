// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemTrackingPermission
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DataContract]
  public class WorkItemTrackingPermission : ISecuredObject
  {
    private readonly string Token;
    private readonly int Permission;

    [DataMember]
    public bool HasPermission { get; set; }

    public WorkItemTrackingPermission(Guid projectId, int permission, string token)
    {
      this.Permission = permission;
      this.Token = string.Format("/{0}/{1}/{2}", (object) "WorkItemTracking", (object) projectId, (object) token);
    }

    Guid ISecuredObject.NamespaceId => WorkItemTrackingNamespaceSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => this.Permission;

    string ISecuredObject.GetToken() => this.Token;
  }
}
