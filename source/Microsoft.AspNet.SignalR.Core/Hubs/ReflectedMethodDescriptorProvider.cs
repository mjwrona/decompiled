// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.ReflectedMethodDescriptorProvider
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class ReflectedMethodDescriptorProvider : IMethodDescriptorProvider
  {
    private readonly ConcurrentDictionary<string, IDictionary<string, IEnumerable<MethodDescriptor>>> _methods;
    private readonly ConcurrentDictionary<string, MethodDescriptor> _executableMethods;

    public ReflectedMethodDescriptorProvider()
    {
      this._methods = new ConcurrentDictionary<string, IDictionary<string, IEnumerable<MethodDescriptor>>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this._executableMethods = new ConcurrentDictionary<string, MethodDescriptor>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public IEnumerable<MethodDescriptor> GetMethods(HubDescriptor hub) => (IEnumerable<MethodDescriptor>) this.FetchMethodsFor(hub).SelectMany<KeyValuePair<string, IEnumerable<MethodDescriptor>>, MethodDescriptor>((Func<KeyValuePair<string, IEnumerable<MethodDescriptor>>, IEnumerable<MethodDescriptor>>) (kv => kv.Value)).ToList<MethodDescriptor>();

    private IDictionary<string, IEnumerable<MethodDescriptor>> FetchMethodsFor(HubDescriptor hub) => this._methods.GetOrAdd(hub.Name, (Func<string, IDictionary<string, IEnumerable<MethodDescriptor>>>) (key => ReflectedMethodDescriptorProvider.BuildMethodCacheFor(hub)));

    private static IDictionary<string, IEnumerable<MethodDescriptor>> BuildMethodCacheFor(
      HubDescriptor hub)
    {
      return (IDictionary<string, IEnumerable<MethodDescriptor>>) ReflectionHelper.GetExportedHubMethods(hub.HubType).GroupBy<MethodInfo, string>(new Func<MethodInfo, string>(ReflectedMethodDescriptorProvider.GetMethodName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToDictionary<IGrouping<string, MethodInfo>, string, IEnumerable<MethodDescriptor>>((Func<IGrouping<string, MethodInfo>, string>) (group => group.Key), (Func<IGrouping<string, MethodInfo>, IEnumerable<MethodDescriptor>>) (group => group.Select<MethodInfo, MethodDescriptor>((Func<MethodInfo, MethodDescriptor>) (oload => ReflectedMethodDescriptorProvider.GetMethodDescriptor(group.Key, hub, oload)))), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    private static MethodDescriptor GetMethodDescriptor(
      string methodName,
      HubDescriptor hub,
      MethodInfo methodInfo)
    {
      Type progressReportingType;
      IEnumerable<ParameterInfo> progressParameter = ReflectedMethodDescriptorProvider.ExtractProgressParameter(methodInfo.GetParameters(), out progressReportingType);
      MethodDescriptor methodDescriptor = new MethodDescriptor();
      methodDescriptor.ReturnType = methodInfo.ReturnType;
      methodDescriptor.Name = methodName;
      methodDescriptor.NameSpecified = ReflectedMethodDescriptorProvider.GetMethodAttributeName(methodInfo) != null;
      methodDescriptor.Invoker = new Func<IHub, object[], object>(new HubMethodDispatcher(methodInfo).Execute);
      methodDescriptor.Hub = hub;
      methodDescriptor.Attributes = methodInfo.GetCustomAttributes(typeof (Attribute), true).Cast<Attribute>();
      methodDescriptor.ProgressReportingType = progressReportingType;
      methodDescriptor.Parameters = (IList<ParameterDescriptor>) progressParameter.Select<ParameterInfo, ParameterDescriptor>((Func<ParameterInfo, ParameterDescriptor>) (p => new ParameterDescriptor()
      {
        Name = p.Name,
        ParameterType = p.ParameterType
      })).ToList<ParameterDescriptor>();
      return methodDescriptor;
    }

    private static IEnumerable<ParameterInfo> ExtractProgressParameter(
      ParameterInfo[] parameters,
      out Type progressReportingType)
    {
      ParameterInfo parameter = ((IEnumerable<ParameterInfo>) parameters).LastOrDefault<ParameterInfo>();
      progressReportingType = (Type) null;
      if (!ReflectedMethodDescriptorProvider.IsProgressType(parameter))
        return (IEnumerable<ParameterInfo>) parameters;
      progressReportingType = parameter.ParameterType.GenericTypeArguments[0];
      return ((IEnumerable<ParameterInfo>) parameters).Take<ParameterInfo>(parameters.Length - 1);
    }

    private static bool IsProgressType(ParameterInfo parameter) => parameter != null && parameter.ParameterType.IsGenericType && parameter.ParameterType.GetGenericTypeDefinition() == typeof (IProgress<>);

    public bool TryGetMethod(
      HubDescriptor hub,
      string method,
      out MethodDescriptor descriptor,
      IList<IJsonValue> parameters)
    {
      string key = ReflectedMethodDescriptorProvider.BuildHubExecutableMethodCacheKey(hub, method, parameters);
      if (!this._executableMethods.TryGetValue(key, out descriptor))
      {
        IEnumerable<MethodDescriptor> source;
        if (this.FetchMethodsFor(hub).TryGetValue(method, out source))
        {
          List<MethodDescriptor> list = source.Where<MethodDescriptor>((Func<MethodDescriptor, bool>) (o => o.Matches(parameters))).ToList<MethodDescriptor>();
          descriptor = list.Count == 1 ? list[0] : (MethodDescriptor) null;
        }
        else
          descriptor = (MethodDescriptor) null;
        if (descriptor != null)
          this._executableMethods.TryAdd(key, descriptor);
      }
      return descriptor != null;
    }

    private static string BuildHubExecutableMethodCacheKey(
      HubDescriptor hub,
      string method,
      IList<IJsonValue> parameters)
    {
      string str = parameters == null ? "0" : parameters.Count.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      string upperInvariant = method.ToUpperInvariant();
      return hub.Name + "::" + upperInvariant + "(" + str + ")";
    }

    private static string GetMethodName(MethodInfo method) => ReflectedMethodDescriptorProvider.GetMethodAttributeName(method) ?? method.Name;

    private static string GetMethodAttributeName(MethodInfo method) => ReflectionHelper.GetAttributeValue<HubMethodNameAttribute, string>((ICustomAttributeProvider) method, (Func<HubMethodNameAttribute, string>) (a => a.MethodName));
  }
}
