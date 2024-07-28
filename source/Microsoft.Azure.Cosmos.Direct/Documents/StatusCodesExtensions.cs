// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.StatusCodesExtensions
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents
{
  internal static class StatusCodesExtensions
  {
    private static readonly Dictionary<int, string> CodeNameMap = new Dictionary<int, string>();

    static StatusCodesExtensions()
    {
      StatusCodesExtensions.CodeNameMap[0] = string.Empty;
      foreach (StatusCodes key in Enum.GetValues(typeof (StatusCodes)))
        StatusCodesExtensions.CodeNameMap[(int) key] = key.ToString();
    }

    public static string ToStatusCodeString(this StatusCodes code)
    {
      string str;
      return !StatusCodesExtensions.CodeNameMap.TryGetValue((int) code, out str) ? code.ToString() : str;
    }
  }
}
