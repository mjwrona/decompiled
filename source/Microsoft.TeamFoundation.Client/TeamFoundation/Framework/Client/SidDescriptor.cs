// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.SidDescriptor
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public static class SidDescriptor
  {
    internal static IdentityDescriptor Create(
      string identityType,
      SecurityIdentifierInfo securityIdInfo)
    {
      if (securityIdInfo == null)
        throw new ArgumentNullException(nameof (securityIdInfo));
      return new IdentityDescriptor(identityType, securityIdInfo.SecurityId.Value)
      {
        Data = (object) securityIdInfo
      };
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
