// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.LegacyAccessCheckException
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Net;

namespace Microsoft.TeamFoundation.Server.Core
{
  [Serializable]
  public class LegacyAccessCheckException : TeamFoundationServiceException
  {
    public LegacyAccessCheckException(AccessCheckException ex)
      : base(ex.Message, HttpStatusCode.Forbidden)
    {
      this.IdentityDisplayName = ex.IdentityDisplayName;
      this.IdentityDescriptor = ex.Descriptor;
      this.Token = ex.Token;
      this.RequestedPermissions = ex.RequestedPermissions;
      this.NamespaceId = ex.NamespaceId;
    }

    public override string SerializedExceptionName => "AccessCheckException";

    public override void GetExceptionProperties(ExceptionPropertyCollection properties)
    {
      base.GetExceptionProperties(properties);
      if (this.IdentityDisplayName != null)
        properties.Set(AccessCheckExceptionProperties.DisplayName, this.IdentityDisplayName);
      if (this.Token != null)
        properties.Set(AccessCheckExceptionProperties.Token, this.Token);
      properties.Set(AccessCheckExceptionProperties.RequestedPermissions, this.RequestedPermissions);
      properties.Set(AccessCheckExceptionProperties.NamespaceId, this.NamespaceId);
      if (!(this.IdentityDescriptor != (IdentityDescriptor) null))
        return;
      properties.Set(AccessCheckExceptionProperties.DescriptorIdentifier, this.IdentityDescriptor.Identifier);
      properties.Set(AccessCheckExceptionProperties.DescriptorIdentityType, this.IdentityDescriptor.IdentityType);
    }

    public string IdentityDisplayName { get; private set; }

    public IdentityDescriptor IdentityDescriptor { get; internal set; }

    public string Token { get; internal set; }

    public int RequestedPermissions { get; internal set; }

    public Guid NamespaceId { get; internal set; }
  }
}
