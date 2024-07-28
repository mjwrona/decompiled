// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Configuration.KeyValueConfigurationManager
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security;
using System.Text.RegularExpressions;
using System.Web.Configuration;

namespace Microsoft.Azure.NotificationHubs.Messaging.Configuration
{
  internal class KeyValueConfigurationManager
  {
    public const string ServiceBusConnectionKeyName = "Microsoft.Azure.NotificationHubs.ConnectionString";
    public const string OperationTimeoutConfigName = "OperationTimeout";
    public const string EntityPathConfigName = "EntityPath";
    public const string EndpointConfigName = "Endpoint";
    public const string SharedSecretIssuerConfigName = "SharedSecretIssuer";
    public const string SharedSecretValueConfigName = "SharedSecretValue";
    public const string SharedAccessKeyName = "SharedAccessKeyName";
    public const string SharedAccessValueName = "SharedAccessKey";
    public const string RuntimePortConfigName = "RuntimePort";
    public const string ManagementPortConfigName = "ManagementPort";
    public const string StsEndpointConfigName = "StsEndpoint";
    public const string WindowsDomainConfigName = "WindowsDomain";
    public const string WindowsUsernameConfigName = "WindowsUsername";
    public const string WindowsPasswordConfigName = "WindowsPassword";
    public const string OAuthDomainConfigName = "OAuthDomain";
    public const string OAuthUsernameConfigName = "OAuthUsername";
    public const string OAuthPasswordConfigName = "OAuthPassword";
    internal const string ValueSeparator = ",";
    internal const string KeyValueSeparator = "=";
    internal const string KeyDelimiter = ";";
    private const string KeyAttributeEnumRegexString = "(OperationTimeout|Endpoint|EntityPath|RuntimePort|ManagementPort|StsEndpoint|WindowsDomain|WindowsUsername|WindowsPassword|OAuthDomain|OAuthUsername|OAuthPassword|SharedSecretIssuer|SharedSecretValue|SharedAccessKeyName|SharedAccessKey)";
    private const string KeyDelimiterRegexString = ";(OperationTimeout|Endpoint|EntityPath|RuntimePort|ManagementPort|StsEndpoint|WindowsDomain|WindowsUsername|WindowsPassword|OAuthDomain|OAuthUsername|OAuthPassword|SharedSecretIssuer|SharedSecretValue|SharedAccessKeyName|SharedAccessKey)=";
    private static readonly Regex KeyRegex = new Regex("(OperationTimeout|Endpoint|EntityPath|RuntimePort|ManagementPort|StsEndpoint|WindowsDomain|WindowsUsername|WindowsPassword|OAuthDomain|OAuthUsername|OAuthPassword|SharedSecretIssuer|SharedSecretValue|SharedAccessKeyName|SharedAccessKey)", RegexOptions.IgnoreCase);
    private static readonly Regex ValueRegex = new Regex("([^\\s]+)");
    internal NameValueCollection connectionProperties;
    internal string connectionString;

    public KeyValueConfigurationManager()
    {
      string connection = (string) null;
      try
      {
        if (WebConfigurationManager.AppSettings.Count > 0)
          connection = WebConfigurationManager.AppSettings["Microsoft.Azure.NotificationHubs.ConnectionString"];
      }
      catch (ConfigurationErrorsException ex)
      {
      }
      if (string.IsNullOrWhiteSpace(connection))
        connection = ConfigurationManager.AppSettings["Microsoft.Azure.NotificationHubs.ConnectionString"];
      this.Initialize(connection);
    }

    public KeyValueConfigurationManager(string connectionString) => this.Initialize(connectionString);

    private void Initialize(string connection)
    {
      this.connectionString = connection;
      this.connectionProperties = KeyValueConfigurationManager.CreateNameValueCollectionFromConnectionString(this.connectionString);
    }

    public string this[string key] => this.connectionProperties[key];

    public SecureString GetWindowsPassword() => this.GetSecurePassword("WindowsPassword");

    public SecureString GetOAuthPassword() => this.GetSecurePassword("OAuthPassword");

