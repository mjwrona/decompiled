// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationIdentityGroupAdminRemovedEvent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationIdentityGroupAdminRemovedEvent
  {
    public TeamFoundationIdentity GroupIdentity { get; set; }

    public IdentityDescriptor AdminIdentityDescriptor { get; set; }

    public TeamFoundationIdentityGroupAdminRemovedEvent(
      TeamFoundationIdentity groupIdentity,
      IdentityDescriptor adminIdentityDescriptor)
    {
      this.GroupIdentity = groupIdentity;
      this.AdminIdentityDescriptor = adminIdentityDescriptor;
    }
  }
}
