// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle.TokenSigningKeyNamespace
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle
{
  public class TokenSigningKeyNamespace
  {
    public string Name { get; set; }

    public int SigningKeyBatchSize { get; set; }

    public int SigningLifetimeInDays { get; set; }

    public int ValidationLifetimeInDays { get; set; }

    public IReadOnlyList<int> SigningKeyIds { get; set; }

    public IReadOnlyDictionary<int, TokenSigningKeyMetadata> ValidationKeys { get; set; }
  }
}
