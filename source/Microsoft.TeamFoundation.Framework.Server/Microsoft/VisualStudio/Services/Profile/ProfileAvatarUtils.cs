// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.ProfileAvatarUtils
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Drawing;

namespace Microsoft.VisualStudio.Services.Profile
{
  public static class ProfileAvatarUtils
  {
    public const int DefaultAvatarFileId = -1;

    public static byte[] GenerateAvatar(
      string displayName,
      Color backgroundColor,
      Microsoft.VisualStudio.Services.Users.AvatarSize avatarSize,
      AvatarImageFormat avatarImageFormat)
    {
      return AvatarUtils.GenerateAvatar(displayName, backgroundColor, ProfileAvatarUtils.MapToAvatarSizeInPixels(avatarSize), avatarImageFormat);
    }

    public static AvatarSize MapToAvatarSize(string size)
    {
      if (StringComparer.CurrentCultureIgnoreCase.Equals(size, "Small"))
        return AvatarSize.Small;
      return StringComparer.CurrentCultureIgnoreCase.Equals(size, "Medium") || !StringComparer.CurrentCultureIgnoreCase.Equals(size, "Large") ? AvatarSize.Medium : AvatarSize.Large;
    }

    public static int MapToAvatarSizeInPixels(Microsoft.VisualStudio.Services.Users.AvatarSize size) => ProfileAvatarUtils.MapToAvatarSizeInPixels((AvatarSize) size);

    public static int MapToAvatarSizeInPixels(AvatarSize size)
    {
      switch (size)
      {
        case AvatarSize.Small:
          return 34;
        case AvatarSize.Medium:
          return 44;
        case AvatarSize.Large:
          return 220;
        default:
          throw new ArgumentException("The argument is out of the supported enum range", nameof (size));
      }
    }
  }
}
