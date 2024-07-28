// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.EqualityHelper
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class EqualityHelper
  {
    public static int GetCombinedHashCode(params object[] objs)
    {
      int combinedHashCode = 23;
      foreach (object obj in objs)
        combinedHashCode += obj == null ? 0 : obj.GetHashCode() * 17;
      return combinedHashCode;
    }
  }
}
