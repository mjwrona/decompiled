// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd.ServerNodeRecord
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd
{
  internal class ServerNodeRecord
  {
    public ServerNodeRecord(
      string roleName,
      string roleInstance,
      string ipAddress,
      DateTime lastUpdated,
      DateTime expiration)
    {
      this.RoleName = roleName.CheckArgumentIsNotNullOrEmpty(nameof (roleName));
      this.RoleInstance = roleInstance.CheckArgumentIsNotNullOrEmpty(nameof (roleInstance));
      this.IPAddress = ipAddress.CheckArgumentIsNotNullOrEmpty(nameof (ipAddress));
      this.LastUpdated = lastUpdated;
      this.Expiration = expiration;
    }

    public static ServerNodeRecord CreateWithDefaults(
      IVssRequestContext requestContext,
      string roleName,
      string roleInstance,
      string ipAddress)
    {
      DateTime utcNow = DateTime.UtcNow;
      DateTime expiration = DateTime.UtcNow + requestContext.GetSmartRouterServerTimeToLiveSetting();
      return new ServerNodeRecord(roleName, roleInstance, ipAddress, utcNow, expiration);
    }

    public string RoleName { get; }

    public string RoleInstance { get; }

    public string IPAddress { get; }

    public DateTime LastUpdated { get; }

    public DateTime Expiration { get; }
  }
}
