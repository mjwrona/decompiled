// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Auth.QueueSharedAccessSignatureHelper
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Queue;
using System;
using System.Globalization;

namespace Microsoft.Azure.Storage.Core.Auth
{
  internal static class QueueSharedAccessSignatureHelper
  {
    internal static UriQueryBuilder GetSignature(
      SharedAccessQueuePolicy policy,
      string accessPolicyIdentifier,
      string signature,
      string accountKeyName,
      string sasVersion,
      SharedAccessProtocol? protocols,
      IPAddressOrRange ipAddressOrRange)
    {
      CommonUtility.AssertNotNull(nameof (signature), (object) signature);
      UriQueryBuilder builder = new UriQueryBuilder();
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sv", sasVersion);
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "si", accessPolicyIdentifier);
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sk", accountKeyName);
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sig", signature);
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "spr", SharedAccessSignatureHelper.GetProtocolString(protocols));
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sip", ipAddressOrRange == null ? (string) null : ipAddressOrRange.ToString());
      if (policy != null)
      {
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "st", SharedAccessSignatureHelper.GetDateTimeOrNull(policy.SharedAccessStartTime));
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "se", SharedAccessSignatureHelper.GetDateTimeOrNull(policy.SharedAccessExpiryTime));
        string str = SharedAccessQueuePolicy.PermissionsToString(policy.Permissions);
        if (!string.IsNullOrEmpty(str))
          SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sp", str);
      }
      return builder;
    }

    internal static string GetHash(
      SharedAccessQueuePolicy policy,
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
      string str = (string) null;
      DateTimeOffset? nullable1 = new DateTimeOffset?();
      DateTimeOffset? nullable2 = new DateTimeOffset?();
      if (policy != null)
      {
        str = SharedAccessQueuePolicy.PermissionsToString(policy.Permissions);
        nullable1 = policy.SharedAccessStartTime;
        nullable2 = policy.SharedAccessExpiryTime;
      }
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}", (object) str, (object) SharedAccessSignatureHelper.GetDateTimeOrEmpty(nullable1), (object) SharedAccessSignatureHelper.GetDateTimeOrEmpty(nullable2), (object) resourceName, (object) accessPolicyIdentifier, ipAddressOrRange == null ? (object) string.Empty : (object) ipAddressOrRange.ToString(), (object) SharedAccessSignatureHelper.GetProtocolString(protocols), (object) sasVersion);
      Logger.LogVerbose((OperationContext) null, "StringToSign = {0}.", (object) message);
      return CryptoUtility.ComputeHmac256(keyValue, message);
    }
  }
}
