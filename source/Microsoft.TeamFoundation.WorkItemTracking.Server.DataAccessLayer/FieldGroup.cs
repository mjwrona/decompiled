// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FieldGroup
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class FieldGroup
  {
    internal const int Int16Index = 0;
    internal const int Int32Index = 1;
    internal const int BoolIndex = 2;
    internal const int ObjectIndex = 3;
    internal const int Count = 4;

    internal static int[] CreateGroupArray() => FieldGroup.CreateGroupArray(0);

    internal static int[] CreateGroupArray(int initialValue)
    {
      int[] groupArray = new int[4];
      for (int index = 0; index < 4; ++index)
        groupArray[index] = initialValue;
      return groupArray;
    }

    internal static int GetFieldGroupIndex(Type type)
    {
      if (type.Equals(typeof (short)))
        return 0;
      if (type.Equals(typeof (int)))
        return 1;
      return type.Equals(typeof (bool)) ? 2 : 3;
    }
  }
}
