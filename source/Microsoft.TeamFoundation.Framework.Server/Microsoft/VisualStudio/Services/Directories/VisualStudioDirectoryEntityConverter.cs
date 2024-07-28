// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.VisualStudioDirectoryEntityConverter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories.DiscoveryService;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories
{
  public static class VisualStudioDirectoryEntityConverter
  {
    private static readonly string accountNameKey = typeof (VisualStudioDirectoryEntityConverter).FullName + ".AccountName";

    public static IDirectoryGroup ConvertGroup(Microsoft.VisualStudio.Services.Identity.Identity identity) => VisualStudioDirectoryEntityConverter.ConvertGroup((IVssRequestContext) null, identity, (IEnumerable<string>) new string[2]
    {
      "PrincipalName",
      "DisplayName"
    });

    public static IList<IDirectoryEntity> ConvertIdentities(
      IVssRequestContext context,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      IEnumerable<string> properties = null)
    {
      return (IList<IDirectoryEntity>) identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, IDirectoryEntity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IDirectoryEntity>) (identity => VisualStudioDirectoryEntityConverter.ConvertIdentity(context, identity, properties))).ToList<IDirectoryEntity>();
    }

    public static IDirectoryEntity ConvertIdentity(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IEnumerable<string> properties = null)
    {
      if (identity.IsContainer)
        return (IDirectoryEntity) VisualStudioDirectoryEntityConverter.ConvertGroup(context, identity, properties);
      return identity.IsAADServicePrincipal ? (IDirectoryEntity) VisualStudioDirectoryEntityConverter.ConvertServicePrincipal(context, identity, properties) : (IDirectoryEntity) VisualStudioDirectoryEntityConverter.ConvertUser(context, identity, properties);
    }

    public static IDirectoryUser ConvertUser(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IEnumerable<string> properties = null)
    {
      properties = (IEnumerable<string>) ((object) properties ?? (object) Array.Empty<string>());
      string str1 = "vsd";
      string str2 = identity.Id.ToString("D");
      Guid objectId = Guid.Empty;
      string str3;
      string originDirectory;
      string id;
      if (VisualStudioDirectoryEntityConverter.AllowAADResult(context) && AadIdentityHelper.IsAadUser((IReadOnlyVssIdentity) identity) && AadIdentityHelper.TryExtractObjectId((IReadOnlyVssIdentity) identity, out objectId))
      {
        str3 = new DirectoryEntityIdentifierV1("aad", "user", objectId.ToString("N")).Encode();
        originDirectory = "aad";
        id = objectId.ToString("D");
      }
      else if (VisualStudioDirectoryEntityConverter.IsWindowsUser(identity, out originDirectory))
      {
        id = identity.Descriptor.Identifier;
        str3 = new DirectoryEntityIdentifierV1(originDirectory, "user", id).Encode();
      }
      else if (VisualStudioDirectoryEntityConverter.IsGitHubUser(identity))
      {
        originDirectory = "ghb";
        id = identity.SocialDescriptor.Identifier;
        str3 = new DirectoryEntityIdentifierV1("ghb", "user", id).Encode();
      }
      else
      {
        str3 = new DirectoryEntityIdentifierV1("ims", "user", identity.Id.ToString("N")).Encode();
        originDirectory = str1;
        id = str2;
      }
      DirectoryUser directoryUser = new DirectoryUser();
      directoryUser.EntityId = str3;
      directoryUser.OriginDirectory = originDirectory;
      directoryUser.OriginId = id;
      directoryUser.LocalDirectory = str1;
      directoryUser.LocalId = str2;
      directoryUser.Properties = (IDictionary<string, object>) properties.ToDictionary<string, string, object>((Func<string, string>) (property => property), (Func<string, object>) (property =>
      {
        if (property != null)
        {
          switch (property.Length)
          {
            case 4:
              if (property == "Mail")
                return (object) VisualStudioDirectoryEntityConverter.GetMailAddress(context, identity);
              goto label_19;
            case 6:
              if (property == "Active")
                return (object) identity.IsActive;
              goto label_19;
            case 9:
              if (property == "ScopeName")
                return (object) VisualStudioDirectoryEntityConverter.GetScopeName(context, identity);
              goto label_19;
            case 11:
              if (property == "DisplayName")
                return (object) identity.DisplayName;
              goto label_19;
            case 13:
              switch (property[0])
              {
                case 'P':
                  if (property == "PrincipalName")
                    break;
                  goto label_19;
                case 'S':
                  if (property == "SignInAddress")
                    break;
                  goto label_19;
                default:
                  goto label_19;
              }
              break;
            case 14:
              if (property == "SamAccountName")
                break;
              goto label_19;
            case 15:
              if (property == "LocalDescriptor")
                return (object) identity.Descriptor;
              goto label_19;
            case 17:
              if (property == "SubjectDescriptor")
                return (object) identity.SubjectDescriptor;
              goto label_19;
            default:
              goto label_19;
          }
          return (object) VisualStudioDirectoryEntityConverter.GetAccountName(context, identity);
        }
label_19:
        return (object) null;
      }));
      return (IDirectoryUser) directoryUser;
    }

    public static IDirectoryServicePrincipal ConvertServicePrincipal(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IEnumerable<string> properties = null)
    {
      properties = (IEnumerable<string>) ((object) properties ?? (object) Array.Empty<string>());
      string str1 = "vsd";
      string str2 = identity.Id.ToString("D");
      Guid objectId = Guid.Empty;
      string str3;
      string str4;
      string str5;
      if (VisualStudioDirectoryEntityConverter.AllowAADResult(context) && AadIdentityHelper.TryExtractObjectId((IReadOnlyVssIdentity) identity, out objectId))
      {
        str3 = new DirectoryEntityIdentifierV1("aad", "servicePrincipal", objectId.ToString("N")).Encode();
        str4 = "aad";
        str5 = objectId.ToString("D");
      }
      else
      {
        str3 = new DirectoryEntityIdentifierV1("ims", "servicePrincipal", identity.Id.ToString("N")).Encode();
        str4 = str1;
        str5 = str2;
      }
      Guid appId = identity.GetProperty<Guid>("ApplicationId", Guid.Empty);
      string str6 = str3;
      string originDirectory = str4;
      string originId = str5;
      string entityId = str6;
      string str7 = str2;
      string localDirectory = str1;
      string localId = str7;
      Dictionary<string, object> dictionary = properties.ToDictionary<string, string, object>((Func<string, string>) (property => property), (Func<string, object>) (property =>
      {
        if (property != null)
        {
          switch (property.Length)
          {
            case 4:
              if (property == "Mail")
                break;
              goto label_19;
            case 5:
              if (property == "AppId")
                break;
              goto label_19;
            case 6:
              if (property == "Active")
                return (object) identity.IsActive;
              goto label_19;
            case 11:
              if (property == "DisplayName")
                return (object) identity.DisplayName;
              goto label_19;
            case 13:
              switch (property[0])
              {
                case 'P':
                  if (property == "PrincipalName")
                    break;
                  goto label_19;
                case 'S':
                  if (property == "SignInAddress")
                    break;
                  goto label_19;
                default:
                  goto label_19;
              }
              break;
            case 14:
              if (property == "SamAccountName")
                break;
              goto label_19;
            case 15:
              if (property == "LocalDescriptor")
                return (object) identity.Descriptor;
              goto label_19;
            case 17:
              if (property == "SubjectDescriptor")
                return (object) identity.SubjectDescriptor;
              goto label_19;
            default:
              goto label_19;
          }
          return !(Guid.Empty == appId) ? (object) appId : (object) null;
        }
label_19:
        return (object) null;
      }));
      return DirectoryEntityBuilder.BuildEntity<IDirectoryServicePrincipal>(originDirectory, originId, entityId, localDirectory, localId, properties: (IDictionary<string, object>) dictionary);
    }

    private static bool AllowAADResult(IVssRequestContext context) => !context.IsFeatureEnabled("VisualStudio.Services.Directories.VisualStudioDirectoryEntityConverter.AllowAADResult") || !context.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || context.GetOrganizationAadTenantId() != new Guid();

    public static IDirectoryGroup ConvertGroup(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IEnumerable<string> properties = null)
    {
      properties = (IEnumerable<string>) ((object) properties ?? (object) Array.Empty<string>());
      string str1 = "vsd";
      string str2 = identity.Id.ToString("D");
      string str3;
      string originDirectory;
      string id;
      if (AadIdentityHelper.IsAadGroup(identity.Descriptor))
      {
        Guid aadGroupId = AadIdentityHelper.ExtractAadGroupId(identity.Descriptor);
        str3 = new DirectoryEntityIdentifierV1("aad", "group", aadGroupId.ToString("N")).Encode();
        originDirectory = "aad";
        id = aadGroupId.ToString("D");
      }
      else if (VisualStudioDirectoryEntityConverter.IsWindowsGroup(identity, out originDirectory))
      {
        id = identity.Descriptor.Identifier;
        str3 = new DirectoryEntityIdentifierV1(originDirectory, "group", id).Encode();
      }
      else
      {
        str3 = new DirectoryEntityIdentifierV1("ims", "group", identity.Id.ToString("N")).Encode();
        originDirectory = str1;
        id = str2;
      }
      DirectoryGroup directoryGroup = new DirectoryGroup();
      directoryGroup.EntityId = str3;
      directoryGroup.OriginDirectory = originDirectory;
      directoryGroup.OriginId = id;
      directoryGroup.LocalDirectory = str1;
      directoryGroup.LocalId = str2;
      directoryGroup.Properties = (IDictionary<string, object>) properties.ToDictionary<string, string, object>((Func<string, string>) (property => property), (Func<string, object>) (property =>
      {
        if (property != null)
        {
          switch (property.Length)
          {
            case 4:
              if (property == "Mail")
                return (object) identity.GetProperty<string>("Mail", (string) null);
              break;
            case 6:
              if (property == "Active")
                return (object) identity.IsActive;
              break;
            case 9:
              if (property == "ScopeName")
                return (object) VisualStudioDirectoryEntityConverter.GetScopeName(context, identity);
              break;
            case 11:
              if (property == "DisplayName")
                return (object) identity.DisplayName;
              break;
            case 14:
              if (property == "SamAccountName")
                return (object) VisualStudioDirectoryEntityConverter.GetAccountName(context, identity);
              break;
            case 15:
              if (property == "LocalDescriptor")
                return (object) identity.Descriptor;
              break;
            case 17:
              if (property == "SubjectDescriptor")
                return (object) identity.SubjectDescriptor;
              break;
          }
        }
        return (object) null;
      }));
      return (IDirectoryGroup) directoryGroup;
    }

    private static string GetAccountName(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity) => identity.GetProperty<string>("Account", string.Empty);

    private static string GetMailAddress(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity) => identity.GetProperty<string>("Mail", string.Empty);

    private static string GetScopeName(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return identity.GetProperty<string>(identity.Descriptor.IdentityType.Equals("System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase) ? "Domain" : "ScopeName", string.Empty);
      return requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? VisualStudioDirectoryEntityConverter.GetAccountName(requestContext) : identity.GetProperty<string>(!identity.IsContainer ? "Domain" : "ScopeName", string.Empty);
    }

    private static string GetAccountName(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      context.CheckProjectCollectionRequestContext();
      context.CheckHostedDeployment();
      string accountName;
      if (context.Items.TryGetValue<string>(VisualStudioDirectoryEntityConverter.accountNameKey, out accountName))
        return accountName;
      string name = context.GetService<ICollectionService>().GetCollection(context.Elevate(), (IEnumerable<string>) null)?.Name;
      if (string.IsNullOrEmpty(name))
        return (string) null;
      context.Items[VisualStudioDirectoryEntityConverter.accountNameKey] = (object) name;
      return name;
    }

    private static bool IsWindowsUser(Microsoft.VisualStudio.Services.Identity.Identity identity, out string originDirectory)
    {
      originDirectory = (string) null;
      return identity?.Descriptor != (IdentityDescriptor) null && !identity.IsContainer && VssStringComparer.IdentityType.Compare("System.Security.Principal.WindowsIdentity", identity.Descriptor.IdentityType) == 0 && string.Equals(identity.GetProperty<string>("SchemaClassName", (string) null), "User") && VisualStudioDirectoryEntityConverter.TryGetWindowsDirectory(identity, out originDirectory);
    }

    private static bool IsWindowsGroup(Microsoft.VisualStudio.Services.Identity.Identity identity, out string originDirectory)
    {
      originDirectory = (string) null;
      return identity?.Descriptor != (IdentityDescriptor) null && identity.IsContainer && VssStringComparer.IdentityType.Compare("System.Security.Principal.WindowsIdentity", identity.Descriptor.IdentityType) == 0 && string.Equals(identity.GetProperty<string>("SchemaClassName", (string) null), "Group") && VisualStudioDirectoryEntityConverter.TryGetWindowsDirectory(identity, out originDirectory);
    }

    public static bool TryGetWindowsDirectory(Microsoft.VisualStudio.Services.Identity.Identity identity, out string originDirectory)
    {
      originDirectory = (string) null;
      string property = identity.GetProperty<string>("Domain", string.Empty);
      if (WindowsMachineDirectoryHelper.IsWindowsMachineSecurityIdentifier(identity.Descriptor.Identifier, property))
      {
        originDirectory = "wmd";
        return true;
      }
      if (!ActiveDirectoryHelper.IsActiveDirectorySecurityIdentifier(identity.Descriptor.Identifier))
        return false;
      originDirectory = "ad";
      return true;
    }

    private static bool IsGitHubUser(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity != null)
      {
        SocialDescriptor socialDescriptor = identity.SocialDescriptor;
        if (!identity.IsContainer)
          return VssStringComparer.SocialType.Compare("ghb", identity.SocialDescriptor.SocialType) == 0;
      }
      return false;
    }
  }
}
