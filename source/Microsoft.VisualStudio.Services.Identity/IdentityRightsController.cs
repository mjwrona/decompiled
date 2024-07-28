// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityRightsController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DirectoryService;
using Microsoft.VisualStudio.Services.Identity.Internal;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(0.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "Rights")]
  [ClientInternalUseOnly(true)]
  public class IdentityRightsController : IdentitiesControllerBase
  {
    private static readonly Dictionary<System.Type, HttpStatusCode> s_httpExceptions = new Dictionary<System.Type, HttpStatusCode>()
    {
      {
        typeof (IdentityNotFoundException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidTransferIdentityRightsRequestException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MultipleIdentitiesFoundException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (FailedTransferIdentityRightsException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (IdentityMaterializationFailedException),
        HttpStatusCode.InternalServerError
      }
    };
    private const string TransferIdentityRightsBatchFeatureFlag = "VisualStudio.Services.Identity.TransferIdentityRightsBatch.RESTAPI";
    private const string DisableEnforcingIdentityDomainBatchMatchingDomain = "VisualStudio.Services.Identity.TransferIdentityRightsBatch.DisableDomainBatchMatching";
    private static readonly char[] s_invalidUPNCharacters = new char[8]
    {
      ' ',
      '\t',
      '\r',
      '\n',
      ']',
      '[',
      ',',
      '\\'
    };
    private static readonly string s_IdentityIdTranslationExceptionErrorMessage = "An error occurred while updating the mapping from {0} to {1}. All the previous rights transfers succeeded. The following translations are disallowed: 1. Cycles (A-B, B-A), 2. Multi-step translations (A-B, B-C or A-B, C-A), 3. Same target identity with different source identities (A-B, C-B or A-B, B-B or A-A, B-A)";
    private static readonly string s_UnexpectedExceptionDuringTransferRightsErrorMessage = "An unexpected error occurred while transferring rights from {0} to {1}. All the previous rights transfers succeeded. Exception message: {2}";
    private static readonly IReadOnlyDictionary<string, object> CommonEntityDescriptorProperties = (IReadOnlyDictionary<string, object>) new Dictionary<string, object>()
    {
      {
        "InvitationMethod",
        (object) nameof (IdentityRightsController)
      }
    };

    [HttpPost]
    [ClientIgnore]
    public HttpResponseMessage TransferIdentityRights(Guid fromIdentity, IdentityIdType toIdentity)
    {
      ArgumentUtility.CheckForNull<IdentityIdType>(toIdentity, nameof (toIdentity));
      if (toIdentity.ToIdentity == Guid.Empty)
      {
        this.CheckPermissions();
        ArgumentUtility.CheckForEmptyGuid(toIdentity.SourceMasterId, "SourceMasterId");
        ArgumentUtility.CheckForEmptyGuid(toIdentity.TargetMasterId, "TargetMasterId");
        IVssRequestContext context = this.TfsRequestContext.To(TeamFoundationHostType.ProjectCollection);
        ITransferIdentityRightsService service = context.GetService<ITransferIdentityRightsService>();
        IdentityRightsTransfer identityRightsTransfer = new IdentityRightsTransfer()
        {
          SourceId = fromIdentity,
          SourceMasterId = toIdentity.SourceMasterId,
          TargetMasterId = toIdentity.TargetMasterId
        };
        IVssRequestContext accountContext = context;
        IdentityRightsTransfer[] identityRightsTransfers = new IdentityRightsTransfer[1]
        {
          identityRightsTransfer
        };
        service.TransferIdentityRights(accountContext, (IList<IdentityRightsTransfer>) identityRightsTransfers);
        return this.Request.CreateResponse<HttpStatusCode>(HttpStatusCode.NoContent);
      }
      ArgumentUtility.CheckForEmptyGuid(toIdentity.ToIdentity, "ToIdentity");
      Guid toIdentity1 = toIdentity.ToIdentity;
      Guid guid = fromIdentity;
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      this.CheckPermissions();
      PlatformIdentityService service1 = vssRequestContext.GetService<PlatformIdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = service1.ReadIdentities(vssRequestContext, (IList<Guid>) new List<Guid>()
      {
        guid
      }, QueryMembership.None, (IEnumerable<string>) null, false).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = service1.ReadIdentities(vssRequestContext, (IList<Guid>) new List<Guid>()
      {
        toIdentity1
      }, QueryMembership.None, (IEnumerable<string>) null, false).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (IdentityHelper.IsServiceIdentity(vssRequestContext, (IReadOnlyVssIdentity) identity1) || IdentityHelper.IsServiceIdentity(vssRequestContext, (IReadOnlyVssIdentity) identity2))
        throw new InvalidTransferIdentityRightsRequestException("Transfering rights from/to service identity is not allowed");
      IdentityRightsController.ValidateDomain(identity1, identity2);
      this.EnsureNoExistingMembershipsInCollection(identity2);
      this.EnsureIdentityExistsAndResolve(identity2.GetProperty<string>("Account", (string) null));
      IdentityRightsTransferHelper.TransferIdentityRights(this.TfsRequestContext.Elevate(), identity1, identity2);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        RequestMessage = this.Request
      };
    }

    [HttpPost]
    [ClientLocationId("908b4edc-4c6a-41e8-88ed-07a1f01a9a59")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage TransferIdentityRightsBatch(
      IdentityRightsTransferData identityRightsTransferData)
    {
      this.ValidateRequestContext(this.TfsRequestContext);
      this.CheckPermissions();
      this.ValidateFeatureEnabled(this.TfsRequestContext);
      string domain;
      this.ValidateAndExtractBackingDomain(this.TfsRequestContext, out domain);
      this.ValidateIdentityRightsTransferDataIsWellFormed(this.TfsRequestContext, identityRightsTransferData, domain);
      foreach (KeyValuePair<string, string> principalNameMapping in identityRightsTransferData.UserPrincipalNameMappings)
      {
        Microsoft.VisualStudio.Services.Identity.Identity fromIdentity = this.ResolveExistingIdentity(principalNameMapping.Key, domain);
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.EnsureIdentityExistsAndResolve(principalNameMapping.Value);
        if (!this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.TransferIdentityRightsBatch.DisableDomainBatchMatching"))
          IdentityRightsController.ValidateDomain(fromIdentity, identity);
        try
        {
          this.EnsureNoExistingMembershipsInCollection(identity);
          IdentityRightsTransferHelper.TransferIdentityRights(this.TfsRequestContext.Elevate(), fromIdentity, identity);
        }
        catch (InvalidIdentityIdTranslationException ex)
        {
          throw new InvalidTransferIdentityRightsRequestException(string.Format(IdentityRightsController.s_IdentityIdTranslationExceptionErrorMessage, (object) principalNameMapping.Key, (object) principalNameMapping.Value), (Exception) ex);
        }
        catch (Exception ex)
        {
          throw new FailedTransferIdentityRightsException(string.Format(IdentityRightsController.s_UnexpectedExceptionDuringTransferRightsErrorMessage, (object) principalNameMapping.Key, (object) principalNameMapping.Value, (object) ex.Message), ex);
        }
      }
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        RequestMessage = this.Request
      };
    }

    private void EnsureNoExistingMembershipsInCollection(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (this.TfsRequestContext.GetService<IdentityService>().ReadIdentities(this.TfsRequestContext, (IList<Guid>) new List<Guid>()
      {
        identity.MasterId
      }, QueryMembership.None, (IEnumerable<string>) null)[0] != null)
        throw new InvalidTransferIdentityRightsRequestException(string.Format("Identity with VSID {0} has active or inactive memberships in the account", (object) identity.MasterId));
    }

    private Microsoft.VisualStudio.Services.Identity.Identity ResolveExistingIdentity(
      string userPrincipalName,
      string domain)
    {
      IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity> fromAccountByUpn = this.GetIdentitiesFromAccountByUPN(userPrincipalName, domain);
      if (fromAccountByUpn == null || fromAccountByUpn.Count == 0)
        throw new IdentityNotFoundException("Could not find the identity " + userPrincipalName + " in IMS");
      Microsoft.VisualStudio.Services.Identity.Identity identity = fromAccountByUpn.Count <= 1 ? fromAccountByUpn.Single<Microsoft.VisualStudio.Services.Identity.Identity>() : throw new MultipleIdentitiesFoundException(userPrincipalName, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) fromAccountByUpn);
      if (identity == null)
        throw new IdentityNotFoundException("Could not find the identity " + userPrincipalName + " in IMS");
      return !IdentityHelper.IsServiceIdentity(this.TfsRequestContext.To(TeamFoundationHostType.Deployment), (IReadOnlyVssIdentity) identity) ? identity : throw new InvalidTransferIdentityRightsRequestException("Transfering rights from service identity " + userPrincipalName + " is not allowed");
    }

    private IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentitiesFromAccountByUPN(
      string userPrincipalName,
      string domain)
    {
      string filterValue = string.Format("{0}\\{1}", (object) domain, (object) userPrincipalName);
      return this.TfsRequestContext.GetService<IdentitySearchService>().FindActiveOrHistoricalMembers(this.TfsRequestContext, IdentitySearchFilter.AccountName, filterValue, (string) null);
    }

    private Microsoft.VisualStudio.Services.Identity.Identity EnsureIdentityExistsAndResolve(
      string userPrincipalName)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      IdentityDirectoryEntityResult<Microsoft.VisualStudio.Services.Identity.Identity> directoryEntityResult = vssRequestContext.GetService<IDirectoryService>().IncludeIdentities(vssRequestContext).AddMember(vssRequestContext, (IDirectoryEntityDescriptor) new DirectoryEntityDescriptor(principalName: userPrincipalName, properties: IdentityRightsController.CommonEntityDescriptorProperties), license: "None", propertiesToReturn: (IEnumerable<string>) new string[1]
      {
        "LocalDescriptor"
      });
      if (directoryEntityResult.Identity == null)
      {
        if (directoryEntityResult.Status == "NoResults")
          throw new InvalidTransferIdentityRightsRequestException("Could not obtain an identity across any backing directories for user principal name: " + userPrincipalName);
        throw new IdentityMaterializationFailedException(string.Format("Unexpected exception while trying to materialize user principal name {0}." + Environment.NewLine + "Obtained status: {1}." + Environment.NewLine + "Exception message: {2}", (object) userPrincipalName, (object) directoryEntityResult.Status, (object) directoryEntityResult.Exception?.Message), directoryEntityResult.Exception);
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity = directoryEntityResult.Identity;
      return !IdentityHelper.IsServiceIdentity(this.TfsRequestContext.To(TeamFoundationHostType.Deployment), (IReadOnlyVssIdentity) identity) ? identity : throw new InvalidTransferIdentityRightsRequestException("Transfering rights to service identity " + userPrincipalName + " is not allowed");
    }

    private void ValidateAndExtractBackingDomain(
      IVssRequestContext requestContext,
      out string domain)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      Guid organizationAadTenantId = vssRequestContext.GetOrganizationAadTenantId();
      if (organizationAadTenantId == Guid.Empty && vssRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.TransferIdentityRightsBatch.NotSupportBatchTransferInMsa"))
        throw new InvalidTransferIdentityRightsRequestException("Bulk transfer rights is only supported on AAD-backed accounts. The account " + requestContext.ServiceHost.Name + " is not AAD-backed");
      domain = organizationAadTenantId != Guid.Empty ? organizationAadTenantId.ToString() : "Windows Live ID";
    }

    private void ValidateFeatureEnabled(IVssRequestContext requestContext)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.TransferIdentityRightsBatch.RESTAPI"))
        throw new InvalidTransferIdentityRightsRequestException("Bulk transfer rights is temporarily disabled");
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidTransferIdentityRightsRequestException("Bulk transfer rights is only supported in the hosted environment");
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new InvalidTransferIdentityRightsRequestException(string.Format("Bulk transfer rights expects a project collection host, but found {0}", (object) requestContext.ServiceHost.HostType));
    }

    private void ValidateIdentityRightsTransferDataIsWellFormed(
      IVssRequestContext requestContext,
      IdentityRightsTransferData identityRightsTransferData,
      string domain)
    {
      try
      {
        ArgumentUtility.CheckForNull<IdentityRightsTransferData>(identityRightsTransferData, nameof (identityRightsTransferData));
        Dictionary<string, string> principalNameMappings = identityRightsTransferData.UserPrincipalNameMappings;
        ArgumentUtility.CheckForNull<Dictionary<string, string>>(principalNameMappings, "userPrincipalNameMappings");
        HashSet<string> allUPNs = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (KeyValuePair<string, string> keyValuePair in principalNameMappings)
        {
          this.ValidateUserPrincipalNameString(keyValuePair.Key, allUPNs);
          allUPNs.Add(keyValuePair.Key);
          this.ResolveExistingIdentity(keyValuePair.Key, domain);
          this.ValidateUserPrincipalNameString(keyValuePair.Value, allUPNs);
          allUPNs.Add(keyValuePair.Value);
        }
      }
      catch (IdentityNotFoundException ex)
      {
        throw;
      }
      catch (MultipleIdentitiesFoundException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new InvalidTransferIdentityRightsRequestException(ex.Message);
      }
    }

    private void ValidateUserPrincipalNameString(string userPrincipalName, HashSet<string> allUPNs)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(userPrincipalName, nameof (userPrincipalName));
      ArgumentUtility.CheckStringForAnyWhiteSpace(userPrincipalName, nameof (userPrincipalName));
      ArgumentUtility.CheckStringForInvalidCharacters(userPrincipalName, nameof (userPrincipalName), IdentityRightsController.s_invalidUPNCharacters);
      if (Guid.TryParse(userPrincipalName, out Guid _))
        throw new InvalidTransferIdentityRightsRequestException("Expected a user principal name, but received a Guid: " + userPrincipalName);
      if (!userPrincipalName.Contains<char>('@'))
        throw new InvalidTransferIdentityRightsRequestException("User principal name should contain the @ character: " + userPrincipalName);
      if (allUPNs.Contains(userPrincipalName))
        throw new InvalidTransferIdentityRightsRequestException("Found multiple occurrences of user principal name in the mapping file: " + userPrincipalName);
    }

    private static void ValidateDomain(Microsoft.VisualStudio.Services.Identity.Identity fromIdentity, Microsoft.VisualStudio.Services.Identity.Identity toIdentity)
    {
      if (!string.Equals(fromIdentity.GetProperty<string>("Domain", (string) null), toIdentity.GetProperty<string>("Domain", (string) null), StringComparison.OrdinalIgnoreCase))
        throw new InvalidTransferIdentityRightsRequestException("Domains of the two identities should match");
    }

    private void CheckPermissions()
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false);
      if (!ServicePrincipals.IsServicePrincipal(this.TfsRequestContext, this.TfsRequestContext.GetAuthenticatedDescriptor()))
        throw new UnauthorizedAccessException("Calling identity must be a service principal.");
    }

    public override string ActivityLogArea => "Identities";

    public override string TraceArea => "Identities";
  }
}
