// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxSigningConsumer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class StrongBoxSigningConsumer : ISigningServiceConsumer
  {
    private static readonly string s_area = "StrongBox";
    private static readonly string s_layer = nameof (StrongBoxSigningConsumer);

    public IEnumerable<Guid> GetSigningKeysInUse(IVssRequestContext requestContext)
    {
      IList<Guid> keysInUse;
      using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
        keysInUse = component.QueryStrongBoxSigningKeys();
      this.AddMigrationSigningKeyIfNecessary(requestContext, keysInUse);
      return (IEnumerable<Guid>) keysInUse;
    }

    public IList<Guid> GetSigningKeysInUseExcludingKeyType(
      IVssRequestContext requestContext,
      SigningKeyType signingKeyType)
    {
      IList<Guid> keysInUse;
      using (StrongBoxComponent13 component = requestContext.CreateComponent<StrongBoxComponent13>())
        keysInUse = component.QueryStrongBoxSigningKeysExcludingKeyType(signingKeyType);
      if (signingKeyType != SigningKeyType.DeploymentCertificateSecured)
        this.AddMigrationSigningKeyIfNecessary(requestContext, keysInUse);
      return keysInUse;
    }

    public ReencryptResults Reencrypt(IVssRequestContext requestContext, Guid identifier)
    {
      TeamFoundationStrongBoxService service = requestContext.GetService<TeamFoundationStrongBoxService>();
      ReencryptResults reencryptResults = new ReencryptResults();
      requestContext.Trace(109150, TraceLevel.Verbose, StrongBoxSigningConsumer.s_area, StrongBoxSigningConsumer.s_layer, "Searching for strongbox items to re-encrypt to signing key {0}", (object) identifier);
      if (requestContext.IsFeatureEnabled("Microsoft.AzureDevOps.StrongBox.BatchedItemsReencryption"))
      {
        try
        {
          reencryptResults = service.ReencryptItemsInBatches(requestContext);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(109185, StrongBoxSigningConsumer.s_area, StrongBoxSigningConsumer.s_layer, ex);
          ++reencryptResults.FailureCount;
          reencryptResults.Failures.Add(new Exception(string.Format("Error re-encrypting strongbox items: {0}", (object) ex), ex));
        }
      }
      else
      {
        if (requestContext.IsFeatureEnabled(FrameworkServerConstants.StrongBoxReentrantItemRotationFeatureName))
        {
          foreach (StrongBoxItemInfo strongBoxItemInfo in (IEnumerable<StrongBoxItemInfo>) service.GetItemsToReencrypt(requestContext))
          {
            try
            {
              requestContext.Trace(109152, TraceLevel.Verbose, StrongBoxSigningConsumer.s_area, StrongBoxSigningConsumer.s_layer, "Re-encrypting strongbox item {0};{1}", (object) strongBoxItemInfo.DrawerId, (object) strongBoxItemInfo.LookupKey);
              service.ReencryptItem(requestContext, strongBoxItemInfo, identifier);
              ++reencryptResults.SuccessCount;
            }
            catch (StrongBoxItemNotFoundException ex)
            {
              requestContext.TraceCatch(109154, StrongBoxSigningConsumer.s_area, StrongBoxSigningConsumer.s_layer, (Exception) ex);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(109155, StrongBoxSigningConsumer.s_area, StrongBoxSigningConsumer.s_layer, ex);
              ++reencryptResults.FailureCount;
              reencryptResults.Failures.Add(new Exception(string.Format("Error re-encrypting strongbox item {0}\\{1}: {2}", (object) strongBoxItemInfo.DrawerId, (object) strongBoxItemInfo.LookupKey, (object) ex), ex));
            }
          }
        }
        else
        {
          foreach (StrongBoxItemInfo strongBoxItemInfo in (IEnumerable<StrongBoxItemInfo>) service.GetItemsToReencrypt(requestContext, new Guid?(identifier)))
          {
            try
            {
              requestContext.Trace(109101, TraceLevel.Verbose, StrongBoxSigningConsumer.s_area, StrongBoxSigningConsumer.s_layer, "Re-encrypting strongbox item {0};{1}", (object) strongBoxItemInfo.DrawerId, (object) strongBoxItemInfo.LookupKey);
              service.ReencryptItem(requestContext, strongBoxItemInfo, identifier);
              ++reencryptResults.SuccessCount;
            }
            catch (Exception ex)
            {
              requestContext.TraceException(109102, StrongBoxSigningConsumer.s_area, StrongBoxSigningConsumer.s_layer, ex);
              ++reencryptResults.FailureCount;
              reencryptResults.Failures.Add(new Exception(string.Format("Error re-encrypting strongbox item {0}\\{1}: {2}", (object) strongBoxItemInfo?.DrawerId, (object) strongBoxItemInfo?.LookupKey, (object) ex), ex));
            }
          }
        }
        requestContext.Trace(109151, TraceLevel.Verbose, StrongBoxSigningConsumer.s_area, StrongBoxSigningConsumer.s_layer, string.Format("Re-encrypted {0} items successfully and encountered {1} failures", (object) reencryptResults.SuccessCount, (object) reencryptResults.FailureCount));
      }
      return reencryptResults;
    }

    private void AddMigrationSigningKeyIfNecessary(
      IVssRequestContext requestContext,
      IList<Guid> keysInUse)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      string migrationCertificate = MigrationRegistryUtil.GetDefaultMigrationCertificate(requestContext);
      if (string.IsNullOrWhiteSpace(migrationCertificate))
        return;
      switch (MigrationRegistryUtil.GetMigrationSigningState(requestContext))
      {
        case HostMigrationSigningState.Preparing:
        case HostMigrationSigningState.Migrating:
          Guid certificateKey = requestContext.GetService<ITeamFoundationSigningService>().FindCertificateKey(requestContext, migrationCertificate);
          if (certificateKey == Guid.Empty)
            break;
          keysInUse.Add(certificateKey);
          break;
      }
    }
  }
}
