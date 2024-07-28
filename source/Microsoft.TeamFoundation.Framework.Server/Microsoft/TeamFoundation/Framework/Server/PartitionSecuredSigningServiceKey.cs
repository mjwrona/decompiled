// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PartitionSecuredSigningServiceKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PartitionSecuredSigningServiceKey : StrongboxSecuredSigningServiceKey
  {
    internal PartitionSecuredSigningServiceKey(Guid identifier)
      : base(SigningKeyType.PartitionSecured, identifier)
    {
    }

    protected override IVssRequestContext GetKeyTargetRequestContext(
      IVssRequestContext requestContext)
    {
      return requestContext;
    }

    protected override Guid UnlockOrCreateKeyStorageDrawer(
      IVssRequestContext keyTargetRequestContext,
      ITeamFoundationStrongBoxService strongBoxService)
    {
      keyTargetRequestContext.CheckProjectCollectionOrOrganizationRequestContext();
      Guid keyStorageDrawer = strongBoxService.UnlockDrawer(keyTargetRequestContext, FrameworkServerConstants.SigningServiceStrongBoxDrawerName, false);
      if (keyStorageDrawer != Guid.Empty)
        return keyStorageDrawer;
      string migrationCertificate = MigrationRegistryUtil.GetDefaultMigrationCertificate(keyTargetRequestContext);
      TeamFoundationSigningService service = keyTargetRequestContext.GetService<TeamFoundationSigningService>();
      Guid guid = Guid.NewGuid();
      service.CreateDeploymentSecuredKey(keyTargetRequestContext, guid, migrationCertificate);
      try
      {
        return strongBoxService.CreateDrawerWithExplicitSigningKey(keyTargetRequestContext, FrameworkServerConstants.SigningServiceStrongBoxDrawerName, guid);
      }
      catch (StrongBoxDrawerExistsException ex)
      {
        service.RemoveKeys(keyTargetRequestContext, (IEnumerable<Guid>) new List<Guid>()
        {
          guid
        });
        return strongBoxService.UnlockDrawer(keyTargetRequestContext, FrameworkServerConstants.SigningServiceStrongBoxDrawerName, true);
      }
    }
  }
}
