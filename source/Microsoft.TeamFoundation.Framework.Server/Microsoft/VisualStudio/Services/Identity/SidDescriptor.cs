// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SidDescriptor
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class SidDescriptor
  {
    internal static IdentityDescriptor Create(
      string identityType,
      SecurityIdentifierInfo securityIdInfo)
    {
      return SidDescriptor.Create(identityType, securityIdInfo, false);
    }

    internal static IdentityDescriptor Create(
      string identityType,
      SecurityIdentifierInfo securityIdInfo,
      bool readOnly)
    {
      if (securityIdInfo == null)
        throw new ArgumentNullException(nameof (securityIdInfo));
      return readOnly ? (IdentityDescriptor) new ReadOnlyIdentityDescriptor(identityType, securityIdInfo.SecurityId.Value, (object) securityIdInfo) : new IdentityDescriptor(identityType, securityIdInfo.SecurityId.Value, (object) securityIdInfo);
    }

    internal static SecurityIdentifierInfo GetData(IdentityDescriptor descriptor)
    {
      SecurityIdentifierInfo data = (SecurityIdentifierInfo) null;
      if (descriptor.Data != null)
        data = descriptor.Data as SecurityIdentifierInfo;
      if (data == null)
      {
        data = new SecurityIdentifierInfo(new SecurityIdentifier(descriptor.Identifier));
        descriptor.Data = (object) data;
      }
      return data;
    }
  }
}
