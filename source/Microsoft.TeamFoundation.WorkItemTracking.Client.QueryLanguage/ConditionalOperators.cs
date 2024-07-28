// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.ConditionalOperators
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ConditionalOperators
  {
    private static string[] s_Strings = new string[15]
    {
      "=",
      "<>",
      "<",
      ">",
      "<=",
      ">=",
      "under",
      "in",
      "contains",
      "contains words",
      "in group",
      "is empty",
      "is not empty",
      "==",
      "!="
    };

    public static Condition Find(string op)
    {
      op = op.ToLower(CultureInfo.InvariantCulture);
      Condition condition = (Condition) (Array.IndexOf<string>(ConditionalOperators.s_Strings, op) + 1);
      switch (condition)
      {
        case Condition.EqualsAlias:
          condition = Condition.Equals;
          break;
        case Condition.NotEqualsAlias:
          condition = Condition.NotEquals;
          break;
      }
      return condition;
    }

    public static string GetString(Condition condition) => ConditionalOperators.s_Strings[(int) (condition - 1)];
  }
}
