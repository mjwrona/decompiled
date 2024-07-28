// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.InvitationPendingException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [Serializable]
  public class InvitationPendingException : TenantSwitchException
  {
    public string AccountName { get; }

    public string OrganizationName { get; }

    public InvitationPendingException()
    {
    }

    public InvitationPendingException(string message)
      : base(message)
    {
    }

    public InvitationPendingException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected InvitationPendingException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public InvitationPendingException(string accountName, string organizationName)
      : base(IdentityResources.InvitationPendingMessage((object) accountName, (object) organizationName))
    {
      this.AccountName = accountName;
      this.OrganizationName = organizationName;
    }
  }
}