    public NamespaceManager CreateNamespaceManager()
    {
      this.Validate();
      string connectionProperty1 = this.connectionProperties["OperationTimeout"];
      IEnumerable<Uri> endpointAddresses1 = (IEnumerable<Uri>) KeyValueConfigurationManager.GetEndpointAddresses(this.connectionProperties["Endpoint"], this.connectionProperties["ManagementPort"]);
      IEnumerable<Uri> endpointAddresses2 = (IEnumerable<Uri>) KeyValueConfigurationManager.GetEndpointAddresses(this.connectionProperties["StsEndpoint"], (string) null);
      string connectionProperty2 = this.connectionProperties["SharedSecretIssuer"];
      string connectionProperty3 = this.connectionProperties["SharedSecretValue"];
      string connectionProperty4 = this.connectionProperties["SharedAccessKeyName"];
      string connectionProperty5 = this.connectionProperties["SharedAccessKey"];
      string connectionProperty6 = this.connectionProperties["WindowsDomain"];
      string connectionProperty7 = this.connectionProperties["WindowsUsername"];
      SecureString windowsPassword = this.GetWindowsPassword();
      string connectionProperty8 = this.connectionProperties["OAuthDomain"];
      string connectionProperty9 = this.connectionProperties["OAuthUsername"];
      SecureString oauthPassword = this.GetOAuthPassword();
      try
      {
        TokenProvider tokenProvider = KeyValueConfigurationManager.CreateTokenProvider(endpointAddresses2, connectionProperty2, connectionProperty3, connectionProperty4, connectionProperty5, connectionProperty6, connectionProperty7, windowsPassword, connectionProperty8, connectionProperty9, oauthPassword);
        if (string.IsNullOrEmpty(connectionProperty1))
          return new NamespaceManager(endpointAddresses1, tokenProvider);
        return new NamespaceManager(endpointAddresses1, new NamespaceManagerSettings()
        {
          OperationTimeout = TimeSpan.Parse(connectionProperty1, (IFormatProvider) CultureInfo.CurrentCulture),
          TokenProvider = tokenProvider
        });
      }
      catch (ArgumentException ex)
      {
        throw new ConfigurationErrorsException(SRClient.AppSettingsCreateManagerWithInvalidConnectionString((object) ex.Message), (Exception) ex);
      }
      catch (UriFormatException ex)
      {
        throw new ConfigurationErrorsException(SRClient.AppSettingsCreateManagerWithInvalidConnectionString((object) ex.Message), (Exception) ex);
      }
    }

    internal IList<Uri> GetEndpointAddresses() => KeyValueConfigurationManager.GetEndpointAddresses(this.connectionProperties["Endpoint"], this.connectionProperties["RuntimePort"]);

