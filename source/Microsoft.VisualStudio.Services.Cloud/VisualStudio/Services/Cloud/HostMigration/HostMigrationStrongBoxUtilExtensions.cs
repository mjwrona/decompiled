// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.HostMigrationStrongBoxUtilExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.HostMigration;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public static class HostMigrationStrongBoxUtilExtensions
  {
    public static string GetLookupKey(this TargetHostMigration migrationEntry, string containerUri) => HostMigrationStrongBoxUtil.GetLookupKey(migrationEntry.HostProperties.Id, migrationEntry.MigrationId, containerUri);

    public static bool PerformRSASecuredKeyFaultInjection(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>();
      (bool IsInjectionEnabled, string InjectionValue) = HostMigrationInjectionUtil.CheckInjection<string>(requestContext, FrameworkServerConstants.MigrationFaultInjectionAction, string.Empty);
      if (!IsInjectionEnabled || !(InjectionValue == "KeyNotConverted") || requestContext.ServiceHost.IsProduction)
        return false;
      string name = "TestRSASecuredDrawer";
      string lookupKey = "TestLookupKey";
      string str = "TestValue";
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawer = service.UnlockOrCreateDrawer(requestContext, name);
      service.RotateSigningKey(requestContext, drawer, SigningKeyType.RSASecured);
      service.AddString(requestContext, drawer, lookupKey, str);
      if (service.GetSigningKeyType(requestContext, drawer) != SigningKeyType.RSASecured)
        throw new UnexpectedItemKindException();
      return true;
    }
  }
}
