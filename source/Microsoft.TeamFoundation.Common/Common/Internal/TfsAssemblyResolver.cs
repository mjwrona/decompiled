// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.TfsAssemblyResolver
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TfsAssemblyResolver : IDisposable
  {
    private readonly object m_lock = new object();
    private Dictionary<string, Assembly> m_replacementAssemblies;
    private static TfsAssemblyResolver.AssemblyRedirectInfo[] s_assemblyRedirects = new TfsAssemblyResolver.AssemblyRedirectInfo[1]
    {
      new TfsAssemblyResolver.AssemblyRedirectInfo()
      {
        AssemblyName = "Microsoft.SharePoint",
        From = new Version("14.0.0.0"),
        To = new Version[1]{ new Version("12.0.0.0") }
      }
    };

    public TfsAssemblyResolver() => AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.OnAssemblyResolve);

    public void Dispose() => AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(this.OnAssemblyResolve);

    private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
    {
      Assembly assembly = (Assembly) null;
      AssemblyName requestedAssemblyName = new AssemblyName(args.Name);
      TfsAssemblyResolver.AssemblyRedirectInfo assemblyRedirectInfo = ((IEnumerable<TfsAssemblyResolver.AssemblyRedirectInfo>) TfsAssemblyResolver.s_assemblyRedirects).FirstOrDefault<TfsAssemblyResolver.AssemblyRedirectInfo>((Func<TfsAssemblyResolver.AssemblyRedirectInfo, bool>) (ri => ri.AssemblyName.Equals(requestedAssemblyName.Name, StringComparison.OrdinalIgnoreCase) && ri.From == requestedAssemblyName.Version));
      if (assemblyRedirectInfo != null)
      {
        for (int index = 0; index < assemblyRedirectInfo.To.Length; ++index)
        {
          AssemblyName assemblyRef = (AssemblyName) requestedAssemblyName.Clone();
          assemblyRef.Version = assemblyRedirectInfo.To[index];
          lock (this.m_lock)
          {
            if (this.m_replacementAssemblies == null)
              this.m_replacementAssemblies = new Dictionary<string, Assembly>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            else if (this.m_replacementAssemblies.TryGetValue(assemblyRef.FullName, out assembly))
            {
              if (!(assembly == (Assembly) null))
                return assembly;
              continue;
            }
          }
          this.m_replacementAssemblies.Add(assemblyRef.FullName, (Assembly) null);
          try
          {
            assembly = Assembly.Load(assemblyRef);
          }
          catch (FileNotFoundException ex)
          {
          }
          if (assembly != (Assembly) null)
          {
            lock (this.m_lock)
              this.m_replacementAssemblies[assemblyRef.FullName] = assembly;
            return assembly;
          }
        }
      }
      return assembly;
    }

    private class AssemblyRedirectInfo
    {
      public string AssemblyName { get; set; }

      public Version From { get; set; }

      public Version[] To { get; set; }
    }
  }
}
