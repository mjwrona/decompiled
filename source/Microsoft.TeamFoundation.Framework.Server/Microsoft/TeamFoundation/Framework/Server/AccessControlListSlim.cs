// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControlListSlim
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class AccessControlListSlim : AccessControlList<AccessControlEntrySlim>
  {
    public AccessControlListSlim(string token, bool inherit)
      : base(token, inherit)
    {
    }

    public AccessControlListSlim(
      string token,
      bool inherit,
      IEnumerable<AccessControlEntrySlim> aces)
      : base(token, inherit, aces)
    {
    }

    public AccessControlListSlim(string token, bool inherit, IEnumerable<IAccessControlEntry> aces)
      : base(token, inherit, aces != null ? aces.Select<IAccessControlEntry, AccessControlEntrySlim>((Func<IAccessControlEntry, AccessControlEntrySlim>) (s => new AccessControlEntrySlim(s))) : (IEnumerable<AccessControlEntrySlim>) null)
    {
    }

    public AccessControlListSlim(AccessControlListSlim acl)
      : base((AccessControlList<AccessControlEntrySlim>) acl)
    {
    }

    public AccessControlListSlim(IAccessControlList acl)
      : this(acl.Token, acl.InheritPermissions, acl.AccessControlEntries)
    {
    }

    public override object Clone() => (object) new AccessControlListSlim(this);

    protected override AccessControlEntrySlim Convert(IAccessControlEntry ace) => new AccessControlEntrySlim(ace);

    internal void UnsafeAddUnsorted(AccessControlEntrySlim ace) => this.m_aces.Add(ace);
  }
}
