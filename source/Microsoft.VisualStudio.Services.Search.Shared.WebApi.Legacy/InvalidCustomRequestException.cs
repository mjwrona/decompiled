// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.InvalidCustomRequestException
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [ExceptionMapping("0.0", "3.0", "InvalidCustomRequestException", "Microsoft.VisualStudio.Services.Search.WebApi.Legacy.InvalidCustomRequestException, Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class InvalidCustomRequestException : SearchException
  {
    public InvalidCustomRequestException(string message)
      : base(message)
    {
    }

    public InvalidCustomRequestException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
