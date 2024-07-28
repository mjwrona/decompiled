// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemLockedException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class ItemLockedException : ServerItemException
  {
    private string m_lockedBy;
    private string m_lockedWorkspace;

    public ItemLockedException(
      IVssRequestContext requestContext,
      string serverItem,
      string lockedItem,
      string lockedBy,
      string lockedWorkspace)
      : base(Resources.Format(nameof (ItemLockedException), (object) serverItem, (object) lockedItem, (object) WorkspaceSpec.Combine(lockedWorkspace, lockedBy)))
    {
      this.m_serverItem = serverItem;
      this.m_lockedWorkspace = lockedWorkspace;
      this.m_lockedBy = lockedBy;
    }

    public ItemLockedException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(requestContext, ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "targetItem"), ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "lockedItem"), ServerException.ExtractIdentityName(requestContext, sqlError, "lockedBy"), TeamFoundationServiceException.ExtractString(sqlError, "workspace"))
    {
    }

    public override void SetFailureInfo(Failure failure)
    {
      base.SetFailureInfo(failure);
      failure.IdentityName = this.m_lockedBy;
    }
  }
}
