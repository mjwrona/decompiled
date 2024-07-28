// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IAccessControlEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface IAccessControlEntry
  {
    IdentityDescriptor Descriptor { get; set; }

    int Allow { get; set; }

    int Deny { get; set; }

    bool IsEmpty { get; }

    bool IncludesExtendedInfo { get; }

    int InheritedAllow { get; }

    int InheritedDeny { get; }

    int EffectiveAllow { get; }

    int EffectiveDeny { get; }

    IAccessControlEntry Clone();

    Microsoft.VisualStudio.Services.Security.AccessControlEntry ToRestContractType();
  }
}
