// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MailMessageExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Net.Mail;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class MailMessageExtensions
  {
    public static bool IsMessageBodyOversized(this MailMessage message, int maxSizeInBytes)
    {
      ArgumentUtility.CheckForNull<MailMessage>(message, nameof (message));
      ArgumentUtility.CheckForOutOfRange(maxSizeInBytes, nameof (maxSizeInBytes), 0);
      if (string.IsNullOrEmpty(message.Body))
        return false;
      int num = message.BodyEncoding == null ? 2 : (message.BodyEncoding.IsSingleByte ? 1 : 2);
      return message.Body.Length * num > maxSizeInBytes;
    }
  }
}
