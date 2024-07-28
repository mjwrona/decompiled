// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcCommonUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  internal class TfvcCommonUtility
  {
    public static VersionControlRecursionType ParseVersionControlRecursionLevel(
      string recursionLevel)
    {
      VersionControlRecursionType result = VersionControlRecursionType.None;
      if (!string.IsNullOrWhiteSpace(recursionLevel) && !Enum.TryParse<VersionControlRecursionType>(recursionLevel, true, out result))
        throw new ArgumentException(Resources.Format("InvalidQueryParam", (object) recursionLevel, (object) nameof (recursionLevel), (object) string.Join(", ", Enum.GetNames(typeof (VersionControlRecursionType)))));
      return result;
    }

    public static RecursionType ConvertVersionControlRecursionType(
      VersionControlRecursionType vcRecursionType)
    {
      if (vcRecursionType == VersionControlRecursionType.OneLevel || vcRecursionType == VersionControlRecursionType.OneLevelPlusNestedEmptyFolders)
        return RecursionType.OneLevel;
      return vcRecursionType == VersionControlRecursionType.Full ? RecursionType.Full : RecursionType.None;
    }
  }
}
