// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.BrokeredMessageExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.ServiceBus.Messaging;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  public static class BrokeredMessageExtensions
  {
    public static BrokeredMessage ToBrokeredMessage(
      this string content,
      bool bypassSerializer,
      string contentType = "application/json")
    {
      BrokeredMessage brokeredMessage = bypassSerializer ? new BrokeredMessage((Stream) new MemoryStream(Encoding.UTF8.GetBytes(content)), true) : new BrokeredMessage((object) content);
      brokeredMessage.ContentType = contentType;
      return brokeredMessage;
    }
  }
}
