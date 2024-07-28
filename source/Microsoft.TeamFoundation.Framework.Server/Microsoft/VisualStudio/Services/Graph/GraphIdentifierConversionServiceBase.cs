// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphIdentifierConversionServiceBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal abstract class GraphIdentifierConversionServiceBase : 
    IGraphIdentifierConversionService,
    IVssFrameworkService
  {
    private static readonly IConfigPrototype<bool> respectSubjectTypePrototype = ConfigPrototype.Create<bool>("Identity.RespectSubjectTypeWhenConvertingDescriptorToStorageKey", false);
    private IConfigQueryable<bool> respectSubjectTypeConfig;
    protected const string Area = "Graph";
    protected const string Layer = "GraphIdentifierConversionServiceBase";
    private static readonly Guid s_UserService = new Guid("00000038-0000-8888-8000-000000000000");
    private const string RespectSubjectTypePrototypePath = "Identity.RespectSubjectTypeWhenConvertingDescriptorToStorageKey";

    public GraphIdentifierConversionServiceBase()
      : this(ConfigProxy.Create<bool>(GraphIdentifierConversionServiceBase.respectSubjectTypePrototype))
    {
    }

    public GraphIdentifierConversionServiceBase(IConfigQueryable<bool> respectSubjectType) => this.respectSubjectTypeConfig = respectSubjectType;

    public virtual void ServiceStart(IVssRequestContext systemRequestContext) => this.ServiceHostId = systemRequestContext.ServiceHost.InstanceId;

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Guid GetStorageKeyByDescriptor(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      GraphValidation.CheckDescriptor(descriptor, nameof (descriptor));
      requestContext.TraceDataConditionally(15270442, TraceLevel.Verbose, "Graph", nameof (GraphIdentifierConversionServiceBase), "Received input parameters", (Func<object>) (() => (object) new
      {
        descriptor = descriptor
      }), nameof (GetStorageKeyByDescriptor));
      requestContext.CheckServiceHostId(this.ServiceHostId, (IVssFrameworkService) this);
      Guid storageKey = new Guid();
      if (descriptor.IsGroupScopeType())
        storageKey = this.GetStorageKeyByGroupScopeDescriptor(requestContext, descriptor);
      else if (descriptor.IsCuidBased() && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IUserIdentifierConversionService service = requestContext.GetService<IUserIdentifierConversionService>();
        try
        {
          storageKey = service.GetStorageKeyByDescriptor(requestContext, descriptor);
          requestContext.TraceDataConditionally(15270450, TraceLevel.Verbose, "Graph", nameof (GraphIdentifierConversionServiceBase), "Found storage key by descriptor from user service", (Func<object>) (() => (object) new
          {
            storageKey = storageKey
          }), nameof (GetStorageKeyByDescriptor));
        }
        catch (UserDoesNotExistException ex)
        {
          requestContext.Trace(15270451, TraceLevel.Verbose, "Graph", nameof (GraphIdentifierConversionServiceBase), "User: {0} does not exist.", (object) descriptor);
        }
      }
      else if (descriptor.IsCuidBased() && requestContext.ServiceHost.IsOnly(TeamFoundationHostType.Application))
      {
        storageKey = this.GetStorageKeyByCuidBasedDescriptor(requestContext, descriptor);
        requestContext.TraceDataConditionally(15270447, TraceLevel.Verbose, "Graph", nameof (GraphIdentifierConversionServiceBase), "Found storage key by descriptor", (Func<object>) (() => (object) new
        {
          storageKey = storageKey
        }), nameof (GetStorageKeyByDescriptor));
      }
      else
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
        {
          descriptor
        }, QueryMembership.None, (IEnumerable<string>) null).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
        requestContext.TraceDataConditionally(15270447, TraceLevel.Verbose, "Graph", nameof (GraphIdentifierConversionServiceBase), "Found identity by descriptor", (Func<object>) (() => (object) new
        {
          Id = identity?.Id
        }), nameof (GetStorageKeyByDescriptor));
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = identity;
        storageKey = identity1 != null ? identity1.Id : new Guid();
      }
      requestContext.TraceDataConditionally(15270437, TraceLevel.Verbose, "Graph", nameof (GraphIdentifierConversionServiceBase), "Returning storage key", (Func<object>) (() => (object) new
      {
        storageKey = storageKey
      }), nameof (GetStorageKeyByDescriptor));
      return storageKey;
    }

    public SubjectDescriptor GetDescriptorByStorageKey(
      IVssRequestContext requestContext,
      Guid storageKey)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(storageKey, nameof (storageKey));
      requestContext.TraceDataConditionally(15270431, TraceLevel.Verbose, "Graph", nameof (GraphIdentifierConversionServiceBase), "Received input parameters", (Func<object>) (() => (object) new
      {
        storageKey = storageKey
      }), nameof (GetDescriptorByStorageKey));
      requestContext.CheckServiceHostId(this.ServiceHostId, (IVssFrameworkService) this);
      SubjectDescriptor subjectDescriptor;
      if (Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsInternalServicePrincipalId(storageKey))
        subjectDescriptor = this.GetServicePrincipalDescriptorByStorageKey(requestContext, storageKey);
      else if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          storageKey
        }, QueryMembership.None, (IEnumerable<string>) null).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
        subjectDescriptor = identity != null ? identity.SubjectDescriptor : new SubjectDescriptor();
      }
      else
        subjectDescriptor = this.GetDescriptorByStorageKeyFromRemote(requestContext, storageKey);
      requestContext.TraceDataConditionally(15270439, TraceLevel.Verbose, "Graph", nameof (GraphIdentifierConversionServiceBase), "Returning descriptor", (Func<object>) (() => (object) new
      {
        storageKey = storageKey,
        subjectDescriptor = subjectDescriptor
      }), nameof (GetDescriptorByStorageKey));
      return subjectDescriptor;
    }

    public IReadOnlyDictionary<SubjectDescriptor, Guid> GetStorageKeysByDescriptors(
      IVssRequestContext requestContext,
      IEnumerable<SubjectDescriptor> descriptors)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<SubjectDescriptor>>(descriptors, nameof (descriptors));
      requestContext.CheckServiceHostId(this.ServiceHostId, (IVssFrameworkService) this);
      requestContext.TraceDataConditionally(15260441, TraceLevel.Verbose, "Graph", nameof (GraphIdentifierConversionServiceBase), "Received input parameters", (Func<object>) (() => (object) new
      {
        descriptors = descriptors
      }), nameof (GetStorageKeysByDescriptors));
      Dictionary<SubjectDescriptor, Guid> descriptorToStorageKeyMap = new Dictionary<SubjectDescriptor, Guid>();
      foreach (SubjectDescriptor descriptor in descriptors)
        descriptorToStorageKeyMap.Add(descriptor, this.GetStorageKeyByDescriptor(requestContext, descriptor));
      requestContext.TraceDataConditionally(15260437, TraceLevel.Verbose, "Graph", nameof (GraphIdentifierConversionServiceBase), "Returning descriptor to storage keys map", (Func<object>) (() => (object) new
      {
        descriptorToStorageKeyMap = descriptorToStorageKeyMap
      }), nameof (GetStorageKeysByDescriptors));
      return (IReadOnlyDictionary<SubjectDescriptor, Guid>) descriptorToStorageKeyMap;
    }

    public IReadOnlyDictionary<Guid, SubjectDescriptor> GetDescriptorsByStorageKeys(
      IVssRequestContext requestContext,
      IEnumerable<Guid> storageKeys)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(storageKeys, nameof (storageKeys));
      requestContext.CheckServiceHostId(this.ServiceHostId, (IVssFrameworkService) this);
      requestContext.TraceDataConditionally(15260442, TraceLevel.Verbose, "Graph", nameof (GraphIdentifierConversionServiceBase), "Received input parameters", (Func<object>) (() => (object) new
      {
        storageKeys = storageKeys
      }), nameof (GetDescriptorsByStorageKeys));
      Dictionary<Guid, SubjectDescriptor> storageKeyToDescriptorMap = new Dictionary<Guid, SubjectDescriptor>();
      foreach (Guid storageKey in storageKeys)
        storageKeyToDescriptorMap.Add(storageKey, this.GetDescriptorByStorageKey(requestContext, storageKey));
      requestContext.TraceDataConditionally(15260443, TraceLevel.Verbose, "Graph", nameof (GraphIdentifierConversionServiceBase), "Returning storage key to descriptors map", (Func<object>) (() => (object) new
      {
        storageKeyToDescriptorMap = storageKeyToDescriptorMap
      }), nameof (GetDescriptorsByStorageKeys));
      return (IReadOnlyDictionary<Guid, SubjectDescriptor>) storageKeyToDescriptorMap;
    }

    public SubjectDescriptor GetCuidBasedDescriptorByLegacyDescriptor(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (identityDescriptor == (IdentityDescriptor) null)
        return new SubjectDescriptor();
      Microsoft.VisualStudio.Services.Identity.Identity identity = identityDescriptor.IsCuidBased() ? requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identityDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null)[0] : throw new ArgumentException(FrameworkResources.InvalidDescriptor((object) identityDescriptor), nameof (identityDescriptor));
      if (identity != null)
        return new SubjectDescriptor(identity.GetSubjectTypeForCuidBasedIdentity(requestContext), identity.Cuid().ToString("D"));
      requestContext.Trace(80656, TraceLevel.Warning, "Graph", nameof (GraphIdentifierConversionServiceBase), "Cannot find identity by descriptor {0}, {1}", (object) identityDescriptor.IdentityType, (object) identityDescriptor.Identifier.GetHashCode());
      return new SubjectDescriptor();
    }

    public IdentityDescriptor GetLegacyDescriptorByCuidBasedDescriptor(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (subjectDescriptor == new SubjectDescriptor())
        return (IdentityDescriptor) null;
      if (!subjectDescriptor.IsCuidBased())
        throw new ArgumentException(FrameworkResources.InvalidDescriptor((object) subjectDescriptor), nameof (subjectDescriptor));
      if (requestContext.ServiceHost.DeploymentServiceHost.ServiceInstanceType == GraphIdentifierConversionServiceBase.s_UserService)
      {
        IUserPrivateAttributesService service = requestContext.GetService<IUserPrivateAttributesService>();
        if (subjectDescriptor.IsAadUserType())
        {
          IList<UserAttribute> source = service.QueryPrivateAttributes(requestContext, subjectDescriptor, WellKnownUserAttributeNames.ProviderContainerName + ".*");
          return new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", string.Format("{0}\\{1}", (object) source.Single<UserAttribute>((Func<UserAttribute, bool>) (a => StringComparer.OrdinalIgnoreCase.Equals(a.Name, WellKnownUserAttributeNames.TenantId))).Value, (object) source.Single<UserAttribute>((Func<UserAttribute, bool>) (a => StringComparer.OrdinalIgnoreCase.Equals(a.Name, WellKnownUserAttributeNames.UserPrincipalName))).Value));
        }
        if (subjectDescriptor.IsMsaUserType())
          return new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", service.GetPrivateAttribute(requestContext, subjectDescriptor, WellKnownUserAttributeNames.Puid).Value + "@Live.com");
        if (subjectDescriptor.IsAadServicePrincipalType())
        {
          IList<UserAttribute> source = service.QueryPrivateAttributes(requestContext, subjectDescriptor, WellKnownUserAttributeNames.ProviderContainerName + ".*");
          return IdentityHelper.CreateAadServicePrincipalDescriptor(source.Single<UserAttribute>((Func<UserAttribute, bool>) (a => StringComparer.OrdinalIgnoreCase.Equals(a.Name, WellKnownUserAttributeNames.TenantId))).Value, source.Single<UserAttribute>((Func<UserAttribute, bool>) (a => StringComparer.OrdinalIgnoreCase.Equals(a.Name, WellKnownUserAttributeNames.ObjectId))).Value);
        }
      }
      Guid cuid = subjectDescriptor.GetCuid();
      IVssRequestContext vssRequestContext1 = requestContext.Elevate();
      IVssRequestContext vssRequestContext2 = vssRequestContext1.To(TeamFoundationHostType.Application);
      Guid organizationStorageKey = vssRequestContext2.GetService<IGraphIdentifierConversionService>().GetStorageKeyByDescriptor(vssRequestContext2, subjectDescriptor);
      if (organizationStorageKey == new Guid())
      {
        requestContext.TraceDataConditionally(1030459, TraceLevel.Info, "Graph", nameof (GraphIdentifierConversionServiceBase), "Organization storage key not found", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor,
          cuid = cuid
        }), nameof (GetLegacyDescriptorByCuidBasedDescriptor));
        return (IdentityDescriptor) null;
      }
      requestContext.TraceDataConditionally(1030459, TraceLevel.Info, "Graph", nameof (GraphIdentifierConversionServiceBase), "Found storage key for descriptor: ", (Func<object>) (() => (object) new
      {
        subjectDescriptor = subjectDescriptor,
        cuid = cuid,
        organizationStorageKey = organizationStorageKey
      }), nameof (GetLegacyDescriptorByCuidBasedDescriptor));
      return vssRequestContext1.GetService<IIdentityIdentifierConversionService>().GetDescriptorByMasterId(vssRequestContext1, organizationStorageKey);
    }

    public SubjectDescriptor GetDescriptorByProviderInfo(
      IVssRequestContext context,
      string domain,
      string originId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      context.CheckHostedDeployment();
      ArgumentUtility.CheckStringForNullOrWhiteSpace(domain, nameof (domain));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(originId, nameof (originId));
      string empty1 = string.Empty;
      Guid empty2 = Guid.Empty;
      string subjectType;
      Guid cuid;
      if (domain.Equals("Windows Live ID", StringComparison.OrdinalIgnoreCase))
      {
        subjectType = "msa";
        cuid = IdentityCuidHelper.ComputeCuid(context, Guid.Empty, originId);
      }
      else
      {
        Guid result;
        if (!Guid.TryParse(domain, out result))
          throw new ArgumentException("Found unsupported domain: " + domain + ". Only Windows Live ID or GUID type AAD domain are supported.");
        subjectType = "aad";
        cuid = IdentityCuidHelper.ComputeCuid(context, result, originId);
      }
      return !(cuid == Guid.Empty) ? new SubjectDescriptor(subjectType, cuid.ToString()) : throw new ArgumentException("Failed to generate CUID for domain: " + domain + ", originId: " + originId + ".");
    }

    public SubjectDescriptor GetDescriptorForCspPartnerByProviderInfo(
      IVssRequestContext context,
      string domain,
      string originId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      context.CheckHostedDeployment();
      ArgumentUtility.CheckStringForNullOrWhiteSpace(domain, nameof (domain));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(originId, nameof (originId));
      Guid empty = Guid.Empty;
      Guid result;
      if (!Guid.TryParse(domain, out result))
        throw new ArgumentException("Found unsupported domain: " + domain + ". Only GUID type AAD domain is supported.");
      Guid cuidForCspPartner = IdentityCuidHelper.ComputeCuidForCspPartner(result, originId);
      return !(cuidForCspPartner == Guid.Empty) ? new SubjectDescriptor("csp", cuidForCspPartner.ToString()) : throw new ArgumentException("Failed to generate CUID for domain: " + domain + ", puid: " + originId + ".");
    }

    private SubjectDescriptor GetServicePrincipalDescriptorByStorageKey(
      IVssRequestContext requestContext,
      Guid storageKey)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      Microsoft.VisualStudio.Services.Identity.Identity identity = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<Guid>) new Guid[1]
      {
        storageKey
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
      requestContext.TraceDataConditionally(15270438, TraceLevel.Verbose, "Graph", nameof (GraphIdentifierConversionServiceBase), "Received service principal identity from IMS", (Func<object>) (() => (object) new
      {
        Id = identity?.Id
      }), nameof (GetServicePrincipalDescriptorByStorageKey));
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = identity;
      return identity1 == null ? new SubjectDescriptor() : identity1.SubjectDescriptor;
    }

    protected abstract Guid GetStorageKeyByGroupScopeDescriptor(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor);

    protected virtual Guid GetStorageKeyByCuidBasedDescriptor(
      IVssRequestContext organizationContext,
      SubjectDescriptor descriptor)
    {
      try
      {
        organizationContext.TraceEnter(10007070, "Graph", nameof (GraphIdentifierConversionServiceBase), nameof (GetStorageKeyByCuidBasedDescriptor));
        organizationContext.CheckOrganizationOnlyRequestContext();
        IdentityKeyMap keyMap = descriptor.IsCuidBased() ? this.GetKeyMapByCuidBasedDescriptor(organizationContext, descriptor) : throw new ArgumentException(FrameworkResources.InvalidDescriptor((object) descriptor), nameof (descriptor));
        if (keyMap.IsValid())
        {
          organizationContext.TraceDataConditionally(10007072, TraceLevel.Info, "Graph", nameof (GraphIdentifierConversionServiceBase), "Received key map from remote", (Func<object>) (() => (object) new
          {
            descriptor = descriptor,
            keyMap = keyMap
          }), nameof (GetStorageKeyByCuidBasedDescriptor));
          return this.respectSubjectTypeConfig.QueryByCtx<bool>(organizationContext) && !keyMap.SubjectType.Equals(descriptor.SubjectType) ? new Guid() : keyMap.StorageKey;
        }
        organizationContext.TraceDataConditionally(15270449, TraceLevel.Error, "Graph", nameof (GraphIdentifierConversionServiceBase), "Did not receive key map from remote", (Func<object>) (() => (object) new
        {
          descriptor = descriptor
        }), nameof (GetStorageKeyByCuidBasedDescriptor));
        return new Guid();
      }
      finally
      {
        organizationContext.TraceLeave(10007071, "Graph", nameof (GraphIdentifierConversionServiceBase), nameof (GetStorageKeyByCuidBasedDescriptor));
      }
    }

    protected abstract IdentityKeyMap GetKeyMapByCuidBasedDescriptor(
      IVssRequestContext organizationContext,
      SubjectDescriptor subjectDescriptor);

    protected abstract SubjectDescriptor GetDescriptorByStorageKeyFromRemote(
      IVssRequestContext collectionOrOrganizationContext,
      Guid storageKey);

    protected bool IsExchangableId(Guid id) => id != Guid.Empty && !Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsInternalServicePrincipalId(id);

    protected Guid ServiceHostId { get; set; }
  }
}
