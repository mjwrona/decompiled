// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.AadGraphClient
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Cloud.Aad.Throttling;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class AadGraphClient : IAadGraphClient
  {
    private readonly GraphConnectionFactory connectionFactory;
    private const string RetryAfterResponseHeaderKey = "Retry-After";
    internal const string EnableAadGraphFallbackOnThrottling = "VisualStudio.Services.Aad.EnableAadGraphFallbackOnThrottling";
    private static readonly CommandPropertiesSetter commandPropertiesSetterDefault = (CommandPropertiesSetter) new AadGraphClient.CommandPropertiesSetterDefault();
    private static readonly CommandPropertiesSetter commandPropertiesSetterWithShortExecutionTimeout = (CommandPropertiesSetter) new AadGraphClient.CommandPropertiesSetterShortExecutionTimeout();
    public static Dictionary<string, CommandPropertiesSetter> CommandPropertiesSetterMap = new Dictionary<string, CommandPropertiesSetter>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "GetAncestorIdsRequest",
        AadGraphClient.commandPropertiesSetterDefault
      },
      {
        "GetAncestorsRequest",
        AadGraphClient.commandPropertiesSetterDefault
      },
      {
        "GetDescendantIdsRequest",
        AadGraphClient.commandPropertiesSetterDefault
      },
      {
        "GetDescendantsRequest",
        AadGraphClient.commandPropertiesSetterDefault
      },
      {
        "GetDirectoryRolesRequest",
        AadGraphClient.commandPropertiesSetterDefault
      },
      {
        "GetDirectoryRolesWithIdsRequest",
        AadGraphClient.commandPropertiesSetterWithShortExecutionTimeout
      },
      {
        "GetGroupsRequest",
        AadGraphClient.commandPropertiesSetterWithShortExecutionTimeout
      },
      {
        "GetGroupsWithIdsRequest",
        AadGraphClient.commandPropertiesSetterWithShortExecutionTimeout
      },
      {
        "GetTenantRequest",
        AadGraphClient.commandPropertiesSetterDefault
      },
      {
        "GetTenantsByAltSecIdRequest",
        AadGraphClient.commandPropertiesSetterWithShortExecutionTimeout
      },
      {
        "GetTenantsByKeyRequest",
        AadGraphClient.commandPropertiesSetterDefault
      },
      {
        "GetUsersRequest",
        AadGraphClient.commandPropertiesSetterWithShortExecutionTimeout
      },
      {
        "GetUsersWithIdsRequest",
        AadGraphClient.commandPropertiesSetterDefault
      },
      {
        "GetUserThumbnailRequest",
        AadGraphClient.commandPropertiesSetterWithShortExecutionTimeout
      }
    };

    public AadGraphClient()
      : this((GraphConnectionFactory) null)
    {
    }

    public AadGraphClient(GraphConnectionFactory connectionFactory = null) => this.connectionFactory = connectionFactory ?? new GraphConnectionFactory();

    public GetAncestorIdsResponse GetAncestorIds(
      IVssRequestContext context,
      GetAncestorIdsRequest request)
    {
      return this.ProcessRequest<GetAncestorIdsResponse>(context, (AadGraphClientRequest<GetAncestorIdsResponse>) request, AadGraphClientCounters.GetAncestorIds, nameof (GetAncestorIds));
    }

    public GetAncestorsResponse GetAncestors(
      IVssRequestContext context,
      GetAncestorsRequest request)
    {
      return this.ProcessRequest<GetAncestorsResponse>(context, (AadGraphClientRequest<GetAncestorsResponse>) request, AadGraphClientCounters.GetAncestors, nameof (GetAncestors));
    }

    public GetDescendantIdsResponse GetDescendantIds(
      IVssRequestContext context,
      GetDescendantIdsRequest request)
    {
      return this.ProcessRequest<GetDescendantIdsResponse>(context, (AadGraphClientRequest<GetDescendantIdsResponse>) request, operation: nameof (GetDescendantIds));
    }

    public GetDescendantsResponse GetDescendants(
      IVssRequestContext context,
      GetDescendantsRequest request)
    {
      return this.ProcessRequest<GetDescendantsResponse>(context, (AadGraphClientRequest<GetDescendantsResponse>) request, AadGraphClientCounters.GetDescendants, nameof (GetDescendants));
    }

    public GetDirectoryRolesResponse GetDirectoryRoles(
      IVssRequestContext context,
      GetDirectoryRolesRequest request)
    {
      return this.ProcessRequest<GetDirectoryRolesResponse>(context, (AadGraphClientRequest<GetDirectoryRolesResponse>) request, operation: nameof (GetDirectoryRoles));
    }

    public GetDirectoryRoleMembersResponse GetDirectoryRoleMembers(
      IVssRequestContext context,
      GetDirectoryRoleMembersRequest request)
    {
      return this.ProcessRequest<GetDirectoryRoleMembersResponse>(context, (AadGraphClientRequest<GetDirectoryRoleMembersResponse>) request, operation: nameof (GetDirectoryRoleMembers));
    }

    public GetDirectoryRolesWithIdsResponse GetDirectoryRolesWithIds(
      IVssRequestContext context,
      GetDirectoryRolesWithIdsRequest request)
    {
      return this.ProcessRequest<GetDirectoryRolesWithIdsResponse>(context, (AadGraphClientRequest<GetDirectoryRolesWithIdsResponse>) request, operation: nameof (GetDirectoryRolesWithIds));
    }

    public GetGroupsResponse GetGroups(IVssRequestContext context, GetGroupsRequest request) => this.ProcessRequest<GetGroupsResponse>(context, (AadGraphClientRequest<GetGroupsResponse>) request, AadGraphClientCounters.GetGroups, nameof (GetGroups));

    public GetGroupsWithIdsResponse GetGroupsWithIds(
      IVssRequestContext context,
      GetGroupsWithIdsRequest request)
    {
      return this.ProcessRequest<GetGroupsWithIdsResponse>(context, (AadGraphClientRequest<GetGroupsWithIdsResponse>) request, AadGraphClientCounters.GetGroupsWithIds, nameof (GetGroupsWithIds));
    }

    public GetSoftDeletedObjectsResponse<T> GetSoftDeletedObjectsWithIds<T>(
      IVssRequestContext context,
      GetSoftDeletedObjectsRequest<T> request)
      where T : AadObject
    {
      return this.ProcessRequest<GetSoftDeletedObjectsResponse<T>>(context, (AadGraphClientRequest<GetSoftDeletedObjectsResponse<T>>) request, operation: nameof (GetSoftDeletedObjectsWithIds));
    }

    public GetTenantResponse GetTenant(IVssRequestContext context, GetTenantRequest request) => this.ProcessRequest<GetTenantResponse>(context, (AadGraphClientRequest<GetTenantResponse>) request, AadGraphClientCounters.GetTenant, nameof (GetTenant));

    public GetTenantsByAltSecIdResponse GetTenantsByAltSecId(
      IVssRequestContext context,
      GetTenantsByAltSecIdRequest request)
    {
      return this.ProcessRequest<GetTenantsByAltSecIdResponse>(context, (AadGraphClientRequest<GetTenantsByAltSecIdResponse>) request, AadGraphClientCounters.GetTenantsByAltSecId, nameof (GetTenantsByAltSecId));
    }

    public GetTenantsByKeyResponse GetTenantsByKey(
      IVssRequestContext context,
      GetTenantsByKeyRequest request)
    {
      return this.ProcessRequest<GetTenantsByKeyResponse>(context, (AadGraphClientRequest<GetTenantsByKeyResponse>) request, AadGraphClientCounters.GetTenantsByKey, nameof (GetTenantsByKey));
    }

    public GetUsersResponse GetUsers(IVssRequestContext context, GetUsersRequest request) => this.ProcessRequest<GetUsersResponse>(context, (AadGraphClientRequest<GetUsersResponse>) request, AadGraphClientCounters.GetUsers, nameof (GetUsers));

    public GetUsersWithIdsResponse GetUsersWithIds(
      IVssRequestContext context,
      GetUsersWithIdsRequest request)
    {
      return this.ProcessRequest<GetUsersWithIdsResponse>(context, (AadGraphClientRequest<GetUsersWithIdsResponse>) request, AadGraphClientCounters.GetUsersWithIds, nameof (GetUsersWithIds));
    }

    public GetUserThumbnailResponse GetUserThumbnail(
      IVssRequestContext context,
      GetUserThumbnailRequest request)
    {
      return this.ProcessRequest<GetUserThumbnailResponse>(context, (AadGraphClientRequest<GetUserThumbnailResponse>) request, operation: nameof (GetUserThumbnail));
    }

    public GetUserRolesAndGroupsResponse GetUserRolesAndGroups(
      IVssRequestContext context,
      GetUserRolesAndGroupsRequest request)
    {
      return this.ProcessRequest<GetUserRolesAndGroupsResponse>(context, (AadGraphClientRequest<GetUserRolesAndGroupsResponse>) request, operation: nameof (GetUserRolesAndGroups));
    }

    private T ProcessRequest<T>(
      IVssRequestContext context,
      AadGraphClientRequest<T> request,
      IAadPerfCounter counter = null,
      [CallerMemberName] string operation = "")
      where T : AadGraphClientResponse
    {
      try
      {
        context.TraceEnter(44744701, "VisualStudio.Services.Aad", "Graph", operation);
        using (AadServiceMetric aadServiceMetric = new AadServiceMetric(context, operation))
        {
          AadGraphClientCounters.All.IncrementEntryCounters();
          counter?.IncrementEntryCounters();
          try
          {
            request.Validate();
          }
          catch (Exception ex)
          {
            aadServiceMetric.Result = AadServiceMetric.ResultValue.Error;
            AadGraphClientCounters.All.IncrementErrorCounters();
            counter?.IncrementErrorCounters();
            context.TraceException(44744702, "VisualStudio.Services.Aad", "Graph", ex);
            throw;
          }
          T obj;
          try
          {
            Guid organizationAadTenantId = context.GetOrganizationAadTenantId();
            using (PerformanceTimer.StartMeasure(context, "AadGraph"))
            {
              if (organizationAadTenantId != new Guid() && context.IsFeatureEnabled("VisualStudio.Services.Aad.EnableAadGraphFallbackOnThrottling"))
              {
                IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                obj = vssRequestContext.GetService<AadThrottlingService>().Execute<T>(vssRequestContext, organizationAadTenantId.ToString(), AadServiceType.AadGraph, (Func<T>) (() => this.ExecuteRequest<T>(context, request, operation)), AadGraphClient.\u003C\u003EO.\u003C0\u003E__IsAadRequestThrottled ?? (AadGraphClient.\u003C\u003EO.\u003C0\u003E__IsAadRequestThrottled = new Func<Exception, AadThrottleInfo>(AadGraphClient.IsAadRequestThrottled)));
              }
              else
                obj = this.ExecuteRequest<T>(context, request, operation);
            }
          }
          catch (AuthenticationException ex)
          {
            aadServiceMetric.Result = AadServiceMetric.ResultValue.Error;
            aadServiceMetric.Cause = AadServiceMetric.CauseValue.Authentication;
            AadGraphClientCounters.All.IncrementErrorCounters();
            counter?.IncrementErrorCounters();
            context.Trace(44744703, TraceLevel.Warning, "VisualStudio.Services.Aad", "Graph", "{0}", (object) request);
            throw new AadGraphAuthenticationException((Exception) ex);
          }
          catch (ObjectNotFoundException ex)
          {
            aadServiceMetric.Result = AadServiceMetric.ResultValue.Error;
            aadServiceMetric.Cause = AadServiceMetric.CauseValue.ObjectNotFound;
            AadGraphClientCounters.All.IncrementErrorCounters();
            counter?.IncrementErrorCounters();
            context.Trace(44744704, TraceLevel.Warning, "VisualStudio.Services.Aad", "Graph", "{0}", (object) request);
            throw new AadGraphObjectNotFoundException((Exception) ex);
          }
          catch (Exception ex)
          {
            aadServiceMetric.Result = AadServiceMetric.ResultValue.Failure;
            AadGraphClientCounters.All.IncrementFailureCounters();
            counter?.IncrementFailureCounters();
            context.TraceException(44744708, "VisualStudio.Services.Aad", "Graph", ex);
            throw ex is GraphException ? (Exception) new AadGraphException("AAD returned exception.", ex) : ex;
          }
          aadServiceMetric.Result = AadServiceMetric.ResultValue.Success;
          AadGraphClientCounters.All.IncrementSuccessCounters();
          counter?.IncrementSuccessCounters();
          return obj;
        }
      }
      finally
      {
        context.TraceLeave(44744709, "VisualStudio.Services.Aad", "Graph", operation);
      }
    }

    private T ExecuteRequest<T>(
      IVssRequestContext context,
      AadGraphClientRequest<T> request,
      string operation)
      where T : AadGraphClientResponse
    {
      CommandPropertiesSetter propertiesSetterDefault;
      AadGraphClient.CommandPropertiesSetterMap.TryGetValue(request.GetType().Name, out propertiesSetterDefault);
      if (propertiesSetterDefault == null)
        propertiesSetterDefault = AadGraphClient.commandPropertiesSetterDefault;
      return new CommandService<T>(context, CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) operation).AndCommandPropertiesDefaults(propertiesSetterDefault), (Func<T>) (() => this.ExecuteRequestInCircuitBreaker<T>(context, request, operation)), (Func<T>) (() =>
      {
        throw new AadServiceNotAvailableException("Aad service not responding to requests.");
      })).Execute();
    }

    private T ExecuteRequestInCircuitBreaker<T>(
      IVssRequestContext context,
      AadGraphClientRequest<T> request,
      string operation)
      where T : AadGraphClientResponse
    {
      try
      {
        return request.Execute(context, this.connectionFactory.CreateGraphConnection(request.AccessToken.RawData, context.ActivityId));
      }
      catch (FormatException ex)
      {
        if (context.IsTracing(44744705, TraceLevel.Info, "VisualStudio.Services.Aad", "Graph"))
        {
          string accessTokenClaims = AadGraphClient.GetAccessTokenClaims(request.AccessToken.RawData);
          if (accessTokenClaims != null)
            context.Trace(44744705, TraceLevel.Info, "VisualStudio.Services.Aad", "Graph", string.Join<int>(",", accessTokenClaims.Select<char, int>((Func<char, int>) (c => (int) c))));
        }
        ex.Data[(object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}"] = (object) true;
        throw;
      }
      catch (AuthenticationException ex)
      {
        ex.Data[(object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}"] = (object) true;
        throw;
      }
      catch (ObjectNotFoundException ex)
      {
        ex.Data[(object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}"] = (object) true;
        throw;
      }
    }

    private static string GetAccessTokenClaims(string accessToken)
    {
      if (string.IsNullOrEmpty(accessToken))
        return (string) null;
      string[] strArray = accessToken.Split(".".ToCharArray());
      return strArray.Length != 3 ? (string) null : strArray[1];
    }

    internal static AadTenant ConvertTenant(TenantDetail tenant)
    {
      if (tenant == null)
        return (AadTenant) null;
      return new AadTenant.Factory()
      {
        ObjectId = AadGraphClient.CreateGuid(tenant.ObjectId),
        DisplayName = tenant.DisplayName,
        VerifiedDomains = AadGraphClient.ConvertDomains((IEnumerable<VerifiedDomain>) tenant.VerifiedDomains),
        CountryLetterCode = tenant.CountryLetterCode,
        DirSyncEnabled = tenant.DirSyncEnabled,
        CompanyLastDirSyncTime = tenant.CompanyLastDirSyncTime
      }.Create();
    }

    internal static AadTenant ConvertGuestTenant(GuestTenantDetail tenant)
    {
      if (tenant == null)
        return (AadTenant) null;
      return new AadTenant.Factory()
      {
        ObjectId = AadGraphClient.CreateGuid(tenant.TenantId),
        DisplayName = tenant.DisplayName
      }.Create();
    }

    internal static IEnumerable<AadDomain> ConvertDomains(IEnumerable<VerifiedDomain> domains) => domains != null ? domains.Select<VerifiedDomain, AadDomain>((Func<VerifiedDomain, AadDomain>) (domain => new AadDomain.Factory()
    {
      Name = domain.Name,
      IsDefault = domain.Default.GetValueOrDefault()
    }.Create())) : (IEnumerable<AadDomain>) null;

    internal static AadUser ConvertUser(User user1, bool skipHasThumbnailPhoto = false)
    {
      if (user1 == null)
        return (AadUser) null;
      object obj;
      return new AadUser.Factory()
      {
        ObjectId = AadGraphClient.CreateGuid(user1.ObjectId),
        AccountEnabled = user1.AccountEnabled.GetValueOrDefault(false),
        DisplayName = user1.DisplayName,
        Mail = user1.Mail,
        OtherMails = ((IEnumerable<string>) user1.OtherMails),
        MailNickname = user1.MailNickname,
        UserPrincipalName = user1.UserPrincipalName,
        SignInAddress = AadGraphUtils.GetSignInAddressFromUpn(user1),
        HasThumbnailPhoto = (!skipHasThumbnailPhoto && user1.NonSerializedProperties.TryGetValue("thumbnailPhoto@odata.mediaContentType", out obj) && obj != null),
        JobTitle = user1.JobTitle,
        Department = user1.Department,
        PhysicalDeliveryOfficeName = user1.PhysicalDeliveryOfficeName,
        Manager = (user1.Manager != null ? AadGraphClient.ConvertUser(user1.Manager as User, skipHasThumbnailPhoto) : (AadUser) null),
        DirectReports = (user1.DirectReports != null ? user1.DirectReports.OfType<User>().Select<User, AadUser>((Func<User, AadUser>) (user2 => AadGraphClient.ConvertUser(user2, skipHasThumbnailPhoto))) : (IEnumerable<AadUser>) null),
        Surname = user1.Surname,
        UserType = user1.UserType,
        UserState = user1.UserState,
        OnPremisesSecurityIdentifier = user1.OnPremisesSecurityIdentifier,
        ImmutableId = user1.ImmutableId,
        TelephoneNumber = user1.TelephoneNumber,
        Country = user1.Country,
        UsageLocation = user1.UsageLocation
      }.Create();
    }

    internal static AadGroup ConvertGroup(Group group)
    {
      if (group == null)
        return (AadGroup) null;
      return new AadGroup.Factory()
      {
        ObjectId = AadGraphClient.CreateGuid(group.ObjectId),
        Description = group.Description,
        DisplayName = group.DisplayName,
        MailNickname = group.MailNickname,
        Mail = group.Mail,
        OnPremisesSecurityIdentifier = group.OnPremisesSecurityIdentifier
      }.Create();
    }

    internal static AadServicePrincipal ConvertServicePrincipal(ServicePrincipal servicePrincipal)
    {
      if (servicePrincipal == null)
        return (AadServicePrincipal) null;
      return new AadServicePrincipal.Factory()
      {
        ObjectId = AadGraphClient.CreateGuid(servicePrincipal.ObjectId),
        DisplayName = servicePrincipal.DisplayName,
        AppId = AadGraphClient.CreateGuid(servicePrincipal.AppId),
        AccountEnabled = (servicePrincipal.AccountEnabled.HasValue && servicePrincipal.AccountEnabled.Value)
      }.Create();
    }

    internal static AadDirectoryRole ConvertDirectoryRole(DirectoryRole directoryRole)
    {
      if (directoryRole == null)
        return (AadDirectoryRole) null;
      return new AadDirectoryRole.Factory()
      {
        ObjectId = AadGraphClient.CreateGuid(directoryRole.ObjectId),
        DisplayName = directoryRole.DisplayName,
        Description = directoryRole.Description,
        RoleTemplateId = directoryRole.RoleTemplateId
      }.Create();
    }

    internal static AadObject ConvertObject(GraphObject obj, bool skipHasThumbnailPhoto)
    {
      switch (obj)
      {
        case User user:
          return (AadObject) AadGraphClient.ConvertUser(user, skipHasThumbnailPhoto);
        case Group group:
          return (AadObject) AadGraphClient.ConvertGroup(group);
        case ServicePrincipal servicePrincipal:
          return (AadObject) AadGraphClient.ConvertServicePrincipal(servicePrincipal);
        default:
          return (AadObject) null;
      }
    }

    internal static Guid CreateGuid(string objectId)
    {
      Guid result;
      if (!Guid.TryParse(objectId, out result))
        throw new AadException(string.Format("Failed to parse Object ID: {0}", (object) objectId));
      return result;
    }

    internal static AadThrottleInfo IsAadRequestThrottled(Exception exception)
    {
      for (int index = 0; exception != null && index < 100; exception = exception.InnerException)
      {
        if (exception is GraphException graphException && graphException.HttpStatusCode == (HttpStatusCode) 429)
        {
          int result;
          if (int.TryParse(graphException.ResponseHeaders?.Get("Retry-After"), out result))
            return new AadThrottleInfo()
            {
              IsThrottled = true,
              RetryAfter = new TimeSpan?(TimeSpan.FromSeconds((double) result))
            };
          return new AadThrottleInfo()
          {
            IsThrottled = true
          };
        }
        ++index;
      }
      return new AadThrottleInfo() { IsThrottled = false };
    }

    public class CommandPropertiesSetterDefault : CommandPropertiesSetter
    {
      public CommandPropertiesSetterDefault() => this.WithCircuitBreakerDisabled(false).WithCircuitBreakerErrorThresholdPercentage(90).WithCircuitBreakerRequestVolumeThreshold(30).WithCircuitBreakerMinBackoff(GraphClientConstants.CircuitBreaker.MinBackoff).WithCircuitBreakerMaxBackoff(GraphClientConstants.CircuitBreaker.MaxBackoff).WithCircuitBreakerDeltaBackoff(GraphClientConstants.CircuitBreaker.DeltaBackoff).WithExecutionTimeout(GraphClientConstants.CircuitBreaker.Long10SecExecutionTimeout).WithFallbackDisabled(true).WithMetricsHealthSnapshotInterval(GraphClientConstants.CircuitBreaker.MetricsHealthSnapshotInterval).WithMetricsRollingStatisticalWindow(GraphClientConstants.CircuitBreaker.MetricsRollingStatisticalWindow).WithMetricsRollingStatisticalWindowBuckets(6);
    }

    public class CommandPropertiesSetterShortExecutionTimeout : 
      AadGraphClient.CommandPropertiesSetterDefault
    {
      public CommandPropertiesSetterShortExecutionTimeout() => this.WithExecutionTimeout(GraphClientConstants.CircuitBreaker.Short5SecExecutionTimeout);
    }

    public class CommandPropertiesSetterLongExecutionTimeout : 
      AadGraphClient.CommandPropertiesSetterDefault
    {
      public CommandPropertiesSetterLongExecutionTimeout() => this.WithExecutionTimeout(GraphClientConstants.CircuitBreaker.ExtraLong15SecExecutionTimeout);
    }
  }
}
