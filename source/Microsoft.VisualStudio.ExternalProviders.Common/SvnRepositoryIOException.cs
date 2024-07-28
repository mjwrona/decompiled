// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.SvnRepositoryIOException
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

using System;

namespace Microsoft.VisualStudio.ExternalProviders.Common
{
  public sealed class SvnRepositoryIOException : SvnException
  {
    public SvnRepositoryIOException()
    {
    }

    public SvnRepositoryIOException(string message)
      : base(message)
    {
    }

    public SvnRepositoryIOException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
