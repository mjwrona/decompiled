// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence.CiEventParamUtility
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence
{
  public static class CiEventParamUtility
  {
    private static HashSet<char> s_validParamCharacters = new HashSet<char>()
    {
      'A',
      'B',
      'C',
      'D',
      'E',
      'F',
      'G',
      'H',
      'I',
      'J',
      'K',
      'L',
      'M',
      'N',
      'O',
      'P',
      'Q',
      'R',
      'S',
      'T',
      'U',
      'V',
      'W',
      'X',
      'Y',
      'Z',
      'a',
      'b',
      'c',
      'd',
      'e',
      'f',
      'g',
      'h',
      'i',
      'j',
      'k',
      'l',
      'm',
      'n',
      'o',
      'p',
      'q',
      'r',
      's',
      't',
      'u',
      'v',
      'w',
      'x',
      'y',
      'z',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      '0',
      '-',
      '_',
      '.',
      ' ',
      '/'
    };

    public static void CheckParameterForValidCharacters(
      string param,
      string paramName,
      bool allowNull = false)
    {
      if (!allowNull)
        ArgumentUtility.CheckStringForNullOrEmpty(param, paramName);
      if (param != null && StringUtil.ContainsIllegalCharacters(param, CiEventParamUtility.s_validParamCharacters))
        throw new ArgumentException("Invalid argument value", paramName);
    }
  }
}
