// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.ContainerHttpResponseParsers
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  public static class ContainerHttpResponseParsers
  {
    public static AccountProperties ReadAccountProperties(HttpResponseMessage response) => HttpResponseParsers.ReadAccountProperties(response);

    public static BlobContainerProperties GetProperties(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      BlobContainerProperties properties = new BlobContainerProperties();
      properties.ETag = response.Headers.ETag == null ? (string) null : response.Headers.ETag.ToString();
      properties.LastModified = (DateTimeOffset?) response?.Content?.Headers?.LastModified;
      properties.LeaseStatus = BlobHttpResponseParsers.GetLeaseStatus(response);
      properties.LeaseState = BlobHttpResponseParsers.GetLeaseState(response);
      properties.LeaseDuration = BlobHttpResponseParsers.GetLeaseDuration(response);
      properties.PublicAccess = new BlobContainerPublicAccessType?(ContainerHttpResponseParsers.GetAcl(response));
      string singleValueOrDefault1 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-has-immutability-policy");
      properties.HasImmutabilityPolicy = string.IsNullOrEmpty(singleValueOrDefault1) ? new bool?() : new bool?(bool.Parse(singleValueOrDefault1));
      string singleValueOrDefault2 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-has-legal-hold");
      properties.HasLegalHold = string.IsNullOrEmpty(singleValueOrDefault2) ? new bool?() : new bool?(bool.Parse(singleValueOrDefault2));
      string singleValueOrDefault3 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-default-encryption-scope");
      if (!string.IsNullOrEmpty(singleValueOrDefault3))
      {
        properties.EncryptionScopeOptions = new BlobContainerEncryptionScopeOptions();
        properties.EncryptionScopeOptions.DefaultEncryptionScope = singleValueOrDefault3;
        string singleValueOrDefault4 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-deny-encryption-scope-override");
        properties.EncryptionScopeOptions.PreventEncryptionScopeOverride = string.Equals(singleValueOrDefault4, "true", StringComparison.OrdinalIgnoreCase);
      }
      return properties;
    }

    public static IDictionary<string, string> GetMetadata(HttpResponseMessage response) => HttpResponseParsers.GetMetadata(response);

    public static BlobContainerPublicAccessType GetAcl(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      return ContainerHttpResponseParsers.GetContainerAcl(response.Headers.GetHeaderSingleValueOrDefault("x-ms-blob-public-access"));
    }

    public static Task ReadSharedAccessIdentifiersAsync(
      Stream inputStream,
      BlobContainerPermissions permissions,
      CancellationToken token)
    {
      CommonUtility.AssertNotNull(nameof (permissions), (object) permissions);
      return Response.ReadSharedAccessIdentifiersAsync<SharedAccessBlobPolicy>((IDictionary<string, SharedAccessBlobPolicy>) permissions.SharedAccessPolicies, (AccessPolicyResponseBase<SharedAccessBlobPolicy>) new BlobAccessPolicyResponse(inputStream), token);
    }

    internal static BlobContainerPublicAccessType GetContainerAcl(string acl)
    {
      BlobContainerPublicAccessType containerAcl = BlobContainerPublicAccessType.Off;
      if (!string.IsNullOrEmpty(acl))
      {
        switch (acl.ToLower())
        {
          case "container":
            containerAcl = BlobContainerPublicAccessType.Container;
            break;
          case "blob":
            containerAcl = BlobContainerPublicAccessType.Blob;
            break;
          default:
            containerAcl = BlobContainerPublicAccessType.Unknown;
            break;
        }
      }
      return containerAcl;
    }
  }
}
