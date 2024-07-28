// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.WebApi.FavoriteServiceException
// Assembly: Microsoft.VisualStudio.Services.Favorites.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86250CA5-6C66-4E9F-9014-5EE4DB12BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Favorites.WebApi
{
  public class FavoriteServiceException : VssServiceException
  {
    public FavoriteServiceException()
    {
    }

    public FavoriteServiceException(string message)
      : base(message)
    {
    }

    public FavoriteServiceException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected FavoriteServiceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
