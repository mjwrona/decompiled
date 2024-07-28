// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ExternalIntegration.Utilities.RequestContextStrongBoxExtensions
// Assembly: Microsoft.TeamFoundation.ExternalIntegration.Utilities, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6309B6D0-0EEE-4299-AA79-F0B62882E0B1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ExternalIntegration.Utilities.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.ExternalIntegration.Utilities
{
  public static class RequestContextStrongBoxExtensions
  {
    public static string GetOrGenerateStrongBoxSecret(
      this IVssRequestContext elevatedRequestContext,
      string strongBoxDrawerName,
      string strongBoxKey,
      int secretStrength = 32)
    {
      ITeamFoundationStrongBoxService service = elevatedRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(elevatedRequestContext, strongBoxDrawerName, false);
      if (drawerId == Guid.Empty)
        drawerId = service.CreateDrawer(elevatedRequestContext, strongBoxDrawerName);
      string base64String;
      try
      {
        base64String = service.GetString(elevatedRequestContext, drawerId, strongBoxKey);
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        base64String = Convert.ToBase64String(RequestContextStrongBoxExtensions.GetRandomBytes(secretStrength));
        service.AddString(elevatedRequestContext, drawerId, strongBoxKey, base64String);
      }
      return base64String;
    }

    private static byte[] GetRandomBytes(int length)
    {
      byte[] data = new byte[length];
      using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
        cryptoServiceProvider.GetBytes(data);
      return data;
    }
  }
}
