// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreContainerSASPolicy
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class LogStoreContainerSASPolicy : ILogStoreContainerSASPolicy
  {
    private string _sharedAccessBlobPolicyName;
    private DateTimeOffset? _sharedAccessExpiryTime;
    private readonly SharedAccessProtocol _sasProtocol;
    private SharedAccessBlobPolicy _sharedAccessBlobPolicy;

    public LogStoreContainerSASPolicy(
      DateTimeOffset? sharedAccessExpiryTime,
      SASPermission permissions,
      bool supportsHttps,
      bool isExternalCustomer)
    {
      this._sharedAccessExpiryTime = sharedAccessExpiryTime;
      this._initializeSASPolicy(sharedAccessExpiryTime, permissions, isExternalCustomer);
      this._sasProtocol = supportsHttps ? SharedAccessProtocol.HttpsOnly : SharedAccessProtocol.HttpsOrHttp;
    }

    public SharedAccessProtocol GetSASProtocol() => this._sasProtocol;

    public string GetSharedAccessBlobPolicyName() => this._sharedAccessBlobPolicyName;

    public SharedAccessBlobPolicy GetSharedAccessBlobPolicy() => this._sharedAccessBlobPolicy;

    public DateTimeOffset? GetSASExpiryTime() => this._sharedAccessExpiryTime;

    private void _initializeSASPolicy(
      DateTimeOffset? sharedAccessExpiryTime,
      SASPermission permissions,
      bool isExternalCustomer)
    {
      switch (permissions)
      {
        case SASPermission.None:
          throw new TestLogStoreValidationFailureException(string.Format(permissions.ToString() + " is not a valid/supported permission"));
        case SASPermission.ReadList_Policy:
          this._sharedAccessBlobPolicyName = "logstore-read-list-policy";
          this._sharedAccessBlobPolicy = new SharedAccessBlobPolicy()
          {
            SharedAccessExpiryTime = sharedAccessExpiryTime,
            Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List
          };
          break;
        case SASPermission.AddCreateWrite_Policy:
          this._sharedAccessBlobPolicyName = "logstore-add-create-write-policy";
          this._sharedAccessBlobPolicy = new SharedAccessBlobPolicy()
          {
            SharedAccessExpiryTime = sharedAccessExpiryTime,
            Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Add | SharedAccessBlobPermissions.Create
          };
          break;
        case SASPermission.ReadAddCreateWriteList_Policy:
          this._sharedAccessBlobPolicyName = !isExternalCustomer ? "logstore-read-add-create-write-list-policy" : "logstore-external-customer-read-add-create-write-list-policy";
          this._sharedAccessBlobPolicy = new SharedAccessBlobPolicy()
          {
            SharedAccessExpiryTime = sharedAccessExpiryTime,
            Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Add | SharedAccessBlobPermissions.Create
          };
          break;
        case SASPermission.ReadAddCreateWriteListDelete_Policy:
          this._sharedAccessBlobPolicyName = "logstore-read-add-create-write-list-delete-policy";
          this._sharedAccessBlobPolicy = new SharedAccessBlobPolicy()
          {
            SharedAccessExpiryTime = sharedAccessExpiryTime,
            Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Delete | SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Add | SharedAccessBlobPermissions.Create
          };
          break;
        default:
          throw new TestLogStoreValidationFailureException(string.Format(permissions.ToString() + " is not a valid/supported permission"));
      }
    }
  }
}
