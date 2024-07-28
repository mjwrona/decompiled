// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IReadOnlyVssIdentity
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  public interface IReadOnlyVssIdentity
  {
    Guid Id { get; }

    IdentityDescriptor Descriptor { get; }

    bool IsContainer { get; }

    bool IsExternalUser { get; }

    string DisplayName { get; }

    string ProviderDisplayName { get; }

    string CustomDisplayName { get; }

    TValue GetProperty<TValue>(string name, TValue defaultValue);
  }
}
