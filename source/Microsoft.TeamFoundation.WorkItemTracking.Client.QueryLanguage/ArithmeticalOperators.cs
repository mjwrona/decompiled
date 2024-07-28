// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.ArithmeticalOperators
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ArithmeticalOperators
  {
    private static string[] s_Strings = new string[2]
    {
      "+",
      "-"
    };

    public static Arithmetic Find(string op) => (Arithmetic) (Array.IndexOf<string>(ArithmeticalOperators.s_Strings, op) + 1);

    public static string GetString(Arithmetic op) => ArithmeticalOperators.s_Strings[(int) (op - 1)];
  }
}
