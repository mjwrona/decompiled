// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControlList
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class AccessControlList : AccessControlList<AccessControlEntry>
  {
    public AccessControlList(string token, bool inherit)
      : base(token, inherit)
    {
    }

    public AccessControlList(string token, bool inherit, IEnumerable<AccessControlEntry> aces)
      : base(token, inherit, aces)
    {
    }

    public AccessControlList(string token, bool inherit, IEnumerable<IAccessControlEntry> aces)
      : base(token, inherit, aces != null ? aces.Select<IAccessControlEntry, AccessControlEntry>((Func<IAccessControlEntry, AccessControlEntry>) (s => new AccessControlEntry(s))) : (IEnumerable<AccessControlEntry>) null)
    {
    }

    public AccessControlList(AccessControlList acl)
      : base((AccessControlList<AccessControlEntry>) acl)
    {
      for (int index = 0; index < this.m_aces.Count; ++index)
        this.m_aces[index] = new AccessControlEntry((IAccessControlEntry) this.m_aces[index]);
    }

    public override object Clone() => (object) new AccessControlList(this);

    protected override AccessControlEntry Convert(IAccessControlEntry ace)
    {
      if (!(ace is AccessControlEntry accessControlEntry))
        accessControlEntry = new AccessControlEntry(ace);
      return accessControlEntry;
    }
  }
}
