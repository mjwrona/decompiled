// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.FileRequest
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Microsoft.Azure.Storage.File.Protocol
{
  public static class FileRequest
  {
    public static void WriteSharedAccessIdentifiers(
      SharedAccessFilePolicies sharedAccessPolicies,
      Stream outputStream)
    {
      Request.WriteSharedAccessIdentifiers<SharedAccessFilePolicy>((IDictionary<string, SharedAccessFilePolicy>) sharedAccessPolicies, outputStream, (Action<SharedAccessFilePolicy, XmlWriter>) ((policy, writer) =>
      {
        writer.WriteElementString("Start", SharedAccessSignatureHelper.GetDateTimeOrEmpty(policy.SharedAccessStartTime));
        writer.WriteElementString("Expiry", SharedAccessSignatureHelper.GetDateTimeOrEmpty(policy.SharedAccessExpiryTime));
        writer.WriteElementString("Permission", SharedAccessFilePolicy.PermissionsToString(policy.Permissions));
      }));
    }
  }
}
