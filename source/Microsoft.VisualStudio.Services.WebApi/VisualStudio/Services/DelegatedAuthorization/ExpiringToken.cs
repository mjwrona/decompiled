// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.ExpiringToken
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class ExpiringToken
  {
    public string DisplayName { get; set; }

    public Guid AuthorizationId { get; set; }

    public Guid UserId { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public string Scopes { get; set; }

    public string Audience { get; set; }

    public override string ToString() => string.Format("{0}\\{1}-({2}) [{3}]", (object) this.Audience, (object) this.DisplayName, (object) this.Scopes, (object) this.ValidTo);
  }
}
