// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemNotFoundException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class ItemNotFoundException : ServerItemException
  {
    private string m_message;

    public ItemNotFoundException(string serverItem)
      : base("")
    {
      this.m_serverItem = serverItem;
      this.m_message = Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Format("ItemNotFound", (object) serverItem);
    }

    public ItemNotFoundException(
      IVssRequestContext requestContext,
      ItemSpec itemSpec,
      VersionSpec versionSpec)
      : this(requestContext, itemSpec, versionSpec, DeletedState.NonDeleted)
    {
    }

    public ItemNotFoundException(
      IVssRequestContext requestContext,
      ItemSpec itemSpec,
      VersionSpec versionSpec,
      DeletedState deleted)
      : this(requestContext, (Workspace) null, itemSpec, versionSpec, deleted)
    {
    }

    public ItemNotFoundException(
      IVssRequestContext requestContext,
      Workspace workspace,
      ItemSpec itemSpec,
      VersionSpec versionSpec,
      DeletedState deleted)
      : base(string.Empty)
    {
      this.m_serverItem = itemSpec.Item;
      bool flag = itemSpec.RecursionType == RecursionType.Full;
      switch (versionSpec)
      {
        case LabelVersionSpec _:
          this.m_message = Resources.Format(flag ? "LabelItemNotFoundRecursive" : "LabelItemNotFound", (object) this.m_serverItem, (object) ((LabelVersionSpec) versionSpec).ToString());
          break;
        case LatestVersionSpec _:
          if (deleted == DeletedState.Deleted)
          {
            this.m_message = Resources.Format(flag ? "DeletedItemNotFoundRecursive" : "DeletedItemNotFound", (object) this.m_serverItem);
            break;
          }
          this.m_message = Resources.Format(flag ? "LatestItemNotFoundRecursive" : "LatestItemNotFound", (object) this.m_serverItem);
          break;
        case WorkspaceVersionSpec _:
          WorkspaceVersionSpec workspaceVersionSpec = (WorkspaceVersionSpec) versionSpec;
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
          if (userIdentity != null && IdentityHelper.IdentityHasName(userIdentity, workspaceVersionSpec.OwnerName) || requestContext.IsRequestingUser(workspaceVersionSpec.OwnerName))
          {
            if (workspace == null || TFStringComparer.WorkspaceName.Compare(workspace.Name, workspaceVersionSpec.Name) == 0)
            {
              this.m_message = Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Format(flag ? "WorkspaceItemNotFoundRecursive" : "WorkspaceItemNotFound", (object) this.m_serverItem);
              break;
            }
            this.m_message = Resources.Format(flag ? "WorkspaceNonDefaultItemNotFoundRecursive" : "WorkspaceNonDefaultItemNotFound", (object) this.m_serverItem, (object) workspace.Name);
            break;
          }
          this.m_message = Resources.Format(flag ? "WorkspaceNonDefaultItemNotFoundRecursive" : "WorkspaceNonDefaultItemNotFound", (object) this.m_serverItem, (object) workspaceVersionSpec.ToString());
          break;
        default:
          this.m_message = Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Format(flag ? "ItemNotFoundRecursive" : "ItemNotFound", (object) this.m_serverItem);
          break;
      }
    }

    public ItemNotFoundException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base("")
    {
      foreach (int num1 in TeamFoundationServiceException.ExtractInts(sqlError, "error"))
      {
        switch (num1)
        {
          case 500014:
            this.m_serverItem = ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "item");
            this.m_message = Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Format("ItemNotFound", (object) this.ServerItem);
            return;
          case 500077:
            int num2 = TeamFoundationServiceException.ExtractInt(sqlError, "itemId");
            this.m_serverItem = num2.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            this.m_message = Resources.Format("ItemIdNotFound", (object) num2);
            return;
          default:
            continue;
        }
      }
    }

    public override string Message => this.m_message;
  }
}
