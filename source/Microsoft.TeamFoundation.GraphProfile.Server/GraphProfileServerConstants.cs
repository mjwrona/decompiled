// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.GraphProfile.Server.GraphProfileServerConstants
// Assembly: Microsoft.TeamFoundation.GraphProfile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E08BE30A-4AE3-40A5-BE5B-3FCDC59E061E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.GraphProfile.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.GraphProfile.Server
{
  public static class GraphProfileServerConstants
  {
    public static class MediaType
    {
      public const string Json = "application/json";
      public const string Png = "image/png";
    }

    public static class MediaTypeFormatter
    {
      public static readonly VssJsonMediaTypeFormatter Json = (VssJsonMediaTypeFormatter) new ServerVssJsonMediaTypeFormatter();
    }

    public static class MemberAvatarClientCache
    {
      public static class ExpirationInHours
      {
        public const int Default = 48;
        public const int WithCacheBusting = 8760;
      }
    }
  }
}
