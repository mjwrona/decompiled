// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CannotUndoItemExistingLockConflictsException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class CannotUndoItemExistingLockConflictsException : ServerException
  {
    public CannotUndoItemExistingLockConflictsException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base(Resources.Format(nameof (CannotUndoItemExistingLockConflictsException), (object) ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "sourceItem"), (object) ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "lockItem"), (object) ServerException.ExtractIdentityName(requestContext, sqlError, "lockedBy"), (object) TeamFoundationServiceException.ExtractString(sqlError, "workspace"), TeamFoundationServiceException.ExtractString(sqlError, "lockType").Equals("checkin", StringComparison.Ordinal) ? (object) Resources.Get("CheckinLockType") : (object) Resources.Get("CheckOutLockType")))
    {
    }
  }
}
