// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.InternalUpstreamFailureException
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public class InternalUpstreamFailureException : VssServiceException
  {
    public InternalUpstreamFailureException(string message, Uri endpoint)
      : base(message)
    {
      this.Endpoint = endpoint;
    }

    public InternalUpstreamFailureException(string message, Exception innerException, Uri endpoint)
      : base(message, innerException)
    {
      this.Endpoint = endpoint;
    }

    public Uri Endpoint { get; }
  }
}
