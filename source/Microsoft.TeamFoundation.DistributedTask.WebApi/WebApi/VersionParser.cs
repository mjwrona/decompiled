// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.VersionParser
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public static class VersionParser
  {
    public static void ParseVersion(
      string version,
      out int major,
      out int minor,
      out int patch,
      out string semanticVersion)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(version, nameof (version));
      string[] strArray = version.Split(new char[2]
      {
        '.',
        '-'
      }, StringSplitOptions.None);
      if (strArray.Length < 3 || strArray.Length > 4)
        throw new ArgumentException("wrong number of segments");
      if (!int.TryParse(strArray[0], out major))
        throw new ArgumentException(nameof (major));
      if (!int.TryParse(strArray[1], out minor))
        throw new ArgumentException(nameof (minor));
      if (!int.TryParse(strArray[2], out patch))
        throw new ArgumentException(nameof (patch));
      semanticVersion = (string) null;
      if (strArray.Length != 4)
        return;
      semanticVersion = strArray[3];
    }
  }
}
