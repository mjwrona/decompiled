// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.SessionToken
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [ClientIncludeModel]
  public class SessionToken
  {
    public Guid ClientId { get; set; }

    public Guid AccessId { get; set; }

    public Guid AuthorizationId { get; set; }

    public Guid HostAuthorizationId { get; set; }

    public Guid UserId { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public string DisplayName { get; set; }

    public string Scope { get; set; }

    public IList<Guid> TargetAccounts { get; set; }

    public string Token { get; set; }

    public string AlternateToken { get; set; }

    public bool IsValid { get; set; }

    public bool IsPublic { get; set; }

    public string PublicData { get; set; }

    public string Source { get; set; }

    public IDictionary<string, string> Claims { get; set; }
  }
}
