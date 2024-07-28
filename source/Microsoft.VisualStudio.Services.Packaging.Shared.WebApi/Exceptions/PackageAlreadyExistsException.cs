// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageAlreadyExistsException
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions
{
  [Serializable]
  public class PackageAlreadyExistsException : VssServiceException
  {
    public PackageAlreadyExistsException()
    {
    }

    public PackageAlreadyExistsException(string message)
      : base(message)
    {
    }

    public PackageAlreadyExistsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected PackageAlreadyExistsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
