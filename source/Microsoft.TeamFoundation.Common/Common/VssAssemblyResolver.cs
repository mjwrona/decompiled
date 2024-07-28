// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.VssAssemblyResolver
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Microsoft.TeamFoundation.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VssAssemblyResolver : IDisposable
  {
    private int m_disposed;
    private static int s_globalAllocated = 0;

    public VssAssemblyResolver()
    {
      this.m_disposed = 0;
      if (Interlocked.Increment(ref VssAssemblyResolver.s_globalAllocated) != 1)
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      AppDomain.CurrentDomain.AssemblyResolve += VssAssemblyResolver.\u003C\u003EO.\u003C0\u003E__OnAssemblyResolve ?? (VssAssemblyResolver.\u003C\u003EO.\u003C0\u003E__OnAssemblyResolve = new ResolveEventHandler(VssAssemblyResolver.OnAssemblyResolve));
    }

    ~VssAssemblyResolver() => this.Dispose();

    public void Dispose()
    {
      if (Interlocked.Exchange(ref this.m_disposed, 1) != 0)
        return;
      if (Interlocked.Decrement(ref VssAssemblyResolver.s_globalAllocated) == 0)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        AppDomain.CurrentDomain.AssemblyResolve -= VssAssemblyResolver.\u003C\u003EO.\u003C0\u003E__OnAssemblyResolve ?? (VssAssemblyResolver.\u003C\u003EO.\u003C0\u003E__OnAssemblyResolve = new ResolveEventHandler(VssAssemblyResolver.OnAssemblyResolve));
      }
      GC.SuppressFinalize((object) this);
    }

    public static Assembly ResolveAssembly(string name)
    {
      Assembly assembly = (Assembly) null;
      AssemblyName assemblyName = new AssemblyName(name);
      foreach (string enumerateVssPath in VssEnvironment.EnumerateVssPaths())
      {
        string str = Path.Combine(enumerateVssPath, assemblyName.Name + ".dll");
        if (File.Exists(str))
        {
          try
          {
            assembly = Assembly.LoadFrom(str);
            break;
          }
          catch
          {
            break;
          }
        }
      }
      return assembly;
    }

    public static Assembly ResolveAssembly(object sender, ResolveEventArgs args) => VssAssemblyResolver.OnAssemblyResolve(sender, args);

    private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args) => VssAssemblyResolver.ResolveAssembly(args.Name);
  }
}
