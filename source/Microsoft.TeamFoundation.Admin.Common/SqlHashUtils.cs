// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.SqlHashUtils
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.Admin
{
  public static class SqlHashUtils
  {
    private const string c_beginNoCompare = "-- BEGIN NOCOMPARE (BEGIN DBG)";
    private const string c_endNoCompare = "-- END NOCOMPARE (END DBG)";

    public static string ComputeHash(
      string[] lines,
      Predicate<string> firstLinePredicate,
      string salt = "4825 Creekstone Drive")
    {
      for (int startIndex = 0; startIndex < lines.Length; ++startIndex)
      {
        if (firstLinePredicate(lines[startIndex]))
          return SqlHashUtils.ComputeHash(lines, startIndex, salt);
      }
      return (string) null;
    }

    public static string ComputeHash(string[] lines, int startIndex, string salt = "4825 Creekstone Drive")
    {
      byte[] hash;
      using (SHA1CryptoServiceProvider cryptoServiceProvider = new SHA1CryptoServiceProvider())
      {
        bool flag = false;
        for (int index = startIndex; index < lines.Length; ++index)
        {
          string line = lines[index];
          if (!string.IsNullOrWhiteSpace(line))
          {
            if (flag)
            {
              if (line.StartsWith("-- END NOCOMPARE (END DBG)"))
                flag = false;
            }
            else if (line.StartsWith("-- BEGIN NOCOMPARE (BEGIN DBG)"))
            {
              flag = true;
            }
            else
            {
              byte[] bytes = Encoding.Unicode.GetBytes(line.TrimEnd());
              cryptoServiceProvider.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
            }
          }
        }
        byte[] bytes1 = Encoding.Unicode.GetBytes(salt);
        cryptoServiceProvider.TransformFinalBlock(bytes1, 0, bytes1.Length);
        hash = cryptoServiceProvider.Hash;
      }
      return HexConverter.ToString(hash);
    }
  }
}
