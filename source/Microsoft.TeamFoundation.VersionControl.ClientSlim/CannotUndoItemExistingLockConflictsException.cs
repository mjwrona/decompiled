// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.CannotUndoItemExistingLockConflictsException
// Assembly: Microsoft.TeamFoundation.VersionControl.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CF6BF9DB-38AD-4731-862B-31BA91580FFB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.ClientSlim.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Client
{
  [Serializable]
  public class CannotUndoItemExistingLockConflictsException : VersionControlException
  {
    public CannotUndoItemExistingLockConflictsException(string message)
      : base(message)
    {
    }

    public CannotUndoItemExistingLockConflictsException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected CannotUndoItemExistingLockConflictsException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
