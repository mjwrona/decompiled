// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ExpectedExceptionExtensions
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class ExpectedExceptionExtensions
  {
    private const string c_expectedKey = "isExpected";

    public static Exception Expected(this Exception ex, string area)
    {
      if (!string.IsNullOrEmpty(area))
        ex.Data[(object) "isExpected"] = (object) area;
      return ex;
    }

    public static bool ExpectedExceptionFilter(this Exception ex, string area)
    {
      ex.Expected(area);
      return false;
    }

    public static bool IsExpected(this Exception ex, string area) => !string.IsNullOrEmpty(area) && area.Equals(ex.Data[(object) "isExpected"] as string, StringComparison.OrdinalIgnoreCase);
  }
}
