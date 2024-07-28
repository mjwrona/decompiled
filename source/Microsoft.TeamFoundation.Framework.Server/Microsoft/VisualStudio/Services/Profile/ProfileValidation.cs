// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.ProfileValidation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Profile
{
  public static class ProfileValidation
  {
    public static readonly TimeSpan RegexTimeout = new TimeSpan(0, 0, 0, 3);
    private static readonly int MaxEmailLength = 256;
    private const int s_maxAvatarSize = 4194304;
    private const int s_minAvatarSize = 196;
    private const string s_dummyIdentifier = "dummyIdentifier";

    public static void CheckAvatarData(byte[] data)
    {
      if (data == null)
        throw new BadAvatarValueException();
      if (data.Length > 4194304)
        throw new AvatarTooBigException();
      if (data.Length < 196)
        throw new AvatarTooSmallException();
    }

    public static string CheckDisplayName(string displayName)
    {
      if (!ProfileValidation.IsValidDisplayName(displayName))
        throw new BadDisplayNameException(TFCommonResources.BAD_DISPLAY_NAME((object) displayName));
      try
      {
        return IdentityHelper.CleanCustomDisplayName(displayName, new IdentityDescriptor("Microsoft.TeamFoundation.Identity", "dummyIdentifier"));
      }
      catch
      {
        throw new BadDisplayNameException(TFCommonResources.BAD_DISPLAY_NAME((object) displayName));
      }
    }

    public static bool IsValidDisplayName(string displayName)
    {
      if (!string.IsNullOrEmpty(displayName))
      {
        if (displayName.Length <= 256)
        {
          try
          {
            TFCommonUtil.CheckDisplayName(ref displayName);
          }
          catch
          {
            return false;
          }
          try
          {
            IdentityHelper.CleanCustomDisplayName(displayName, new IdentityDescriptor("Microsoft.TeamFoundation.Identity", "dummyIdentifier"));
          }
          catch (InvalidDisplayNameException ex)
          {
            return false;
          }
          return true;
        }
      }
      return false;
    }

    public static void CheckEmailAddress(string emailAdress)
    {
      if (emailAdress == null || emailAdress.Length > ProfileValidation.MaxEmailLength || !ArgumentUtility.IsValidEmailAddress(emailAdress))
        throw new BadEmailAddressException(CommonResources.InvalidEmailAddressError());
    }

    public static void CheckPublicAlias(string publicAlias)
    {
      if (!ProfileValidation.IsValidPublicAlias(publicAlias))
        throw new BadPublicAliasException(TFCommonResources.BAD_ALIAS((object) publicAlias));
    }

    public static bool IsValidPublicAlias(string strIn) => !string.IsNullOrEmpty(strIn) && strIn.Length <= 50 && !ArgumentUtility.IsInvalidString(strIn) && strIn.All<char>((Func<char, bool>) (c => ProfileValidation.IsUnicodeLetter(c) || ProfileValidation.IsDecimalDigit(c) || c == '_' || c == '-' || c == ' '));

    public static bool IsUnicodeLetter(char c)
    {
      UnicodeCategory unicodeCategory = char.GetUnicodeCategory(c);
      switch (unicodeCategory)
      {
        case UnicodeCategory.UppercaseLetter:
        case UnicodeCategory.LowercaseLetter:
        case UnicodeCategory.TitlecaseLetter:
        case UnicodeCategory.ModifierLetter:
          return true;
        default:
          return unicodeCategory == UnicodeCategory.OtherLetter;
      }
    }

    public static bool IsDecimalDigit(char c) => char.GetUnicodeCategory(c) == UnicodeCategory.DecimalDigitNumber;
  }
}
