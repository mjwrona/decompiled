// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.AccessTokenKey
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  public class AccessTokenKey
  {
    public Guid AccessId { get; set; }

    public Guid AuthorizationId { get; set; }

    public Guid UserId { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public string DisplayName { get; set; }

    public string Scope { get; set; }

    public string Audience { get; set; }

    public string Source { get; set; }

    public bool IsValid { get; set; }

    public bool IsPublic { get; set; }

    public string PublicData { get; set; }

    public string AccessHash { get; set; }
  }
}
