// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssAssembliesResolverService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web.Http.Dispatcher;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class VssAssembliesResolverService : 
    IVssAssembliesResolverService,
    IVssFrameworkService
  {
    private static readonly ConcurrentDictionary<string, IAssembliesResolver> s_resolvers = new ConcurrentDictionary<string, IAssembliesResolver>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private const string s_area = "VssAssembliesResolverService";
    private const string s_layer = "Service";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      string path = !string.IsNullOrEmpty(systemRequestContext.ServiceHost.PhysicalDirectory) ? systemRequestContext.ServiceHost.PhysicalDirectory : AppDomain.CurrentDomain.BaseDirectory;
      this.GetResolverForPath(systemRequestContext, path, false);
      if (string.IsNullOrEmpty(systemRequestContext.ServiceHost.PlugInDirectory))
        return;
      this.GetResolverForPath(systemRequestContext, systemRequestContext.ServiceHost.PlugInDirectory, true);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => VssAssembliesResolverService.s_resolvers.Clear();

    public IAssembliesResolver GetResolverForPath(
      IVssRequestContext requestContext,
      string path,
      bool recursive = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      VssAssembliesResolverService.ValidatePath(path);
      requestContext.TraceEnter(63990, nameof (VssAssembliesResolverService), "Service", nameof (GetResolverForPath));
      try
      {
        requestContext.Trace(63991, TraceLevel.Info, nameof (VssAssembliesResolverService), "Service", "Retrieving IAssembliesResolver for path: {0}", (object) path);
        IAssembliesResolver orAdd = VssAssembliesResolverService.s_resolvers.GetOrAdd(path, (Func<string, IAssembliesResolver>) (p => (IAssembliesResolver) new VssAssembliesResolverService.WebApiAssembliesResolver(p, recursive)));
        if (orAdd is VssAssembliesResolverService.WebApiAssembliesResolver assembliesResolver && assembliesResolver.IsRecursive != recursive)
          throw new InvalidOperationException("The path " + path + " was requested twice with different values for recursive.");
        return orAdd;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(63992, nameof (VssAssembliesResolverService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(63994, nameof (VssAssembliesResolverService), "Service", nameof (GetResolverForPath));
      }
    }

    internal static IAssembliesResolver GetResolverForPathRaw(string path, bool recursive = false)
    {
      VssAssembliesResolverService.ValidatePath(path);
      TeamFoundationTracingService.TraceEnterRaw(63995, nameof (VssAssembliesResolverService), "Service", nameof (GetResolverForPathRaw), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        IAssembliesResolver orAdd = VssAssembliesResolverService.s_resolvers.GetOrAdd(path, (Func<string, IAssembliesResolver>) (p => (IAssembliesResolver) new VssAssembliesResolverService.WebApiAssembliesResolver(p, recursive)));
        if (orAdd is VssAssembliesResolverService.WebApiAssembliesResolver assembliesResolver && assembliesResolver.IsRecursive != recursive)
          throw new InvalidOperationException("The path " + path + " was requested twice with different values for recursive.");
        return orAdd;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(63996, nameof (VssAssembliesResolverService), "Service", ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(63997, nameof (VssAssembliesResolverService), "Service", nameof (GetResolverForPathRaw));
      }
    }

    private static void ValidatePath(string path)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      if (!Path.IsPathRooted(path))
        throw new ArgumentException(string.Format("Path must be a fully qualified path: {0}", (object) path), nameof (path));
      if (!Directory.Exists(path))
        throw new ArgumentException(string.Format("The directory specified must exist: {0}", (object) path), nameof (path));
    }

    private static bool PathIsUnc(string path) => new Uri(path).IsUnc;

    private sealed class WebApiAssembliesResolver : IAssembliesResolver
    {
      private readonly string m_basePath;
      private readonly bool m_recursive;
      private Lazy<ICollection<Assembly>> m_allAssemblies;

      internal WebApiAssembliesResolver(string basePath, bool recursive)
      {
        this.m_basePath = basePath;
        this.m_recursive = recursive;
        this.m_allAssemblies = new Lazy<ICollection<Assembly>>(new Func<ICollection<Assembly>>(this.LoadAssembliesFromBasePath), LazyThreadSafetyMode.ExecutionAndPublication);
      }

      public ICollection<Assembly> GetAssemblies() => this.m_allAssemblies.Value;

      internal bool IsRecursive => this.m_recursive;

      private ICollection<Assembly> LoadAssembliesFromBasePath()
      {
        AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);
        try
        {
          if (Directory.Exists(this.m_basePath))
            return this.m_recursive ? VssAssembliesResolverService.WebApiAssembliesResolver.LoadRecursive(this.m_basePath) : VssAssembliesResolverService.WebApiAssembliesResolver.LoadFlat(this.m_basePath);
        }
        finally
        {
          AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);
        }
        return (ICollection<Assembly>) new List<Assembly>();
      }

      private static ICollection<Assembly> LoadFlat(string baseDir)
      {
        List<Assembly> assemblyList = new List<Assembly>();
        foreach (string enumerateFile in Directory.EnumerateFiles(baseDir, "*.dll", SearchOption.TopDirectoryOnly))
        {
          Assembly assembly = (Assembly) null;
          string withoutExtension = Path.GetFileNameWithoutExtension(enumerateFile);
          try
          {
            assembly = Assembly.Load(withoutExtension);
          }
          catch (BadImageFormatException ex)
          {
          }
          catch (FileNotFoundException ex)
          {
          }
          catch (Exception ex)
          {
            if (Marshal.GetHRForException(ex) != -2146234344)
              throw;
          }
          if (assembly != (Assembly) null)
            assemblyList.Add(assembly);
        }
        return (ICollection<Assembly>) assemblyList;
      }

      private static ICollection<Assembly> LoadRecursive(string baseDir)
      {
        List<Assembly> assemblyList = new List<Assembly>();
        Dictionary<string, List<AssemblyName>> loadedAssemblies = new Dictionary<string, List<AssemblyName>>((IEqualityComparer<string>) VssStringComparer.AssemblyName);
        IEnumerable<string> strings = Directory.EnumerateFiles(baseDir, "*.dll", SearchOption.AllDirectories).Where<string>((Func<string, bool>) (f => !f.EndsWith(".resources.dll", StringComparison.OrdinalIgnoreCase)));
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          AssemblyName name = assembly.GetName();
          List<AssemblyName> assemblyNameList;
          if (loadedAssemblies.TryGetValue(name.Name, out assemblyNameList))
            assemblyNameList.Add(name);
          else
            loadedAssemblies.Add(name.Name, new List<AssemblyName>()
            {
              name
            });
        }
        foreach (string codeBase in strings)
        {
          Assembly assembly = (Assembly) null;
          try
          {
            assembly = VssAssembliesResolverService.WebApiAssembliesResolver.SafeLoadAssembly(codeBase, loadedAssemblies);
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceRawAlwaysOn(63914, TraceLevel.Warning, nameof (VssAssembliesResolverService), "Service", "WebApiAssembliesResolver handling '{0}' while loading assemblies for directory '{1}'.  Ignoring file '{2}'. Exception Details: {3}", (object) ex.GetType().Name, (object) baseDir, (object) codeBase, (object) ex.ToReadableStackTrace());
          }
          if (assembly != (Assembly) null)
            assemblyList.Add(assembly);
        }
        return (ICollection<Assembly>) assemblyList;
      }

      private static Assembly SafeLoadAssembly(
        string codeBase,
        Dictionary<string, List<AssemblyName>> loadedAssemblies)
      {
        AssemblyName assemblyRef;
        try
        {
          assemblyRef = AssemblyName.GetAssemblyName(codeBase);
        }
        catch (ArgumentException ex)
        {
          assemblyRef = new AssemblyName();
          assemblyRef.CodeBase = codeBase;
        }
        Assembly assembly = Assembly.Load(assemblyRef);
        AssemblyName name = assembly.GetName();
        List<AssemblyName> assemblyNameList;
        if (loadedAssemblies.TryGetValue(name.Name, out assemblyNameList))
        {
          foreach (AssemblyName assemblyName in assemblyNameList)
          {
            if ((assemblyName.Version != name.Version || !VssStringComparer.FilePath.Equals(assemblyName.CodeBase, name.CodeBase)) && assemblyName.CultureInfo.LCID == name.CultureInfo.LCID)
            {
              TeamFoundationTracingService.TraceRaw(63915, TraceLevel.Error, nameof (VssAssembliesResolverService), "Service", "Attempting to load a different version of '{0}', loaded version '{1}' attempting to load version '{2}.'", (object) name.Name, (object) assemblyName.Version, (object) name.Version);
              EventLog.WriteEntry("TFS Services", FrameworkResources.ServerAssemblyDuplicateLoadDifferentVersion((object) name.Name, (object) assemblyName.Version, (object) name.Version), EventLogEntryType.Error, 63915);
            }
          }
        }
        else
          loadedAssemblies.Add(name.Name, new List<AssemblyName>()
          {
            name
          });
        return assembly;
      }

      private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
      {
        string str = Path.Combine(this.m_basePath, args.Name + ".dll");
        if (File.Exists(str))
          return Assembly.LoadFrom(str);
        if (this.m_recursive)
        {
          IEnumerable<string> source = Directory.EnumerateFiles(this.m_basePath, args.Name + ".dll", SearchOption.AllDirectories);
          int num = source.Count<string>();
          if (num > 0)
          {
            if (num > 1)
              TeamFoundationTracingService.TraceRaw(63999, TraceLevel.Error, nameof (VssAssembliesResolverService), "Service", "Found multiple assemblies under path " + this.m_basePath + " matching name " + args.Name + ". Choosing the first one.");
            return Assembly.LoadFrom(source.First<string>());
          }
        }
        return (Assembly) null;
      }
    }
  }
}
