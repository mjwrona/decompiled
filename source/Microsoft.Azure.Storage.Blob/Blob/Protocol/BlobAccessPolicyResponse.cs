// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.BlobAccessPolicyResponse
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.IO;
using System.Xml.Linq;

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  internal class BlobAccessPolicyResponse : AccessPolicyResponseBase<SharedAccessBlobPolicy>
  {
    internal BlobAccessPolicyResponse(Stream stream)
      : base(stream)
    {
    }

    protected override SharedAccessBlobPolicy ParseElement(XElement accessPolicyElement)
    {
      CommonUtility.AssertNotNull(nameof (accessPolicyElement), (object) accessPolicyElement);
      SharedAccessBlobPolicy element = new SharedAccessBlobPolicy();
      string stringToUnescape1 = (string) accessPolicyElement.Element((XName) "Start");
      if (!string.IsNullOrEmpty(stringToUnescape1))
        element.SharedAccessStartTime = new DateTimeOffset?((DateTimeOffset) Uri.UnescapeDataString(stringToUnescape1).ToUTCTime());
      string stringToUnescape2 = (string) accessPolicyElement.Element((XName) "Expiry");
      if (!string.IsNullOrEmpty(stringToUnescape2))
        element.SharedAccessExpiryTime = new DateTimeOffset?((DateTimeOffset) Uri.UnescapeDataString(stringToUnescape2).ToUTCTime());
      string input = (string) accessPolicyElement.Element((XName) "Permission");
      if (!string.IsNullOrEmpty(input))
        element.Permissions = SharedAccessBlobPolicy.PermissionsFromString(input);
      return element;
    }
  }
}
