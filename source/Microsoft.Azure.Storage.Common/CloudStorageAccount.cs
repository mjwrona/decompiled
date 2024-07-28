// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.CloudStorageAccount
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Storage
{
  public class CloudStorageAccount
  {
    private static bool version1MD5 = true;
    internal const string UseDevelopmentStorageSettingString = "UseDevelopmentStorage";
    internal const string DevelopmentStorageProxyUriSettingString = "DevelopmentStorageProxyUri";
    internal const string DefaultEndpointsProtocolSettingString = "DefaultEndpointsProtocol";
    internal const string AccountNameSettingString = "AccountName";
    internal const string AccountKeyNameSettingString = "AccountKeyName";
    internal const string AccountKeySettingString = "AccountKey";
    internal const string BlobEndpointSettingString = "BlobEndpoint";
    internal const string QueueEndpointSettingString = "QueueEndpoint";
    internal const string TableEndpointSettingString = "TableEndpoint";
    internal const string FileEndpointSettingString = "FileEndpoint";
    internal const string BlobSecondaryEndpointSettingString = "BlobSecondaryEndpoint";
    internal const string QueueSecondaryEndpointSettingString = "QueueSecondaryEndpoint";
    internal const string TableSecondaryEndpointSettingString = "TableSecondaryEndpoint";
    internal const string FileSecondaryEndpointSettingString = "FileSecondaryEndpoint";
    internal const string EndpointSuffixSettingString = "EndpointSuffix";
    internal const string SharedAccessSignatureSettingString = "SharedAccessSignature";
    private const string DevstoreAccountName = "devstoreaccount1";
    private const string DevstoreAccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";
    internal const string SecondaryLocationAccountSuffix = "-secondary";
    private const string DefaultEndpointSuffix = "core.windows.net";
    private const string DefaultBlobHostnamePrefix = "blob";
    private const string DefaultQueueHostnamePrefix = "queue";
    private const string DefaultTableHostnamePrefix = "table";
    private const string DefaultFileHostnamePrefix = "file";
    private static readonly KeyValuePair<string, Func<string, bool>> UseDevelopmentStorageSetting = CloudStorageAccount.Setting("UseDevelopmentStorage", "true");
    private static readonly KeyValuePair<string, Func<string, bool>> DevelopmentStorageProxyUriSetting = CloudStorageAccount.Setting("DevelopmentStorageProxyUri", new Func<string, bool>(CloudStorageAccount.IsValidUri));
    private static readonly KeyValuePair<string, Func<string, bool>> DefaultEndpointsProtocolSetting = CloudStorageAccount.Setting("DefaultEndpointsProtocol", "http", "https");
    private static readonly KeyValuePair<string, Func<string, bool>> AccountNameSetting = CloudStorageAccount.Setting("AccountName");
    private static readonly KeyValuePair<string, Func<string, bool>> AccountKeyNameSetting = CloudStorageAccount.Setting("AccountKeyName");
    private static readonly KeyValuePair<string, Func<string, bool>> AccountKeySetting = CloudStorageAccount.Setting("AccountKey", new Func<string, bool>(CloudStorageAccount.IsValidBase64String));
    private static readonly KeyValuePair<string, Func<string, bool>> BlobEndpointSetting = CloudStorageAccount.Setting(nameof (BlobEndpoint), new Func<string, bool>(CloudStorageAccount.IsValidUri));
    private static readonly KeyValuePair<string, Func<string, bool>> QueueEndpointSetting = CloudStorageAccount.Setting(nameof (QueueEndpoint), new Func<string, bool>(CloudStorageAccount.IsValidUri));
    private static readonly KeyValuePair<string, Func<string, bool>> TableEndpointSetting = CloudStorageAccount.Setting(nameof (TableEndpoint), new Func<string, bool>(CloudStorageAccount.IsValidUri));
    private static readonly KeyValuePair<string, Func<string, bool>> FileEndpointSetting = CloudStorageAccount.Setting(nameof (FileEndpoint), new Func<string, bool>(CloudStorageAccount.IsValidUri));
    private static readonly KeyValuePair<string, Func<string, bool>> BlobSecondaryEndpointSetting = CloudStorageAccount.Setting("BlobSecondaryEndpoint", new Func<string, bool>(CloudStorageAccount.IsValidUri));
    private static readonly KeyValuePair<string, Func<string, bool>> QueueSecondaryEndpointSetting = CloudStorageAccount.Setting("QueueSecondaryEndpoint", new Func<string, bool>(CloudStorageAccount.IsValidUri));
    private static readonly KeyValuePair<string, Func<string, bool>> TableSecondaryEndpointSetting = CloudStorageAccount.Setting("TableSecondaryEndpoint", new Func<string, bool>(CloudStorageAccount.IsValidUri));
    private static readonly KeyValuePair<string, Func<string, bool>> FileSecondaryEndpointSetting = CloudStorageAccount.Setting("FileSecondaryEndpoint", new Func<string, bool>(CloudStorageAccount.IsValidUri));
    private static readonly KeyValuePair<string, Func<string, bool>> EndpointSuffixSetting = CloudStorageAccount.Setting(nameof (EndpointSuffix), new Func<string, bool>(CloudStorageAccount.IsValidDomain));
    private static readonly KeyValuePair<string, Func<string, bool>> SharedAccessSignatureSetting = CloudStorageAccount.Setting("SharedAccessSignature");
    private static CloudStorageAccount devStoreAccount;
    private string accountName;
    private static Func<IDictionary<string, string>, IDictionary<string, string>> ValidCredentials = CloudStorageAccount.MatchesOne(CloudStorageAccount.MatchesAll(CloudStorageAccount.AllRequired(CloudStorageAccount.AccountNameSetting, CloudStorageAccount.AccountKeySetting), CloudStorageAccount.Optional(CloudStorageAccount.AccountKeyNameSetting), CloudStorageAccount.None(CloudStorageAccount.SharedAccessSignatureSetting)), CloudStorageAccount.MatchesAll(CloudStorageAccount.AllRequired(CloudStorageAccount.SharedAccessSignatureSetting), CloudStorageAccount.Optional(CloudStorageAccount.AccountNameSetting), CloudStorageAccount.None(CloudStorageAccount.AccountKeySetting, CloudStorageAccount.AccountKeyNameSetting)), CloudStorageAccount.None(CloudStorageAccount.AccountNameSetting, CloudStorageAccount.AccountKeySetting, CloudStorageAccount.AccountKeyNameSetting, CloudStorageAccount.SharedAccessSignatureSetting));

    public static bool UseV1MD5
    {
      get => CloudStorageAccount.version1MD5;
      set => CloudStorageAccount.version1MD5 = value;
    }

    public CloudStorageAccount(
      StorageCredentials storageCredentials,
      Uri blobEndpoint,
      Uri queueEndpoint,
      Uri tableEndpoint,
      Uri fileEndpoint)
      : this(storageCredentials, new StorageUri(blobEndpoint), new StorageUri(queueEndpoint), new StorageUri(tableEndpoint), new StorageUri(fileEndpoint))
    {
    }

    public CloudStorageAccount(
      StorageCredentials storageCredentials,
      StorageUri blobStorageUri,
      StorageUri queueStorageUri,
      StorageUri tableStorageUri,
      StorageUri fileStorageUri)
    {
      this.Credentials = storageCredentials;
      this.BlobStorageUri = blobStorageUri;
      this.QueueStorageUri = queueStorageUri;
      this.TableStorageUri = tableStorageUri;
      this.FileStorageUri = fileStorageUri;
      this.DefaultEndpoints = false;
    }

    public CloudStorageAccount(StorageCredentials storageCredentials, bool useHttps)
      : this(storageCredentials, (string) null, useHttps)
    {
    }

    public CloudStorageAccount(
      StorageCredentials storageCredentials,
      string endpointSuffix,
      bool useHttps)
      : this(storageCredentials, storageCredentials == null ? (string) null : storageCredentials.AccountName, endpointSuffix, useHttps)
    {
    }

    public CloudStorageAccount(
      StorageCredentials storageCredentials,
      string accountName,
      string endpointSuffix,
      bool useHttps)
    {
      CommonUtility.AssertNotNull(nameof (storageCredentials), (object) storageCredentials);
      if (!string.IsNullOrEmpty(storageCredentials.AccountName))
      {
        if (string.IsNullOrEmpty(accountName))
          accountName = storageCredentials.AccountName;
        else if (string.Compare(storageCredentials.AccountName, accountName, StringComparison.Ordinal) != 0)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Account names do not match.  First account name is {0}, second is {1}.", (object) storageCredentials.AccountName, (object) accountName));
      }
      CommonUtility.AssertNotNull("AccountName", (object) accountName);
      string scheme = useHttps ? "https" : "http";
      this.BlobStorageUri = CloudStorageAccount.ConstructBlobEndpoint(scheme, accountName, endpointSuffix);
      this.QueueStorageUri = CloudStorageAccount.ConstructQueueEndpoint(scheme, accountName, endpointSuffix);
      this.TableStorageUri = CloudStorageAccount.ConstructTableEndpoint(scheme, accountName, endpointSuffix);
      this.FileStorageUri = CloudStorageAccount.ConstructFileEndpoint(scheme, accountName, endpointSuffix);
      this.Credentials = storageCredentials;
      this.EndpointSuffix = endpointSuffix;
      this.DefaultEndpoints = true;
    }

    public static CloudStorageAccount DevelopmentStorageAccount
    {
      get
      {
        if (CloudStorageAccount.devStoreAccount == null)
          CloudStorageAccount.devStoreAccount = CloudStorageAccount.GetDevelopmentStorageAccount((Uri) null);
        return CloudStorageAccount.devStoreAccount;
      }
    }

    private bool IsDevStoreAccount { get; set; }

    private string EndpointSuffix { get; set; }

    private IDictionary<string, string> Settings { get; set; }

    private bool DefaultEndpoints { get; set; }

    public Uri BlobEndpoint => this.BlobStorageUri == (StorageUri) null ? (Uri) null : this.BlobStorageUri.PrimaryUri;

    public Uri QueueEndpoint => this.QueueStorageUri == (StorageUri) null ? (Uri) null : this.QueueStorageUri.PrimaryUri;

    public Uri TableEndpoint => this.TableStorageUri == (StorageUri) null ? (Uri) null : this.TableStorageUri.PrimaryUri;

    public Uri FileEndpoint => this.FileStorageUri == (StorageUri) null ? (Uri) null : this.FileStorageUri.PrimaryUri;

    public StorageUri BlobStorageUri { get; private set; }

    public StorageUri QueueStorageUri { get; private set; }

    public StorageUri TableStorageUri { get; private set; }

    public StorageUri FileStorageUri { get; private set; }

    public StorageCredentials Credentials { get; private set; }

    public static CloudStorageAccount Parse(string connectionString)
    {
      if (string.IsNullOrEmpty(connectionString))
        throw new ArgumentNullException(nameof (connectionString));
      CloudStorageAccount accountInformation;
      if (CloudStorageAccount.ParseImpl(connectionString, out accountInformation, (Action<string>) (err =>
      {
        throw new FormatException(err);
      })))
        return accountInformation;
      throw new ArgumentException("Error parsing value");
    }

    public static bool TryParse(string connectionString, out CloudStorageAccount account)
    {
      if (string.IsNullOrEmpty(connectionString))
      {
        account = (CloudStorageAccount) null;
        return false;
      }
      try
      {
        return CloudStorageAccount.ParseImpl(connectionString, out account, (Action<string>) (err => { }));
      }
      catch (Exception ex)
      {
        account = (CloudStorageAccount) null;
        return false;
      }
    }

    public string GetSharedAccessSignature(SharedAccessAccountPolicy policy)
    {
      StorageAccountKey storageAccountKey = this.Credentials.IsSharedKey ? this.Credentials.Key : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot create Shared Access Signature unless Account Key credentials are used."));
      string hash = SharedAccessSignatureHelper.GetHash(policy, this.Credentials.AccountName, "2019-07-07", storageAccountKey.KeyValue);
      return SharedAccessSignatureHelper.GetSignature(policy, hash, storageAccountKey.KeyName, "2019-07-07").ToString();
    }

    public override string ToString() => this.ToString(false);

    public string ToString(bool exportSecrets)
    {
      if (this.Settings == null)
      {
        this.Settings = (IDictionary<string, string>) new Dictionary<string, string>();
        if (this.DefaultEndpoints)
        {
          this.Settings.Add("DefaultEndpointsProtocol", this.BlobEndpoint.Scheme);
          if (this.EndpointSuffix != null)
            this.Settings.Add("EndpointSuffix", this.EndpointSuffix);
        }
        else
        {
          if (this.BlobEndpoint != (Uri) null)
            this.Settings.Add("BlobEndpoint", this.BlobEndpoint.ToString());
          if (this.QueueEndpoint != (Uri) null)
            this.Settings.Add("QueueEndpoint", this.QueueEndpoint.ToString());
          if (this.TableEndpoint != (Uri) null)
            this.Settings.Add("TableEndpoint", this.TableEndpoint.ToString());
          if (this.FileEndpoint != (Uri) null)
            this.Settings.Add("FileEndpoint", this.FileEndpoint.ToString());
        }
      }
      List<string> list = this.Settings.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (pair => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", (object) pair.Key, (object) pair.Value))).ToList<string>();
      if (this.Credentials != null && !this.IsDevStoreAccount)
        list.Add(this.Credentials.ToString(exportSecrets));
      if (!string.IsNullOrWhiteSpace(this.accountName) && (this.Credentials != null ? (string.IsNullOrWhiteSpace(this.Credentials.AccountName) ? 1 : 0) : 1) != 0)
        list.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", (object) "AccountName", (object) this.accountName));
      return string.Join(";", (IEnumerable<string>) list);
    }

    private static CloudStorageAccount GetDevelopmentStorageAccount(Uri proxyUri)
    {
      UriBuilder uriBuilder = proxyUri != (Uri) null ? new UriBuilder(proxyUri.Scheme, proxyUri.Host) : new UriBuilder("http", "127.0.0.1");
      uriBuilder.Path = "devstoreaccount1";
      uriBuilder.Port = 10000;
      Uri uri1 = uriBuilder.Uri;
      uriBuilder.Port = 10001;
      Uri uri2 = uriBuilder.Uri;
      uriBuilder.Port = 10002;
      Uri uri3 = uriBuilder.Uri;
      uriBuilder.Path = "devstoreaccount1-secondary";
      uriBuilder.Port = 10000;
      Uri uri4 = uriBuilder.Uri;
      uriBuilder.Port = 10001;
      Uri uri5 = uriBuilder.Uri;
      uriBuilder.Port = 10002;
      Uri uri6 = uriBuilder.Uri;
      CloudStorageAccount developmentStorageAccount = new CloudStorageAccount(new StorageCredentials("devstoreaccount1", "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="), new StorageUri(uri1, uri4), new StorageUri(uri2, uri5), new StorageUri(uri3, uri6), (StorageUri) null)
      {
        Settings = (IDictionary<string, string>) new Dictionary<string, string>()
      };
      developmentStorageAccount.Settings.Add("UseDevelopmentStorage", "true");
      if (proxyUri != (Uri) null)
        developmentStorageAccount.Settings.Add("DevelopmentStorageProxyUri", proxyUri.ToString());
      developmentStorageAccount.IsDevStoreAccount = true;
      return developmentStorageAccount;
    }

    internal static bool ParseImpl(
      string connectionString,
      out CloudStorageAccount accountInformation,
      Action<string> error)
    {
      IDictionary<string, string> settings = CloudStorageAccount.ParseStringIntoSettings(connectionString, error);
      if (settings == null)
      {
        accountInformation = (CloudStorageAccount) null;
        return false;
      }
      Func<string, string> func1 = (Func<string, string>) (key =>
      {
        string impl = (string) null;
        settings.TryGetValue(key, out impl);
        return impl;
      });
      if (CloudStorageAccount.MatchesSpecification(settings, CloudStorageAccount.AllRequired(CloudStorageAccount.UseDevelopmentStorageSetting), CloudStorageAccount.Optional(CloudStorageAccount.DevelopmentStorageProxyUriSetting)))
      {
        string uriString;
        accountInformation = !settings.TryGetValue("DevelopmentStorageProxyUri", out uriString) ? CloudStorageAccount.DevelopmentStorageAccount : CloudStorageAccount.GetDevelopmentStorageAccount(new Uri(uriString));
        accountInformation.Settings = CloudStorageAccount.ValidCredentials(settings);
        return true;
      }
      Func<IDictionary<string, string>, IDictionary<string, string>> func2 = CloudStorageAccount.Optional(CloudStorageAccount.BlobEndpointSetting, CloudStorageAccount.BlobSecondaryEndpointSetting, CloudStorageAccount.QueueEndpointSetting, CloudStorageAccount.QueueSecondaryEndpointSetting, CloudStorageAccount.TableEndpointSetting, CloudStorageAccount.TableSecondaryEndpointSetting, CloudStorageAccount.FileEndpointSetting, CloudStorageAccount.FileSecondaryEndpointSetting);
      Func<IDictionary<string, string>, IDictionary<string, string>> func3 = CloudStorageAccount.AtLeastOne(CloudStorageAccount.BlobEndpointSetting, CloudStorageAccount.QueueEndpointSetting, CloudStorageAccount.TableEndpointSetting, CloudStorageAccount.FileEndpointSetting);
      Func<IDictionary<string, string>, IDictionary<string, string>> func4 = CloudStorageAccount.Optional(CloudStorageAccount.BlobSecondaryEndpointSetting, CloudStorageAccount.QueueSecondaryEndpointSetting, CloudStorageAccount.TableSecondaryEndpointSetting, CloudStorageAccount.FileSecondaryEndpointSetting);
      Func<IDictionary<string, string>, IDictionary<string, string>> func5 = CloudStorageAccount.MatchesExactly(CloudStorageAccount.MatchesAll(CloudStorageAccount.MatchesOne(CloudStorageAccount.MatchesAll(CloudStorageAccount.AllRequired(CloudStorageAccount.AccountKeySetting), CloudStorageAccount.Optional(CloudStorageAccount.AccountKeyNameSetting)), CloudStorageAccount.AllRequired(CloudStorageAccount.SharedAccessSignatureSetting)), CloudStorageAccount.AllRequired(CloudStorageAccount.AccountNameSetting), func2, CloudStorageAccount.Optional(CloudStorageAccount.DefaultEndpointsProtocolSetting, CloudStorageAccount.EndpointSuffixSetting)));
      Func<IDictionary<string, string>, IDictionary<string, string>> func6 = CloudStorageAccount.MatchesExactly(CloudStorageAccount.MatchesAll(CloudStorageAccount.ValidCredentials, func3, func4));
      bool matchesAutomaticEndpointsSpec = (CloudStorageAccount.MatchesSpecification(settings, func5) ? 1 : 0) != 0;
      bool flag = CloudStorageAccount.MatchesSpecification(settings, func6);
      if (matchesAutomaticEndpointsSpec | flag)
      {
        if (matchesAutomaticEndpointsSpec && !settings.ContainsKey("DefaultEndpointsProtocol"))
          settings.Add("DefaultEndpointsProtocol", "https");
        string str1 = func1("BlobEndpoint");
        string str2 = func1("QueueEndpoint");
        string str3 = func1("TableEndpoint");
        string str4 = func1("FileEndpoint");
        string str5 = func1("BlobSecondaryEndpoint");
        string str6 = func1("QueueSecondaryEndpoint");
        string str7 = func1("TableSecondaryEndpoint");
        string str8 = func1("FileSecondaryEndpoint");
        Func<string, string, bool> func7 = (Func<string, string, bool>) ((primary, secondary) => !string.IsNullOrWhiteSpace(primary) || string.IsNullOrWhiteSpace(secondary));
        Func<string, string, Func<IDictionary<string, string>, StorageUri>, StorageUri> func8 = (Func<string, string, Func<IDictionary<string, string>, StorageUri>, StorageUri>) ((primary, secondary, factory) =>
        {
          if (!string.IsNullOrWhiteSpace(secondary) && !string.IsNullOrWhiteSpace(primary))
            return new StorageUri(new Uri(primary), new Uri(secondary));
          if (!string.IsNullOrWhiteSpace(primary))
            return new StorageUri(new Uri(primary));
          return !matchesAutomaticEndpointsSpec || factory == null ? new StorageUri((Uri) null) : factory(settings);
        });
        if (func7(str1, str5) && func7(str2, str6) && func7(str3, str7) && func7(str4, str8))
        {
          accountInformation = new CloudStorageAccount(CloudStorageAccount.GetCredentials(settings), func8(str1, str5, new Func<IDictionary<string, string>, StorageUri>(CloudStorageAccount.ConstructBlobEndpoint)), func8(str2, str6, new Func<IDictionary<string, string>, StorageUri>(CloudStorageAccount.ConstructQueueEndpoint)), func8(str3, str7, new Func<IDictionary<string, string>, StorageUri>(CloudStorageAccount.ConstructTableEndpoint)), func8(str4, str8, new Func<IDictionary<string, string>, StorageUri>(CloudStorageAccount.ConstructFileEndpoint)))
          {
            DefaultEndpoints = str1 == null && str2 == null && str3 == null && str4 == null,
            EndpointSuffix = func1("EndpointSuffix"),
            Settings = CloudStorageAccount.ValidCredentials(settings)
          };
          accountInformation.accountName = func1("AccountName");
          return true;
        }
      }
      accountInformation = (CloudStorageAccount) null;
      error("No valid combination of account information found.");
      return false;
    }

    private static IDictionary<string, string> ParseStringIntoSettings(
      string connectionString,
      Action<string> error)
    {
      IDictionary<string, string> stringIntoSettings = (IDictionary<string, string>) new Dictionary<string, string>();
      string str1 = connectionString;
      char[] separator1 = new char[1]{ ';' };
      foreach (string str2 in str1.Split(separator1, StringSplitOptions.RemoveEmptyEntries))
      {
        char[] separator2 = new char[1]{ '=' };
        string[] strArray = str2.Split(separator2, 2);
        if (strArray.Length != 2)
        {
          error("Settings must be of the form \"name=value\".");
          return (IDictionary<string, string>) null;
        }
        if (stringIntoSettings.ContainsKey(strArray[0]))
        {
          error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Duplicate setting '{0}' found.", (object) strArray[0]));
          return (IDictionary<string, string>) null;
        }
        stringIntoSettings.Add(strArray[0], strArray[1]);
      }
      return stringIntoSettings;
    }

    private static KeyValuePair<string, Func<string, bool>> Setting(
      string name,
      params string[] validValues)
    {
      return new KeyValuePair<string, Func<string, bool>>(name, (Func<string, bool>) (settingValue => validValues.Length == 0 || ((IEnumerable<string>) validValues).Contains<string>(settingValue)));
    }

    private static KeyValuePair<string, Func<string, bool>> Setting(
      string name,
      Func<string, bool> isValid)
    {
      return new KeyValuePair<string, Func<string, bool>>(name, isValid);
    }

    private static bool IsValidBase64String(string settingValue)
    {
      try
      {
        Convert.FromBase64String(settingValue);
        return true;
      }
      catch (FormatException ex)
      {
        return false;
      }
    }

    private static bool IsValidUri(string settingValue) => Uri.IsWellFormedUriString(settingValue, UriKind.Absolute);

    private static bool IsValidDomain(string settingValue) => Uri.CheckHostName(settingValue).Equals((object) UriHostNameType.Dns);

    private static Func<IDictionary<string, string>, IDictionary<string, string>> AllRequired(
      params KeyValuePair<string, Func<string, bool>>[] requiredSettings)
    {
      return (Func<IDictionary<string, string>, IDictionary<string, string>>) (settings =>
      {
        IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>(settings);
        foreach (KeyValuePair<string, Func<string, bool>> requiredSetting in requiredSettings)
        {
          string str;
          if (!dictionary.TryGetValue(requiredSetting.Key, out str) || !requiredSetting.Value(str))
            return (IDictionary<string, string>) null;
          dictionary.Remove(requiredSetting.Key);
        }
        return dictionary;
      });
    }

    private static Func<IDictionary<string, string>, IDictionary<string, string>> Optional(
      params KeyValuePair<string, Func<string, bool>>[] optionalSettings)
    {
      return (Func<IDictionary<string, string>, IDictionary<string, string>>) (settings =>
      {
        IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>(settings);
        foreach (KeyValuePair<string, Func<string, bool>> optionalSetting in optionalSettings)
        {
          string str;
          if (dictionary.TryGetValue(optionalSetting.Key, out str) && optionalSetting.Value(str))
            dictionary.Remove(optionalSetting.Key);
        }
        return dictionary;
      });
    }

    private static Func<IDictionary<string, string>, IDictionary<string, string>> AtLeastOne(
      params KeyValuePair<string, Func<string, bool>>[] atLeastOneSettings)
    {
      return (Func<IDictionary<string, string>, IDictionary<string, string>>) (settings =>
      {
        IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>(settings);
        bool flag = false;
        foreach (KeyValuePair<string, Func<string, bool>> atLeastOneSetting in atLeastOneSettings)
        {
          string str;
          if (dictionary.TryGetValue(atLeastOneSetting.Key, out str) && atLeastOneSetting.Value(str))
          {
            dictionary.Remove(atLeastOneSetting.Key);
            flag = true;
          }
        }
        return !flag ? (IDictionary<string, string>) null : dictionary;
      });
    }

    private static Func<IDictionary<string, string>, IDictionary<string, string>> None(
      params KeyValuePair<string, Func<string, bool>>[] atLeastOneSettings)
    {
      return (Func<IDictionary<string, string>, IDictionary<string, string>>) (settings =>
      {
        IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>(settings);
        bool flag = false;
        foreach (KeyValuePair<string, Func<string, bool>> atLeastOneSetting in atLeastOneSettings)
        {
          string str;
          if (dictionary.TryGetValue(atLeastOneSetting.Key, out str) && atLeastOneSetting.Value(str))
            flag = true;
        }
        return !flag ? dictionary : (IDictionary<string, string>) null;
      });
    }

    private static Func<IDictionary<string, string>, IDictionary<string, string>> MatchesAll(
      params Func<IDictionary<string, string>, IDictionary<string, string>>[] filters)
    {
      return (Func<IDictionary<string, string>, IDictionary<string, string>>) (settings =>
      {
        IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>(settings);
        foreach (Func<IDictionary<string, string>, IDictionary<string, string>> filter in filters)
        {
          if (dictionary != null)
            dictionary = filter(dictionary);
          else
            break;
        }
        return dictionary;
      });
    }

    private static Func<IDictionary<string, string>, IDictionary<string, string>> MatchesOne(
      params Func<IDictionary<string, string>, IDictionary<string, string>>[] filters)
    {
      return (Func<IDictionary<string, string>, IDictionary<string, string>>) (settings =>
      {
        IDictionary<string, string>[] array = ((IEnumerable<Func<IDictionary<string, string>, IDictionary<string, string>>>) filters).Select<Func<IDictionary<string, string>, IDictionary<string, string>>, IDictionary<string, string>>((Func<Func<IDictionary<string, string>, IDictionary<string, string>>, IDictionary<string, string>>) (filter => filter((IDictionary<string, string>) new Dictionary<string, string>(settings)))).Where<IDictionary<string, string>>((Func<IDictionary<string, string>, bool>) (result => result != null)).Take<IDictionary<string, string>>(2).ToArray<IDictionary<string, string>>();
        return array.Length != 1 ? (IDictionary<string, string>) null : ((IEnumerable<IDictionary<string, string>>) array).First<IDictionary<string, string>>();
      });
    }

    private static Func<IDictionary<string, string>, IDictionary<string, string>> MatchesExactly(
      Func<IDictionary<string, string>, IDictionary<string, string>> filter)
    {
      return (Func<IDictionary<string, string>, IDictionary<string, string>>) (settings =>
      {
        IDictionary<string, string> source = filter(settings);
        return source == null || source.Any<KeyValuePair<string, string>>() ? (IDictionary<string, string>) null : source;
      });
    }

    private static bool MatchesSpecification(
      IDictionary<string, string> settings,
      params Func<IDictionary<string, string>, IDictionary<string, string>>[] constraints)
    {
      foreach (Func<IDictionary<string, string>, IDictionary<string, string>> constraint in constraints)
      {
        IDictionary<string, string> dictionary = constraint(settings);
        if (dictionary == null)
          return false;
        settings = dictionary;
      }
      return settings.Count == 0;
    }

    private static StorageCredentials GetCredentials(IDictionary<string, string> settings)
    {
      string accountName;
      settings.TryGetValue("AccountName", out accountName);
      string keyValue;
      settings.TryGetValue("AccountKey", out keyValue);
      string keyName;
      settings.TryGetValue("AccountKeyName", out keyName);
      string sasToken;
      settings.TryGetValue("SharedAccessSignature", out sasToken);
      if (accountName != null && keyValue != null && sasToken == null)
        return new StorageCredentials(accountName, keyValue, keyName);
      return keyValue == null && keyName == null && sasToken != null ? new StorageCredentials(sasToken) : (StorageCredentials) null;
    }

    private static StorageUri ConstructBlobEndpoint(IDictionary<string, string> settings) => CloudStorageAccount.ConstructBlobEndpoint(settings["DefaultEndpointsProtocol"], settings["AccountName"], settings.ContainsKey("EndpointSuffix") ? settings["EndpointSuffix"] : (string) null);

    private static StorageUri ConstructBlobEndpoint(
      string scheme,
      string accountName,
      string endpointSuffix)
    {
      if (string.IsNullOrEmpty(scheme))
        throw new ArgumentNullException(nameof (scheme));
      if (string.IsNullOrEmpty(accountName))
        throw new ArgumentNullException(nameof (accountName));
      if (string.IsNullOrEmpty(endpointSuffix))
        endpointSuffix = "core.windows.net";
      return new StorageUri(new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}.{2}.{3}/", (object) scheme, (object) accountName, (object) "blob", (object) endpointSuffix)), new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}{2}.{3}.{4}", (object) scheme, (object) accountName, (object) "-secondary", (object) "blob", (object) endpointSuffix)));
    }

    private static StorageUri ConstructFileEndpoint(IDictionary<string, string> settings) => CloudStorageAccount.ConstructFileEndpoint(settings["DefaultEndpointsProtocol"], settings["AccountName"], settings.ContainsKey("EndpointSuffix") ? settings["EndpointSuffix"] : (string) null);

    private static StorageUri ConstructFileEndpoint(
      string scheme,
      string accountName,
      string endpointSuffix)
    {
      if (string.IsNullOrEmpty(scheme))
        throw new ArgumentNullException(nameof (scheme));
      if (string.IsNullOrEmpty(accountName))
        throw new ArgumentNullException(nameof (accountName));
      if (string.IsNullOrEmpty(endpointSuffix))
        endpointSuffix = "core.windows.net";
      return new StorageUri(new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}.{2}.{3}/", (object) scheme, (object) accountName, (object) "file", (object) endpointSuffix)), new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}{2}.{3}.{4}", (object) scheme, (object) accountName, (object) "-secondary", (object) "file", (object) endpointSuffix)));
    }

    private static StorageUri ConstructQueueEndpoint(IDictionary<string, string> settings) => CloudStorageAccount.ConstructQueueEndpoint(settings["DefaultEndpointsProtocol"], settings["AccountName"], settings.ContainsKey("EndpointSuffix") ? settings["EndpointSuffix"] : (string) null);

    private static StorageUri ConstructQueueEndpoint(
      string scheme,
      string accountName,
      string endpointSuffix)
    {
      if (string.IsNullOrEmpty(scheme))
        throw new ArgumentNullException(nameof (scheme));
      if (string.IsNullOrEmpty(accountName))
        throw new ArgumentNullException(nameof (accountName));
      if (string.IsNullOrEmpty(endpointSuffix))
        endpointSuffix = "core.windows.net";
      return new StorageUri(new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}.{2}.{3}/", (object) scheme, (object) accountName, (object) "queue", (object) endpointSuffix)), new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}{2}.{3}.{4}", (object) scheme, (object) accountName, (object) "-secondary", (object) "queue", (object) endpointSuffix)));
    }

    private static StorageUri ConstructTableEndpoint(IDictionary<string, string> settings) => CloudStorageAccount.ConstructTableEndpoint(settings["DefaultEndpointsProtocol"], settings["AccountName"], settings.ContainsKey("EndpointSuffix") ? settings["EndpointSuffix"] : (string) null);

    private static StorageUri ConstructTableEndpoint(
      string scheme,
      string accountName,
      string endpointSuffix)
    {
      if (string.IsNullOrEmpty(scheme))
        throw new ArgumentNullException(nameof (scheme));
      if (string.IsNullOrEmpty(accountName))
        throw new ArgumentNullException(nameof (accountName));
      if (string.IsNullOrEmpty(endpointSuffix))
        endpointSuffix = "core.windows.net";
      return new StorageUri(new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}.{2}.{3}/", (object) scheme, (object) accountName, (object) "table", (object) endpointSuffix)), new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}{2}.{3}.{4}", (object) scheme, (object) accountName, (object) "-secondary", (object) "table", (object) endpointSuffix)));
    }
  }
}
