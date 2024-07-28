// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CannotLockException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class CannotLockException : ServerItemException
  {
    public CannotLockException(
      IVssRequestContext requestContext,
      string serverItem,
      string lockedItem,
      string lockedBy,
      string lockedWorkspace)
      : base(Resources.Format(nameof (CannotLockException), (object) serverItem, (object) lockedItem, (object) lockedBy, (object) lockedWorkspace))
    {
    }

    public CannotLockException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(requestContext, ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "targetItem"), ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "lockedItem"), ServerException.ExtractIdentityName(requestContext, sqlError, "lockedBy"), TeamFoundationServiceException.ExtractString(sqlError, "workspace"))
    {
    }
  }
}
