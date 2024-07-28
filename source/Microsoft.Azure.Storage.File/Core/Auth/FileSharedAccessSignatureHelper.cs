// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Auth.FileSharedAccessSignatureHelper
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.File;
using System;
using System.Globalization;

namespace Microsoft.Azure.Storage.Core.Auth
{
  internal static class FileSharedAccessSignatureHelper
  {
    internal static UriQueryBuilder GetSignature(
      SharedAccessFilePolicy policy,
      SharedAccessFileHeaders headers,
      string accessPolicyIdentifier,
      string resourceType,
      string signature,
      string accountKeyName,
      string sasVersion,
      SharedAccessProtocol? protocols,
      IPAddressOrRange ipAddressOrRange)
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
      if (policy != null)
      {
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "st", SharedAccessSignatureHelper.GetDateTimeOrNull(policy.SharedAccessStartTime));
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "se", SharedAccessSignatureHelper.GetDateTimeOrNull(policy.SharedAccessExpiryTime));
        string str = SharedAccessFilePolicy.PermissionsToString(policy.Permissions);
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
      SharedAccessFilePolicy policy,
      SharedAccessFileHeaders headers,
      string accessPolicyIdentifier,
      string resourceName,
      string sasVersion,
      SharedAccessProtocol? protocols,
      IPAddressOrRange ipAddressOrRange,
      byte[] keyValue)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (resourceName), resourceName);
      CommonUtility.AssertNotNull(nameof (keyValue), (object) keyValue);
      CommonUtility.AssertNotNullOrEmpty(nameof (sasVersion), sasVersion);
      string str1 = (string) null;
      DateTimeOffset? nullable1 = new DateTimeOffset?();
      DateTimeOffset? nullable2 = new DateTimeOffset?();
      if (policy != null)
      {
        str1 = SharedAccessFilePolicy.PermissionsToString(policy.Permissions);
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
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}\n{11}\n{12}", (object) str1, (object) SharedAccessSignatureHelper.GetDateTimeOrEmpty(nullable1), (object) SharedAccessSignatureHelper.GetDateTimeOrEmpty(nullable2), (object) resourceName, (object) accessPolicyIdentifier, ipAddressOrRange == null ? (object) string.Empty : (object) ipAddressOrRange.ToString(), (object) SharedAccessSignatureHelper.GetProtocolString(protocols), (object) sasVersion, (object) str2, (object) str3, (object) str4, (object) str5, (object) str6);
      Logger.LogVerbose((OperationContext) null, "StringToSign = {0}.", (object) message);
      return CryptoUtility.ComputeHmac256(keyValue, message);
    }
  }
}
