// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraphConverters
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Aad.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  internal class MicrosoftGraphConverters
  {
    private static readonly IReadOnlyDictionary<string, AadObjectType> MicrosoftGraphOdataTypeToAadObjectTypeLookup = (IReadOnlyDictionary<string, AadObjectType>) new Dictionary<string, AadObjectType>()
    {
      {
        "#microsoft.graph.user",
        AadObjectType.User
      },
      {
        "#microsoft.graph.group",
        AadObjectType.Group
      },
      {
        "#microsoft.graph.servicePrincipal",
        AadObjectType.ServicePrincipal
      }
    };
    private static readonly IReadOnlyDictionary<Type, AadObjectType> MicrosoftGraphObjectTypeToAadObjectTypeLookup = (IReadOnlyDictionary<Type, AadObjectType>) new Dictionary<Type, AadObjectType>()
    {
      {
        typeof (User),
        AadObjectType.User
      },
      {
        typeof (Group),
        AadObjectType.Group
      },
      {
        typeof (ServicePrincipal),
        AadObjectType.ServicePrincipal
      }
    };

    internal static AadUser ConvertUser(User user)
    {
      if (user == null)
        return (AadUser) null;
      AadUser.Factory factory = new AadUser.Factory();
      factory.ObjectId = MicrosoftGraphConverters.CreateGuid(((Entity) user).Id);
      factory.AccountEnabled = user.AccountEnabled.GetValueOrDefault(false);
      factory.DisplayName = user.DisplayName;
      factory.Mail = user.Mail;
      factory.OtherMails = user.OtherMails;
      factory.MailNickname = user.MailNickname;
      factory.UserPrincipalName = user.UserPrincipalName;
      factory.SignInAddress = MicrosoftGraphUtils.GetSignInAddressFromUpn(user);
      factory.HasThumbnailPhoto = false;
      factory.JobTitle = user.JobTitle;
      factory.Department = user.Department;
      factory.PhysicalDeliveryOfficeName = user.OfficeLocation;
      factory.Manager = user.Manager != null ? MicrosoftGraphConverters.ConvertUser(user.Manager as User) : (AadUser) null;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      factory.DirectReports = user.DirectReports != null ? ((IEnumerable) user.DirectReports).OfType<User>().Select<User, AadUser>(MicrosoftGraphConverters.\u003C\u003EO.\u003C0\u003E__ConvertUser ?? (MicrosoftGraphConverters.\u003C\u003EO.\u003C0\u003E__ConvertUser = new Func<User, AadUser>(MicrosoftGraphConverters.ConvertUser))) : (IEnumerable<AadUser>) null;
      factory.Surname = user.Surname;
      factory.UserType = user.UserType;
      factory.UserState = user.ExternalUserState;
      factory.OnPremisesSecurityIdentifier = user.OnPremisesSecurityIdentifier;
      factory.ImmutableId = user.OnPremisesImmutableId;
      IEnumerable<string> businessPhones = user.BusinessPhones;
      factory.TelephoneNumber = businessPhones != null ? businessPhones.FirstOrDefault<string>() : (string) null;
      factory.Country = user.Country;
      factory.UsageLocation = user.UsageLocation;
      return factory.Create();
    }

    internal static AadServicePrincipal ConvertServicePrincipal(ServicePrincipal servicePrincipal)
    {
      if (servicePrincipal == null)
        return (AadServicePrincipal) null;
      return new AadServicePrincipal.Factory()
      {
        ObjectId = AadGraphClient.CreateGuid(((Entity) servicePrincipal).Id),
        AppId = AadGraphClient.CreateGuid(servicePrincipal.AppId),
        DisplayName = servicePrincipal.DisplayName,
        AccountEnabled = servicePrincipal.AccountEnabled.GetValueOrDefault(),
        ServicePrincipalType = servicePrincipal.ServicePrincipalType
      }.Create();
    }

    internal static ServicePrincipal ConvertAadServicePrincipal(
      AadServicePrincipal aadServicePrincipal)
    {
      if (aadServicePrincipal == null)
        return (ServicePrincipal) null;
      ServicePrincipal servicePrincipal = new ServicePrincipal();
      ((Entity) servicePrincipal).Id = MicrosoftGraphConverters.ConvertGuidToStringWithEmptyGuidAsNull(aadServicePrincipal.ObjectId);
      servicePrincipal.AppId = MicrosoftGraphConverters.ConvertGuidToStringWithEmptyGuidAsNull(aadServicePrincipal.AppId);
      servicePrincipal.DisplayName = aadServicePrincipal.DisplayName;
      servicePrincipal.AccountEnabled = new bool?(aadServicePrincipal.AccountEnabled);
      return servicePrincipal;
    }

    internal static AadTenant ConvertTenant(Organization organization)
    {
      if (organization == null)
        return (AadTenant) null;
      AadTenant.Factory factory = new AadTenant.Factory();
      factory.ObjectId = MicrosoftGraphConverters.CreateGuid(((Entity) organization).Id);
      factory.DisplayName = organization.DisplayName;
      factory.VerifiedDomains = MicrosoftGraphConverters.ConvertDomains(organization.VerifiedDomains);
      factory.CountryLetterCode = organization.CountryLetterCode;
      factory.DirSyncEnabled = organization.OnPremisesSyncEnabled;
      DateTimeOffset? lastSyncDateTime = organization.OnPremisesLastSyncDateTime;
      ref DateTimeOffset? local = ref lastSyncDateTime;
      factory.CompanyLastDirSyncTime = local.HasValue ? new DateTime?(local.GetValueOrDefault().UtcDateTime) : new DateTime?();
      return factory.Create();
    }

    internal static AadTenant ConvertResourceTenant(ResourceTenantDetail tenant)
    {
      if (tenant == null)
        return (AadTenant) null;
      return new AadTenant.Factory()
      {
        ObjectId = MicrosoftGraphConverters.CreateGuid(tenant.TenantId),
        DisplayName = tenant.DisplayName
      }.Create();
    }

    internal static AadApplication ConvertApplication(Application application)
    {
      if (application == null)
        return (AadApplication) null;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new AadApplication.Factory()
      {
        ObjectId = MicrosoftGraphConverters.CreateGuid(((Entity) application).Id),
        DisplayName = application.DisplayName,
        AppId = MicrosoftGraphConverters.CreateGuid(application.AppId),
        PasswordCredentials = ((IList<AadPasswordCredential>) application.PasswordCredentials.Select<PasswordCredential, AadPasswordCredential>(MicrosoftGraphConverters.\u003C\u003EO.\u003C1\u003E__ConvertPasswordCredential ?? (MicrosoftGraphConverters.\u003C\u003EO.\u003C1\u003E__ConvertPasswordCredential = new Func<PasswordCredential, AadPasswordCredential>(MicrosoftGraphConverters.ConvertPasswordCredential))).ToList<AadPasswordCredential>())
      }.Create();
    }

    internal static AadPasswordCredential ConvertPasswordCredential(PasswordCredential credential)
    {
      if (credential == null)
        return (AadPasswordCredential) null;
      return new AadPasswordCredential()
      {
        StartDate = credential.StartDateTime,
        EndDate = credential.EndDateTime,
        KeyId = credential.KeyId,
        Value = credential.SecretText
      };
    }

    internal static AadFederatedIdentityCredential ConvertFederatedIdentityCredential(
      FederatedIdentityCredential credential)
    {
      if (credential == null)
        return (AadFederatedIdentityCredential) null;
      return new AadFederatedIdentityCredential()
      {
        Subject = credential.Subject,
        Audiences = credential.Audiences.ToList<string>(),
        Issuer = MicrosoftGraphConverters.CreateUri(credential.Issuer)
      };
    }

    internal static Application ConvertAadApplication(AadApplication application)
    {
      if (application == null)
        return (Application) null;
      Application application1 = new Application();
      ((Entity) application1).Id = MicrosoftGraphConverters.ConvertGuidToStringWithEmptyGuidAsNull(application.ObjectId);
      application1.DisplayName = application.DisplayName;
      application1.AppId = MicrosoftGraphConverters.ConvertGuidToStringWithEmptyGuidAsNull(application.AppId);
      IList<AadPasswordCredential> passwordCredentials = application.PasswordCredentials;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      application1.PasswordCredentials = passwordCredentials != null ? (IEnumerable<PasswordCredential>) passwordCredentials.Select<AadPasswordCredential, PasswordCredential>(MicrosoftGraphConverters.\u003C\u003EO.\u003C2\u003E__ConvertAadPasswordCredential ?? (MicrosoftGraphConverters.\u003C\u003EO.\u003C2\u003E__ConvertAadPasswordCredential = new Func<AadPasswordCredential, PasswordCredential>(MicrosoftGraphConverters.ConvertAadPasswordCredential))).ToList<PasswordCredential>() : (IEnumerable<PasswordCredential>) null;
      return application1;
    }

    internal static PasswordCredential ConvertAadPasswordCredential(AadPasswordCredential credential)
    {
      if (credential == null)
        return (PasswordCredential) null;
      return new PasswordCredential()
      {
        StartDateTime = credential.StartDate,
        EndDateTime = credential.EndDate,
        KeyId = credential.KeyId,
        SecretText = credential.Value
      };
    }

    internal static Guid CreateGuid(string objectId)
    {
      Guid result;
      if (!Guid.TryParse(objectId, out result))
        throw new AadException(string.Format("Failed to parse Object ID: {0}", (object) objectId));
      return result;
    }

    internal static Uri CreateUri(string uriString)
    {
      Uri result;
      if (!Uri.TryCreate(uriString, UriKind.Absolute, out result))
        throw new AadException(string.Format("Failed to parse Uri: {0}", (object) uriString));
      return result;
    }

    internal static AadGroup ConvertGroup(Group group)
    {
      if (group == null)
        return (AadGroup) null;
      return new AadGroup.Factory()
      {
        ObjectId = MicrosoftGraphConverters.CreateGuid(((Entity) group).Id),
        Description = group.Description,
        DisplayName = group.DisplayName,
        MailNickname = group.MailNickname,
        Mail = group.Mail,
        OnPremisesSecurityIdentifier = group.OnPremisesSecurityIdentifier
      }.Create();
    }

    internal static AadObjectType GetDirectoryObjectAadObjectType(DirectoryObject directoryObject)
    {
      AadObjectType objectAadObjectType;
      MicrosoftGraphConverters.MicrosoftGraphOdataTypeToAadObjectTypeLookup.TryGetValue(((Entity) directoryObject)?.ODataType, out objectAadObjectType);
      if (objectAadObjectType == AadObjectType.Unknown)
        MicrosoftGraphConverters.MicrosoftGraphObjectTypeToAadObjectTypeLookup.TryGetValue(directoryObject?.GetType(), out objectAadObjectType);
      return objectAadObjectType;
    }

    internal static AadDirectoryRole ConvertDirectoryRole(DirectoryRole directoryRole)
    {
      if (directoryRole == null)
        return (AadDirectoryRole) null;
      return new AadDirectoryRole.Factory()
      {
        ObjectId = MicrosoftGraphConverters.CreateGuid(((Entity) directoryRole).Id),
        DisplayName = directoryRole.DisplayName,
        Description = directoryRole.Description,
        RoleTemplateId = directoryRole.RoleTemplateId
      }.Create();
    }

    internal static AadObject ConvertDirectoryObject(DirectoryObject directoryObject)
    {
      if (directoryObject == null)
        return (AadObject) null;
      switch (MicrosoftGraphConverters.GetDirectoryObjectAadObjectType(directoryObject))
      {
        case AadObjectType.User:
          return (AadObject) MicrosoftGraphConverters.ConvertUser(directoryObject as User);
        case AadObjectType.Group:
          return (AadObject) MicrosoftGraphConverters.ConvertGroup(directoryObject as Group);
        case AadObjectType.ServicePrincipal:
          return (AadObject) MicrosoftGraphConverters.ConvertServicePrincipal(directoryObject as ServicePrincipal);
        default:
          return (AadObject) null;
      }
    }

    private static string ConvertGuidToStringWithEmptyGuidAsNull(Guid guid) => !(guid == Guid.Empty) ? guid.ToString() : (string) null;

    private static IEnumerable<AadDomain> ConvertDomains(IEnumerable<VerifiedDomain> domains) => domains != null ? domains.Select<VerifiedDomain, AadDomain>((Func<VerifiedDomain, AadDomain>) (domain => new AadDomain.Factory()
    {
      Name = domain.Name,
      IsDefault = domain.IsDefault.GetValueOrDefault()
    }.Create())) : (IEnumerable<AadDomain>) null;
  }
}
