// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Auth.StorageCredentials
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Globalization;

namespace Microsoft.Azure.Storage.Auth
{
  public sealed class StorageCredentials
  {
    private SasQueryBuilder queryBuilder;

    public string SASToken { get; private set; }

    public string AccountName { get; private set; }

    public string KeyName => this.Key.KeyName;

    internal TokenCredential TokenCredential { get; private set; }

    internal StorageAccountKey Key { get; private set; }

    public bool IsAnonymous => this.SASToken == null && this.AccountName == null;

    public bool IsSAS => this.SASToken != null;

    public bool IsSharedKey => this.SASToken == null && this.TokenCredential == null && this.AccountName != null;

    public bool IsToken => this.TokenCredential != null;

    public string SASSignature => this.IsSAS ? this.queryBuilder["sig"] : (string) null;

    public StorageCredentials()
    {
    }

    public StorageCredentials(string accountName, string keyValue)
      : this(accountName, keyValue, (string) null)
    {
    }

    public StorageCredentials(string accountName, byte[] keyValue)
      : this(accountName, keyValue, (string) null)
    {
    }

    public StorageCredentials(string accountName, string keyValue, string keyName)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (accountName), accountName);
      this.AccountName = accountName;
      this.UpdateKey(keyValue, keyName);
    }

    public StorageCredentials(string accountName, byte[] keyValue, string keyName)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (accountName), accountName);
      this.AccountName = accountName;
      this.UpdateKey(keyValue, keyName);
    }

    public StorageCredentials(string sasToken)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (sasToken), sasToken);
      this.SASToken = sasToken;
      this.UpdateQueryBuilder();
    }

    public StorageCredentials(TokenCredential tokenCredential) => this.TokenCredential = tokenCredential;

    public void UpdateKey(string keyValue) => this.UpdateKey(keyValue, (string) null);

    public void UpdateKey(byte[] keyValue) => this.UpdateKey(keyValue, (string) null);

    public void UpdateKey(string keyValue, string keyName)
    {
      if (!this.IsSharedKey)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot update key unless Account Key credentials are used."));
      CommonUtility.AssertNotNull(nameof (keyValue), (object) keyValue);
      this.Key = new StorageAccountKey(keyName, Convert.FromBase64String(keyValue));
    }

    public void UpdateKey(byte[] keyValue, string keyName)
    {
      if (!this.IsSharedKey)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot update key unless Account Key credentials are used."));
      CommonUtility.AssertNotNull(nameof (keyValue), (object) keyValue);
      this.Key = new StorageAccountKey(keyName, keyValue);
    }

    public void UpdateSASToken(string sasToken)
    {
      if (!this.IsSAS)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot update Shared Access Signature unless Sas credentials are used."));
      CommonUtility.AssertNotNullOrEmpty(nameof (sasToken), sasToken);
      this.SASToken = sasToken;
      this.UpdateQueryBuilder();
    }

    public byte[] ExportKey() => (byte[]) this.Key.KeyValue.Clone();

    public Uri TransformUri(Uri resourceUri)
    {
      CommonUtility.AssertNotNull(nameof (resourceUri), (object) resourceUri);
      return this.IsSAS ? this.queryBuilder.AddToUri(resourceUri) : resourceUri;
    }

    public StorageUri TransformUri(StorageUri resourceUri)
    {
      CommonUtility.AssertNotNull(nameof (resourceUri), (object) resourceUri);
      return new StorageUri(this.TransformUri(resourceUri.PrimaryUri), this.TransformUri(resourceUri.SecondaryUri));
    }

    public string ExportBase64EncodedKey() => StorageCredentials.GetBase64EncodedKey(this.Key);

    private static string GetBase64EncodedKey(StorageAccountKey accountKey) => accountKey.KeyValue != null ? Convert.ToBase64String(accountKey.KeyValue) : (string) null;

    private static string GetTokenValue(TokenCredential tokenCredential) => tokenCredential?.Token;

    internal string ToString(bool exportSecrets) => this.IsSharedKey ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1};{2}={3}", (object) "AccountName", (object) this.AccountName, (object) "AccountKey", exportSecrets ? (object) this.ExportBase64EncodedKey() : (object) "[key hidden]") : (this.IsSAS ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", (object) "SharedAccessSignature", exportSecrets ? (object) this.SASToken : (object) "[signature hidden]") : string.Empty);

    public bool Equals(StorageCredentials other)
    {
      if (other == null)
        return false;
      StorageAccountKey key1 = this.Key;
      StorageAccountKey key2 = other.Key;
      return string.Equals(this.SASToken, other.SASToken) && string.Equals(this.AccountName, other.AccountName) && string.Equals(key1.KeyName, key2.KeyName) && string.Equals(StorageCredentials.GetBase64EncodedKey(key1), StorageCredentials.GetBase64EncodedKey(key2)) && string.Equals(StorageCredentials.GetTokenValue(this.TokenCredential), StorageCredentials.GetTokenValue(other.TokenCredential));
    }

    private void UpdateQueryBuilder()
    {
      SasQueryBuilder sasQueryBuilder = new SasQueryBuilder(this.SASToken);
      if (sasQueryBuilder.ContainsQueryStringName("api-version"))
        throw new ArgumentException(string.Format("The parameter `api-version` should not be included in the SAS token. Please allow the library to set the  `api-version` parameter."));
      sasQueryBuilder.Add("api-version", "2019-07-07");
      this.queryBuilder = sasQueryBuilder;
    }
  }
}
