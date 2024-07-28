// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IAccessControlList
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface IAccessControlList : ICloneable
  {
    string Token { get; set; }

    bool InheritPermissions { get; set; }

    IEnumerable<IAccessControlEntry> AccessControlEntries { get; }

    int Count { get; }

    bool RemovePermissions(
      IdentityDescriptor descriptor,
      int permissionsToRemove,
      out int updatedAllow,
      out int updatedDeny);

    bool RemoveAccessControlEntry(IdentityDescriptor descriptor);

    IAccessControlEntry QueryAccessControlEntry(IdentityDescriptor descriptor);

    IAccessControlEntry SetAccessControlEntry(IAccessControlEntry newAce, bool merge);

    void RemoveZeroEntries();
  }
}
