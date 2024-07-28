// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.WebApi.UnsupportedScopeTypeException
// Assembly: Microsoft.VisualStudio.Services.Favorites.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86250CA5-6C66-4E9F-9014-5EE4DB12BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Favorites.WebApi
{
  public class UnsupportedScopeTypeException : FavoriteServiceException
  {
    public UnsupportedScopeTypeException()
    {
    }

    public UnsupportedScopeTypeException(string message)
      : base(message)
    {
    }

    public UnsupportedScopeTypeException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected UnsupportedScopeTypeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
