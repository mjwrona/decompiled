// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssStringComparer
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Common
{
  public class VssStringComparer : StringComparer
  {
    private readonly StringComparison m_stringComparison;
    private StringComparer m_stringComparer;
    protected static VssStringComparer s_ordinal = new VssStringComparer(StringComparison.Ordinal);
    protected static VssStringComparer s_ordinalIgnoreCase = new VssStringComparer(StringComparison.OrdinalIgnoreCase);
    protected static VssStringComparer s_currentCulture = new VssStringComparer(StringComparison.CurrentCulture);
    protected static VssStringComparer s_currentCultureIgnoreCase = new VssStringComparer(StringComparison.CurrentCultureIgnoreCase);
    private static VssStringComparer s_dataSourceIgnoreProtocol = (VssStringComparer) new VssStringComparer.DataSourceIgnoreProtocolComparer();

    protected VssStringComparer(StringComparison stringComparison) => this.m_stringComparison = stringComparison;

    public override int Compare(string x, string y) => string.Compare(x, y, this.m_stringComparison);

    public override bool Equals(string x, string y) => string.Equals(x, y, this.m_stringComparison);

    public override int GetHashCode(string x) => this.MatchingStringComparer.GetHashCode(x);

    public int Compare(string x, int indexX, string y, int indexY, int length) => string.Compare(x, indexX, y, indexY, length, this.m_stringComparison);

    public bool Contains(string main, string pattern)
    {
      ArgumentUtility.CheckForNull<string>(main, nameof (main));
      ArgumentUtility.CheckForNull<string>(pattern, nameof (pattern));
      return main.IndexOf(pattern, this.m_stringComparison) >= 0;
    }

    public int IndexOf(string main, string pattern)
    {
      ArgumentUtility.CheckForNull<string>(main, nameof (main));
      ArgumentUtility.CheckForNull<string>(pattern, nameof (pattern));
      return main.IndexOf(pattern, this.m_stringComparison);
    }

    public bool StartsWith(string main, string pattern)
    {
      ArgumentUtility.CheckForNull<string>(main, nameof (main));
      ArgumentUtility.CheckForNull<string>(pattern, nameof (pattern));
      return main.StartsWith(pattern, this.m_stringComparison);
    }

    public bool EndsWith(string main, string pattern)
    {
      ArgumentUtility.CheckForNull<string>(main, nameof (main));
      ArgumentUtility.CheckForNull<string>(pattern, nameof (pattern));
      return main.EndsWith(pattern, this.m_stringComparison);
    }

    private StringComparer MatchingStringComparer
    {
      get
      {
        if (this.m_stringComparer == null)
        {
          switch (this.m_stringComparison)
          {
            case StringComparison.CurrentCulture:
              this.m_stringComparer = StringComparer.CurrentCulture;
              break;
            case StringComparison.CurrentCultureIgnoreCase:
              this.m_stringComparer = StringComparer.CurrentCultureIgnoreCase;
              break;
            case StringComparison.Ordinal:
              this.m_stringComparer = StringComparer.Ordinal;
              break;
            case StringComparison.OrdinalIgnoreCase:
              this.m_stringComparer = StringComparer.OrdinalIgnoreCase;
              break;
            default:
              this.m_stringComparer = StringComparer.Ordinal;
              break;
          }
        }
        return this.m_stringComparer;
      }
    }

    public static VssStringComparer ActiveDirectoryEntityIdComparer => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ArtifactType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ArtifactTool => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer AssemblyName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ContentType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer DomainName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer DomainNameUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer DatabaseCategory => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer DatabaseName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer DatabasePool => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer DataSource => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer DataSourceIgnoreProtocol => VssStringComparer.s_dataSourceIgnoreProtocol;

    public static VssStringComparer DirectoryName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer DirectoryEntityIdentifierConstants => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer DirectoryEntityPropertyComparer => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer DirectoryEntityTypeComparer => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer DirectoryEntryNameComparer => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer DirectoryKeyStringComparer => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer EncodingName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer EnvVar => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ExceptionSource => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer FilePath => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer FilePathUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer Guid => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer Hostname => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer HostnameUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer HttpRequestMethod => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer IdentityDescriptor => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer IdentityDomain => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer IdentityOriginId => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer IdentityType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer LinkName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer MachineName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer MailAddress => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer PropertyName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer RegistrationAttributeName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ReservedGroupName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WMDSchemaClassName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer SamAccountName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer AccountName => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer SocialType => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer ServerUrl => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ServerUrlUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer ServiceInterface => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ServicingOperation => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ToolId => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer Url => VssStringComparer.s_ordinal;

    public static VssStringComparer UrlPath => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer UriScheme => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer UriAuthority => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer UserId => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer UserName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer UserNameUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer XmlAttributeName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer XmlNodeName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer XmlElement => VssStringComparer.s_ordinal;

    public static VssStringComparer XmlAttributeValue => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer RegistryPath => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ServiceType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer AccessMappingMoniker => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer CatalogNodePath => VssStringComparer.s_ordinal;

    public static VssStringComparer CatalogServiceReference => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer CatalogNodeDependency => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ServicingTokenName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer IdentityPropertyName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer Collation => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer FeatureAvailabilityName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer TagName => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer HostingAccountPropertyName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer MessageBusName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer MessageBusSubscriptionName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer NamespaceName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer SID => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer FieldName => VssStringComparer.s_ordinal;

    public static VssStringComparer FieldNameUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer FieldType => VssStringComparer.s_ordinal;

    public static VssStringComparer EventType => VssStringComparer.s_ordinal;

    public static VssStringComparer EventTypeIgnoreCase => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer RegistrationEntryName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ServerName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer GroupName => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer RegistrationUtilities => VssStringComparer.s_ordinal;

    public static VssStringComparer RegistrationUtilitiesCaseInsensitive => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer IdentityName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer IdentityNameOrdinal => VssStringComparer.s_ordinal;

    public static VssStringComparer PlugInId => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ExtensionName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ExtensionType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer DomainUrl => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer AccountInfoAccount => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer AccountInfoPassword => VssStringComparer.s_ordinal;

    public static VssStringComparer AttributesDescriptor => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer NetworkSecurityGroupName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer VSSServerPath => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer VSSItemName => VssStringComparer.s_ordinal;

    public static VssStringComparer HtmlElementName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer HtmlAttributeName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer HtmlAttributeValue => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer StringFieldConditionEquality => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer StringFieldConditionOrdinal => VssStringComparer.s_ordinal;

    public static VssStringComparer ServiceEndpointTypeCompararer => VssStringComparer.s_ordinalIgnoreCase;

    private class DataSourceIgnoreProtocolComparer : VssStringComparer
    {
      private const string c_tcpPrefix = "tcp:";
      private const string c_npPrefix = "np:";

      public DataSourceIgnoreProtocolComparer()
        : base(StringComparison.OrdinalIgnoreCase)
      {
      }

      public override int Compare(string x, string y) => base.Compare(VssStringComparer.DataSourceIgnoreProtocolComparer.RemoveProtocolPrefix(x), VssStringComparer.DataSourceIgnoreProtocolComparer.RemoveProtocolPrefix(y));

      public override bool Equals(string x, string y) => base.Equals(VssStringComparer.DataSourceIgnoreProtocolComparer.RemoveProtocolPrefix(x), VssStringComparer.DataSourceIgnoreProtocolComparer.RemoveProtocolPrefix(y));

      private static string RemoveProtocolPrefix(string x)
      {
        if (x != null)
        {
          if (x.StartsWith("tcp:", StringComparison.OrdinalIgnoreCase))
            x = x.Substring("tcp:".Length);
          else if (x.StartsWith("np:", StringComparison.OrdinalIgnoreCase))
            x = x.Substring("np:".Length);
        }
        return x;
      }
    }
  }
}
