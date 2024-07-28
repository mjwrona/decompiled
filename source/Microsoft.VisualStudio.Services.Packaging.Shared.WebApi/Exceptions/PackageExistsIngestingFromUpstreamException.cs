// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageExistsIngestingFromUpstreamException
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions
{
  public class PackageExistsIngestingFromUpstreamException : PackageAlreadyExistsException
  {
    public PackageExistsIngestingFromUpstreamException()
    {
    }

    public PackageExistsIngestingFromUpstreamException(string message)
      : base(message)
    {
    }

    public PackageExistsIngestingFromUpstreamException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
