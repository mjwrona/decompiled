// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.InvalidQueryException
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [ExceptionMapping("0.0", "3.0", "InvalidQueryException", "Microsoft.VisualStudio.Services.Search.WebApi.Legacy.InvalidQueryException, Microsoft.VisualStudio.Services.Search.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
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
