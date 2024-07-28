// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.FileAccessPolicyResponse
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.IO;
using System.Xml.Linq;

namespace Microsoft.Azure.Storage.File.Protocol
{
  internal class FileAccessPolicyResponse : AccessPolicyResponseBase<SharedAccessFilePolicy>
  {
    internal FileAccessPolicyResponse(Stream stream)
      : base(stream)
    {
    }

    protected override SharedAccessFilePolicy ParseElement(XElement accessPolicyElement)
    {
      CommonUtility.AssertNotNull(nameof (accessPolicyElement), (object) accessPolicyElement);
      SharedAccessFilePolicy element = new SharedAccessFilePolicy();
      string stringToUnescape1 = (string) accessPolicyElement.Element((XName) "Start");
      if (!string.IsNullOrEmpty(stringToUnescape1))
        element.SharedAccessStartTime = new DateTimeOffset?((DateTimeOffset) Uri.UnescapeDataString(stringToUnescape1).ToUTCTime());
      string stringToUnescape2 = (string) accessPolicyElement.Element((XName) "Expiry");
      if (!string.IsNullOrEmpty(stringToUnescape2))
        element.SharedAccessExpiryTime = new DateTimeOffset?((DateTimeOffset) Uri.UnescapeDataString(stringToUnescape2).ToUTCTime());
      string input = (string) accessPolicyElement.Element((XName) "Permission");
      if (!string.IsNullOrEmpty(input))
        element.Permissions = SharedAccessFilePolicy.PermissionsFromString(input);
      return element;
    }
  }
}
