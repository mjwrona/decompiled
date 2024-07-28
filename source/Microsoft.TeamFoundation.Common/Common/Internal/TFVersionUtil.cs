// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.TFVersionUtil
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TFVersionUtil
  {
    public static string GetProductVersion() => typeof (TFCommonUtil).Assembly.GetFileVersion();

    public static string GetProductVersion(bool includeRevision) => typeof (TFCommonUtil).Assembly.GetFileVersion(includeRevision);

    internal static string GetFileVersion(this Assembly assembly) => assembly.GetFileVersion(false);

    internal static string GetFileVersion(this Assembly assembly, bool includeRevision)
    {
      string fileVersion = "1.0.0" + (includeRevision ? ".0" : string.Empty);
      foreach (object customAttribute in assembly.GetCustomAttributes(false))
      {
        if (customAttribute is AssemblyFileVersionAttribute versionAttribute)
        {
          try
          {
            fileVersion = new Version(versionAttribute.Version).ToString(includeRevision ? 4 : 3);
            break;
          }
          catch (ArgumentException ex)
          {
            break;
          }
        }
      }
      return fileVersion;
    }
  }
}
