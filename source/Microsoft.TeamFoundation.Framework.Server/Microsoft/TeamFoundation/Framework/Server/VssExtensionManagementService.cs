// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssExtensionManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Http.Dispatcher;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssExtensionManagementService : IVssExtensionManagementService, IVssFrameworkService
  {
    public static string DefaultPluginPath = string.Empty;
    private static ConcurrentDictionary<string, VssExtensionManagementService.ExportTypeMap> s_typeMaps = new ConcurrentDictionary<string, VssExtensionManagementService.ExportTypeMap>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static ConcurrentDictionary<VssExtensionManagementService.ExtensionCacheKey, IDisposable> m_extensionCache = new ConcurrentDictionary<VssExtensionManagementService.ExtensionCacheKey, IDisposable>();
    private static readonly bool s_enableCheckForImports = Environment.GetEnvironmentVariable("VSSF_CHECK_FOR_IMPORTS") != null;
    private const string s_area = "VssExtensionManagementService";
    private const string s_layer = "Service";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      if (string.IsNullOrEmpty(systemRequestContext.ServiceHost.PlugInDirectory))
        return;
      VssExtensionManagementService.ExportTypeMap exportTypeMap = new VssExtensionManagementService.ExportTypeMap(systemRequestContext.GetService<IVssAssembliesResolverService>().GetResolverForPath(systemRequestContext, systemRequestContext.ServiceHost.PlugInDirectory, true), systemRequestContext.ServiceHost.PlugInDirectory);
      VssExtensionManagementService.s_typeMaps.GetOrAdd(systemRequestContext.ServiceHost.PlugInDirectory, exportTypeMap);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      foreach (IDisposable disposable in (IEnumerable<IDisposable>) VssExtensionManagementService.m_extensionCache.Values)
        disposable.Dispose();
      VssExtensionManagementService.m_extensionCache.Clear();
      VssExtensionManagementService.s_typeMaps.Clear();
    }

    public static IDisposableReadOnlyList<T> GetExtensionsRaw<T>(
      string pluginPath,
      bool throwOnError = false)
    {
      TeamFoundationTracingService.TraceEnterRaw(63920, nameof (VssExtensionManagementService), "Service", "GetExtensionsRaw pluginPath: {0}, Type:{1}", (object) pluginPath, (object) typeof (T).Name);
      try
      {
        return VssExtensionManagementService.CreateExtensions<T>(pluginPath ?? string.Empty, (Func<T, bool>) null, TeamFoundationHostType.Unknown, (string) null, throwOnError);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(63950, nameof (VssExtensionManagementService), "Service", ex);
        throw new ExtensionUtilityException(FrameworkResources.ServerExtensionLoadFailure((object) typeof (T).AssemblyQualifiedName, (object) pluginPath), ex);
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(63921, nameof (VssExtensionManagementService), "Service", "GetExtensionRaw");
      }
    }

    public static Dictionary<string, Type> GetTypeMapRaw<T>(string pluginPath)
    {
      TeamFoundationTracingService.TraceEnterRaw(63922, nameof (VssExtensionManagementService), "Service", "GetTypeMapRaw pluginPath: {0}, Type:{1}", (object) pluginPath, (object) typeof (T).Name);
      try
      {
        VssExtensionManagementService.ExportTypeMap typeMap = VssExtensionManagementService.GetTypeMap(pluginPath ?? string.Empty);
        Dictionary<string, Type> typeMapRaw = new Dictionary<string, Type>();
        IList<VssExtensionManagementService.ExtensionRef> extensionRefList;
        if (typeMap.GetTypeMap().TryGetValue(typeof (T), out extensionRefList))
        {
          foreach (VssExtensionManagementService.ExtensionRef extensionRef in (IEnumerable<VssExtensionManagementService.ExtensionRef>) extensionRefList)
            typeMapRaw.Add(extensionRef.ExtensionType.FullName, extensionRef.ExtensionType);
        }
        return typeMapRaw;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(63951, nameof (VssExtensionManagementService), "Service", ex);
        throw new ExtensionUtilityException(FrameworkResources.ServerExtensionLoadFailure((object) typeof (T).AssemblyQualifiedName, (object) pluginPath), ex);
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(63923, nameof (VssExtensionManagementService), "Service", nameof (GetTypeMapRaw));
      }
    }

    internal static void ValidateExtensions(
      string basePath,
      string pluginPath,
      string[] ignoreStrongNames,
      out IList<Exception> assemblyErrors,
      out IList<Exception> typeErrors)
    {
      IAssembliesResolver assembliesResolver = (IAssembliesResolver) null;
      assemblyErrors = (IList<Exception>) null;
      typeErrors = (IList<Exception>) null;
      List<Exception> exceptionList1 = new List<Exception>();
      List<Exception> exceptionList2 = new List<Exception>();
      if (ignoreStrongNames == null)
        ignoreStrongNames = Array.Empty<string>();
      string path = pluginPath;
      if (!string.IsNullOrEmpty(pluginPath))
      {
        if (!string.IsNullOrEmpty(basePath))
        {
          if (!Path.IsPathRooted(basePath))
            throw new ArgumentException("Base path must be a non-relative path: " + basePath, nameof (basePath));
          assembliesResolver = Directory.Exists(basePath) ? VssAssembliesResolverService.GetResolverForPathRaw(basePath) : throw new ArgumentException("Base path does not exist: " + basePath, nameof (basePath));
          if (!Path.IsPathRooted(pluginPath))
            path = Path.Combine(basePath, pluginPath);
        }
        if (!Path.IsPathRooted(path))
          throw new ArgumentException("If base path is not provided, pluginPath must be non-relative: " + pluginPath, nameof (pluginPath));
        if (!Directory.Exists(path))
          throw new ArgumentException("Plugin path does not exist: " + path + ".", nameof (pluginPath));
      }
      IList<Assembly> assemblyList = (IList<Assembly>) new List<Assembly>();
      IList<string> stringList = (IList<string>) new List<string>();
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      AssemblyName name = executingAssembly.GetName();
      if (!((IEnumerable<string>) ignoreStrongNames).Contains<string>(Convert.ToBase64String(name.GetPublicKeyToken()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        assemblyList.Add(executingAssembly);
      if (!string.IsNullOrEmpty(path))
      {
        foreach (string enumerateFile in Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories))
        {
          try
          {
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(enumerateFile);
            if (!((IEnumerable<string>) ignoreStrongNames).Contains<string>(Convert.ToBase64String(assemblyName.GetPublicKeyToken()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
              stringList.Add(enumerateFile);
          }
          catch (BadImageFormatException ex)
          {
          }
        }
      }
      if (assemblyList.Count > 0 || stringList.Count > 0)
      {
        if (stringList.Count > 0 && assembliesResolver != null)
          assembliesResolver.GetAssemblies();
        object typeMapLock = new object();
        IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>> typeMap = (IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>) new Dictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>();
        foreach (string assemblyFile in (IEnumerable<string>) stringList)
        {
          Assembly assembly = Assembly.LoadFrom(assemblyFile);
          assemblyList.Add(assembly);
        }
        ConcurrentDictionary<Type, IList<Type>> exportCache = new ConcurrentDictionary<Type, IList<Type>>();
        foreach (Assembly a in (IEnumerable<Assembly>) assemblyList)
        {
          IList<Exception> collection = VssExtensionManagementService.ExportTypeMap.FillInExtensionRefsFromAssembly(a, typeMapLock, typeMap, (IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>) null, exportCache, true);
          exceptionList1.AddRange((IEnumerable<Exception>) collection);
        }
        foreach (IEnumerable<VssExtensionManagementService.ExtensionRef> extensionRefs in (IEnumerable<IList<VssExtensionManagementService.ExtensionRef>>) typeMap.Values)
        {
          foreach (VssExtensionManagementService.ExtensionRef r in extensionRefs)
          {
            try
            {
              object obj = VssExtensionManagementService.ExportTypeMap.InstantiateExtension(r, true, path);
              if (obj is IDisposable)
                ((IDisposable) obj).Dispose();
            }
            catch (Exception ex)
            {
              exceptionList2.Add(ex);
            }
          }
        }
      }
      assemblyErrors = (IList<Exception>) exceptionList1;
      typeErrors = (IList<Exception>) exceptionList2;
    }

    public T GetExtension<T>(
      IVssRequestContext requestContext,
      ExtensionLifetime lifetime = ExtensionLifetime.Instance,
      string strategy = null,
      bool throwOnError = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(63942, nameof (VssExtensionManagementService), "Service", nameof (GetExtension));
      string str = requestContext.ServiceHost.PlugInDirectory ?? string.Empty;
      try
      {
        if (lifetime != ExtensionLifetime.Service)
          return VssExtensionManagementService.CreateExtension<T>(str, (Func<T, bool>) null, requestContext.ServiceHost.HostType, strategy, throwOnError);
        IDisposableReadOnlyList<T> serviceLifetimeImpl = this.GetExtensionsServiceLifetimeImpl<T>(str, requestContext.ServiceHost.HostType, strategy, throwOnError);
        if (serviceLifetimeImpl.Count > 1)
        {
          StringBuilder stringBuilder = new StringBuilder("Called GetExtension, but more than one extension of the type is available.\r\n");
          stringBuilder.Append("Requested Type: " + typeof (T).FullName + "\r\n");
          stringBuilder.Append(string.Format("Found {0} extensions:\r\n", (object) serviceLifetimeImpl.Count));
          foreach (T obj in (IEnumerable<T>) serviceLifetimeImpl)
            stringBuilder.Append("\t" + obj.GetType().FullName + "\r\n");
          requestContext.Trace(63960, TraceLevel.Error, nameof (VssExtensionManagementService), "Service", stringBuilder.ToString());
        }
        return throwOnError ? serviceLifetimeImpl.SingleOrDefault<T>() : serviceLifetimeImpl.FirstOrDefault<T>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(63952, nameof (VssExtensionManagementService), "Service", ex);
        throw new ExtensionUtilityException(FrameworkResources.ServerExtensionLoadFailure((object) typeof (T).AssemblyQualifiedName, (object) str), ex);
      }
      finally
      {
        requestContext.TraceLeave(63943, nameof (VssExtensionManagementService), "Service", nameof (GetExtension));
      }
    }

    public T GetExtension<T>(
      IVssRequestContext requestContext,
      Func<T, bool> filter,
      bool throwOnError = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Func<T, bool>>(filter, nameof (filter));
      requestContext.TraceEnter(63955, nameof (VssExtensionManagementService), "Service", nameof (GetExtension));
      string pluginDirectory = requestContext.ServiceHost.PlugInDirectory ?? string.Empty;
      try
      {
        return VssExtensionManagementService.CreateExtension<T>(pluginDirectory, filter, requestContext.ServiceHost.HostType, (string) null, throwOnError);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(63952, nameof (VssExtensionManagementService), "Service", ex);
        throw new ExtensionUtilityException(FrameworkResources.ServerExtensionLoadFailure((object) typeof (T).AssemblyQualifiedName, (object) pluginDirectory), ex);
      }
      finally
      {
        requestContext.TraceLeave(63943, nameof (VssExtensionManagementService), "Service", nameof (GetExtension));
      }
    }

    public virtual IDisposableReadOnlyList<T> GetExtensions<T>(
      IVssRequestContext requestContext,
      ExtensionLifetime lifetime = ExtensionLifetime.Instance,
      string strategy = null,
      bool throwOnError = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(63944, nameof (VssExtensionManagementService), "Service", nameof (GetExtensions));
      string str = requestContext.ServiceHost.PlugInDirectory ?? string.Empty;
      try
      {
        return lifetime == ExtensionLifetime.Service ? this.GetExtensionsServiceLifetimeImpl<T>(str, requestContext.ServiceHost.HostType, strategy, throwOnError) : VssExtensionManagementService.CreateExtensions<T>(str, (Func<T, bool>) null, requestContext.ServiceHost.HostType, strategy, throwOnError);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(63952, nameof (VssExtensionManagementService), "Service", ex);
        throw new ExtensionUtilityException(FrameworkResources.ServerExtensionLoadFailure((object) typeof (T).AssemblyQualifiedName, (object) str), ex);
      }
      finally
      {
        requestContext.TraceLeave(63944, nameof (VssExtensionManagementService), "Service", nameof (GetExtensions));
      }
    }

    public IDisposableReadOnlyList<T> GetExtensions<T>(
      IVssRequestContext requestContext,
      Func<T, bool> filter,
      bool throwOnError = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Func<T, bool>>(filter, nameof (filter));
      requestContext.TraceEnter(63944, nameof (VssExtensionManagementService), "Service", nameof (GetExtensions));
      string pluginDirectory = requestContext.ServiceHost.PlugInDirectory ?? string.Empty;
      try
      {
        return VssExtensionManagementService.CreateExtensions<T>(pluginDirectory, filter, requestContext.ServiceHost.HostType, (string) null, throwOnError);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(63952, nameof (VssExtensionManagementService), "Service", ex);
        throw new ExtensionUtilityException(FrameworkResources.ServerExtensionLoadFailure((object) typeof (T).AssemblyQualifiedName, (object) pluginDirectory), ex);
      }
      finally
      {
        requestContext.TraceLeave(63944, nameof (VssExtensionManagementService), "Service", nameof (GetExtensions));
      }
    }

    private static VssExtensionManagementService.ExportTypeMap GetTypeMap(string pluginPath)
    {
      IAssembliesResolver resolver = (IAssembliesResolver) null;
      if (!string.IsNullOrEmpty(pluginPath))
        resolver = VssAssembliesResolverService.GetResolverForPathRaw(pluginPath, true);
      VssExtensionManagementService.ExportTypeMap exportTypeMap = new VssExtensionManagementService.ExportTypeMap(resolver, pluginPath);
      return VssExtensionManagementService.s_typeMaps.GetOrAdd(pluginPath, exportTypeMap);
    }

    private static IDisposableReadOnlyList<T> CreateExtensions<T>(
      string pluginDirectory,
      Func<T, bool> filter,
      TeamFoundationHostType context,
      string strategy,
      bool throwOnError)
    {
      return (IDisposableReadOnlyList<T>) VssExtensionManagementService.GetTypeMap(pluginDirectory).CreateExtensions<T>(filter, context, strategy, throwOnError);
    }

    private static T CreateExtension<T>(
      string pluginDirectory,
      Func<T, bool> filter,
      TeamFoundationHostType context,
      string strategy,
      bool throwOnError)
    {
      return VssExtensionManagementService.GetTypeMap(pluginDirectory).CreateExtension<T>(filter, context, strategy, throwOnError);
    }

    private IDisposableReadOnlyList<T> GetExtensionsServiceLifetimeImpl<T>(
      string pluginDir,
      TeamFoundationHostType context,
      string strategy,
      bool throwOnError)
    {
      VssExtensionManagementService.ExtensionCacheKey key = new VssExtensionManagementService.ExtensionCacheKey()
      {
        Type = typeof (T),
        Context = context,
        Strategy = strategy
      };
      IDisposable extensions;
      if (!VssExtensionManagementService.m_extensionCache.TryGetValue(key, out extensions))
      {
        extensions = (IDisposable) VssExtensionManagementService.CreateExtensions<T>(pluginDir, (Func<T, bool>) null, context, strategy, throwOnError);
        if (!VssExtensionManagementService.m_extensionCache.TryAdd(key, extensions))
        {
          extensions.Dispose();
          extensions = VssExtensionManagementService.m_extensionCache[key];
        }
      }
      return (IDisposableReadOnlyList<T>) extensions;
    }

    internal struct ExtensionCacheKey
    {
      internal Type Type { get; set; }

      internal TeamFoundationHostType Context { get; set; }

      internal string Strategy { get; set; }

      public override int GetHashCode()
      {
        int hashCode = this.Type.GetHashCode() ^ this.Context.GetHashCode();
        if (this.Strategy != null)
          hashCode ^= this.Strategy.GetHashCode();
        return hashCode;
      }

      public override bool Equals(object obj)
      {
        if (obj == null || !(obj is VssExtensionManagementService.ExtensionCacheKey extensionCacheKey) || !this.Type.Equals(extensionCacheKey.Type) || !this.Context.Equals((object) extensionCacheKey.Context))
          return false;
        return this.Strategy == null && extensionCacheKey.Strategy == null || this.Strategy.Equals(extensionCacheKey.Strategy, StringComparison.Ordinal);
      }

      public static bool operator ==(
        VssExtensionManagementService.ExtensionCacheKey a,
        VssExtensionManagementService.ExtensionCacheKey b)
      {
        return b.Equals((object) a);
      }

      public static bool operator !=(
        VssExtensionManagementService.ExtensionCacheKey a,
        VssExtensionManagementService.ExtensionCacheKey b)
      {
        return !a.Equals((object) b);
      }
    }

    [DebuggerDisplay("ExtensionType={ExtensionType}  Priority={Priority}  Strategy={Strategy}")]
    private class ExtensionRef
    {
      internal const int DefaultPriority = 65535;

      internal ExtensionRef(Type extensionType, ConstructorInfo extensionConstructor)
      {
        this.ExtensionType = extensionType;
        this.Constructor = extensionConstructor;
        int num = (int) ushort.MaxValue;
        string str = string.Empty;
        TeamFoundationHostType foundationHostType = TeamFoundationHostType.Unknown;
        ExtensionContextAttribute customAttribute1 = extensionType.GetCustomAttribute<ExtensionContextAttribute>();
        if (customAttribute1 != null && customAttribute1.Context != TeamFoundationHostType.Parent)
          foundationHostType = customAttribute1.Context;
        ExtensionPriorityAttribute customAttribute2 = extensionType.GetCustomAttribute<ExtensionPriorityAttribute>();
        if (customAttribute2 != null)
          num = customAttribute2.Priority;
        ExtensionStrategyAttribute customAttribute3 = extensionType.GetCustomAttribute<ExtensionStrategyAttribute>();
        if (customAttribute3 != null)
          str = customAttribute3.Strategy;
        this.Context = foundationHostType;
        this.Priority = num;
        this.Strategy = str;
      }

      internal Type ExtensionType { get; private set; }

      internal ConstructorInfo Constructor { get; private set; }

      internal int Priority { get; private set; }

      internal TeamFoundationHostType Context { get; private set; }

      internal string Strategy { get; private set; }

      public bool MatchContext(TeamFoundationHostType context)
      {
        if (this.Context == TeamFoundationHostType.Unknown)
          return true;
        if (context == TeamFoundationHostType.Unknown || context == TeamFoundationHostType.Parent)
          return false;
        bool flag1 = (this.Context & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment;
        bool flag2 = (this.Context & TeamFoundationHostType.Application) == TeamFoundationHostType.Application;
        bool flag3 = (this.Context & TeamFoundationHostType.ProjectCollection) == TeamFoundationHostType.ProjectCollection;
        if ((context & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
          return flag1;
        return (context & TeamFoundationHostType.Application) == TeamFoundationHostType.Application ? flag2 : flag3;
      }

      public bool MatchStrategy(string strategy) => strategy == null || strategy.Equals(this.Strategy, StringComparison.Ordinal);
    }

    private class ExportTypeMap
    {
      private IAssembliesResolver m_resolver;
      private Lazy<IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>> m_typeMap;
      private readonly bool m_loadThisAssembly;
      private string m_path;

      internal ExportTypeMap(IAssembliesResolver resolver, string path)
      {
        if (resolver != null)
        {
          this.m_resolver = resolver;
          this.m_loadThisAssembly = !File.Exists(Path.Combine(path, Path.GetFileName(Assembly.GetExecutingAssembly().Location)));
          this.m_path = path;
        }
        else
        {
          this.m_resolver = (IAssembliesResolver) null;
          this.m_path = string.Empty;
          this.m_loadThisAssembly = true;
        }
        this.m_typeMap = new Lazy<IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>>(new Func<IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>>(this.LoadTypeMap), LazyThreadSafetyMode.ExecutionAndPublication);
      }

      internal IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>> GetTypeMap() => this.m_typeMap.Value;

      private IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>> LoadTypeMap()
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        Dictionary<Type, IList<VssExtensionManagementService.ExtensionRef>> typeMap = new Dictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>();
        Dictionary<Type, IList<VssExtensionManagementService.ExtensionRef>> priorityTypeMap = new Dictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>();
        object typeMapLock = new object();
        ConcurrentDictionary<Type, IList<Type>> exportCache = new ConcurrentDictionary<Type, IList<Type>>();
        if (this.m_loadThisAssembly)
          VssExtensionManagementService.ExportTypeMap.FillInExtensionRefsFromAssembly(Assembly.GetExecutingAssembly(), typeMapLock, (IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>) typeMap, (IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>) priorityTypeMap, exportCache);
        if (this.m_resolver != null)
        {
          int degreeOfParallelism = Debugger.IsAttached ? 1 : 4;
          this.m_resolver.GetAssemblies().AsParallel<Assembly>().WithDegreeOfParallelism<Assembly>(degreeOfParallelism).ForAll<Assembly>((Action<Assembly>) (a => VssExtensionManagementService.ExportTypeMap.FillInExtensionRefsFromAssembly(a, typeMapLock, (IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>) typeMap, (IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>) priorityTypeMap, exportCache)));
        }
        VssExtensionManagementService.ExportTypeMap.MergePriorityExtensions((IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>) typeMap, (IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>) priorityTypeMap);
        TeamFoundationTracingService.TraceRaw(63940, TraceLevel.Info, nameof (VssExtensionManagementService), "Service", "LoadTypeMap took {0} ms. Path: {1}", (object) stopwatch.ElapsedMilliseconds, (object) this.m_path);
        HashSet<Assembly> assemblySet = new HashSet<Assembly>();
        foreach (VssExtensionManagementService.ExtensionRef extensionRef in typeMap.Values.SelectMany<IList<VssExtensionManagementService.ExtensionRef>, VssExtensionManagementService.ExtensionRef>((Func<IList<VssExtensionManagementService.ExtensionRef>, IEnumerable<VssExtensionManagementService.ExtensionRef>>) (v => (IEnumerable<VssExtensionManagementService.ExtensionRef>) v)))
        {
          Assembly assembly = extensionRef.ExtensionType.Assembly;
          if (!assemblySet.Contains(assembly))
          {
            TeamFoundationTracingService.TraceRawAlwaysOn(667360888, TraceLevel.Info, nameof (VssExtensionManagementService), "Service", JsonConvert.SerializeObject((object) new VssExtensionManagementService.ExportTypeMap.ExtensionAssemblyEntry()
            {
              UniqueProcessId = ProcessUtility.UniqueProcessId,
              Description = "TypeMap Extension Assembly",
              Assembly = assembly.GetName().Name,
              PluginPath = this.m_path,
              AssemblyPath = assembly.Location,
              AssemblyFullName = assembly.FullName
            }));
            assemblySet.Add(extensionRef.ExtensionType.Assembly);
          }
        }
        return (IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>>) typeMap;
      }

      internal DisposableCollection<T> CreateExtensions<T>(
        Func<T, bool> filter,
        TeamFoundationHostType context,
        string strategy,
        bool throwOnError)
      {
        IList<VssExtensionManagementService.ExtensionRef> extensionRefList;
        if (!this.m_typeMap.Value.TryGetValue(typeof (T), out extensionRefList))
          return new DisposableCollection<T>((IReadOnlyList<T>) new List<T>());
        List<T> elements = new List<T>();
        foreach (VssExtensionManagementService.ExtensionRef r in (IEnumerable<VssExtensionManagementService.ExtensionRef>) extensionRefList)
        {
          if (r.MatchContext(context) && r.MatchStrategy(strategy))
          {
            object obj = VssExtensionManagementService.ExportTypeMap.InstantiateExtension(r, throwOnError, this.m_path);
            if (obj != null && obj is T)
            {
              if (obj is T && (filter == null || filter((T) obj)))
                elements.Add((T) obj);
              else if (obj is IDisposable disposable)
                disposable.Dispose();
            }
          }
        }
        return new DisposableCollection<T>((IReadOnlyList<T>) elements);
      }

      internal T CreateExtension<T>(
        Func<T, bool> filter,
        TeamFoundationHostType context,
        string strategy,
        bool throwOnError)
      {
        T extension = default (T);
        IList<VssExtensionManagementService.ExtensionRef> source;
        if (this.m_typeMap.Value.TryGetValue(typeof (T), out source))
        {
          List<VssExtensionManagementService.ExtensionRef> list = source.Where<VssExtensionManagementService.ExtensionRef>((Func<VssExtensionManagementService.ExtensionRef, bool>) (c => c.MatchContext(context) && c.MatchStrategy(strategy))).ToList<VssExtensionManagementService.ExtensionRef>();
          if (filter == null)
          {
            if (list.Count > 1)
            {
              StringBuilder stringBuilder = new StringBuilder("Called GetExtension, but more than one extension of the type is available.\r\n");
              stringBuilder.Append("Requested Type: " + typeof (T).FullName + "\r\n");
              stringBuilder.Append(string.Format("Found {0} extensions:\r\n", (object) list.Count));
              foreach (VssExtensionManagementService.ExtensionRef extensionRef in list)
                stringBuilder.Append("\t" + extensionRef.ExtensionType.FullName + "\r\n");
              TeamFoundationTracingService.TraceRaw(63960, TraceLevel.Error, nameof (VssExtensionManagementService), "Service", stringBuilder.ToString());
            }
            VssExtensionManagementService.ExtensionRef r = throwOnError ? list.SingleOrDefault<VssExtensionManagementService.ExtensionRef>() : list.FirstOrDefault<VssExtensionManagementService.ExtensionRef>();
            if (r != null)
              extension = (T) VssExtensionManagementService.ExportTypeMap.InstantiateExtension(r, throwOnError, this.m_path);
          }
          else
          {
            foreach (VssExtensionManagementService.ExtensionRef r in list)
            {
              T obj = (T) VssExtensionManagementService.ExportTypeMap.InstantiateExtension(r, throwOnError, this.m_path);
              if (filter(obj))
              {
                if ((object) extension != null & throwOnError)
                {
                  if (extension is IDisposable disposable)
                    disposable.Dispose();
                  throw new InvalidOperationException("Found Multiple Extensions which match the filter function.");
                }
                extension = obj;
                if (!throwOnError)
                  break;
              }
              else if (obj is IDisposable disposable1)
                disposable1.Dispose();
            }
          }
        }
        return extension;
      }

      internal static IList<Exception> FillInExtensionRefsFromAssembly(
        Assembly a,
        object typeMapLock,
        IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>> typeMap,
        IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>> priorityTypeMap,
        ConcurrentDictionary<Type, IList<Type>> exportCache,
        bool reportErrors = false)
      {
        IList<Exception> exceptionList = (IList<Exception>) new List<Exception>();
        try
        {
          Type[] typeArray = (Type[]) null;
          try
          {
            typeArray = a.GetTypes();
          }
          catch (ReflectionTypeLoadException ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(63926, nameof (VssExtensionManagementService), "Service", (Exception) ex);
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(FrameworkResources.ServerExtensionLoadFailure((object) a.GetName().FullName, (object) Path.GetDirectoryName(a.Location)));
            VssExtensionManagementService.ExportTypeMap.AppendLoaderExceptions(ex, builder);
            string message = string.Format("ReflectionTypeLoadException loading assembly: {0}. Not all plugins may be loaded. Exception Details: {1}. Loader Exceptions: {2}", (object) a.GetName().FullName, (object) ex, (object) builder);
            TeamFoundationTracingService.TraceRaw(63927, TraceLevel.Error, nameof (VssExtensionManagementService), "Service", VssExtensionManagementService.ExportTypeMap.GetMissingAssembliesFromLoaderExceptions(ex.LoaderExceptions).ToArray<string>(), message);
            typeArray = ex.Types;
            if (reportErrors)
              exceptionList.Add((Exception) new ExtensionUtilityAssemblyTypeLoadException(message, a.GetName(), (Exception) ex));
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(63928, nameof (VssExtensionManagementService), "Service", ex);
            if (reportErrors)
              exceptionList.Add((Exception) new ExtensionUtilityAssemblyTypeLoadException(a.GetName(), ex));
          }
          if (typeArray != null)
          {
            List<Tuple<VssExtensionManagementService.ExtensionRef, IList<Type>>> tupleList = new List<Tuple<VssExtensionManagementService.ExtensionRef, IList<Type>>>();
            foreach (Type t in typeArray)
            {
              if (t != (Type) null && t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters)
              {
                IList<Type> typeList = VssExtensionManagementService.ExportTypeMap.CheckExport(t, exportCache);
                if (typeList != null && typeList.Count > 0)
                {
                  VssExtensionManagementService.ExtensionRef extensionRef = VssExtensionManagementService.ExportTypeMap.ExtractRef(t);
                  if (extensionRef != null)
                    tupleList.Add(new Tuple<VssExtensionManagementService.ExtensionRef, IList<Type>>(extensionRef, typeList));
                }
              }
            }
            lock (typeMapLock)
            {
              foreach (Tuple<VssExtensionManagementService.ExtensionRef, IList<Type>> tuple in tupleList)
              {
                VssExtensionManagementService.ExtensionRef extensionRef = tuple.Item1;
                foreach (Type key in (IEnumerable<Type>) tuple.Item2)
                {
                  IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>> dictionary = priorityTypeMap == null || extensionRef.Priority == (int) ushort.MaxValue ? typeMap : priorityTypeMap;
                  IList<VssExtensionManagementService.ExtensionRef> extensionRefList;
                  if (!dictionary.TryGetValue(key, out extensionRefList))
                  {
                    extensionRefList = (IList<VssExtensionManagementService.ExtensionRef>) new List<VssExtensionManagementService.ExtensionRef>();
                    dictionary.Add(key, extensionRefList);
                  }
                  extensionRefList.Add(extensionRef);
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(63929, nameof (VssExtensionManagementService), "Service", ex);
          if (reportErrors)
            exceptionList.Add((Exception) new ExtensionUtilityAssemblyTypeLoadException(a.GetName(), ex));
        }
        return exceptionList;
      }

      private static void MergePriorityExtensions(
        IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>> typeMap,
        IDictionary<Type, IList<VssExtensionManagementService.ExtensionRef>> priorityTypeMap)
      {
        foreach (KeyValuePair<Type, IList<VssExtensionManagementService.ExtensionRef>> priorityType in (IEnumerable<KeyValuePair<Type, IList<VssExtensionManagementService.ExtensionRef>>>) priorityTypeMap)
        {
          IList<VssExtensionManagementService.ExtensionRef> extensionRefList;
          if (!typeMap.TryGetValue(priorityType.Key, out extensionRefList))
          {
            extensionRefList = (IList<VssExtensionManagementService.ExtensionRef>) new List<VssExtensionManagementService.ExtensionRef>();
            typeMap.Add(priorityType.Key, extensionRefList);
          }
          foreach (VssExtensionManagementService.ExtensionRef extensionRef in (IEnumerable<VssExtensionManagementService.ExtensionRef>) priorityType.Value)
          {
            int index;
            if (extensionRef.Priority < (int) ushort.MaxValue)
            {
              index = 0;
              while (index < extensionRefList.Count && extensionRef.Priority >= extensionRefList[index].Priority)
                ++index;
            }
            else
            {
              index = extensionRefList.Count;
              while (index > 0 && extensionRef.Priority < extensionRefList[index - 1].Priority)
                --index;
            }
            extensionRefList.Insert(index, extensionRef);
          }
        }
      }

      private static void AppendLoaderExceptions(
        ReflectionTypeLoadException rtle,
        StringBuilder builder)
      {
        builder.AppendLine("Loader Exception(s):");
        foreach (Exception loaderException in rtle.LoaderExceptions)
        {
          builder.AppendLine("Loader Exception Type: " + loaderException.GetType().FullName);
          builder.AppendLine("Message: " + loaderException.Message);
          if (loaderException is FileNotFoundException notFoundException && !string.IsNullOrEmpty(notFoundException.FusionLog))
          {
            builder.AppendLine("Fusion Log Information:");
            builder.AppendLine(notFoundException.FusionLog);
          }
          if (loaderException is TypeLoadException typeLoadException)
            builder.AppendLine("Failed to load type: " + typeLoadException.TypeName);
          if (loaderException is ReflectionTypeLoadException rtle1)
            VssExtensionManagementService.ExportTypeMap.AppendLoaderExceptions(rtle1, builder);
          builder.AppendLine();
        }
      }

      private static HashSet<string> GetMissingAssembliesFromLoaderExceptions(
        Exception[] loaderExceptions)
      {
        HashSet<string> collection = new HashSet<string>();
        foreach (Exception loaderException in loaderExceptions)
        {
          switch (loaderException)
          {
            case FileNotFoundException notFoundException:
              collection.Add(notFoundException.FileName);
              break;
            case ReflectionTypeLoadException typeLoadException:
              collection.AddRange<string, HashSet<string>>((IEnumerable<string>) VssExtensionManagementService.ExportTypeMap.GetMissingAssembliesFromLoaderExceptions(typeLoadException.LoaderExceptions));
              break;
          }
        }
        return collection;
      }

      private static IList<Type> CheckExport(
        Type t,
        ConcurrentDictionary<Type, IList<Type>> exportCache)
      {
        return VssExtensionManagementService.ExportTypeMap.FindExports(t, exportCache);
      }

      private static VssExtensionManagementService.ExtensionRef ExtractRef(Type t)
      {
        ConstructorInfo extensionConstructor = (ConstructorInfo) null;
        try
        {
          ConstructorInfo[] constructors = t.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
          ConstructorInfo constructorInfo = (ConstructorInfo) null;
          foreach (ConstructorInfo element in constructors)
          {
            if (element.GetCustomAttribute<ImportingConstructorAttribute>() != null)
            {
              TeamFoundationTracingService.TraceRaw(63941, TraceLevel.Error, nameof (VssExtensionManagementService), "Service", "Found a type with the ImportingConstructorAttribute, this is no longer supported: {0}", (object) t.FullName);
              break;
            }
            ParameterInfo[] parameters = element.GetParameters();
            if (parameters.Length == 0)
              constructorInfo = element;
            else if (((IEnumerable<ParameterInfo>) parameters).All<ParameterInfo>((Func<ParameterInfo, bool>) (p => p.IsOptional)))
              constructorInfo = element;
          }
          if (extensionConstructor == (ConstructorInfo) null && constructorInfo != (ConstructorInfo) null)
            extensionConstructor = constructorInfo;
          if (extensionConstructor != (ConstructorInfo) null)
            return new VssExtensionManagementService.ExtensionRef(t, extensionConstructor);
        }
        catch (FileNotFoundException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(63930, nameof (VssExtensionManagementService), "Service", (Exception) ex);
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendFormat("Error occurred inspecting type {0}. A dependent assembly could not be loaded: {1}", (object) t.FullName, (object) ex.FileName);
          stringBuilder.AppendLine(ex.FusionLog);
          TeamFoundationTracingService.TraceRaw(63931, TraceLevel.Error, nameof (VssExtensionManagementService), "Service", stringBuilder.ToString());
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(63932, nameof (VssExtensionManagementService), "Service", ex);
        }
        return (VssExtensionManagementService.ExtensionRef) null;
      }

      private static IList<Type> FindExports(
        Type t,
        ConcurrentDictionary<Type, IList<Type>> exportCache)
      {
        IList<Type> exports1;
        if (exportCache.TryGetValue(t, out exports1))
          return exports1;
        HashSet<Type> source = new HashSet<Type>();
        try
        {
          if (t.IsInterface || t.IsClass && t.IsAbstract)
          {
            if (t.GetCustomAttribute<InheritedExportAttribute>() != null)
              source.Add(t);
          }
          else if (t.IsClass)
          {
            ExportAttribute customAttribute = t.GetCustomAttribute<ExportAttribute>();
            if (customAttribute != null)
            {
              if (customAttribute.ContractType != (Type) null)
                source.Add(customAttribute.ContractType);
              else
                source.Add(t);
            }
          }
          foreach (Type t1 in t.GetInterfaces())
          {
            IList<Type> exports2 = VssExtensionManagementService.ExportTypeMap.FindExports(t1, exportCache);
            if (exports2.Count > 0)
            {
              foreach (Type type in (IEnumerable<Type>) exports2)
              {
                if (!source.Contains(type))
                  source.Add(type);
              }
            }
          }
          Type baseType = t.BaseType;
          if (baseType != (Type) null)
          {
            if (baseType != typeof (object))
            {
              IList<Type> exports3 = VssExtensionManagementService.ExportTypeMap.FindExports(baseType, exportCache);
              if (exports3.Count > 0)
              {
                foreach (Type type in (IEnumerable<Type>) exports3)
                {
                  if (!source.Contains(type))
                    source.Add(type);
                }
              }
            }
          }
        }
        catch (FileNotFoundException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(63933, nameof (VssExtensionManagementService), "Service", (Exception) ex);
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendFormat("Error occurred inspecting type {0}. A dependent assembly could not be loaded: {1}", (object) t.FullName, (object) ex.FileName);
          stringBuilder.AppendLine(ex.FusionLog);
          TeamFoundationTracingService.TraceRaw(63934, TraceLevel.Error, nameof (VssExtensionManagementService), "Service", stringBuilder.ToString());
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(63935, nameof (VssExtensionManagementService), "Service", ex);
        }
        List<Type> list = source.ToList<Type>();
        exportCache.TryAdd(t, (IList<Type>) list);
        return (IList<Type>) list;
      }

      [Conditional("DEBUG")]
      private static void CheckForImports(Type t)
      {
        if (!VssExtensionManagementService.s_enableCheckForImports)
          return;
        MemberInfo[] members = t.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        StringBuilder stringBuilder = new StringBuilder();
        foreach (MemberInfo element in members)
        {
          IList<CustomAttributeData> customAttributesData = element.GetCustomAttributesData();
          if (customAttributesData.Count > 0 && customAttributesData.Any<CustomAttributeData>((Func<CustomAttributeData, bool>) (ca => ca.AttributeType == typeof (ImportAttribute))))
          {
            ImportAttribute customAttribute = element.GetCustomAttribute<ImportAttribute>();
            if (customAttribute != null)
            {
              stringBuilder.AppendFormat("Found an import on member {0} of {1} {2}. This is no longer supported.\n", (object) element.Name, t.IsInterface ? (object) "interface" : (object) "class", (object) t.FullName);
              if (customAttribute.ContractType != (Type) null)
                stringBuilder.AppendFormat("\tThe imported type (from the ImportAttribute) is: {0}\n", (object) customAttribute.ContractType.FullName);
              else if (element.MemberType == MemberTypes.Property)
                stringBuilder.AppendFormat("\tThe imported type (from the Property type) is: {0}\n", (object) ((PropertyInfo) element).PropertyType.FullName);
              else if (element.MemberType == MemberTypes.Field)
                stringBuilder.AppendFormat("\tThe imported type (from the Field type) is: {0}\n", (object) ((FieldInfo) element).FieldType.FullName);
              else
                stringBuilder.AppendFormat("\tEncountered an ImportAttribute on an unexpected member type: {0}\n", (object) element.MemberType.ToString());
            }
          }
        }
        TeamFoundationTracingService.TraceRaw(63936, TraceLevel.Info, nameof (VssExtensionManagementService), "Service", stringBuilder.ToString());
      }

      internal static object InstantiateExtension(
        VssExtensionManagementService.ExtensionRef r,
        bool throwOnError,
        string path)
      {
        try
        {
          try
          {
            ConstructorInfo constructor = r.Constructor;
            ParameterInfo[] parameters = constructor.GetParameters();
            if (parameters != null && parameters.Length != 0 && ((IEnumerable<ParameterInfo>) parameters).Any<ParameterInfo>((Func<ParameterInfo, bool>) (p => !p.IsOptional)))
              throw new NotImplementedException();
            return constructor.Invoke((object[]) null);
          }
          catch (TargetInvocationException ex)
          {
            if (ex.InnerException != null)
              throw ex.InnerException;
            throw;
          }
        }
        catch (ReflectionTypeLoadException ex)
        {
          StringBuilder builder = new StringBuilder();
          builder.AppendLine(FrameworkResources.ServerExtensionLoadFailure((object) r.ExtensionType.AssemblyQualifiedName, (object) path));
          VssExtensionManagementService.ExportTypeMap.AppendLoaderExceptions(ex, builder);
          string message = string.Format("ReflectionTypeLoadException loading plugin type: {0}. Not all plugins may be loaded. Exception Details: {1}. Loader Exceptions: {2}", (object) r.ExtensionType.Name, (object) ex, (object) builder);
          TeamFoundationTracingService.TraceRaw(63924, TraceLevel.Error, nameof (VssExtensionManagementService), "Service", message);
          if (throwOnError)
            throw new ExtensionUtilityTypeCreateException(message, r.ExtensionType, (Exception) ex);
        }
        catch (Exception ex)
        {
          string message = "Exception loading plugin type: " + r.ExtensionType.Name + ". Not all plugins may be loaded. Exception Details: " + ex.ToReadableStackTrace();
          TeamFoundationTracingService.TraceRaw(63925, TraceLevel.Error, nameof (VssExtensionManagementService), "Service", message);
          if (throwOnError)
            throw new ExtensionUtilityTypeCreateException(message, r.ExtensionType, ex);
        }
        return (object) null;
      }

      private struct ExtensionAssemblyEntry
      {
        public Guid UniqueProcessId { get; set; }

        public string Description { get; set; }

        public string Assembly { get; set; }

        public string PluginPath { get; set; }

        public string AssemblyPath { get; set; }

        public string AssemblyFullName { get; set; }
      }
    }
  }
}
