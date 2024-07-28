// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.Replication
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class Replication
  {
    public const string ReplicationEnabledHttpHeader = "X-TFS-Replication-Enabled";
    public const string ReplicationEnabledHttpHeaderValue = "1";
    public const string ReplicationHttpHeaderPrefix = "X-TFS-Replication-";
    public const string ReplicationWitHttpHeaderName = "X-TFS-Replication-WITP";
  }
}