    public static IList<Uri> GetEndpointAddresses(string uriEndpoints, string portString)
    {
      List<Uri> endpointAddresses = new List<Uri>();
      if (string.IsNullOrWhiteSpace(uriEndpoints))
        return (IList<Uri>) endpointAddresses;
      string[] strArray = uriEndpoints.Split(new string[1]
      {
        ","
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray == null || strArray.Length == 0)
        return (IList<Uri>) endpointAddresses;
      int result;
      if (!int.TryParse(portString, out result))
        result = -1;
      foreach (string uri in strArray)
      {
        UriBuilder uriBuilder = new UriBuilder(uri);
        if (result > 0)
          uriBuilder.Port = result;
        endpointAddresses.Add(uriBuilder.Uri);
      }
      return (IList<Uri>) endpointAddresses;
    }

    private static NameValueCollection CreateNameValueCollectionFromConnectionString(
      string connectionString)
    {
      NameValueCollection connectionString1 = new NameValueCollection();
      if (!string.IsNullOrWhiteSpace(connectionString))
      {
        string[] strArray = Regex.Split(";" + connectionString, ";(OperationTimeout|Endpoint|EntityPath|RuntimePort|ManagementPort|StsEndpoint|WindowsDomain|WindowsUsername|WindowsPassword|OAuthDomain|OAuthUsername|OAuthPassword|SharedSecretIssuer|SharedSecretValue|SharedAccessKeyName|SharedAccessKey)=", RegexOptions.IgnoreCase);
        if (strArray.Length != 0)
        {
          if (!string.IsNullOrWhiteSpace(strArray[0]))
            throw new ConfigurationErrorsException(SRClient.AppSettingsConfigSettingInvalidKey((object) connectionString));
          if (strArray.Length % 2 != 1)
            throw new ConfigurationErrorsException(SRClient.AppSettingsConfigSettingInvalidKey((object) connectionString));
          for (int index = 1; index < strArray.Length; index = index + 1 + 1)
          {
            string str = strArray[index];
            if (string.IsNullOrWhiteSpace(str) || !KeyValueConfigurationManager.KeyRegex.IsMatch(str))
              throw new ConfigurationErrorsException(SRClient.AppSettingsConfigSettingInvalidKey((object) str));
            string input = strArray[index + 1];
            if (string.IsNullOrWhiteSpace(input) || !KeyValueConfigurationManager.ValueRegex.IsMatch(input))
              throw new ConfigurationErrorsException(SRClient.AppSettingsConfigSettingInvalidValue((object) str, (object) input));
            if (connectionString1[str] != null)
              throw new ConfigurationErrorsException(SRClient.AppSettingsConfigDuplicateSetting((object) str));
            connectionString1[str] = input;
          }
        }
      }
      return connectionString1;
    }

    private unsafe SecureString GetSecurePassword(string configName)
    {
      SecureString securePassword = (SecureString) null;
      string connectionProperty = this.connectionProperties[configName];
      if (!string.IsNullOrWhiteSpace(connectionProperty))
      {
        char[] charArray = connectionProperty.ToCharArray();
        fixed (char* chPtr = charArray)
          securePassword = new SecureString(chPtr, charArray.Length);
      }
      return securePassword;
    }

    internal TokenProvider CreateTokenProvider()
    {
      IList<Uri> endpointAddresses = KeyValueConfigurationManager.GetEndpointAddresses(this.connectionProperties["StsEndpoint"], (string) null);
      string connectionProperty1 = this.connectionProperties["SharedSecretIssuer"];
      string connectionProperty2 = this.connectionProperties["SharedSecretValue"];
      string connectionProperty3 = this.connectionProperties["SharedAccessKeyName"];
      string connectionProperty4 = this.connectionProperties["SharedAccessKey"];
      string connectionProperty5 = this.connectionProperties["WindowsDomain"];
      string connectionProperty6 = this.connectionProperties["WindowsUsername"];
      SecureString windowsPassword1 = this.GetWindowsPassword();
      string connectionProperty7 = this.connectionProperties["OAuthDomain"];
      string connectionProperty8 = this.connectionProperties["OAuthUsername"];
      SecureString oauthPassword1 = this.GetOAuthPassword();
      string issuerName = connectionProperty1;
      string issuerKey = connectionProperty2;
      string sharedAccessKeyName = connectionProperty3;
      string sharedAccessKey = connectionProperty4;
      string windowsDomain = connectionProperty5;
      string windowsUser = connectionProperty6;
      SecureString windowsPassword2 = windowsPassword1;
      string oauthDomain = connectionProperty7;
      string oauthUser = connectionProperty8;
      SecureString oauthPassword2 = oauthPassword1;
      return KeyValueConfigurationManager.CreateTokenProvider((IEnumerable<Uri>) endpointAddresses, issuerName, issuerKey, sharedAccessKeyName, sharedAccessKey, windowsDomain, windowsUser, windowsPassword2, oauthDomain, oauthUser, oauthPassword2);
    }

    private static TokenProvider CreateTokenProvider(
      IEnumerable<Uri> stsEndpoints,
      string issuerName,
      string issuerKey,
      string sharedAccessKeyName,
      string sharedAccessKey,
      string windowsDomain,
      string windowsUser,
      SecureString windowsPassword,
      string oauthDomain,
      string oauthUser,
      SecureString oauthPassword)
    {
      if (!string.IsNullOrWhiteSpace(sharedAccessKey))
        return TokenProvider.CreateSharedAccessSignatureTokenProvider(sharedAccessKeyName, sharedAccessKey);
      if (string.IsNullOrWhiteSpace(issuerName))
      {
        int num = stsEndpoints == null ? 0 : (stsEndpoints.Any<Uri>() ? 1 : 0);
        bool flag1 = !string.IsNullOrWhiteSpace(windowsUser) && windowsPassword != null && windowsPassword.Length > 0;
        bool flag2 = !string.IsNullOrWhiteSpace(oauthUser) && oauthPassword != null && oauthPassword.Length > 0;
        if (num == 0)
          return (TokenProvider) null;
        if (flag2)
        {
          NetworkCredential credential = string.IsNullOrWhiteSpace(oauthDomain) ? new NetworkCredential(oauthUser, oauthPassword) : new NetworkCredential(oauthUser, oauthPassword, oauthDomain);
          return TokenProvider.CreateOAuthTokenProvider(stsEndpoints, credential);
        }
        if (!flag1)
          return TokenProvider.CreateWindowsTokenProvider(stsEndpoints);
        NetworkCredential credential1 = string.IsNullOrWhiteSpace(windowsDomain) ? new NetworkCredential(windowsUser, windowsPassword) : new NetworkCredential(windowsUser, windowsPassword, windowsDomain);
        return TokenProvider.CreateWindowsTokenProvider(stsEndpoints, credential1);
      }
      return stsEndpoints != null && stsEndpoints.Any<Uri>() ? TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerKey, stsEndpoints.First<Uri>()) : TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerKey);
    }

