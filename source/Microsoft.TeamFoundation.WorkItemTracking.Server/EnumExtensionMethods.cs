// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.EnumExtensionMethods
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class EnumExtensionMethods
  {
    public static IEnumerable<Enum> GetFlags(this Enum @enum) => Enum.GetValues(@enum.GetType()).Cast<Enum>().Where<Enum>((Func<Enum, bool>) (value => !object.Equals((object) (int) value, (object) 0) && @enum.HasFlag(value)));
  }
}
