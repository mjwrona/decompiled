// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Auth.BlobSharedAccessSignatureHelper
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Globalization;

namespace Microsoft.Azure.Storage.Core.Auth
{
  internal static class BlobSharedAccessSignatureHelper
  {
    internal static UriQueryBuilder GetSignature(
      SharedAccessBlobPolicy policy,
      SharedAccessBlobHeaders headers,
      string accessPolicyIdentifier,
      string resourceType,
      string signature,
      string accountKeyName,
      string sasVersion,
      SharedAccessProtocol? protocols,
      IPAddressOrRange ipAddressOrRange,
      UserDelegationKey delegationKey = null)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (resourceType), resourceType);
      UriQueryBuilder builder = new UriQueryBuilder();
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sv", sasVersion);
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sr", resourceType);
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "si", accessPolicyIdentifier);
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sk", accountKeyName);
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sig", signature);
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "spr", SharedAccessSignatureHelper.GetProtocolString(protocols));
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sip", ipAddressOrRange == null ? (string) null : ipAddressOrRange.ToString());
      if (delegationKey != null)
      {
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "skoid", delegationKey.SignedOid.ToString());
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sktid", delegationKey.SignedTid.ToString());
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "skt", SharedAccessSignatureHelper.GetDateTimeOrNull(delegationKey.SignedStart));
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "ske", SharedAccessSignatureHelper.GetDateTimeOrNull(delegationKey.SignedExpiry));
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sks", delegationKey.SignedService);
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "skv", delegationKey.SignedVersion);
      }
      if (policy != null)
      {
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "st", SharedAccessSignatureHelper.GetDateTimeOrNull(policy.SharedAccessStartTime));
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "se", SharedAccessSignatureHelper.GetDateTimeOrNull(policy.SharedAccessExpiryTime));
        string str = SharedAccessBlobPolicy.PermissionsToString(policy.Permissions);
        if (!string.IsNullOrEmpty(str))
          SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sp", str);
      }
      if (headers != null)
      {
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "rscc", headers.CacheControl);
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "rsct", headers.ContentType);
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "rsce", headers.ContentEncoding);
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "rscl", headers.ContentLanguage);
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "rscd", headers.ContentDisposition);
      }
      return builder;
    }

    internal static string GetHash(
      SharedAccessBlobPolicy policy,
      SharedAccessBlobHeaders headers,
      string resourceName,
      string sasVersion,
      string resourceIdentifier,
      DateTimeOffset? snapTime,
      SharedAccessProtocol? protocols,
      IPAddressOrRange ipAddressOrRange,
      UserDelegationKey delegationKey)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (resourceName), resourceName);
      CommonUtility.AssertNotNullOrEmpty(nameof (sasVersion), sasVersion);
      CommonUtility.AssertNotNull(nameof (delegationKey), (object) delegationKey);
      CommonUtility.AssertNotNull("delegationKey.SignedOid", (object) delegationKey.SignedOid);
      CommonUtility.AssertNotNull("delegationKey.SignedTid", (object) delegationKey.SignedTid);
      CommonUtility.AssertNotNull("delegationKey.SignedStart", (object) delegationKey.SignedStart);
      CommonUtility.AssertNotNull("delegationKey.SignedExpiry", (object) delegationKey.SignedExpiry);
      CommonUtility.AssertNotNullOrEmpty("delegationKey.SignedService", delegationKey.SignedService);
      CommonUtility.AssertNotNullOrEmpty("delegationKey.SignedVersion", delegationKey.SignedVersion);
      CommonUtility.AssertNotNullOrEmpty("delegationKey.Value", delegationKey.Value);
      CommonUtility.AssertNotNull(nameof (policy), (object) policy);
      CommonUtility.AssertNotNull("policy.SharedAccessExpiryTime", (object) policy.SharedAccessExpiryTime);
      CommonUtility.AssertNotNullOrEmpty("policy.Permissions", SharedAccessBlobPolicy.PermissionsToString(policy.Permissions));
      string str1 = SharedAccessBlobPolicy.PermissionsToString(policy.Permissions);
      DateTimeOffset? sharedAccessStartTime = policy.SharedAccessStartTime;
      DateTimeOffset? accessExpiryTime = policy.SharedAccessExpiryTime;
      string str2 = (string) null;
      string str3 = (string) null;
      string str4 = (string) null;
      string str5 = (string) null;
      string str6 = (string) null;
      if (headers != null)
      {
        str2 = headers.CacheControl;
        str3 = headers.ContentDisposition;
        str4 = headers.ContentEncoding;
        str5 = headers.ContentLanguage;
        str6 = headers.ContentType;
      }
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}\n{11}\n{12}\n{13}\n{14}\n{15}\n{16}\n{17}\n{18}\n{19}", (object) str1, (object) SharedAccessSignatureHelper.GetDateTimeOrEmpty(sharedAccessStartTime), (object) SharedAccessSignatureHelper.GetDateTimeOrEmpty(accessExpiryTime), (object) resourceName, (object) delegationKey.SignedOid, (object) delegationKey.SignedTid, (object) delegationKey.SignedStart.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssK"), (object) delegationKey.SignedExpiry.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssK"), (object) delegationKey.SignedService, (object) delegationKey.SignedVersion, ipAddressOrRange == null ? (object) string.Empty : (object) ipAddressOrRange.ToString(), (object) SharedAccessSignatureHelper.GetProtocolString(protocols), (object) sasVersion, (object) resourceIdentifier, (object) snapTime.ToString(), (object) str2, (object) str3, (object) str4, (object) str5, (object) str6);
      Logger.LogVerbose((OperationContext) null, "StringToSign = {0}.", (object) message);
      return CryptoUtility.ComputeHmac256(Convert.FromBase64String(delegationKey.Value), message);
    }

    internal static string GetHash(
      SharedAccessBlobPolicy policy,
      SharedAccessBlobHeaders headers,
      string accessPolicyIdentifier,
      string resourceName,
      string sasVersion,
      SharedAccessProtocol? protocols,
      IPAddressOrRange ipAddressOrRange,
      byte[] keyValue,
      string resource,
      DateTimeOffset? snapshotTimestamp = null)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (resourceName), resourceName);
      CommonUtility.AssertNotNull(nameof (keyValue), (object) keyValue);
      CommonUtility.AssertNotNullOrEmpty(nameof (sasVersion), sasVersion);
      CommonUtility.AssertNotNullOrEmpty(nameof (resource), resource);
      string str1 = (string) null;
      DateTimeOffset? nullable1 = new DateTimeOffset?();
      DateTimeOffset? nullable2 = new DateTimeOffset?();
      if (policy != null)
      {
        str1 = SharedAccessBlobPolicy.PermissionsToString(policy.Permissions);
        nullable1 = policy.SharedAccessStartTime;
        nullable2 = policy.SharedAccessExpiryTime;
      }
      string str2 = (string) null;
      string str3 = (string) null;
      string str4 = (string) null;
      string str5 = (string) null;
      string str6 = (string) null;
      if (headers != null)
      {
        str2 = headers.CacheControl;
        str3 = headers.ContentDisposition;
        str4 = headers.ContentEncoding;
        str5 = headers.ContentLanguage;
        str6 = headers.ContentType;
      }
      string str7 = (string) null;
      if (snapshotTimestamp.HasValue)
        str7 = Request.ConvertDateTimeToSnapshotString(snapshotTimestamp.Value);
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}\n{11}\n{12}\n{13}\n{14}", (object) str1, (object) SharedAccessSignatureHelper.GetDateTimeOrEmpty(nullable1), (object) SharedAccessSignatureHelper.GetDateTimeOrEmpty(nullable2), (object) resourceName, (object) accessPolicyIdentifier, ipAddressOrRange == null ? (object) string.Empty : (object) ipAddressOrRange.ToString(), (object) SharedAccessSignatureHelper.GetProtocolString(protocols), (object) sasVersion, (object) resource, (object) str7, (object) str2, (object) str3, (object) str4, (object) str5, (object) str6);
      Logger.LogVerbose((OperationContext) null, "StringToSign = {0}.", (object) message);
      return CryptoUtility.ComputeHmac256(keyValue, message);
    }
  }
}
