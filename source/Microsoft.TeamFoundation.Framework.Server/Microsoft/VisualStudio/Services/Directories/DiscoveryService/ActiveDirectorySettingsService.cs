// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.ActiveDirectorySettingsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class ActiveDirectorySettingsService : IVssFrameworkService
  {
    private const string DefaultLdapUserSearchQueryFormat = "(&(&(|(objectCategory=person)(objectCategory=msDS-ManagedServiceAccount)(objectCategory=msDS-GroupManagedServiceAccount))(objectClass=user))(|{0}))";
    private const string DefaultLdapSecurityGroupSearchQueryFormat = "(&(groupType:1.2.840.113556.1.4.803:=2147483648)(|{0}))";
    private const string DefaultLdapSecurityGroupOrUserSearchStringFormat = "(&(|(groupType:1.2.840.113556.1.4.803:=2147483648)(&(|(objectCategory=person)(objectCategory=msDS-ManagedServiceAccount)(objectCategory=msDS-GroupManagedServiceAccount))(objectClass=user)))(|{0}))";
    private const string DefaultSecurityGroupPredicate = "(groupType:1.2.840.113556.1.4.803:=2147483648)";
    private const string DefaultUserPredicate = "(&(|(objectCategory=person)(objectCategory=msDS-ManagedServiceAccount)(objectCategory=msDS-GroupManagedServiceAccount))(objectClass=user))";
    private const int DefaultDirectorySearcherClientTimeoutInSecs = 60;
    private const int DefaultDirectorySearcherMaxSizeLimit = 1000;
    private const string ActiveDirectorySettingsRoot = "/Configuration/DirectoryDiscoveryService/ActiveDirectory/Settings/";
    private const string ActiveDirectorySettingsRootFilter = "/Configuration/DirectoryDiscoveryService/ActiveDirectory/Settings/...";
    private const string LdapUserSearchQueryFormatPath = "/Configuration/DirectoryDiscoveryService/ActiveDirectory/Settings/LdapUserSearchQueryFormat";
    private const string LdapSecurityGroupSearchQueryFormatPath = "/Configuration/DirectoryDiscoveryService/ActiveDirectory/Settings/LdapSecurityGroupSearchQueryFormat";
    private const string LdapSecurityGroupOrUserSearchStringFormatPath = "/Configuration/DirectoryDiscoveryService/ActiveDirectory/Settings/LdapSecurityGroupOrUserSearchStringFormat";
    private const string DirectorySearcherClientTimeoutInSecsPath = "/Configuration/DirectoryDiscoveryService/ActiveDirectory/Settings/DirectorySearcherClientTimeoutInSecs";
    private const string DirectorySearcherMaxSizeLimitPath = "/Configuration/DirectoryDiscoveryService/ActiveDirectory/Settings/DirectorySearcherMaxSizeLimit";
    internal const string LdapDirectMembersSearchQueryFormat = "(memberOf={0})";
    internal const string LdapDirectReportsSearchQueryFormat = "(manager={0})";
    internal const string LdapPrefixQueryFormat = "({0}={1}*)";
    internal const string LdapObjectSidSearchQueryFormat = "(objectSid={0})";
    internal const string LdapDistinguishedNameSearchQueryFormat = "(distinguishedName={0})";

    internal TimeSpan DirectorySearcherClientTimeoutInSecs { get; private set; }

    internal int DirectorySearcherMaxSizeLimit { get; private set; }

    internal string LdapUserSearchQueryFormat { get; private set; }

    internal string LdapSecurityGroupSearchQueryFormat { get; private set; }

    internal string LdapSecurityGroupOrUserSearchStringFormat { get; private set; }

    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Configuration/DirectoryDiscoveryService/ActiveDirectory/Settings/...");
      this.PopulateSettings(requestContext);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    internal ActiveDirectorySettingsService()
    {
      this.LdapUserSearchQueryFormat = "(&(&(|(objectCategory=person)(objectCategory=msDS-ManagedServiceAccount)(objectCategory=msDS-GroupManagedServiceAccount))(objectClass=user))(|{0}))";
      this.LdapSecurityGroupSearchQueryFormat = "(&(groupType:1.2.840.113556.1.4.803:=2147483648)(|{0}))";
      this.LdapSecurityGroupOrUserSearchStringFormat = "(&(|(groupType:1.2.840.113556.1.4.803:=2147483648)(&(|(objectCategory=person)(objectCategory=msDS-ManagedServiceAccount)(objectCategory=msDS-GroupManagedServiceAccount))(objectClass=user)))(|{0}))";
      this.DirectorySearcherClientTimeoutInSecs = TimeSpan.FromSeconds(60.0);
      this.DirectorySearcherMaxSizeLimit = 1000;
    }

    private void PopulateSettings(IVssRequestContext context)
    {
      IVssRegistryService service = context.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>();
      this.LdapUserSearchQueryFormat = service.GetValue<string>(context, (RegistryQuery) "/Configuration/DirectoryDiscoveryService/ActiveDirectory/Settings/LdapUserSearchQueryFormat", "(&(&(|(objectCategory=person)(objectCategory=msDS-ManagedServiceAccount)(objectCategory=msDS-GroupManagedServiceAccount))(objectClass=user))(|{0}))");
      this.LdapSecurityGroupSearchQueryFormat = service.GetValue<string>(context, (RegistryQuery) "/Configuration/DirectoryDiscoveryService/ActiveDirectory/Settings/LdapSecurityGroupSearchQueryFormat", "(&(groupType:1.2.840.113556.1.4.803:=2147483648)(|{0}))");
      this.LdapSecurityGroupOrUserSearchStringFormat = service.GetValue<string>(context, (RegistryQuery) "/Configuration/DirectoryDiscoveryService/ActiveDirectory/Settings/LdapSecurityGroupOrUserSearchStringFormat", "(&(|(groupType:1.2.840.113556.1.4.803:=2147483648)(&(|(objectCategory=person)(objectCategory=msDS-ManagedServiceAccount)(objectCategory=msDS-GroupManagedServiceAccount))(objectClass=user)))(|{0}))");
      this.DirectorySearcherClientTimeoutInSecs = TimeSpan.FromSeconds((double) service.GetValue<int>(context, (RegistryQuery) "/Configuration/DirectoryDiscoveryService/ActiveDirectory/Settings/DirectorySearcherClientTimeoutInSecs", 60));
      this.DirectorySearcherMaxSizeLimit = service.GetValue<int>(context, (RegistryQuery) "/Configuration/DirectoryDiscoveryService/ActiveDirectory/Settings/DirectorySearcherMaxSizeLimit", 1000);
    }

    private void OnRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      this.PopulateSettings(context);
    }
  }
}
