// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.AzureActiveDirectoryEntityConverter
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class AzureActiveDirectoryEntityConverter
  {
    private static readonly string tenantNameKey = typeof (AzureActiveDirectoryEntityConverter).FullName + ".TenantName";

    internal static IDirectoryEntity ConvertObject(
      IVssRequestContext context,
      AadObject obj,
      IEnumerable<string> properties)
    {
      switch (obj)
      {
        case AadUser _:
          return (IDirectoryEntity) AzureActiveDirectoryEntityConverter.ConvertUser(context, (AadUser) obj, properties);
        case AadGroup _:
          return (IDirectoryEntity) AzureActiveDirectoryEntityConverter.ConvertGroup(context, (AadGroup) obj, properties);
        case AadDirectoryRole _:
          return (IDirectoryEntity) AzureActiveDirectoryEntityConverter.ConvertDirectoryRoleToDirectoryGroup(context, (AadDirectoryRole) obj, properties);
        case AadServicePrincipal _:
          return (IDirectoryEntity) AzureActiveDirectoryEntityConverter.ConvertServicePrincipal(context, (AadServicePrincipal) obj, properties);
        default:
          return (IDirectoryEntity) null;
      }
    }

    internal static IDirectoryUser ConvertUser(
      IVssRequestContext context,
      AadUser user,
      IEnumerable<string> properties)
    {
      properties = (IEnumerable<string>) ((object) properties ?? (object) Array.Empty<string>());
      string entityId = AzureActiveDirectoryEntityIdentifierHelper.CreateUserId(user.ObjectId).Encode();
      return DirectoryEntityBuilder.BuildEntity<IDirectoryUser>("aad", user.ObjectId.ToString("D"), entityId, properties: (IDictionary<string, object>) properties.ToDictionary<string, string, object>((Func<string, string>) (property => property), (Func<string, object>) (property =>
      {
        if (property != null)
        {
          switch (property.Length)
          {
            case 4:
              if (property == "Mail")
                return (object) user.Mail;
              break;
            case 5:
              if (property == "Guest")
                return (object) "guest".Equals(user.UserType, StringComparison.InvariantCultureIgnoreCase);
              break;
            case 7:
              if (property == "Surname")
                return (object) user.Surname;
              break;
            case 8:
              if (property == "JobTitle")
                return (object) user.JobTitle;
              break;
            case 9:
              if (property == "ScopeName")
                return (object) AzureActiveDirectoryEntityConverter.GetTenantName(context);
              break;
            case 10:
              if (property == "Department")
                return (object) user.Department;
              break;
            case 11:
              if (property == "DisplayName")
                return (object) user.DisplayName;
              break;
            case 12:
              if (property == "MailNickname")
                return user.MailNickname != null && user.MailNickname.Contains("#EXT#") ? (object) null : (object) user.MailNickname;
              break;
            case 13:
              switch (property[0])
              {
                case 'P':
                  if (property == "PrincipalName")
                    return (object) user.UserPrincipalName;
                  break;
                case 'S':
                  if (property == "SignInAddress")
                    return (object) user.SignInAddress;
                  break;
              }
              break;
            case 15:
              if (property == "TelephoneNumber")
                return (object) user.TelephoneNumber;
              break;
            case 17:
              if (property == "SubjectDescriptor")
                return (object) AzureActiveDirectoryEntityConverter.GetSubjectDescriptorFromProviderInfo(context, context.GetOrganizationAadTenantId(), user.ObjectId);
              break;
            case 26:
              if (property == "PhysicalDeliveryOfficeName")
                return (object) user.PhysicalDeliveryOfficeName;
              break;
          }
        }
        return (object) null;
      })));
    }

    internal static IDirectoryGroup ConvertGroup(
      IVssRequestContext context,
      AadGroup group,
      IEnumerable<string> properties)
    {
      properties = (IEnumerable<string>) ((object) properties ?? (object) Array.Empty<string>());
      string entityId = AzureActiveDirectoryEntityIdentifierHelper.CreateGroupId(group.ObjectId).Encode();
      return DirectoryEntityBuilder.BuildEntity<IDirectoryGroup>("aad", group.ObjectId.ToString("D"), entityId, properties: (IDictionary<string, object>) properties.ToDictionary<string, string, object>((Func<string, string>) (property => property), (Func<string, object>) (property =>
      {
        switch (property)
        {
          case "DisplayName":
            return (object) group.DisplayName;
          case "Mail":
            return (object) group.Mail;
          case "MailNickname":
            return (object) group.MailNickname;
          case "ScopeName":
            return (object) AzureActiveDirectoryEntityConverter.GetTenantName(context);
          case "Description":
            return (object) group.Description;
          default:
            return (object) null;
        }
      })));
    }

    internal static IDirectoryServicePrincipal ConvertServicePrincipal(
      IVssRequestContext context,
      AadServicePrincipal servicePrincipal,
      IEnumerable<string> properties)
    {
      properties = (IEnumerable<string>) ((object) properties ?? (object) Array.Empty<string>());
      string entityId = AzureActiveDirectoryEntityIdentifierHelper.CreateServicePrincipalId(servicePrincipal.ObjectId).Encode();
      return DirectoryEntityBuilder.BuildEntity<IDirectoryServicePrincipal>("aad", servicePrincipal.ObjectId.ToString("D"), entityId, properties: (IDictionary<string, object>) properties.ToDictionary<string, string, object>((Func<string, string>) (property => property), (Func<string, object>) (property =>
      {
        switch (property)
        {
          case "DisplayName":
            return (object) servicePrincipal.DisplayName;
          case "ScopeName":
            return (object) AzureActiveDirectoryEntityConverter.GetTenantName(context);
          case "MailNickname":
          case "AppId":
            return (object) servicePrincipal.AppId.ToString();
          default:
            return (object) null;
        }
      })));
    }

    private static IDirectoryGroup ConvertDirectoryRoleToDirectoryGroup(
      IVssRequestContext context,
      AadDirectoryRole directoryRole,
      IEnumerable<string> properties)
    {
      properties = (IEnumerable<string>) ((object) properties ?? (object) Array.Empty<string>());
      string entityId = AzureActiveDirectoryEntityIdentifierHelper.CreateGroupId(directoryRole.ObjectId).Encode();
      return DirectoryEntityBuilder.BuildEntity<IDirectoryGroup>("aad", directoryRole.ObjectId.ToString("D"), entityId, properties: (IDictionary<string, object>) properties.ToDictionary<string, string, object>((Func<string, string>) (property => property), (Func<string, object>) (property =>
      {
        switch (property)
        {
          case "DisplayName":
            return (object) directoryRole.DisplayName;
          case "ScopeName":
            return (object) AzureActiveDirectoryEntityConverter.GetTenantName(context);
          case "Description":
            return (object) directoryRole.Description;
          default:
            return (object) null;
        }
      })));
    }

    private static string GetTenantName(IVssRequestContext context)
    {
      string displayName;
      if (!context.Items.TryGetValue<string>(AzureActiveDirectoryEntityConverter.tenantNameKey, out displayName))
      {
        displayName = context.GetService<AadService>().GetTenant(context, new GetTenantRequest()).Tenant.DisplayName;
        context.Items[AzureActiveDirectoryEntityConverter.tenantNameKey] = (object) displayName;
      }
      return displayName;
    }

    private static SubjectDescriptor GetSubjectDescriptorFromProviderInfo(
      IVssRequestContext context,
      Guid tenantId,
      Guid objectId)
    {
      ArgumentUtility.CheckForEmptyGuid(tenantId, nameof (tenantId));
      ArgumentUtility.CheckForEmptyGuid(objectId, nameof (objectId));
      return context.GetService<IGraphIdentifierConversionService>().GetDescriptorByProviderInfo(context, tenantId.ToString(), objectId.ToString());
    }
  }
}
