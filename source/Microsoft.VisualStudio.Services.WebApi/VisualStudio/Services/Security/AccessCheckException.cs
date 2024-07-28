// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.AccessCheckException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Security
{
  [ExceptionMapping("0.0", "3.0", "AccessCheckException", "Microsoft.TeamFoundation.Framework.Server.AccessCheckException, Microsoft.TeamFoundation.Framework.Server, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class AccessCheckException : SecurityException
  {
    public AccessCheckException(
      IdentityDescriptor descriptor,
      string identityDisplayName,
      string token,
      int requestedPermissions,
      Guid namespaceId,
      string message)
      : this(descriptor, token, requestedPermissions, namespaceId, message)
    {
      this.IdentityDisplayName = identityDisplayName;
    }

    public AccessCheckException(
      IdentityDescriptor descriptor,
      string token,
      int requestedPermissions,
      Guid namespaceId,
      string message)
      : base(message)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      ArgumentUtility.CheckForNull<string>(message, nameof (message));
      this.Descriptor = descriptor;
      this.Token = token;
      this.RequestedPermissions = requestedPermissions;
      this.NamespaceId = namespaceId;
    }

    public AccessCheckException(string message)
      : base(message)
    {
    }

    public AccessCheckException(string message, Exception ex)
      : base(message, ex)
    {
    }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public IdentityDescriptor Descriptor { get; private set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string IdentityDisplayName { get; private set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string Token { get; private set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public int RequestedPermissions { get; private set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public Guid NamespaceId { get; private set; }
  }
}
