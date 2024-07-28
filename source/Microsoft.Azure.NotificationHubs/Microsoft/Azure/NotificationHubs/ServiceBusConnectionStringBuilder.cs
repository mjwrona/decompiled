// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.ServiceBusConnectionStringBuilder
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Azure.NotificationHubs.Messaging.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;

namespace Microsoft.Azure.NotificationHubs
{
  public class ServiceBusConnectionStringBuilder
  {
    public ServiceBusConnectionStringBuilder()
    {
      this.WindowsCredentialPassword = (SecureString) null;
      this.RuntimePort = -1;
      this.ManagementPort = -1;
      this.Endpoints = new HashSet<Uri>();
      this.StsEndpoints = new HashSet<Uri>();
      this.OperationTimeout = Constants.DefaultOperationTimeout;
    }

    public ServiceBusConnectionStringBuilder(string connectionString)
      : this()
    {
      if (string.IsNullOrWhiteSpace(connectionString))
        return;
      this.InitializeFromString(connectionString);
    }

    internal ServiceBusConnectionStringBuilder(KeyValueConfigurationManager keyValueManager)
      : this()
    {
      if (keyValueManager == null)
        return;
      this.InitializeFromKeyValueManager(keyValueManager);
    }

    public HashSet<Uri> Endpoints { get; private set; }

    public HashSet<Uri> StsEndpoints { get; private set; }

    public TimeSpan OperationTimeout { get; set; }

    public int RuntimePort { get; set; }

    public int ManagementPort { get; set; }

    public string SharedSecretIssuerName { get; set; }

    public string SharedSecretIssuerSecret { get; set; }

    public string SharedAccessKeyName { get; set; }

    public string SharedAccessKey { get; set; }

    public string WindowsCredentialDomain { get; set; }

    public string WindowsCredentialUsername { get; set; }

    public SecureString WindowsCredentialPassword { get; set; }

    public string OAuthUsername { get; set; }

    public string OAuthDomain { get; set; }

    public SecureString OAuthPassword { get; set; }

    public IList<Uri> GetAbsoluteManagementEndpoints() => this.GetAbsoluteEndpoints(this.ManagementPort);

    public IList<Uri> GetAbsoluteRuntimeEndpoints() => this.GetAbsoluteEndpoints(this.RuntimePort, "sb");

