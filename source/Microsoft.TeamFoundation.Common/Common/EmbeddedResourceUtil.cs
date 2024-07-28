// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.EmbeddedResourceUtil
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Microsoft.TeamFoundation.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class EmbeddedResourceUtil
  {
    public static string GetResourceAsString(string resourceName) => EmbeddedResourceUtil.GetResourceAsString(resourceName, Assembly.GetCallingAssembly());

    public static string GetResourceAsString(string resourceName, Assembly assembly)
    {
      string resourceAsString = (string) null;
      using (Stream manifestResourceStream = assembly.GetManifestResourceStream(resourceName))
      {
        if (manifestResourceStream != null)
        {
          using (StreamReader streamReader = new StreamReader(manifestResourceStream))
            resourceAsString = streamReader.ReadToEnd();
        }
      }
      return resourceAsString;
    }
  }
}
