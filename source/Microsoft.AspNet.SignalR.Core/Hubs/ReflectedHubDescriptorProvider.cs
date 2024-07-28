// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.ReflectedHubDescriptorProvider
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class ReflectedHubDescriptorProvider : IHubDescriptorProvider
  {
    private readonly Lazy<IDictionary<string, HubDescriptor>> _hubs;
    private readonly Lazy<IAssemblyLocator> _locator;
    private readonly TraceSource _trace;

    public ReflectedHubDescriptorProvider(IDependencyResolver resolver)
    {
      this._locator = new Lazy<IAssemblyLocator>(new Func<IAssemblyLocator>(((DependencyResolverExtensions) resolver).Resolve<IAssemblyLocator>));
      this._hubs = new Lazy<IDictionary<string, HubDescriptor>>(new Func<IDictionary<string, HubDescriptor>>(this.BuildHubsCache));
      this._trace = resolver.Resolve<ITraceManager>()["SignalR." + typeof (ReflectedHubDescriptorProvider).Name];
    }

    public IList<HubDescriptor> GetHubs() => (IList<HubDescriptor>) this._hubs.Value.Select<KeyValuePair<string, HubDescriptor>, HubDescriptor>((Func<KeyValuePair<string, HubDescriptor>, HubDescriptor>) (kv => kv.Value)).Distinct<HubDescriptor>().ToList<HubDescriptor>();

    public bool TryGetHub(string hubName, out HubDescriptor descriptor) => this._hubs.Value.TryGetValue(hubName, out descriptor);

    protected IDictionary<string, HubDescriptor> BuildHubsCache()
    {
      IEnumerable<HubDescriptor> hubDescriptors = this._locator.Value.GetAssemblies().SelectMany<Assembly, Type>(new Func<Assembly, IEnumerable<Type>>(this.GetTypesSafe)).Where<Type>(new Func<Type, bool>(ReflectedHubDescriptorProvider.IsHubType)).Select<Type, HubDescriptor>((Func<Type, HubDescriptor>) (type =>
      {
        return new HubDescriptor()
        {
          NameSpecified = type.GetHubAttributeName() != null,
          Name = type.GetHubName(),
          HubType = type
        };
      }));
      Dictionary<string, HubDescriptor> dictionary = new Dictionary<string, HubDescriptor>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (HubDescriptor hubDescriptor1 in hubDescriptors)
      {
        HubDescriptor hubDescriptor2 = (HubDescriptor) null;
        if (!dictionary.TryGetValue(hubDescriptor1.Name, out hubDescriptor2))
          dictionary[hubDescriptor1.Name] = hubDescriptor1;
        else
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_DuplicateHubNames, new object[3]
          {
            (object) hubDescriptor2.HubType.AssemblyQualifiedName,
            (object) hubDescriptor1.HubType.AssemblyQualifiedName,
            (object) hubDescriptor1.Name
          }));
      }
      return (IDictionary<string, HubDescriptor>) dictionary;
    }

    private static bool IsHubType(Type type)
    {
      try
      {
        return typeof (IHub).IsAssignableFrom(type) && !type.IsAbstract && (type.Attributes.HasFlag((Enum) TypeAttributes.Public) || type.Attributes.HasFlag((Enum) TypeAttributes.NestedPublic));
      }
      catch
      {
        return false;
      }
    }

    private IEnumerable<Type> GetTypesSafe(Assembly a)
    {
      try
      {
        return (IEnumerable<Type>) a.GetTypes();
      }
      catch (ReflectionTypeLoadException ex)
      {
        this._trace.TraceWarning("Some of the classes from assembly \"{0}\" could Not be loaded when searching for Hubs. [{1}]\r\nOriginal exception type: {2}\r\nOriginal exception message: {3}\r\n", (object) a.FullName, a.IsDynamic ? (object) "<<dynamic>>" : (object) a.Location, (object) ex.GetType().Name, (object) ex.Message);
        if (ex.LoaderExceptions != null)
        {
          this._trace.TraceWarning("Loader exceptions messages: ");
          foreach (Exception loaderException in ex.LoaderExceptions)
            this._trace.TraceWarning("{0}\r\n", (object) loaderException);
        }
        return ((IEnumerable<Type>) ex.Types).Where<Type>((Func<Type, bool>) (t => t != (Type) null));
      }
      catch (Exception ex)
      {
        this._trace.TraceWarning("None of the classes from assembly \"{0}\" could be loaded when searching for Hubs. [{1}]\r\nOriginal exception type: {2}\r\nOriginal exception message: {3}\r\n", (object) a.FullName, a.IsDynamic ? (object) "<<dynamic>>" : (object) a.Location, (object) ex.GetType().Name, (object) ex.Message);
        return Enumerable.Empty<Type>();
      }
    }
  }
}
