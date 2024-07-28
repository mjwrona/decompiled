// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Auth.SharedAccessSignatureHelper
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Storage.Core.Auth
{
  internal static class SharedAccessSignatureHelper
  {
    internal static UriQueryBuilder GetSignature(
      SharedAccessAccountPolicy policy,
      string signature,
      string accountKeyName,
      string sasVersion)
    {
      CommonUtility.AssertNotNull(nameof (signature), (object) signature);
      CommonUtility.AssertNotNull(nameof (policy), (object) policy);
      UriQueryBuilder builder = new UriQueryBuilder();
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sv", sasVersion);
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sk", accountKeyName);
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sig", signature);
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "spr", !policy.Protocols.HasValue ? (string) null : SharedAccessSignatureHelper.GetProtocolString(new SharedAccessProtocol?(policy.Protocols.Value)));
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sip", policy.IPAddressOrRange == null ? (string) null : policy.IPAddressOrRange.ToString());
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "st", SharedAccessSignatureHelper.GetDateTimeOrNull(policy.SharedAccessStartTime));
      SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "se", SharedAccessSignatureHelper.GetDateTimeOrNull(policy.SharedAccessExpiryTime));
      string str1 = SharedAccessAccountPolicy.ResourceTypesToString(policy.ResourceTypes);
      if (!string.IsNullOrEmpty(str1))
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "srt", str1);
      string str2 = SharedAccessAccountPolicy.ServicesToString(policy.Services);
      if (!string.IsNullOrEmpty(str2))
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "ss", str2);
      string str3 = SharedAccessAccountPolicy.PermissionsToString(policy.Permissions);
      if (!string.IsNullOrEmpty(str3))
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, "sp", str3);
      return builder;
    }

    internal static string GetDateTimeOrEmpty(DateTimeOffset? value) => SharedAccessSignatureHelper.GetDateTimeOrNull(value) ?? string.Empty;

    internal static string GetDateTimeOrNull(DateTimeOffset? value) => !value.HasValue ? (string) null : value.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ", (IFormatProvider) CultureInfo.InvariantCulture);

    internal static string GetProtocolString(SharedAccessProtocol? protocols)
    {
      if (!protocols.HasValue)
        return (string) null;
      if (protocols.Value != SharedAccessProtocol.HttpsOnly && protocols.Value != SharedAccessProtocol.HttpsOrHttp)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid value {0} for the SharedAccessProtocol parameter when creating a SharedAccessSignature.  Use 'null' if you do not wish to include a SharedAccessProtocol.", (object) protocols.Value));
      return protocols.Value != SharedAccessProtocol.HttpsOnly ? "https,http" : "https";
    }

    internal static void AddEscapedIfNotNull(UriQueryBuilder builder, string name, string value)
    {
      if (value == null)
        return;
      builder.Add(name, value);
    }

    internal static StorageCredentials ParseQuery(IDictionary<string, string> queryParameters)
    {
      bool flag = false;
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<string, string> queryParameter in (IEnumerable<KeyValuePair<string, string>>) queryParameters)
      {
        switch (queryParameter.Key.ToLower())
        {
          case "sig":
            flag = true;
            continue;
          case "restype":
          case "comp":
          case "snapshot":
          case "api-version":
          case "sharesnapshot":
            stringList.Add(queryParameter.Key);
            continue;
          default:
            continue;
        }
      }
      foreach (string key in stringList)
        queryParameters.Remove(key);
      if (!flag)
        return (StorageCredentials) null;
      UriQueryBuilder builder = new UriQueryBuilder();
      foreach (KeyValuePair<string, string> queryParameter in (IEnumerable<KeyValuePair<string, string>>) queryParameters)
        SharedAccessSignatureHelper.AddEscapedIfNotNull(builder, queryParameter.Key.ToLower(), queryParameter.Value);
      return new StorageCredentials(builder.ToString());
    }

    internal static string GetHash(
      SharedAccessAccountPolicy policy,
      string accountName,
      string sasVersion,
      byte[] keyValue)
    {
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}", (object) accountName, (object) SharedAccessAccountPolicy.PermissionsToString(policy.Permissions), (object) SharedAccessAccountPolicy.ServicesToString(policy.Services), (object) SharedAccessAccountPolicy.ResourceTypesToString(policy.ResourceTypes), (object) SharedAccessSignatureHelper.GetDateTimeOrEmpty(policy.SharedAccessStartTime), (object) SharedAccessSignatureHelper.GetDateTimeOrEmpty(policy.SharedAccessExpiryTime), policy.IPAddressOrRange == null ? (object) string.Empty : (object) policy.IPAddressOrRange.ToString(), (object) SharedAccessSignatureHelper.GetProtocolString(policy.Protocols), (object) sasVersion, (object) string.Empty);
      Logger.LogVerbose((OperationContext) null, "StringToSign = {0}.", (object) message);
      return CryptoUtility.ComputeHmac256(keyValue, message);
    }
  }
}
