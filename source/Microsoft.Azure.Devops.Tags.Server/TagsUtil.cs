// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.TagsUtil
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Devops.Tags.Server
{
  public static class TagsUtil
  {
    public static void ValidateTagName(string name)
    {
      string reason;
      if (!TagsUtil.IsValidTagName(name, out reason))
        throw new InvalidTagNameException(reason);
    }

    public static bool IsValidTagName(string name, out string reason)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        reason = Resources.InvalidTagName_Empty();
        return false;
      }
      if (TagsUtil.IsTagNameTooLong(name))
      {
        reason = Resources.InvalidTagName_TooLong((object) 400);
        return false;
      }
      if (TagsUtil.ContainsTagSeparatorCharacters(name))
      {
        reason = Resources.InvalidTagName_Separator((object) name);
        return false;
      }
      if (ArgumentUtility.IsInvalidString(name, false))
      {
        reason = Resources.InvalidTagName_InvalidCharacters((object) name);
        return false;
      }
      reason = string.Empty;
      return true;
    }

    private static bool IsTagNameTooLong(string name) => name.Length > 400;

    private static bool ContainsTagSeparatorCharacters(string name) => name.Any<char>((Func<char, bool>) (c => ((IEnumerable<char>) TaggingHelper.TagSeparators).Contains<char>(c)));
  }
}
