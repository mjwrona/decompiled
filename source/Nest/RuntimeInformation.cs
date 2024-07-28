// Decompiled with JetBrains decompiler
// Type: Nest.RuntimeInformation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nest
{
  internal static class RuntimeInformation
  {
    private static string _frameworkDescription;
    private static string _osDescription;

    public static string FrameworkDescription
    {
      get
      {
        if (RuntimeInformation._frameworkDescription == null)
          RuntimeInformation._frameworkDescription = ".NET Framework " + ((IEnumerable<AssemblyFileVersionAttribute>) Attribute.GetCustomAttributes(typeof (object).Assembly, typeof (AssemblyFileVersionAttribute))).OrderByDescending<AssemblyFileVersionAttribute, string>((Func<AssemblyFileVersionAttribute, string>) (a => a.Version)).First<AssemblyFileVersionAttribute>().Version;
        return RuntimeInformation._frameworkDescription;
      }
    }

    public static string OSDescription
    {
      get
      {
        if (RuntimeInformation._osDescription == null)
        {
          int platform = (int) Environment.OSVersion.Platform;
          int num;
          switch (platform)
          {
            case 4:
            case 6:
              num = 0;
              break;
            default:
              num = platform != 128 ? 1 : 0;
              break;
          }
          RuntimeInformation._osDescription = num == 0 ? Environment.OSVersion.VersionString : NativeMethods.Windows.RtlGetVersion() ?? "Microsoft Windows";
        }
        return RuntimeInformation._osDescription;
      }
    }
  }
}
