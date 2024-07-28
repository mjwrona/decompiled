// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.TooManyVersionsException
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions
{
  public class TooManyVersionsException : VssServiceException
  {
    public TooManyVersionsException(string message)
      : base(message)
    {
    }

    public TooManyVersionsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
