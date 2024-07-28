// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.WitQueryFolderPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class WitQueryFolderPermissionsManager : SecurityNamespacePermissionsManager
  {
    private Dictionary<Guid, string> m_tokenCache;

    public WitQueryFolderPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token,
      string projectName)
      : base(permissionsIdentifier)
    {
      this.m_tokenCache = new Dictionary<Guid, string>();
      WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
      Project project = service.GetProject(requestContext, projectName);
      this.m_tokenCache[project.Guid] = projectName;
      IEnumerable<QueryItem> publicQueries;
      service.GetQueryItems(requestContext, project.Id, out publicQueries, out IEnumerable<QueryItem> _);
      Dictionary<Guid, QueryItem> dictionary = new Dictionary<Guid, QueryItem>();
      Guid result;
      bool flag = Guid.TryParse(token, out result);
      foreach (QueryItem queryItem in publicQueries)
      {
        this.m_tokenCache[queryItem.Id] = queryItem.Name;
        if (flag)
          dictionary[queryItem.Id] = queryItem;
      }
      if (flag && dictionary.ContainsKey(result))
      {
        QueryItem queryItem = dictionary[result];
        Stack<string> values = new Stack<string>();
        string str1 = queryItem.Id.ToString();
        Guid guid;
        while (!string.IsNullOrEmpty(str1))
        {
          values.Push(str1);
          if (object.Equals((object) Guid.Empty, (object) queryItem.ParentId))
          {
            str1 = (string) null;
          }
          else
          {
            queryItem = dictionary[queryItem.ParentId];
            guid = queryItem.Id;
            str1 = guid.ToString();
          }
        }
        Stack<string> stringStack = values;
        guid = project.Guid;
        string str2 = guid.ToString();
        stringStack.Push(str2);
        values.Push(QueryItemSecurityConstants.RootFolder);
        string token1 = string.Join(QueryItemSecurityConstants.PathSeparator.ToString(), (IEnumerable<string>) values);
        this.Initialize(requestContext, token1);
      }
      else
        this.Initialize(requestContext, token);
      if (!this.UserHasReadAccess)
        return;
      this.InheritPermissions = this.PermissionSets[QueryItemSecurityConstants.NamespaceGuid].GetAccessControlList(requestContext).InheritPermissions;
    }

    public override bool CanEditAdminPermissions => true;

    public override bool CanTokenInheritPermissions => this.Token.Split(QueryItemSecurityConstants.PathSeparator).Length > 3;

    public override void ChangeInheritance(
      IVssRequestContext requestContext,
      bool inheritPermissions)
    {
      SecurityNamespacePermissionSet permissionSet = this.PermissionSets[QueryItemSecurityConstants.NamespaceGuid];
      this.ChangeInheritance(requestContext, permissionSet, inheritPermissions);
    }

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, SecurityNamespacePermissionSet> permissionSets = new Dictionary<Guid, SecurityNamespacePermissionSet>();
      int permissionsToDisplay = 15;
      SecurityNamespacePermissionSet namespacePermissionSet = new SecurityNamespacePermissionSet(requestContext, QueryItemSecurityConstants.NamespaceGuid, this.Token, permissionsToDisplay);
      permissionSets.Add(QueryItemSecurityConstants.NamespaceGuid, namespacePermissionSet);
      return permissionSets;
    }

    protected override string GetTokenDisplayName(IVssRequestContext requestContext, string token)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      StringBuilder stringBuilder = new StringBuilder();
      string[] strArray = token.Split(QueryItemSecurityConstants.PathSeparator);
      List<string> values = new List<string>(strArray.Length - 1);
      for (int index = 1; index < strArray.Length; ++index)
      {
        string input = strArray[index];
        Guid result;
        if (Guid.TryParse(input, out result))
        {
          if (this.m_tokenCache.ContainsKey(result))
            values.Add(this.m_tokenCache[result]);
          else
            values.Add(input);
        }
        else
          values.Add(input);
      }
      return string.Join(QueryItemSecurityConstants.PathSeparator.ToString(), (IEnumerable<string>) values);
    }

    public Guid QueryItemId
    {
      get
      {
        Guid result;
        return Guid.TryParse(((IEnumerable<string>) this.Token.Split(QueryItemSecurityConstants.PathSeparator)).LastOrDefault<string>(), out result) ? result : Guid.Empty;
      }
    }

    private string GetParentToken(string token)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      int length = token.LastIndexOf(QueryItemSecurityConstants.PathSeparator);
      return length < 0 ? (string) null : token.Substring(0, length);
    }

    protected override IAccessControlList GetParentAccessControlList(
      IVssRequestContext requestContext,
      string token,
      List<IdentityDescriptor> descriptors)
    {
      string parentToken = this.GetParentToken(token);
      return this.PermissionSets[QueryItemSecurityConstants.NamespaceGuid].SecuredSecurityNamespace.QueryAccessControlLists(requestContext, parentToken, (IEnumerable<IdentityDescriptor>) descriptors, true, false).FirstOrDefault<IAccessControlList>();
    }
  }
}
