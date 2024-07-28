// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.CodeChurnUtility
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public static class CodeChurnUtility
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static byte[] ConvertToByte(
      int linesCount,
      int linesAdded,
      int linesDeleted,
      int linesModified)
    {
      byte[] byteValue;
      if (linesModified > 0)
      {
        byteValue = new byte[16];
        CodeChurnUtility.PopulateBytes(byteValue, 0, linesCount);
        CodeChurnUtility.PopulateBytes(byteValue, 4, linesAdded);
        CodeChurnUtility.PopulateBytes(byteValue, 8, linesDeleted);
        CodeChurnUtility.PopulateBytes(byteValue, 12, linesModified);
      }
      else if (linesDeleted > 0)
      {
        byteValue = new byte[12];
        CodeChurnUtility.PopulateBytes(byteValue, 0, linesCount);
        CodeChurnUtility.PopulateBytes(byteValue, 4, linesAdded);
        CodeChurnUtility.PopulateBytes(byteValue, 8, linesDeleted);
      }
      else if (linesAdded > 0)
      {
        byteValue = new byte[8];
        CodeChurnUtility.PopulateBytes(byteValue, 0, linesCount);
        CodeChurnUtility.PopulateBytes(byteValue, 4, linesAdded);
      }
      else
      {
        byteValue = new byte[4];
        CodeChurnUtility.PopulateBytes(byteValue, 0, linesCount);
      }
      return byteValue;
    }

    private static void PopulateBytes(byte[] byteValue, int offset, int value)
    {
      byteValue[offset] = (byte) (value >> 24);
      byteValue[offset + 1] = (byte) (value >> 16);
      byteValue[offset + 2] = (byte) (value >> 8);
      byteValue[offset + 3] = (byte) value;
    }

    private static int GetInt(byte[] byteValue, int offset) => ((int) byteValue[offset] << 24) + ((int) byteValue[offset + 1] << 16) + ((int) byteValue[offset + 2] << 8) + (int) byteValue[offset + 3];

    public static void ConvertToInt(
      byte[] byteValue,
      out int linesCount,
      out int linesAdded,
      out int linesDeleted,
      out int linesModified)
    {
      if (byteValue.Length > 16 || byteValue.Length % 4 != 0)
        throw new ArgumentOutOfRangeException(nameof (byteValue));
      linesCount = 0;
      linesAdded = 0;
      linesDeleted = 0;
      linesModified = 0;
      if (byteValue.Length == 16)
        linesModified = CodeChurnUtility.GetInt(byteValue, 12);
      if (byteValue.Length >= 12)
        linesDeleted = CodeChurnUtility.GetInt(byteValue, 8);
      if (byteValue.Length >= 8)
        linesAdded = CodeChurnUtility.GetInt(byteValue, 4);
      if (byteValue.Length < 4)
        return;
      linesCount = CodeChurnUtility.GetInt(byteValue, 0);
    }
  }
}
