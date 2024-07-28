// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.Hubs.VssHubDescriptorProvider
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Tracing;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.SignalR.Hubs
{
  public class VssHubDescriptorProvider : IHubDescriptorProvider
  {
    private readonly TraceSource m_trace;
    private readonly Lazy<IAssemblyLocator> m_locator;
    private readonly Lazy<IDictionary<string, HubDescriptor>> m_hubs;

    public VssHubDescriptorProvider(IDependencyResolver resolver)
    {
      this.m_hubs = new Lazy<IDictionary<string, HubDescriptor>>(new Func<IDictionary<string, HubDescriptor>>(this.BuildHubsCache));
      this.m_locator = new Lazy<IAssemblyLocator>((Func<IAssemblyLocator>) (() => resolver.Resolve<IAssemblyLocator>()));
      this.m_trace = resolver.Resolve<ITraceManager>()["SignalR." + typeof (VssHubDescriptorProvider).Name];
    }

    public IList<HubDescriptor> GetHubs() => (IList<HubDescriptor>) this.m_hubs.Value.Select<KeyValuePair<string, HubDescriptor>, HubDescriptor>((Func<KeyValuePair<string, HubDescriptor>, HubDescriptor>) (kv => kv.Value)).Distinct<HubDescriptor>().ToList<HubDescriptor>();

    public bool TryGetHub(string hubName, out HubDescriptor descriptor) => this.m_hubs.Value.TryGetValue(hubName, out descriptor);

    private IDictionary<string, HubDescriptor> BuildHubsCache()
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IEnumerable<HubDescriptor> hubDescriptors = this.m_locator.Value.GetAssemblies().SelectMany<Assembly, Type>(new Func<Assembly, IEnumerable<Type>>(this.GetTypesSafe)).Where<Type>(VssHubDescriptorProvider.\u003C\u003EO.\u003C0\u003E__IsHubType ?? (VssHubDescriptorProvider.\u003C\u003EO.\u003C0\u003E__IsHubType = new Func<Type, bool>(VssHubDescriptorProvider.IsHubType))).Select<Type, HubDescriptor>((Func<Type, HubDescriptor>) (type =>
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
        if (dictionary.TryGetValue(hubDescriptor1.Name, out hubDescriptor2))
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Hub name {0} is duplicated in assembly {1} and {2}", (object) hubDescriptor2.HubType.AssemblyQualifiedName, (object) hubDescriptor1.HubType.AssemblyQualifiedName, (object) hubDescriptor1.Name));
        dictionary[hubDescriptor1.Name] = hubDescriptor1;
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
      catch (ReflectionTypeLoadException ex1)
      {
        try
        {
          this.m_trace.TraceWarning("Some of the classes from assembly \"{0}\" could Not be loaded when searching for Hubs. [{1}]\r\nOriginal exception type: {2}\r\nOriginal exception message: {3}\r\n", (object) a.FullName, (object) VssHubDescriptorProvider.GetLocationSafe(a), (object) ex1.GetType().Name, (object) ex1.Message);
        }
        catch (Exception ex2)
        {
          this.m_trace.TraceError("Encountered exception trying to trace assembly load error\r\nOriginal exception type: {0}\r\nOriginal exception message: {1}\r\nTrace exception: {2}", (object) ex1.GetType().Name, (object) ex1.Message, (object) ex2.ToReadableStackTrace());
        }
        return ((IEnumerable<Type>) ex1.Types).Where<Type>((Func<Type, bool>) (t => t != (Type) null));
      }
      catch (Exception ex3)
      {
        try
        {
          this.m_trace.TraceWarning("None of the classes from assembly \"{0}\" could be loaded when searching for Hubs. [{1}]\r\nOriginal exception type: {2}\r\nOriginal exception message: {3}\r\n", (object) a.FullName, (object) VssHubDescriptorProvider.GetLocationSafe(a), (object) ex3.GetType().Name, (object) ex3.Message);
        }
        catch (Exception ex4)
        {
          this.m_trace.TraceError("Encountered exception trying to trace assembly load error\r\nOriginal exception type: {0}\r\nOriginal exception message: {1}\r\nTrace exception: {2}", (object) ex3.GetType().Name, (object) ex3.Message, (object) ex4.ToReadableStackTrace());
        }
        return Enumerable.Empty<Type>();
      }
    }

    private static string GetLocationSafe(Assembly assembly)
    {
      try
      {
        return assembly.IsDynamic ? string.Empty : assembly.Location;
      }
      catch (Exception ex)
      {
        return string.Empty;
      }
    }
  }
}
