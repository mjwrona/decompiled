// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlRoutineComparer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class SqlRoutineComparer : SqlComparer
  {
    protected const string c_beginNoCompare = "-- BEGIN NOCOMPARE (BEGIN DBG)";
    protected const string c_endNoCompare = "-- END NOCOMPARE (END DBG)";
    protected static readonly char[] s_crlf = new char[2]
    {
      '\r',
      '\n'
    };

    public SqlRoutineComparer(string db1, string db2)
      : base(db1, db2)
    {
    }

    protected override string PrepareForOrdinalComparison(string definition)
    {
      definition = this.RemoveNoCompareBlocks(definition);
      definition = this.RemoveComments(definition);
      definition = this.NormalizeKeywords(definition);
      definition = this.Trim(definition);
      definition = definition.ToLowerInvariant();
      return definition;
    }

    protected abstract string NormalizeKeywords(string definition);

    protected string RemoveComments(string definition)
    {
      if (definition != null)
      {
        int startIndex1;
        int length1;
        for (startIndex1 = 0; (length1 = definition.IndexOf("/*", startIndex1, StringComparison.Ordinal)) >= 0; startIndex1 = length1)
        {
          int num = definition.IndexOf("*/", length1 + 2, StringComparison.Ordinal);
          if (num >= 0)
            definition = definition.Substring(0, length1) + definition.Substring(num + 2);
          else
            break;
        }
        int length2;
        for (; (length2 = definition.IndexOf("--", startIndex1, StringComparison.Ordinal)) >= 0; startIndex1 = length2)
        {
          int startIndex2 = definition.IndexOfAny(SqlRoutineComparer.s_crlf, length2 + 2);
          if (startIndex2 < 0)
          {
            definition = definition.Substring(0, length2);
            break;
          }
          definition = definition.Substring(0, length2) + definition.Substring(startIndex2);
        }
      }
      return definition;
    }

    protected string RemoveNoCompareBlocks(string definition)
    {
      if (definition != null)
      {
        int num1 = 0;
        while (num1 >= 0)
        {
          num1 = definition.IndexOf("-- BEGIN NOCOMPARE (BEGIN DBG)", num1);
          if (num1 >= 0)
          {
            int num2 = definition.IndexOf("-- END NOCOMPARE (END DBG)", num1 + "-- BEGIN NOCOMPARE (BEGIN DBG)".Length);
            if (num2 >= 0)
              definition = definition.Substring(0, num1) + definition.Substring(num2 + "-- END NOCOMPARE (END DBG)".Length);
            else
              break;
          }
        }
      }
      return definition;
    }

    protected string Trim(string definition) => definition.Replace(" ", "").Replace("\t", "").Replace("\r\n", "\n").Replace("\r", "\n").Replace("\nas\n", "AS").Replace("\nAs\n", "AS").Replace("\n", "");
  }
}
