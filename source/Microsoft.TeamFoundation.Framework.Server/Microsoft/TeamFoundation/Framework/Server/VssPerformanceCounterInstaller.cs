// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssPerformanceCounterInstaller
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssPerformanceCounterInstaller : MarshalByRefObject
  {
    public static VssPerformanceCounterInstaller CreateInstanceInDomain(AppDomain appDomain) => (VssPerformanceCounterInstaller) appDomain.CreateInstanceAndUnwrap(typeof (VssPerformanceCounterInstaller).Assembly.FullName, typeof (VssPerformanceCounterInstaller).FullName);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public IReadOnlyCollection<string> ProbeAssembliesAndExtractManifestFilesForInstall(
      string assemblyPath)
    {
      return this.ProbeAssembliesAndExtractManifestFilesForInstall(assemblyPath, assemblyPath);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public IReadOnlyCollection<string> ProbeAssembliesAndExtractManifestFilesForInstall(
      string assemblyPath,
      string manifestsDirectory)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(assemblyPath, nameof (assemblyPath));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(manifestsDirectory, nameof (manifestsDirectory));
      List<string> manifestFilesForInstall = new List<string>();
      string path1 = Path.Combine(assemblyPath, "plugins");
      IEnumerable<string> strings = Directory.EnumerateFiles(assemblyPath, "Microsoft*.dll", SearchOption.TopDirectoryOnly).Concat<string>(Directory.Exists(path1) ? Directory.EnumerateFiles(path1, "Microsoft*.dll", SearchOption.TopDirectoryOnly) : Enumerable.Empty<string>());
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (string str in strings)
      {
        if (stringSet.Add(Path.GetFileName(str)))
        {
          try
          {
            Assembly assembly = Assembly.ReflectionOnlyLoadFrom(str);
            foreach (string name in ((IEnumerable<string>) assembly.GetManifestResourceNames()).Where<string>((Func<string, bool>) (n => n.EndsWith("PerfCounters.man.template", StringComparison.OrdinalIgnoreCase))))
            {
              string contents;
              using (Stream manifestResourceStream = assembly.GetManifestResourceStream(name))
              {
                using (MemoryStream destination = new MemoryStream())
                {
                  manifestResourceStream?.CopyTo((Stream) destination);
                  contents = Encoding.UTF8.GetString(destination.ToArray());
                }
              }
              if (!string.IsNullOrWhiteSpace(contents))
              {
                string path2 = name.Substring(0, name.IndexOf(".template", StringComparison.Ordinal));
                string path3 = Path.Combine(manifestsDirectory, path2);
                contents = contents.Replace("@RESOURCE_ASSEMBLY_PATH@", Path.GetDirectoryName(str));
                File.WriteAllText(path3, contents);
                manifestFilesForInstall.Add(path3);
              }
            }
          }
          catch (BadImageFormatException ex)
          {
          }
        }
      }
      return (IReadOnlyCollection<string>) manifestFilesForInstall;
    }
  }
}
