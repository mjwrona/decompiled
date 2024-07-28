// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions
{
  [ExceptionMapping("0.0", "3.0", "InvalidQueryException", "Microsoft.VisualStudio.Services.Search.WebApi.InvalidQueryException, Microsoft.VisualStudio.Services.Search.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class InvalidQueryException : SearchException
  {
    public InvalidQueryException(string message)
      : base(message)
    {
    }

    public InvalidQueryException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
