// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemTypeNameToStateColors
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class WorkItemTypeNameToStateColors : 
    Dictionary<string, IReadOnlyCollection<WorkItemStateColor>>,
    ISecuredObject
  {
    private int m_requiredPermissions;
    private string m_token;
    private Guid m_namespaceId;

    public WorkItemTypeNameToStateColors(
      IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> initialDictionary)
    {
      if (initialDictionary == null)
        return;
      foreach (KeyValuePair<string, IReadOnlyCollection<WorkItemStateColor>> initial in (IEnumerable<KeyValuePair<string, IReadOnlyCollection<WorkItemStateColor>>>) initialDictionary)
        this.Add(initial.Key, initial.Value);
    }

    public ProcessReadSecuredObject SecureForProject(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ProcessReadSecuredObject processReadSecuredObject = (ProcessReadSecuredObject) null;
      if (requestContext.WitContext().ProcessReadPermissionChecker.HasProcessReadPermissionForProject(projectId, out processReadSecuredObject))
        this.SetSecuredObject(processReadSecuredObject);
      return processReadSecuredObject;
    }

    protected void SetSecuredObject(ProcessReadSecuredObject securedObject)
    {
      ISecuredObject securedObject1 = (ISecuredObject) securedObject;
      this.m_token = securedObject1.GetToken();
      this.m_requiredPermissions = securedObject1.RequiredPermissions;
      this.m_namespaceId = securedObject1.NamespaceId;
    }

    Guid ISecuredObject.NamespaceId => this.m_namespaceId;

    int ISecuredObject.RequiredPermissions => this.m_requiredPermissions;

    string ISecuredObject.GetToken() => this.m_token;
  }
}