    public void Validate()
    {
      if (string.IsNullOrWhiteSpace(this.connectionProperties["Endpoint"]))
        throw new ConfigurationErrorsException(SRClient.AppSettingsConfigMissingSetting((object) "Endpoint", (object) "Microsoft.Azure.NotificationHubs.ConnectionString"));
      int num = !string.IsNullOrWhiteSpace(this.connectionProperties["SharedSecretIssuer"]) ? 1 : 0;
      bool flag1 = !string.IsNullOrWhiteSpace(this.connectionProperties["SharedSecretValue"]);
      if (num != 0 && !flag1)
        throw new ConfigurationErrorsException(SRClient.AppSettingsConfigMissingSetting((object) "SharedSecretValue", (object) "Microsoft.Azure.NotificationHubs.ConnectionString"));
      if (num == 0 & flag1)
        throw new ConfigurationErrorsException(SRClient.AppSettingsConfigMissingSetting((object) "SharedSecretIssuer", (object) "Microsoft.Azure.NotificationHubs.ConnectionString"));
      bool flag2 = !string.IsNullOrWhiteSpace(this.connectionProperties["WindowsUsername"]);
      bool flag3 = !string.IsNullOrWhiteSpace(this.connectionProperties["WindowsPassword"]);
      if (!(flag2 & flag3) && (flag2 || flag3))
        throw new ConfigurationErrorsException(SRClient.AppSettingsConfigIncompleteSettingCombination((object) "Microsoft.Azure.NotificationHubs.ConnectionString", (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0},{1}", new object[2]
        {
          (object) "WindowsUsername",
          (object) "WindowsPassword"
        })));
      bool flag4 = !string.IsNullOrWhiteSpace(this.connectionProperties["OAuthUsername"]);
      bool flag5 = !string.IsNullOrWhiteSpace(this.connectionProperties["OAuthPassword"]);
      if (!(flag4 & flag5) && (flag4 || flag5))
        throw new ConfigurationErrorsException(SRClient.AppSettingsConfigIncompleteSettingCombination((object) "Microsoft.Azure.NotificationHubs.ConnectionString", (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0},{1}", new object[2]
        {
          (object) "OAuthUsername",
          (object) "OAuthPassword"
        })));
      string connectionProperty1 = this.connectionProperties["OperationTimeout"];
      if (!string.IsNullOrWhiteSpace(connectionProperty1) && !TimeSpan.TryParse(connectionProperty1, (IFormatProvider) CultureInfo.CurrentCulture, out TimeSpan _))
        throw new ConfigurationErrorsException(SRClient.AppSettingsConfigSettingInvalidValue((object) "OperationTimeout", (object) connectionProperty1));
      string connectionProperty2 = this.connectionProperties["RuntimePort"];
      if (!string.IsNullOrWhiteSpace(connectionProperty2) && !int.TryParse(connectionProperty2, out int _))
        throw new ConfigurationErrorsException(SRClient.AppSettingsConfigSettingInvalidValue((object) "RuntimePort", (object) connectionProperty2));
      string connectionProperty3 = this.connectionProperties["ManagementPort"];
      if (!string.IsNullOrWhiteSpace(connectionProperty3) && !int.TryParse(connectionProperty3, out int _))
        throw new ConfigurationErrorsException(SRClient.AppSettingsConfigSettingInvalidValue((object) "ManagementPort", (object) connectionProperty3));
    }
  }
}
