// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.CannotRenameDueToChildConflictException
// Assembly: Microsoft.TeamFoundation.VersionControl.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CF6BF9DB-38AD-4731-862B-31BA91580FFB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.ClientSlim.dll

using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Client
{
  [Serializable]
  public class CannotRenameDueToChildConflictException : VersionControlException
  {
    public CannotRenameDueToChildConflictException(string message)
      : base(message)
    {
    }

    public CannotRenameDueToChildConflictException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected CannotRenameDueToChildConflictException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }

    public CannotRenameDueToChildConflictException(string renamedItem, string offendingItem)
      : base(Resources.Format("CannotUndoRenameDueToChildConflictMessage", (object) renamedItem, (object) offendingItem))
    {
    }
  }
}
