// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.RegistrationExtensions
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using System;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal static class RegistrationExtensions
  {
    public static RSACryptoServiceProvider GetPublicKey(this Registration registration)
    {
      if (registration.ClientType != ClientType.MediumTrust)
        throw new NotSupportedException();
      RSACryptoServiceProvider publicKey = new RSACryptoServiceProvider(2048);
      publicKey.FromXmlString(registration.PublicKey);
      return publicKey;
    }

    public static bool IsRestricted(this Registration registration) => registration.ClientType == ClientType.FullTrust || registration.ClientType == ClientType.HighTrust || registration.ClientType == ClientType.MediumTrust;
  }
}
