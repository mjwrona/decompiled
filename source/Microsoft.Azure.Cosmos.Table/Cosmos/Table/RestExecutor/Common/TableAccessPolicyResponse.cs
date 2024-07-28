// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Common.TableAccessPolicyResponse
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand;
using System;
using System.IO;
using System.Xml.Linq;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Common
{
  internal class TableAccessPolicyResponse : AccessPolicyResponseBase<SharedAccessTablePolicy>
  {
    internal TableAccessPolicyResponse(Stream stream)
      : base(stream)
    {
    }

    protected override SharedAccessTablePolicy ParseElement(XElement accessPolicyElement)
    {
      CommonUtility.AssertNotNull(nameof (accessPolicyElement), (object) accessPolicyElement);
      SharedAccessTablePolicy element = new SharedAccessTablePolicy();
      string stringToUnescape1 = (string) accessPolicyElement.Element((XName) "Start");
      if (!string.IsNullOrEmpty(stringToUnescape1))
        element.SharedAccessStartTime = new DateTimeOffset?((DateTimeOffset) Uri.UnescapeDataString(stringToUnescape1).ToUTCTime());
      string stringToUnescape2 = (string) accessPolicyElement.Element((XName) "Expiry");
      if (!string.IsNullOrEmpty(stringToUnescape2))
        element.SharedAccessExpiryTime = new DateTimeOffset?((DateTimeOffset) Uri.UnescapeDataString(stringToUnescape2).ToUTCTime());
      string input = (string) accessPolicyElement.Element((XName) "Permission");
      if (!string.IsNullOrEmpty(input))
        element.Permissions = SharedAccessTablePolicy.PermissionsFromString(input);
      return element;
    }
  }
}
