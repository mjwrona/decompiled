// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.RequestContextStrongBoxExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
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
