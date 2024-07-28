// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadServiceRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Aad
{
  public abstract class AadServiceRequest
  {
    public string ToTenant { get; set; }

    public bool ToUserTenant { get; set; }

    public bool ToMicrosoftServicesTenant { get; set; }

    public string AccessToken { get; set; }

    public string MicrosoftGraphBaseUrlOverride { get; set; }

    internal virtual GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.AadGraphOnly;

    internal virtual bool UseBetaGraphVersion => false;

    internal virtual void Validate()
    {
    }

    internal virtual AadServiceResponse Execute(AadServiceRequestContext context) => throw new NotImplementedException();

    internal virtual AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      throw new NotImplementedException();
    }

    internal void CopyPropertiesTo(AadServiceRequest request) => AadServiceRequest.CopyProperties(this, request);

    internal void CopyPropertiesFrom(AadServiceRequest request) => AadServiceRequest.CopyProperties(request, this);

    internal static void CopyProperties(AadServiceRequest from, AadServiceRequest to)
    {
      to.ToTenant = from.ToTenant;
      to.ToUserTenant = from.ToUserTenant;
      to.ToMicrosoftServicesTenant = from.ToMicrosoftServicesTenant;
    }
  }
}
