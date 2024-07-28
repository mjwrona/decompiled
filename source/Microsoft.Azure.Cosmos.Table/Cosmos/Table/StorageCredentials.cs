// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.StorageCredentials
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class StorageCredentials
  {
    private SasQueryBuilder queryBuilder;

    public string SASToken { get; private set; }

    public string AccountName { get; private set; }

    public string Key { get; private set; }

    public string KeyName { get; private set; }

    public StorageCredentials()
    {
    }

    public StorageCredentials(string sasToken)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (sasToken), sasToken);
      this.SASToken = sasToken;
      this.UpdateQueryBuilder();
    }

    public StorageCredentials(string accountName, string keyValue)
      : this(accountName, keyValue, (string) null)
    {
    }

    public StorageCredentials(string accountName, string keyValue, string keyName)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (accountName), accountName);
      this.AccountName = accountName;
      this.UpdateKey(keyValue, keyName);
    }

    public void UpdateKey(string keyValue)
    {
      if (!this.IsSharedKey)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot update key unless Account Key credentials are used."));
      CommonUtility.AssertNotNull(nameof (keyValue), (object) keyValue);
      this.Key = keyValue;
    }

    public void UpdateKey(string keyValue, string keyName)
    {
      if (!this.IsSharedKey)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot update key unless Account Key credentials are used."));
      CommonUtility.AssertNotNull(nameof (keyValue), (object) keyValue);
      this.KeyName = keyName;
      this.Key = keyValue;
    }

    public bool Equals(StorageCredentials other) => other != null && string.Equals(this.AccountName, other.AccountName) && string.Equals(this.Key, other.Key, StringComparison.OrdinalIgnoreCase);

    public bool IsSharedKey => this.SASToken == null && this.AccountName != null;

    public bool IsAnonymous => this.SASToken == null && this.AccountName == null;

    public bool IsSAS => this.SASToken != null;

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

    internal string ToString(bool exportSecrets) => this.IsSharedKey ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1};{2}={3}", (object) "AccountName", (object) this.AccountName, (object) "AccountKey", exportSecrets ? (object) this.Key : (object) "[key hidden]") : (this.IsSAS ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", (object) "SharedAccessSignature", exportSecrets ? (object) this.SASToken : (object) "[signature hidden]") : string.Empty);

    public string SASSignature => this.IsSAS ? this.queryBuilder["sig"] : (string) null;

    public void UpdateSASToken(string sasToken)
    {
      if (!this.IsSAS)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot update Shared Access Signature unless Sas credentials are used."));
      CommonUtility.AssertNotNullOrEmpty(nameof (sasToken), sasToken);
      this.SASToken = sasToken;
      this.UpdateQueryBuilder();
    }

    private void UpdateQueryBuilder()
    {
      SasQueryBuilder sasQueryBuilder = new SasQueryBuilder(this.SASToken);
      if (sasQueryBuilder.ContainsQueryStringName("api-version"))
        throw new ArgumentException(string.Format("The parameter `api-version` should not be included in the SAS token. Please allow the library to set the  `api-version` parameter."));
      sasQueryBuilder.Add("api-version", "2017-07-29");
      this.queryBuilder = sasQueryBuilder;
    }
  }
}
