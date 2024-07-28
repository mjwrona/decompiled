// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.LocalWorkspaceTransactionAlreadyRunningException
// Assembly: Microsoft.TeamFoundation.VersionControl.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CF6BF9DB-38AD-4731-862B-31BA91580FFB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.ClientSlim.dll

using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Client
{
  [Serializable]
  public class LocalWorkspaceTransactionAlreadyRunningException : VersionControlException
  {
    public LocalWorkspaceTransactionAlreadyRunningException()
      : base(Resources.Get("LocalWorkspaceTransactionAlreadyRunning"))
    {
    }
  }
}
