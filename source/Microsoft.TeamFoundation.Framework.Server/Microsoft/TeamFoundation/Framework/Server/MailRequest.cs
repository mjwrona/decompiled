// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MailRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net.Mail;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class MailRequest
  {
    public MailRequest(MailMessage message, Guid requestId, Guid requesterTfId)
    {
      ArgumentUtility.CheckForNull<MailMessage>(message, nameof (message));
      this.Message = message;
      this.Id = requestId;
      this.RequesterTfId = requesterTfId;
    }

    public MailMessage Message { get; private set; }

    public Guid Id { get; private set; }

    public Guid RequesterTfId { get; private set; }
  }
}
