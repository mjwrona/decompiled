// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.Protocol.QueueAccessPolicyResponse
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.IO;
using System.Xml.Linq;

namespace Microsoft.Azure.Storage.Queue.Protocol
{
  internal class QueueAccessPolicyResponse : AccessPolicyResponseBase<SharedAccessQueuePolicy>
  {
    internal QueueAccessPolicyResponse(Stream stream)
      : base(stream)
    {
    }

    protected override SharedAccessQueuePolicy ParseElement(XElement accessPolicyElement)
    {
      CommonUtility.AssertNotNull(nameof (accessPolicyElement), (object) accessPolicyElement);
      SharedAccessQueuePolicy element = new SharedAccessQueuePolicy();
      string stringToUnescape1 = (string) accessPolicyElement.Element((XName) "Start");
      if (!string.IsNullOrEmpty(stringToUnescape1))
        element.SharedAccessStartTime = new DateTimeOffset?((DateTimeOffset) Uri.UnescapeDataString(stringToUnescape1).ToUTCTime());
      string stringToUnescape2 = (string) accessPolicyElement.Element((XName) "Expiry");
      if (!string.IsNullOrEmpty(stringToUnescape2))
        element.SharedAccessExpiryTime = new DateTimeOffset?((DateTimeOffset) Uri.UnescapeDataString(stringToUnescape2).ToUTCTime());
      string input = (string) accessPolicyElement.Element((XName) "Permission");
      if (!string.IsNullOrEmpty(input))
        element.Permissions = SharedAccessQueuePolicy.PermissionsFromString(input);
      return element;
    }
  }
}
