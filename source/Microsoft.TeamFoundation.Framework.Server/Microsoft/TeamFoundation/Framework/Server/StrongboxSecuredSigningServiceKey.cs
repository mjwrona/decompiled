// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongboxSecuredSigningServiceKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal abstract class StrongboxSecuredSigningServiceKey : SigningServiceKey
  {
    private const string c_area = "StrongboxSecuredSigningServiceKey";
    private const string c_layer = "Service";

    internal StrongboxSecuredSigningServiceKey(SigningKeyType keyType, Guid identifier)
      : base(keyType, identifier)
    {
    }

    protected abstract IVssRequestContext GetKeyTargetRequestContext(
      IVssRequestContext requestContext);

    protected virtual Guid UnlockOrCreateKeyStorageDrawer(
      IVssRequestContext keyTargetRequestContext,
      ITeamFoundationStrongBoxService strongBoxService)
    {
      return strongBoxService.UnlockOrCreateDrawer(keyTargetRequestContext, FrameworkServerConstants.SigningServiceStrongBoxDrawerName);
    }

    protected virtual string GetKeyStorageDrawer() => FrameworkServerConstants.SigningServiceStrongBoxDrawerName;

    protected override byte[] GetKeyData(IVssRequestContext requestContext)
    {
      this.CheckKeyDataIsNotNull();
      IVssRequestContext targetRequestContext = this.GetKeyTargetRequestContext(requestContext);
      ITeamFoundationStrongBoxService service = targetRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(targetRequestContext, this.GetKeyStorageDrawer(), true);
      string lookupKey = new Guid(this.KeyData).ToString("D");
      long streamLength;
      return new BinaryReader(service.RetrieveFile(targetRequestContext, drawerId, lookupKey, out streamLength)).ReadBytes(Convert.ToInt32(streamLength));
    }

    protected override void StoreKeyData(
      IVssRequestContext requestContext,
      byte[] rawKeyData,
      bool overwriteExisting)
    {
      this.CheckKeyDataIsNull();
      IVssRequestContext targetRequestContext = this.GetKeyTargetRequestContext(requestContext);
      ITeamFoundationStrongBoxService service = targetRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid keyStorageDrawer = this.UnlockOrCreateKeyStorageDrawer(targetRequestContext, service);
      Guid guid = Guid.NewGuid();
      if (overwriteExisting)
      {
        SigningComponent.SigningServiceKey signingServiceKey = SigningServiceKey.LoadRawKeyFromDatabase(requestContext, ((ISigningServiceKey) this).Identifier);
        if (signingServiceKey != null && signingServiceKey.KeyType == ((ISigningServiceKey) this).KeyType)
          guid = new Guid(signingServiceKey.KeyData);
      }
      string lookupKey = guid.ToString("D");
      service.UploadFile(targetRequestContext, keyStorageDrawer, lookupKey, (Stream) new MemoryStream(rawKeyData));
      SigningComponent.SigningServiceKey database;
      try
      {
        database = this.StoreKeyDataToDatabase(requestContext, guid.ToByteArray(), overwriteExisting);
      }
      catch (Exception ex1)
      {
        requestContext.TraceException(1090110, nameof (StrongboxSecuredSigningServiceKey), "Service", ex1);
        try
        {
          service.DeleteItem(targetRequestContext, keyStorageDrawer, lookupKey);
        }
        catch (Exception ex2)
        {
          requestContext.TraceCatch(1090111, nameof (StrongboxSecuredSigningServiceKey), "Service", ex2);
        }
        throw;
      }
      if (!overwriteExisting)
      {
        if (guid != new Guid(database.KeyData))
        {
          try
          {
            service.DeleteItem(targetRequestContext, keyStorageDrawer, lookupKey);
          }
          catch (Exception ex)
          {
            requestContext.TraceCatch(1090111, nameof (StrongboxSecuredSigningServiceKey), "Service", ex);
          }
        }
      }
      this.KeyData = database.KeyData;
    }

    protected override void DeleteKeyData(IVssRequestContext requestContext)
    {
      if (this.KeyData == null)
        return;
      try
      {
        string lookupKey = new Guid(this.KeyData).ToString("D");
        IVssRequestContext targetRequestContext = this.GetKeyTargetRequestContext(requestContext);
        ITeamFoundationStrongBoxService service = targetRequestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerId = service.UnlockDrawer(targetRequestContext, FrameworkServerConstants.SigningServiceStrongBoxDrawerName, true);
        service.DeleteItem(targetRequestContext, drawerId, lookupKey);
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(1090111, nameof (StrongboxSecuredSigningServiceKey), "Service", ex);
      }
    }

    protected Guid UnlockOrCreateDrawerSignedByKey(
      IVssRequestContext keyTargetRequestContext,
      ITeamFoundationStrongBoxService strongBoxService,
      string drawerName,
      Func<TeamFoundationSigningService, Guid> generateKey)
    {
      Guid drawerSignedByKey = strongBoxService.UnlockDrawer(keyTargetRequestContext, drawerName, false);
      if (drawerSignedByKey != Guid.Empty)
        return drawerSignedByKey;
      TeamFoundationSigningService service = keyTargetRequestContext.GetService<TeamFoundationSigningService>();
      Guid signingKeyId = generateKey(service);
      try
      {
        return strongBoxService.CreateDrawerWithExplicitSigningKey(keyTargetRequestContext, drawerName, signingKeyId);
      }
      catch (StrongBoxDrawerExistsException ex)
      {
        service.RemoveKeys(keyTargetRequestContext, (IEnumerable<Guid>) new List<Guid>()
        {
          signingKeyId
        });
        return strongBoxService.UnlockDrawer(keyTargetRequestContext, drawerName, true);
      }
    }
  }
}
