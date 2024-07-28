// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.WrongWorkOrPersonalException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [Serializable]
  public class WrongWorkOrPersonalException : TenantSwitchException
  {
    public string AccountName { get; }

    public bool ShouldBePersonal { get; }

    public bool ShouldCreatePersonal { get; }

    public WrongWorkOrPersonalException()
    {
    }

    public WrongWorkOrPersonalException(string message)
      : base(message)
    {
    }

    public WrongWorkOrPersonalException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected WrongWorkOrPersonalException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public WrongWorkOrPersonalException(
      string accountName,
      bool shouldBePersonal,
      bool shouldCreatePersonal)
      : base(WrongWorkOrPersonalException.GetMessage(shouldBePersonal, shouldCreatePersonal))
    {
      this.AccountName = accountName;
      this.ShouldBePersonal = shouldBePersonal;
      this.ShouldCreatePersonal = shouldCreatePersonal;
    }

    private static string GetMessage(bool shouldBePersonal, bool shouldCreatePersonal)
    {
      if (!shouldBePersonal)
        return IdentityResources.ShouldBeWorkAccountMessage();
      return shouldCreatePersonal ? IdentityResources.ShouldCreatePersonalAccountMessage() : IdentityResources.ShouldBePersonalAccountMessage();
    }
  }
}
