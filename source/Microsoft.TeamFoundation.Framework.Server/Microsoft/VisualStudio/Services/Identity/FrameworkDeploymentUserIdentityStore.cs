// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.FrameworkDeploymentUserIdentityStore
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkDeploymentUserIdentityStore : 
    IDeploymentUserIdentityStore,
    IVssFrameworkService
  {
    private IDeploymentUserIdentityCacheService m_cache;
    private int m_batchedReadThreshold;
    private static readonly RegistryQuery BatchedReadThresholdQuery = new RegistryQuery("/Configuration/DeploymentIdentityService/BatchedReadThreshold");
    private const string DisableUpdateById = "VisualStudio.Services.Identity.FrameworkDeploymentUserIdentityStore.DisableUpdateById";
    private const string BatchedReadIdentitiesEnabled = "VisualStudio.Services.Identity.FrameworkDeploymentUserIdentityStore.EnableBatchedRead";
    private const string Area = "Identity";
    private const string Layer = "FrameworkDeploymentUserIdentityStore";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      FrameworkDeploymentUserIdentityStore.ValidateRequestContext(requestContext);
      this.m_cache = requestContext.GetService<IDeploymentUserIdentityCacheService>();
      this.m_batchedReadThreshold = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in FrameworkDeploymentUserIdentityStore.BatchedReadThresholdQuery, 100);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds)
    {
      FrameworkDeploymentUserIdentityStore.ValidateRequestContext(requestContext);
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[identityIds.Count];
      List<Tuple<int, Guid>> source = new List<Tuple<int, Guid>>();
      for (int index = 0; index < identityIds.Count; ++index)
      {
        Guid identityId = identityIds[index];
        if (!(identityId == Guid.Empty))
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = this.m_cache.Get(requestContext, identityId);
          if (identity == null)
            source.Add(new Tuple<int, Guid>(index, identityId));
          identityArray[index] = identity;
        }
      }
      if (source.Count > 0)
      {
        UserIdentityHttpClient httpClient = this.GetHttpClient(requestContext);
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList;
        if (source.Count >= this.m_batchedReadThreshold && requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.FrameworkDeploymentUserIdentityStore.EnableBatchedRead"))
        {
          List<Guid> identityIdsToFetch = source.Select<Tuple<int, Guid>, Guid>((Func<Tuple<int, Guid>, Guid>) (cacheMiss => cacheMiss.Item2)).ToList<Guid>();
          requestContext.TraceDataConditionally(282827, TraceLevel.Info, "Identity", nameof (FrameworkDeploymentUserIdentityStore), "ReadIdentities called in a batch. IdentityIdsToFetch: ", (Func<object>) (() => (object) identityIdsToFetch), nameof (ReadIdentities));
          try
          {
            identityList = httpClient.ReadIdentities((IList<Guid>) identityIdsToFetch, requestContext.CancellationToken).SyncResult<IList<Microsoft.VisualStudio.Services.Identity.Identity>>();
          }
          catch (Exception ex) when (ex is UserDoesNotExistException || ex is IdentityNotFoundException)
          {
            requestContext.TraceException(282828, TraceLevel.Error, "Identity", nameof (FrameworkDeploymentUserIdentityStore), ex, "ReadIdentities call in a batch, failed.");
            identityList = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[source.Count];
            for (int index = 0; index < identityIdsToFetch.Count; ++index)
              identityList[index] = httpClient.ReadIdentity(identityIdsToFetch[index], requestContext.CancellationToken).SyncResult<Microsoft.VisualStudio.Services.Identity.Identity>();
          }
        }
        else
        {
          identityList = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[source.Count];
          for (int index = 0; index < source.Count; ++index)
            identityList[index] = httpClient.ReadIdentity(source[index].Item2, requestContext.CancellationToken).SyncResult<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
        for (int index1 = 0; index1 < identityList.Count; ++index1)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index1];
          if (identity != null)
          {
            int index2 = source[index1].Item1;
            identityArray[index2] = identity;
            this.m_cache.Set(requestContext, identity);
          }
        }
      }
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray;
    }

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> identityDescriptors)
    {
      FrameworkDeploymentUserIdentityStore.ValidateRequestContext(requestContext);
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[identityDescriptors.Count];
      UserIdentityHttpClient identityHttpClient = (UserIdentityHttpClient) null;
      for (int index = 0; index < identityDescriptors.Count; ++index)
      {
        IdentityDescriptor identityDescriptor = identityDescriptors[index];
        if (!(identityDescriptor == (IdentityDescriptor) null) && identityDescriptor.IsCuidBased())
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = this.m_cache.Get(requestContext, identityDescriptor);
          if (identity == null)
          {
            identityHttpClient = identityHttpClient ?? this.GetHttpClient(requestContext);
            identity = identityHttpClient.ReadIdentity(identityDescriptor, requestContext.CancellationToken).SyncResult<Microsoft.VisualStudio.Services.Identity.Identity>();
            if (identity != null)
              this.m_cache.Set(requestContext, identity);
          }
          identityArray[index] = identity;
        }
      }
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray;
    }

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SubjectDescriptor> subjectDescriptors)
    {
      FrameworkDeploymentUserIdentityStore.ValidateRequestContext(requestContext);
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[subjectDescriptors.Count];
      UserIdentityHttpClient identityHttpClient = (UserIdentityHttpClient) null;
      for (int index = 0; index < subjectDescriptors.Count; ++index)
      {
        SubjectDescriptor subjectDescriptor = subjectDescriptors[index];
        if (subjectDescriptor.IsCuidBased())
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = this.m_cache.Get(requestContext, subjectDescriptor);
          if (identity == null)
          {
            identityHttpClient = identityHttpClient ?? this.GetHttpClient(requestContext);
            identity = identityHttpClient.ReadIdentity(subjectDescriptor, requestContext.CancellationToken).SyncResult<Microsoft.VisualStudio.Services.Identity.Identity>();
            if (identity != null)
              this.m_cache.Set(requestContext, identity);
          }
          identityArray[index] = identity;
        }
      }
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray;
    }

    public void CreateIdentities(IVssRequestContext requestContext, IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      FrameworkDeploymentUserIdentityStore.ValidateRequestContext(requestContext);
      UserIdentityHttpClient httpClient = this.GetHttpClient(requestContext);
      for (int index = 0; index < identities.Count; ++index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = identities[index];
        identities[index] = httpClient.CreateIdentity(identity, requestContext.CancellationToken).SyncResult<Microsoft.VisualStudio.Services.Identity.Identity>();
        this.m_cache.Remove(requestContext, identity.Id);
        this.m_cache.SendIdentityChangedNotification(requestContext, identity);
      }
    }

    public bool UpdateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool allowMetadataUpdates)
    {
      FrameworkDeploymentUserIdentityStore.ValidateRequestContext(requestContext);
      UserIdentityHttpClient httpClient = this.GetHttpClient(requestContext);
      for (int index = 0; index < identities.Count; ++index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = identities[index];
        if (!identity1.SubjectDescriptor.IsCuidBased())
          throw new NotSupportedException("Cannot update the identity with subject type '" + identity1.SubjectDescriptor.SubjectType + "' because it is not supported.");
        Microsoft.VisualStudio.Services.Identity.Identity identity2;
        try
        {
          identity2 = requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.FrameworkDeploymentUserIdentityStore.DisableUpdateById") ? httpClient.UpdateIdentity(identity1.SubjectDescriptor, identity1, requestContext.CancellationToken).SyncResult<Microsoft.VisualStudio.Services.Identity.Identity>() : (!(identity1.Id == Guid.Empty) ? httpClient.UpdateIdentity(identity1.Id, identity1, requestContext.CancellationToken).SyncResult<Microsoft.VisualStudio.Services.Identity.Identity>() : httpClient.CreateIdentity(identity1, requestContext.CancellationToken).SyncResult<Microsoft.VisualStudio.Services.Identity.Identity>());
        }
        catch (Exception ex1) when (ex1 is UserDoesNotExistException || ex1 is IdentityNotFoundException)
        {
          try
          {
            identity2 = httpClient.CreateIdentity(identity1, requestContext.CancellationToken).SyncResult<Microsoft.VisualStudio.Services.Identity.Identity>();
          }
          catch (Exception ex2) when (ex2 is UserAlreadyExistsException || ex2 is IdentityAlreadyExistsException)
          {
            identity2 = httpClient.UpdateIdentity(identity1.Id, identity1, requestContext.CancellationToken).SyncResult<Microsoft.VisualStudio.Services.Identity.Identity>();
          }
        }
        if (identity2 != null)
        {
          identities[index].Id = identity2.Id;
          identities[index].MasterId = identity2.MasterId;
          this.m_cache.Remove(requestContext, identity2.Id);
          this.m_cache.SendIdentityChangedNotification(requestContext, identity2);
        }
      }
      return true;
    }

    private UserIdentityHttpClient GetHttpClient(IVssRequestContext requestContext) => requestContext.GetClient<UserIdentityHttpClient>(InstanceManagementHelper.UserSvcPrincipal);

    private static void ValidateRequestContext(IVssRequestContext requestContext)
    {
      requestContext.CheckHostedDeployment();
      requestContext.CheckDeploymentRequestContext();
    }
  }
}
