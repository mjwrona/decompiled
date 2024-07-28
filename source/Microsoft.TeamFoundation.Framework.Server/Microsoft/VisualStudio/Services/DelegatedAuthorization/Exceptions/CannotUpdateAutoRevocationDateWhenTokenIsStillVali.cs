// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.Exceptions.CannotUpdateAutoRevocationDateWhenTokenIsStillValidException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.Exceptions
{
  [Serializable]
  public class CannotUpdateAutoRevocationDateWhenTokenIsStillValidException : VssServiceException
  {
    public CannotUpdateAutoRevocationDateWhenTokenIsStillValidException(string message)
      : base(message)
    {
    }

    public CannotUpdateAutoRevocationDateWhenTokenIsStillValidException(
      string message,
      Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
