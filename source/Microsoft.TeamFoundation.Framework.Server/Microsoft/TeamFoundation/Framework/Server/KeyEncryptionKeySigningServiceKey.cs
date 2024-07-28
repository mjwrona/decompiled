// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.KeyEncryptionKeySigningServiceKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class KeyEncryptionKeySigningServiceKey : StrongboxSecuredSigningServiceKey
  {
    public KeyEncryptionKeySigningServiceKey(Guid identifier)
      : base(SigningKeyType.KeyEncryptionKey, identifier)
    {
    }

    protected override IVssRequestContext GetKeyTargetRequestContext(
      IVssRequestContext requestContext)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).Elevate();
    }

    protected override Guid UnlockOrCreateKeyStorageDrawer(
      IVssRequestContext keyTargetRequestContext,
      ITeamFoundationStrongBoxService strongBoxService)
    {
      return this.UnlockOrCreateDrawerSignedByKey(keyTargetRequestContext, strongBoxService, FrameworkServerConstants.KeyEncryptionKeyStrongBoxDrawerName, (Func<TeamFoundationSigningService, Guid>) (signingService =>
      {
        Guid identifier = Guid.NewGuid();
        string keyvaultKeyIdentifier = keyTargetRequestContext.GetService<ICachedRegistryService>().GetValue<string>(keyTargetRequestContext, (RegistryQuery) FrameworkServerConstants.DefaultMasterWrappingKeyPath, false, (string) null);
        signingService.CreateMasterWrappingKey(keyTargetRequestContext, identifier, keyvaultKeyIdentifier);
        return identifier;
      }));
    }

    protected override string GetKeyStorageDrawer() => FrameworkServerConstants.KeyEncryptionKeyStrongBoxDrawerName;
  }
}
