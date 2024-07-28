// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.InvalidUserRequestException
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions
{
  public class InvalidUserRequestException : VssServiceException
  {
    public InvalidUserRequestException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public InvalidUserRequestException(string message)
      : base(message)
    {
    }

    protected InvalidUserRequestException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
