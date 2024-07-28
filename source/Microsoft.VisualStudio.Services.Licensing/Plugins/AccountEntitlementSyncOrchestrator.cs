// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Plugins.AccountEntitlementSyncOrchestrator
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DocDB;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess;
using Microsoft.VisualStudio.Services.Licensing.DataAccess;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.Plugins
{
  internal class AccountEntitlementSyncOrchestrator
  {
    private ILicensingRepository m_licensingRepository;
    public readonly string Area = "Licensing";
    public readonly string Layer = nameof (AccountEntitlementSyncOrchestrator);

    public AccountEntitlementSyncOrchestrator(ILicensingRepository licensingRepository) => this.m_licensingRepository = licensingRepository;

    public AccountEntitlementSyncResult SyncAccountEntitlements(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      string continuation = (string) null;
      IEnumerable<UserLicenseCosmosSerializableDocument> serializableDocuments = (IEnumerable<UserLicenseCosmosSerializableDocument>) new List<UserLicenseCosmosSerializableDocument>();
      do
      {
        IPagedList<UserLicenseCosmosSerializableDocument> userDocuments = this.m_licensingRepository.GetUserDocuments(requestContext, continuation);
        continuation = userDocuments.ContinuationToken;
        serializableDocuments = serializableDocuments.Concat<UserLicenseCosmosSerializableDocument>((IEnumerable<UserLicenseCosmosSerializableDocument>) userDocuments);
      }
      while (continuation != null);
      int num = this.SyncAccountEntitlementsInternal(requestContext, (IList<UserLicenseCosmosSerializableDocument>) serializableDocuments.ToList<UserLicenseCosmosSerializableDocument>());
      return new AccountEntitlementSyncResult()
      {
        CollectionSize = serializableDocuments.Count<UserLicenseCosmosSerializableDocument>(),
        NumberSynced = num
      };
    }

    private int SyncAccountEntitlementsInternal(
      IVssRequestContext requestContext,
      IList<UserLicenseCosmosSerializableDocument> sourceDocuments)
    {
      IDictionary<Guid, LicensedIdentity> licensedIdentityMap = this.GetSourceLicensedIdentityMap(requestContext, sourceDocuments.Select<UserLicenseCosmosSerializableDocument, Guid>((Func<UserLicenseCosmosSerializableDocument, Guid>) (x => x.Document.UserId)));
      Guid empty = Guid.Empty;
      int num = 0;
      foreach (UserLicenseCosmosSerializableDocument sourceDocument in (IEnumerable<UserLicenseCosmosSerializableDocument>) sourceDocuments)
      {
        Guid userId = sourceDocument.Document.UserId;
        LicensedIdentity licensedIdentity;
        licensedIdentityMap.TryGetValue(userId, out licensedIdentity);
        if (licensedIdentity == null)
          requestContext.Trace(1035527, TraceLevel.Error, this.Area, this.Layer, string.Format("Failed to find valid identity for cosmos item storage key, not updating and keeping current info found in db: {0}", (object) userId));
        else if (this.TryUpdateAccountEntitlementLicensedIdentityIfNeeded(requestContext, sourceDocument, licensedIdentity))
          ++num;
      }
      return num;
    }

    public bool TryUpdateAccountEntitlementLicensedIdentityIfNeeded(
      IVssRequestContext requestContext,
      UserLicenseCosmosSerializableDocument sourceDocument,
      LicensedIdentity licensedIdentity)
    {
      Guid userId = sourceDocument.Document.UserId;
      if (this.DoesLicensedIdentityNeedUpdating(sourceDocument.Document.LicensedIdentity, licensedIdentity))
      {
        try
        {
          this.UpdateAccountEntitlementWithLicensedIdentity(requestContext, sourceDocument, licensedIdentity);
          requestContext.Trace(1035528, TraceLevel.Info, this.Area, this.Layer, string.Format("Updated licensed identity for {0} in {1}. ", (object) sourceDocument.Document.UserId, (object) ((DocDBSerializableDocument) sourceDocument).HostIdUsedInPartitionKey) + "License information before licensed identity overwrite is: " + sourceDocument.Document.License.Serialize<UserLicense>());
          return true;
        }
        catch (JsonSerializationException ex)
        {
          requestContext.Trace(1035525, TraceLevel.Warning, this.Area, this.Layer, string.Format("Failed initial update for ${0}. ", (object) userId) + "because of a serialization exception: " + ex.Message + ". Getting updated document and using upsert instead");
          sourceDocument = this.m_licensingRepository.GetUserDocument(requestContext, sourceDocument.Document.UserId);
          if (sourceDocument != null)
            this.UpsertAccountEntitlementWithLicensedIdentity(requestContext, sourceDocument, licensedIdentity);
        }
        catch (Exception ex1)
        {
          sourceDocument = this.m_licensingRepository.GetUserDocument(requestContext, sourceDocument.Document.UserId);
          if (this.DoesLicensedIdentityNeedUpdating(sourceDocument.Document.LicensedIdentity, licensedIdentity))
          {
            requestContext.Trace(1035525, TraceLevel.Warning, this.Area, this.Layer, string.Format("Failed initial update for ${0}. ", (object) userId) + "Identity still needed update. ExceptionMessage: " + ex1.Message);
            try
            {
              this.UpdateAccountEntitlementWithLicensedIdentity(requestContext, sourceDocument, licensedIdentity);
              requestContext.Trace(1035528, TraceLevel.Info, this.Area, this.Layer, string.Format("Updated licensed identity for {0} in {1}. ", (object) sourceDocument.Document.UserId, (object) ((DocDBSerializableDocument) sourceDocument).HostIdUsedInPartitionKey) + "License information before licensed identity overwrite is: " + sourceDocument.Document.License.Serialize<UserLicense>());
              return true;
            }
            catch (Exception ex2)
            {
              string message = string.Format("Failed retry update for ${0}. Identity failed to be updated ExceptionMessage: {1}", (object) userId, (object) ex2.Message);
              requestContext.Trace(1035526, TraceLevel.Error, this.Area, this.Layer, message);
              throw new LicensingFailedSyncException(message);
            }
          }
          else
            requestContext.Trace(1035525, TraceLevel.Info, this.Area, this.Layer, string.Format("Failed initial update for ${0}. ", (object) userId) + "but no changes were detected when pulling updated document. No longer need to update.ExceptionMessage: " + ex1.Message);
        }
      }
      return false;
    }

    public void UpsertAccountEntitlementWithLicensedIdentity(
      IVssRequestContext requestContext,
      UserLicenseCosmosSerializableDocument sourceDocument,
      LicensedIdentity licensedIdentity)
    {
      sourceDocument.Document.LicensedIdentity = licensedIdentity;
      this.m_licensingRepository.UpsertUserDocument(requestContext, sourceDocument, true);
    }

    public void UpdateAccountEntitlementWithLicensedIdentity(
      IVssRequestContext requestContext,
      UserLicenseCosmosSerializableDocument sourceDocument,
      LicensedIdentity licensedIdentity)
    {
      sourceDocument.Document.LicensedIdentity = licensedIdentity;
      this.m_licensingRepository.UpdateUserDocument(requestContext, sourceDocument, optimisticConcurrency: true);
    }

    private IDictionary<Guid, LicensedIdentity> GetSourceLicensedIdentityMap(
      IVssRequestContext requestContext,
      IEnumerable<Guid> storageKeys)
    {
      Dictionary<Guid, LicensedIdentity> result = new Dictionary<Guid, LicensedIdentity>();
      this.BatchReadIdentities(requestContext, storageKeys).ForEach<Microsoft.VisualStudio.Services.Identity.Identity>((Action<Microsoft.VisualStudio.Services.Identity.Identity>) (i =>
      {
        try
        {
          Guid key = i.EnterpriseStorageKey(requestContext);
          LicensedIdentity licensedIdentity = i.ToLicensedIdentity();
          if (result.ContainsKey(key))
          {
            LicensedIdentity valueOrDefault = result.GetValueOrDefault<Guid, LicensedIdentity>(key, (LicensedIdentity) null);
            requestContext.TraceAlways(1035530, TraceLevel.Error, this.Area, this.Layer, string.Format("Found duplicate storage key for EnterpriseStorageKey {0}: ", (object) key) + string.Format("Identity trying to place in dictionary: {0}, {1}, {2}. ", (object) i.DisplayName, (object) i.Id, (object) i.SubjectDescriptor) + "LicensedIdentity already found in dictionary: " + valueOrDefault.Name + " " + valueOrDefault.Email);
          }
          else
            result.Add(key, licensedIdentity);
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(1035529, TraceLevel.Error, this.Area, this.Layer, string.Format("Failed to get enterprise storage key for: {0}, {1}, {2}. ", (object) i.DisplayName, (object) i.Id, (object) i.SubjectDescriptor) + "Error coming from identity: " + ex.Message + " - " + ex.StackTrace);
        }
      }));
      return (IDictionary<Guid, LicensedIdentity>) result;
    }

    public IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> BatchReadIdentities(
      IVssRequestContext requestContext,
      IEnumerable<Guid> storageKeys,
      int batchSize = 5000)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> first = (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      int count = 0;
      IEnumerable<Guid> guids;
      do
      {
        guids = storageKeys.Skip<Guid>(count).Take<Guid>(batchSize);
        count += guids.Count<Guid>();
        first = first.Concat<Microsoft.VisualStudio.Services.Identity.Identity>(service.ReadIdentitiesWithFallback(requestContext, guids));
      }
      while (guids != null && guids.Count<Guid>() != 0);
      return first;
    }

    public bool DoesLicensedIdentityNeedUpdating(
      LicensedIdentity targetLicensedIdentity,
      LicensedIdentity sourceLicensedIdentity)
    {
      return targetLicensedIdentity == null || targetLicensedIdentity.IsEmpty() || this.DoesLicenseIdentityHaveDiff(targetLicensedIdentity, sourceLicensedIdentity);
    }

    public bool DoesLicenseIdentityHaveDiff(LicensedIdentity source, LicensedIdentity target)
    {
      if (source == null && target == null)
        return false;
      return source == null || target == null || !source.Equals(target);
    }
  }
}
