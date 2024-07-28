// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.QueryAccessControlEntry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class QueryAccessControlEntry
  {
    public QueryAccessControlEntry(int allow, int deny, string identityType, string identifier)
    {
      this.Allow = allow;
      this.Deny = deny;
      this.Identifier = identifier;
      this.IdentityType = identityType;
    }

    internal string IdentityType { get; private set; }

    internal string Identifier { get; private set; }

    internal int Deny { get; private set; }

    internal int Allow { get; private set; }
  }
}
