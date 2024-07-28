// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.Protocol.QueueRequest
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.Azure.Storage.Queue.Protocol
{
  public static class QueueRequest
  {
    public static void WriteSharedAccessIdentifiers(
      SharedAccessQueuePolicies sharedAccessPolicies,
      Stream outputStream)
    {
      Request.WriteSharedAccessIdentifiers<SharedAccessQueuePolicy>((IDictionary<string, SharedAccessQueuePolicy>) sharedAccessPolicies, outputStream, (Action<SharedAccessQueuePolicy, XmlWriter>) ((policy, writer) =>
      {
        writer.WriteElementString("Start", SharedAccessSignatureHelper.GetDateTimeOrEmpty(policy.SharedAccessStartTime));
        writer.WriteElementString("Expiry", SharedAccessSignatureHelper.GetDateTimeOrEmpty(policy.SharedAccessExpiryTime));
        writer.WriteElementString("Permission", SharedAccessQueuePolicy.PermissionsToString(policy.Permissions));
      }));
    }

    public static void WriteMessageContent(string messageContent, Stream outputStream)
    {
      CommonUtility.AssertNotNull(nameof (outputStream), (object) outputStream);
      using (XmlWriter xmlWriter = XmlWriter.Create(outputStream, new XmlWriterSettings()
      {
        Encoding = Encoding.UTF8,
        NewLineHandling = NewLineHandling.Entitize
      }))
      {
        xmlWriter.WriteStartElement("QueueMessage");
        xmlWriter.WriteElementString("MessageText", messageContent);
        xmlWriter.WriteEndDocument();
      }
    }
  }
}
