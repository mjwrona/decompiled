// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Utils.NameValidator
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Feed.Server.Utils
{
  public class NameValidator
  {
    protected const int MaxNameLength = 64;

    protected virtual char[] IllegalChars => new char[24]
    {
      '@',
      '~',
      ';',
      '{',
      '}',
      '\'',
      '+',
      '=',
      ',',
      '<',
      '>',
      '|',
      '/',
      '\\',
      '?',
      ':',
      '&',
      '$',
      '*',
      '"',
      '#',
      '[',
      ']',
      '%'
    };

    public virtual bool IsValidName(string input, out string errorMessage)
    {
      errorMessage = string.Empty;
      if (input == null)
      {
        errorMessage = Resources.Error_NullParameter((object) nameof (input));
        return false;
      }
      string str = input.Trim();
      int length = str.Length;
      if (length == 0 || length > 64)
      {
        errorMessage = Resources.Error_NameLengthInvalid((object) 64);
        return false;
      }
      switch (str[0])
      {
        case '.':
        case '_':
          errorMessage = Resources.Error_NamePrefixInvalid();
          return false;
        default:
          if (str[length - 1] == '.')
          {
            errorMessage = Resources.Error_NameSuffixInvalid();
            return false;
          }
          if (str.IndexOfAny(this.IllegalChars) != -1)
          {
            errorMessage = Resources.Error_NameContainsIllegalCharacters();
            return false;
          }
          if (CssUtils.IsReservedProjectName(str, true))
          {
            errorMessage = Resources.Error_ReservedName();
            return false;
          }
          if (!NameValidator.TryCreateUri("http://www.cpandl.com/" + str, UriKind.Absolute, out Uri _))
          {
            errorMessage = Resources.Error_NameInvalid();
            return false;
          }
          bool isChineseOs = OSDetails.IsChineseOS;
          if (!ArgumentUtility.IsInvalidString(str, false, isChineseOs) && (isChineseOs || !ArgumentUtility.HasSurrogates(str)))
            return true;
          errorMessage = Resources.Error_NameContainsIllegalCharacters();
          return false;
      }
    }

    private static bool TryCreateUri(string uriString, UriKind uriKind, out Uri result)
    {
      try
      {
        return Uri.TryCreate(uriString, uriKind, out result);
      }
      catch (IndexOutOfRangeException ex)
      {
        result = (Uri) null;
        return false;
      }
    }
  }
}
