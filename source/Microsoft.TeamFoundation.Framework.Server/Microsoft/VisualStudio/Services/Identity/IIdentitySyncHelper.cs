// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IIdentitySyncHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  public interface IIdentitySyncHelper
  {
    IDictionary<string, int> SyncCounters { get; }

    void PreserveMember(IdentityDescriptor descriptor);

    bool HasIdentityBeenSeen(IdentityDescriptor descriptor);

    void ProcessIdentity(Microsoft.VisualStudio.Services.Identity.Identity member);

    void Submit(out bool identifierChanged);
  }
}
