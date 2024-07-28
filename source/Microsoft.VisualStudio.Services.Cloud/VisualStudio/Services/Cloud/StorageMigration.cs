// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.StorageMigration
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DataContract]
  public class StorageMigration
  {
    [DataMember]
    public Guid MigrationId { get; set; }

    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Uri { get; set; }

    [DataMember]
    public string SasToken { get; set; }

    [DataMember]
    public string VsoArea { get; set; }

    [DataMember]
    public StorageMigrationStatus Status { get; set; }

    [DataMember]
    public string StatusReason { get; set; }

    [DataMember]
    public StorageType StorageType { get; set; }

    [DataMember]
    public bool IsSharded { get; set; }

    [DataMember]
    public int? ShardIndex { get; set; }

    [DataMember]
    public string FilterKey { get; set; }

    [IgnoreDataMember]
    internal Guid SigningKeyId { get; set; }

    [IgnoreDataMember]
    internal byte[] SignedSasToken { get; set; }

    public string StorageAccountName => StorageMigration.GetStorageAccountName(this.Uri);

    public override string ToString() => string.Format("Id: {0}, Uri: '{1}', VsoArea: {2}, StorageType: {3}, IsSharded: {4}, ShardIndex: {5}, FilterKey: '{6}', Status: '{7}', StatusReason: '{8}'", (object) this.Id, (object) this.Uri, (object) this.VsoArea, (object) this.StorageType, (object) this.IsSharded, (object) this.ShardIndex, (object) this.FilterKey, (object) this.Status, (object) this.StatusReason);

    public static string GetStorageAccountName(string uriString)
    {
      string str = uriString != null ? new System.Uri(uriString).Host : throw new ArgumentException("Could not parse Azure storage account name from " + uriString);
      int length = str.IndexOf('.');
      if (length > 0)
        return str.Substring(0, length);
    }
  }
}
