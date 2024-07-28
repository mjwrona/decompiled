// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BackingStoreAccessControlEntryHelpers
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Security.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class BackingStoreAccessControlEntryHelpers
  {
    private const int c_maxIdentityBatchSize = 5000;
    private const string c_area = "Security";
    private const string c_layer = "BackingStoreAccessControlEntryHelpers";

    public static IDictionary<IdentityDescriptor, Guid> BuildReverseIdentityMap(
      IVssRequestContext requestContext,
      IEnumerable<IdentityDescriptor> descriptors)
    {
      requestContext.CheckHostedDeployment();
      requestContext.TraceEnter(56151, "Security", nameof (BackingStoreAccessControlEntryHelpers), nameof (BuildReverseIdentityMap));
      try
      {
        HashSet<IdentityDescriptor> identityDescriptorSet = new HashSet<IdentityDescriptor>(descriptors, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        Dictionary<IdentityDescriptor, Guid> toReturn = new Dictionary<IdentityDescriptor, Guid>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        Dictionary<IdentityDescriptor, SecuritySubjectEntry> dictionary = vssRequestContext.GetService<IVssSecuritySubjectService>().GetSecuritySubjectEntries(vssRequestContext).ToDictionary<SecuritySubjectEntry, IdentityDescriptor>((Func<SecuritySubjectEntry, IdentityDescriptor>) (s => s.ToDescriptor()), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        foreach (IdentityDescriptor identityDescriptor in identityDescriptorSet)
        {
          Guid spGuid;
          Guid tenantGuid;
          if (ServicePrincipals.TryParse(identityDescriptor, out spGuid, out tenantGuid) && tenantGuid == requestContext.ServiceHost.DeploymentServiceHost.DeploymentServiceHostInternal().S2STenantId)
          {
            toReturn.Add(identityDescriptor, spGuid);
          }
          else
          {
            SecuritySubjectEntry securitySubjectEntry;
            if (dictionary.TryGetValue(identityDescriptor, out securitySubjectEntry))
            {
              toReturn.Add(identityDescriptor, securitySubjectEntry.Id);
            }
            else
            {
              Guid guid;
              if (BackingStoreAccessControlEntryHelpers.GroupWellKnownVSIDs.DescriptorToVsid.TryGetValue(identityDescriptor, out guid))
                toReturn.Add(identityDescriptor, guid);
            }
          }
        }
        identityDescriptorSet.ExceptWith((IEnumerable<IdentityDescriptor>) toReturn.Keys);
        IdentityService identityService = requestContext.GetService<IdentityService>();
        List<IdentityDescriptor> batch = new List<IdentityDescriptor>(Math.Min(identityDescriptorSet.Count, 5000));
        Action action = (Action) (() =>
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = identityService.ReadIdentities(requestContext.Elevate(), (IList<IdentityDescriptor>) batch, QueryMembership.None, (IEnumerable<string>) null);
          for (int index = 0; index < batch.Count; ++index)
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
            if (identity == null)
            {
              requestContext.Trace(56154, TraceLevel.Warning, "Security", nameof (BackingStoreAccessControlEntryHelpers), "Can't resolve descriptor {0}. ACEs for this identity will be dropped.", (object) batch[index].ToString());
            }
            else
            {
              IdentityDescriptor wellKnownIdentifier = identityService.MapToWellKnownIdentifier(identity.Descriptor);
              if (!IdentityDescriptorComparer.Instance.Equals(batch[index], wellKnownIdentifier))
                requestContext.Trace(56155, TraceLevel.Warning, "Security", nameof (BackingStoreAccessControlEntryHelpers), "Descriptor failed to round-trip. Input: {0}. Output: {1} {2}. ACEs for this identity will be dropped.", (object) batch[index].ToString(), (object) wellKnownIdentifier.ToString(), (object) identity.Id.ToString("D"));
              else
                toReturn[batch[index]] = identity.Id;
            }
          }
        });
        foreach (IdentityDescriptor identityDescriptor in identityDescriptorSet)
        {
          if (batch.Count == batch.Capacity)
          {
            action();
            batch.Clear();
          }
          batch.Add(identityDescriptor);
        }
        if (batch.Count > 0)
          action();
        return (IDictionary<IdentityDescriptor, Guid>) toReturn;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56152, "Security", nameof (BackingStoreAccessControlEntryHelpers), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56153, "Security", nameof (BackingStoreAccessControlEntryHelpers), nameof (BuildReverseIdentityMap));
      }
    }

    public static IDictionary<Guid, IdentityDescriptor> BuildIdentityMap(
      IVssRequestContext requestContext,
      Guid namespaceId,
      IEnumerable<Guid> subjectIds)
    {
      requestContext.TraceEnter(56062, "Security", nameof (BackingStoreAccessControlEntryHelpers), nameof (BuildIdentityMap));
      try
      {
        HashSet<Guid> guidSet = new HashSet<Guid>(subjectIds);
        using (new TraceWatch(requestContext, 56344, TraceLevel.Error, TimeSpan.FromSeconds(60.0), "Security", nameof (BackingStoreAccessControlEntryHelpers), "NamespaceId: {0}, SubjectId count: {1}", new object[2]
        {
          (object) namespaceId,
          (object) guidSet.Count
        }))
        {
          Guid accountTenantId = new Guid();
          TeamFoundationExecutionEnvironment executionEnvironment = requestContext.ExecutionEnvironment;
          if (executionEnvironment.IsHostedDeployment && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.ServiceInstanceType() == ServiceInstanceTypes.TFS)
            accountTenantId = requestContext.GetOrganizationAadTenantId();
          List<Guid> batch = new List<Guid>(Math.Min(guidSet.Count, 5000));
          IDictionary<Guid, IdentityDescriptor> toReturn = (IDictionary<Guid, IdentityDescriptor>) new Dictionary<Guid, IdentityDescriptor>();
          BackingStoreAccessControlEntryHelpers.MapSubjectStoreSubjects(requestContext, toReturn, (ISet<Guid>) guidSet);
          executionEnvironment = requestContext.ExecutionEnvironment;
          if (executionEnvironment.IsHostedDeployment)
            BackingStoreAccessControlEntryHelpers.MapServicePrincipals(requestContext, toReturn, (ISet<Guid>) guidSet);
          IdentityMapper identityMapper = requestContext.GetService<IdentityService>().IdentityMapper;
          executionEnvironment = requestContext.ExecutionEnvironment;
          Func<List<Guid>, IList<Microsoft.VisualStudio.Services.Identity.Identity>> vsidToDescriptorMapper;
          if (executionEnvironment.IsHostedDeployment && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.To(TeamFoundationHostType.Deployment).GetService<SecuritySettingsService>().Settings.BypassIdentityServiceCache)
          {
            IdentityHttpClient identityHttpClient = requestContext.Elevate().GetClient<IdentityHttpClient>();
            vsidToDescriptorMapper = (Func<List<Guid>, IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (toResolve => (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityHttpClient.ReadIdentitiesAsync((IList<Guid>) toResolve, QueryMembership.None, (IEnumerable<string>) null, false, (object) null, new CancellationToken()).SyncResult<IdentitiesCollection>());
          }
          else
          {
            IdentityService identityService = requestContext.GetService<IdentityService>();
            vsidToDescriptorMapper = (Func<List<Guid>, IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (toResolve => identityService.ReadIdentities(requestContext.Elevate(), (IList<Guid>) toResolve, QueryMembership.None, (IEnumerable<string>) null));
          }
          Action action = (Action) (() =>
          {
            IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = vsidToDescriptorMapper(batch);
            identityMapper.MapToWellKnownIdentifiers((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList);
            for (int index = 0; index < batch.Count; ++index)
            {
              Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
              if (identity == null || (IdentityDescriptor) null == identity.Descriptor || identity.Descriptor.IsUnknownIdentityType())
              {
                requestContext.Trace(56333, TraceLevel.Warning, "Security", nameof (BackingStoreAccessControlEntryHelpers), "Can't resolve TFID {0}. ACEs for this identity will be dropped.", (object) batch[index]);
                identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
              }
              else if (new Guid() != accountTenantId && identity.Descriptor.IdentityType.Equals("Microsoft.IdentityModel.Claims.ClaimsIdentity") && identity.GetProperty<string>("Domain", string.Empty).Equals("Windows Live ID"))
              {
                requestContext.Trace(56341, TraceLevel.Error, "Security", nameof (BackingStoreAccessControlEntryHelpers), "This service host is bound to tenant ID {0} but ReadIdentities returned an Identity object representing an MSA identity. Input VSID was {1}, output VSID was {2}, output descriptor was {3}. ACEs for this identity will be dropped.", (object) accountTenantId, (object) batch[index], (object) identityList[index].Id, (object) identityList[index].Descriptor);
                identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
              }
              if (identity != null)
                toReturn[identity.Id] = identity.Descriptor;
            }
          });
          List<Guid> guidList = new List<Guid>(guidSet.Count);
          guidList.AddRange((IEnumerable<Guid>) guidSet);
          Random random = new Random();
          for (int index1 = 0; index1 < guidList.Count; ++index1)
          {
            int index2 = index1 + random.Next(guidList.Count - index1);
            Guid guid = guidList[index1];
            guidList[index1] = guidList[index2];
            guidList[index2] = guid;
          }
          foreach (Guid guid in guidList)
          {
            if (batch.Count == batch.Capacity)
            {
              action();
              batch.Clear();
            }
            batch.Add(guid);
          }
          if (batch.Count > 0)
            action();
          return toReturn;
        }
      }
      finally
      {
        requestContext.TraceLeave(56063, "Security", nameof (BackingStoreAccessControlEntryHelpers), nameof (BuildIdentityMap));
      }
    }

    private static void MapServicePrincipals(
      IVssRequestContext requestContext,
      IDictionary<Guid, IdentityDescriptor> map,
      ISet<Guid> identities)
    {
      requestContext.CheckHostedDeployment();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Guid guid1 = Guid.Parse(vssRequestContext.GetService<IVssRegistryService>().GetValue(vssRequestContext, (RegistryQuery) OAuth2RegistryConstants.S2STenantId, false, (string) null));
      List<Guid> guidList = (List<Guid>) null;
      foreach (Guid identity in (IEnumerable<Guid>) identities)
      {
        if (ServicePrincipals.IsInternalServicePrincipalId(identity))
        {
          if (guidList == null)
            guidList = new List<Guid>();
          guidList.Add(identity);
          map.Add(identity, new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", string.Format("{0}@{1}", (object) identity, (object) guid1)));
        }
      }
      if (guidList == null)
        return;
      foreach (Guid guid2 in guidList)
        identities.Remove(guid2);
    }

    private static void MapSubjectStoreSubjects(
      IVssRequestContext requestContext,
      IDictionary<Guid, IdentityDescriptor> map,
      ISet<Guid> identities)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IEnumerable<SecuritySubjectEntry> securitySubjectEntries = vssRequestContext.GetService<IVssSecuritySubjectService>().GetSecuritySubjectEntries(vssRequestContext);
      foreach (SecuritySubjectEntry securitySubjectEntry in new HashSet<Guid>((IEnumerable<Guid>) identities).Join<Guid, SecuritySubjectEntry, Guid, SecuritySubjectEntry>(securitySubjectEntries, (Func<Guid, Guid>) (id => id), (Func<SecuritySubjectEntry, Guid>) (entry => entry.Id), (Func<Guid, SecuritySubjectEntry, SecuritySubjectEntry>) ((id, entry) => entry)))
      {
        if (identities.Remove(securitySubjectEntry.Id))
          map.Add(securitySubjectEntry.Id, securitySubjectEntry.ToDescriptor());
      }
    }

    internal static class GroupWellKnownVSIDs
    {
      private static readonly Guid NamespaceAdministratorsGroup = new Guid("00000001-E8EB-47B1-871F-A06706DA8E8E");
      private static readonly Guid BuildServicesGroup = new Guid("00000011-E8EB-47B1-871F-A06706DA8E8E");
      private static readonly Guid ProxyServicesGroup = new Guid("55461769-DC0A-4E09-9522-103A88A43585");
      public static readonly Guid EveryoneGroup = new Guid("00000003-E8EB-47B1-871F-A06706DA8E8E");
      public static readonly Guid ServiceUsersGroup = new Guid("00000002-E8EB-47B1-871F-A06706DA8E8E");
      public static readonly Guid ServicePrincipalGroup = new Guid("00000008-E8EB-47B1-871F-A06706DA8E8E");
      public static readonly Guid UsersGroup = new Guid("142BDF43-01F7-49EB-B3EC-BE1D5AE9813F");
      public static readonly Guid AnyServicePrincipal = new Guid("DB39E447-A6DF-4C89-8B33-AB045F403616");
      public static readonly IReadOnlyDictionary<Guid, IdentityDescriptor> VsidToDescriptor = (IReadOnlyDictionary<Guid, IdentityDescriptor>) new Dictionary<Guid, IdentityDescriptor>()
      {
        {
          BackingStoreAccessControlEntryHelpers.GroupWellKnownVSIDs.NamespaceAdministratorsGroup,
          GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
        },
        {
          BackingStoreAccessControlEntryHelpers.GroupWellKnownVSIDs.ServiceUsersGroup,
          GroupWellKnownIdentityDescriptors.ServiceUsersGroup
        },
        {
          BackingStoreAccessControlEntryHelpers.GroupWellKnownVSIDs.EveryoneGroup,
          GroupWellKnownIdentityDescriptors.EveryoneGroup
        },
        {
          BackingStoreAccessControlEntryHelpers.GroupWellKnownVSIDs.ServicePrincipalGroup,
          GroupWellKnownIdentityDescriptors.ServicePrincipalGroup
        },
        {
          BackingStoreAccessControlEntryHelpers.GroupWellKnownVSIDs.UsersGroup,
          GroupWellKnownIdentityDescriptors.UsersGroup
        },
        {
          BackingStoreAccessControlEntryHelpers.GroupWellKnownVSIDs.AnyServicePrincipal,
          new IdentityDescriptor("System:ServicePrincipal", "*")
        },
        {
          BackingStoreAccessControlEntryHelpers.GroupWellKnownVSIDs.BuildServicesGroup,
          new IdentityDescriptor("Microsoft.TeamFoundation.Identity", BuildGroupWellKnownSecurityIds.BuildServicesGroup.Value)
        },
        {
          BackingStoreAccessControlEntryHelpers.GroupWellKnownVSIDs.ProxyServicesGroup,
          GroupWellKnownIdentityDescriptors.Proxy.ServiceAccounts
        }
      };
      public static readonly IReadOnlyDictionary<IdentityDescriptor, Guid> DescriptorToVsid = (IReadOnlyDictionary<IdentityDescriptor, Guid>) BackingStoreAccessControlEntryHelpers.GroupWellKnownVSIDs.VsidToDescriptor.ToDictionary<KeyValuePair<Guid, IdentityDescriptor>, IdentityDescriptor, Guid>((Func<KeyValuePair<Guid, IdentityDescriptor>, IdentityDescriptor>) (s => s.Value), (Func<KeyValuePair<Guid, IdentityDescriptor>, Guid>) (s => s.Key), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
    }
  }
}
