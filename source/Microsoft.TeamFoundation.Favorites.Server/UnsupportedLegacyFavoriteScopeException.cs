// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Favorites.UnsupportedLegacyFavoriteScopeException
// Assembly: Microsoft.TeamFoundation.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4742830F-DF0E-4509-8C2D-2540DAED73F4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Favorites
{
  [Serializable]
  internal class UnsupportedLegacyFavoriteScopeException : TeamFoundationServiceException
  {
    public UnsupportedLegacyFavoriteScopeException()
    {
    }

    public UnsupportedLegacyFavoriteScopeException(string message)
      : base(message)
    {
    }

    public UnsupportedLegacyFavoriteScopeException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected UnsupportedLegacyFavoriteScopeException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