    public override string ToString()
    {
      this.Validate();
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      string str1 = string.Empty;
      string empty = string.Empty;
      foreach (Uri endpoint in this.Endpoints)
      {
        UriBuilder uriBuilder = new UriBuilder(endpoint)
        {
          Scheme = "sb",
          Port = -1
        };
        stringBuilder2.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", new object[2]
        {
          (object) str1,
          (object) uriBuilder.Uri.AbsoluteUri
        }));
        str1 = ",";
      }
      stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) empty, (object) "Endpoint", (object) "=", (object) stringBuilder2));
      stringBuilder2.Clear();
      string str2 = ";";
      string str3 = string.Empty;
      if (this.StsEndpoints.Count > 0)
      {
        foreach (Uri stsEndpoint in this.StsEndpoints)
        {
          stringBuilder2.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", new object[2]
          {
            (object) str3,
            (object) stsEndpoint.AbsoluteUri
          }));
          str3 = ",";
        }
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) str2, (object) "StsEndpoint", (object) "=", (object) stringBuilder2));
        stringBuilder2.Clear();
      }
      if (this.RuntimePort > 0)
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) str2, (object) "RuntimePort", (object) "=", (object) this.RuntimePort));
      if (this.ManagementPort > 0)
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) str2, (object) "ManagementPort", (object) "=", (object) this.ManagementPort));
      if (this.OperationTimeout != Constants.DefaultOperationTimeout)
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) str2, (object) "OperationTimeout", (object) "=", (object) this.OperationTimeout));
      if (!string.IsNullOrWhiteSpace(this.SharedSecretIssuerName))
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) str2, (object) "SharedSecretIssuer", (object) "=", (object) this.SharedSecretIssuerName));
      if (!string.IsNullOrWhiteSpace(this.SharedSecretIssuerSecret))
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) str2, (object) "SharedSecretValue", (object) "=", (object) this.SharedSecretIssuerSecret));
      if (!string.IsNullOrWhiteSpace(this.SharedAccessKeyName))
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) str2, (object) "SharedAccessKeyName", (object) "=", (object) this.SharedAccessKeyName));
      if (!string.IsNullOrWhiteSpace(this.SharedAccessKey))
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) str2, (object) "SharedAccessKey", (object) "=", (object) this.SharedAccessKey));
      if (!string.IsNullOrWhiteSpace(this.WindowsCredentialUsername))
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) str2, (object) "WindowsUsername", (object) "=", (object) this.WindowsCredentialUsername));
      if (!string.IsNullOrWhiteSpace(this.WindowsCredentialDomain))
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) str2, (object) "WindowsDomain", (object) "=", (object) this.WindowsCredentialDomain));
      if (this.WindowsCredentialPassword != null && this.WindowsCredentialPassword.Length > 0)
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) str2, (object) "WindowsPassword", (object) "=", (object) this.WindowsCredentialPassword.ConvertToString()));
      if (!string.IsNullOrWhiteSpace(this.OAuthUsername))
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) str2, (object) "OAuthUsername", (object) "=", (object) this.OAuthUsername));
      if (!string.IsNullOrWhiteSpace(this.OAuthDomain))
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) str2, (object) "OAuthDomain", (object) "=", (object) this.OAuthDomain));
      if (this.OAuthPassword != null && this.OAuthPassword.Length > 0)
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) str2, (object) "OAuthPassword", (object) "=", (object) this.OAuthPassword.ConvertToString()));
      return stringBuilder1.ToString();
    }

    private void Validate()
    {
      if (this.Endpoints.Count == 0)
        throw Fx.Exception.ArgumentNullOrEmpty("Endpoints");
      bool flag1 = !string.IsNullOrWhiteSpace(this.SharedSecretIssuerName);
      bool flag2 = !string.IsNullOrWhiteSpace(this.SharedSecretIssuerSecret);
      if (!(flag1 & flag2) && (flag1 || flag2))
        throw Fx.Exception.Argument("SharedSecretIssuerName, SharedSecretIssuerSecret", SRClient.ArgumentInvalidCombination((object) "SharedSecretIssuerName, SharedSecretIssuerSecret"));
      bool flag3 = !string.IsNullOrWhiteSpace(this.SharedAccessKeyName);
      bool flag4 = !string.IsNullOrWhiteSpace(this.SharedAccessKey);
      if (!(flag3 & flag4) && (flag3 || flag4))
        throw Fx.Exception.Argument("SharedAccessKeyName, SharedAccessSecret", SRClient.ArgumentInvalidCombination((object) "SharedAccessKeyName, SharedAccessSecret"));
      bool flag5 = !string.IsNullOrWhiteSpace(this.WindowsCredentialUsername);
      bool flag6 = this.WindowsCredentialPassword != null && this.WindowsCredentialPassword.Length > 0;
      if (!(flag5 & flag6) && (flag5 || flag6))
        throw Fx.Exception.Argument("WindowsCredentialUsername, WindowsCredentialPassword", SRClient.ArgumentInvalidCombination((object) "WindowsCredentialUsername, WindowsCredentialPassword"));
      bool flag7 = !string.IsNullOrWhiteSpace(this.OAuthUsername);
      bool flag8 = this.OAuthPassword != null && this.OAuthPassword.Length > 0;
      if (!(flag7 & flag8) && (flag7 || flag8))
        throw Fx.Exception.Argument("OAuthUsername, OAuthPassword", SRClient.ArgumentInvalidCombination((object) "OAuthUsername, OAuthPassword"));
    }

    private IList<Uri> GetAbsoluteEndpoints(int port, string uriScheme = null)
    {
      IList<Uri> absoluteEndpoints = (IList<Uri>) new List<Uri>();
      foreach (Uri endpoint in this.Endpoints)
      {
        UriBuilder uriBuilder = new UriBuilder(endpoint);
        if (port >= 0)
          uriBuilder.Port = port;
        if (!string.IsNullOrWhiteSpace(uriScheme))
          uriBuilder.Scheme = uriScheme;
        absoluteEndpoints.Add(uriBuilder.Uri);
      }
      return absoluteEndpoints;
    }

    private void InitializeFromString(string connection) => this.InitializeFromKeyValueManager(new KeyValueConfigurationManager(connection));

    private void InitializeFromKeyValueManager(KeyValueConfigurationManager manager)
    {
      try
      {
        manager.Validate();
        foreach (Uri endpointAddress in (IEnumerable<Uri>) KeyValueConfigurationManager.GetEndpointAddresses(manager.connectionProperties["Endpoint"], string.Empty))
          this.Endpoints.Add(endpointAddress);
        foreach (Uri endpointAddress in (IEnumerable<Uri>) KeyValueConfigurationManager.GetEndpointAddresses(manager.connectionProperties["StsEndpoint"], string.Empty))
          this.StsEndpoints.Add(endpointAddress);
        int result1;
        if (int.TryParse(manager.connectionProperties["RuntimePort"], out result1))
          this.RuntimePort = result1;
        int result2;
        if (int.TryParse(manager.connectionProperties["ManagementPort"], out result2))
          this.ManagementPort = result2;
        string connectionProperty1 = manager.connectionProperties["OperationTimeout"];
        TimeSpan result3;
        if (!string.IsNullOrWhiteSpace(connectionProperty1) && TimeSpan.TryParse(connectionProperty1, (IFormatProvider) CultureInfo.CurrentCulture, out result3) && !result3.Equals(this.OperationTimeout))
          this.OperationTimeout = result3;
        string connectionProperty2 = manager.connectionProperties["SharedSecretIssuer"];
        if (!string.IsNullOrWhiteSpace(connectionProperty2))
          this.SharedSecretIssuerName = connectionProperty2;
        string connectionProperty3 = manager.connectionProperties["SharedSecretValue"];
        if (!string.IsNullOrWhiteSpace(connectionProperty3))
          this.SharedSecretIssuerSecret = connectionProperty3;
        string connectionProperty4 = manager.connectionProperties["SharedAccessKeyName"];
        if (!string.IsNullOrWhiteSpace(connectionProperty4))
          this.SharedAccessKeyName = connectionProperty4;
        string connectionProperty5 = manager.connectionProperties["SharedAccessKey"];
        if (!string.IsNullOrWhiteSpace(connectionProperty5))
          this.SharedAccessKey = connectionProperty5;
        string connectionProperty6 = manager.connectionProperties["WindowsDomain"];
        if (!string.IsNullOrWhiteSpace(connectionProperty6))
          this.WindowsCredentialDomain = connectionProperty6;
        string connectionProperty7 = manager.connectionProperties["WindowsUsername"];
        if (!string.IsNullOrWhiteSpace(connectionProperty7))
          this.WindowsCredentialUsername = connectionProperty7;
        this.WindowsCredentialPassword = manager.GetWindowsPassword();
        string connectionProperty8 = manager.connectionProperties["OAuthDomain"];
        if (!string.IsNullOrWhiteSpace(connectionProperty8))
          this.OAuthDomain = connectionProperty8;
        string connectionProperty9 = manager.connectionProperties["OAuthUsername"];
        if (!string.IsNullOrWhiteSpace(connectionProperty9))
          this.OAuthUsername = connectionProperty9;
        this.OAuthPassword = manager.GetOAuthPassword();
      }
      catch (Exception ex)
      {
        throw new ArgumentException(ex.Message, "connectionString", ex);
      }
    }

    public static string CreateUsingSharedAccessKey(Uri endpoint, string keyName, string key)
    {
      if (endpoint == (Uri) null)
        throw new ArgumentNullException(nameof (endpoint));
      if (string.IsNullOrWhiteSpace(keyName))
        throw new ArgumentNullException(nameof (keyName));
      if (string.IsNullOrWhiteSpace(key))
        throw new ArgumentNullException(nameof (key));
      return ServiceBusConnectionStringBuilder.CreateUsingSharedAccessKey((IEnumerable<Uri>) new Uri[1]
      {
        endpoint
      }, -1, -1, keyName, key);
    }

    public static string CreateUsingSharedAccessKey(
      IEnumerable<Uri> endpoints,
      int runtimePort,
      int managementPort,
      string keyName,
      string key)
    {
      if (!(endpoints is Uri[] uriArray))
        uriArray = endpoints.ToArray<Uri>();
      Uri[] source = uriArray;
      if (endpoints == null || !((IEnumerable<Uri>) source).Any<Uri>())
        throw new ArgumentNullException(nameof (endpoints));
      if (string.IsNullOrWhiteSpace(keyName))
        throw new ArgumentNullException(nameof (keyName));
      if (string.IsNullOrWhiteSpace(key))
        throw new ArgumentNullException(nameof (key));
      ServiceBusConnectionStringBuilder connectionStringBuilder = new ServiceBusConnectionStringBuilder();
      foreach (Uri uri in source)
        connectionStringBuilder.Endpoints.Add(uri);
      connectionStringBuilder.RuntimePort = runtimePort;
      connectionStringBuilder.ManagementPort = managementPort;
      connectionStringBuilder.SharedAccessKeyName = keyName;
      connectionStringBuilder.SharedAccessKey = key;
      return connectionStringBuilder.ToString();
    }

    public static string CreateUsingSharedSecret(Uri endpoint, string issuer, string issuerSecret)
    {
      if (endpoint == (Uri) null)
        throw new ArgumentNullException(nameof (endpoint));
      if (string.IsNullOrWhiteSpace(issuer))
        throw new ArgumentNullException(nameof (issuer));
      if (string.IsNullOrWhiteSpace(issuerSecret))
        throw new ArgumentNullException(nameof (issuerSecret));
      return ServiceBusConnectionStringBuilder.CreateUsingSharedSecret((IEnumerable<Uri>) new Uri[1]
      {
        endpoint
      }, (IEnumerable<Uri>) null, -1, -1, issuer, issuerSecret);
    }

    public static string CreateUsingSharedSecret(
      IEnumerable<Uri> endpoints,
      IEnumerable<Uri> stsEndpoints,
      int runtimePort,
      int managementPort,
      string issuer,
      string issuerSecret)
    {
      if (!(endpoints is Uri[] uriArray))
        uriArray = endpoints.ToArray<Uri>();
      Uri[] source = uriArray;
      if (endpoints == null || !((IEnumerable<Uri>) source).Any<Uri>())
        throw new ArgumentNullException(nameof (endpoints));
      if (string.IsNullOrWhiteSpace(issuer))
        throw new ArgumentNullException(nameof (issuer));
      if (string.IsNullOrWhiteSpace(issuerSecret))
        throw new ArgumentNullException(nameof (issuerSecret));
      ServiceBusConnectionStringBuilder connectionStringBuilder = new ServiceBusConnectionStringBuilder();
      foreach (Uri uri in source)
        connectionStringBuilder.Endpoints.Add(uri);
      if (stsEndpoints != null)
      {
        foreach (Uri stsEndpoint in stsEndpoints)
          connectionStringBuilder.StsEndpoints.Add(stsEndpoint);
      }
      connectionStringBuilder.RuntimePort = runtimePort;
      connectionStringBuilder.ManagementPort = managementPort;
      connectionStringBuilder.SharedSecretIssuerName = issuer;
      connectionStringBuilder.SharedSecretIssuerSecret = issuerSecret;
      return connectionStringBuilder.ToString();
    }

    public static string CreateUsingWindowsCredential(
      IEnumerable<Uri> endpoints,
      IEnumerable<Uri> stsEndpoints,
      int runtimePort,
      int managementPort,
      string domain,
      string user,
      SecureString password)
    {
      if (!(endpoints is Uri[] uriArray))
        uriArray = endpoints.ToArray<Uri>();
      Uri[] source = uriArray;
      if (endpoints == null || !((IEnumerable<Uri>) source).Any<Uri>())
        throw new ArgumentNullException(nameof (endpoints));
      if (string.IsNullOrWhiteSpace(user))
        throw new ArgumentNullException(nameof (user));
      if (password == null || password.Length == 0)
        throw new ArgumentNullException(nameof (password));
      ServiceBusConnectionStringBuilder connectionStringBuilder = new ServiceBusConnectionStringBuilder();
      foreach (Uri uri in source)
        connectionStringBuilder.Endpoints.Add(uri);
      if (stsEndpoints != null)
      {
        foreach (Uri stsEndpoint in stsEndpoints)
          connectionStringBuilder.StsEndpoints.Add(stsEndpoint);
      }
      connectionStringBuilder.RuntimePort = runtimePort;
      connectionStringBuilder.ManagementPort = managementPort;
      connectionStringBuilder.WindowsCredentialUsername = user;
      connectionStringBuilder.WindowsCredentialPassword = password;
      if (!string.IsNullOrWhiteSpace(domain))
        connectionStringBuilder.WindowsCredentialDomain = domain;
      return connectionStringBuilder.ToString();
    }

    public static string CreateUsingOAuthCredential(
      IEnumerable<Uri> endpoints,
      IEnumerable<Uri> stsEndpoints,
      int runtimePort,
      int managementPort,
      string domain,
      string user,
      SecureString password)
    {
      if (!(endpoints is Uri[] uriArray))
        uriArray = endpoints.ToArray<Uri>();
      Uri[] source = uriArray;
      if (endpoints == null || !((IEnumerable<Uri>) source).Any<Uri>())
        throw new ArgumentNullException(nameof (endpoints));
      if (string.IsNullOrWhiteSpace(user))
        throw new ArgumentNullException(nameof (user));
      if (password == null || password.Length == 0)
        throw new ArgumentNullException(nameof (password));
      ServiceBusConnectionStringBuilder connectionStringBuilder = new ServiceBusConnectionStringBuilder();
      foreach (Uri uri in source)
        connectionStringBuilder.Endpoints.Add(uri);
      if (stsEndpoints != null)
      {
        foreach (Uri stsEndpoint in stsEndpoints)
          connectionStringBuilder.StsEndpoints.Add(stsEndpoint);
      }
      connectionStringBuilder.RuntimePort = runtimePort;
      connectionStringBuilder.ManagementPort = managementPort;
      connectionStringBuilder.OAuthUsername = user;
      connectionStringBuilder.OAuthPassword = password;
      if (!string.IsNullOrWhiteSpace(domain))
        connectionStringBuilder.OAuthDomain = domain;
      return connectionStringBuilder.ToString();
    }
  }
}
