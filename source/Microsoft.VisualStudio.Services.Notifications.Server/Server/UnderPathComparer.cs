// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.UnderPathComparer
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class UnderPathComparer : IComparer<string>
  {
    public int Compare(string arrayValue, string targetValue) => UnderPathComparer.UnderCompare(arrayValue, targetValue);

    private static int UnderCompare(string path, string target)
    {
      if (path.Length > target.Length)
      {
        switch (path[target.Length])
        {
          case '/':
          case '\\':
            return VssStringComparer.StringFieldConditionEquality.Compare(path, 0, target, 0, target.Length);
        }
      }
      return VssStringComparer.StringFieldConditionEquality.Compare(path, target);
    }

    public static bool IsTargetUnder(string[] paths, string target)
    {
      target = target.TrimEnd('/', '\\');
      return Array.BinarySearch<string>(paths, target, (IComparer<string>) new UnderPathComparer()) >= 0;
    }

    public static bool IsTargetUnder(string path, string target)
    {
      target = target.TrimEnd('/', '\\');
      return UnderPathComparer.UnderCompare(path, target) == 0;
    }
  }
}
