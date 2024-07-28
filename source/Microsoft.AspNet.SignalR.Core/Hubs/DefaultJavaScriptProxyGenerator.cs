// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.DefaultJavaScriptProxyGenerator
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Json;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class DefaultJavaScriptProxyGenerator : IJavaScriptProxyGenerator
  {
    private static readonly Lazy<string> _templateFromResource = new Lazy<string>(new Func<string>(DefaultJavaScriptProxyGenerator.GetTemplateFromResource));
    private static readonly Type[] _numberTypes = new Type[7]
    {
      typeof (byte),
      typeof (short),
      typeof (int),
      typeof (long),
      typeof (float),
      typeof (Decimal),
      typeof (double)
    };
    private static readonly Type[] _dateTypes = new Type[2]
    {
      typeof (DateTime),
      typeof (DateTimeOffset)
    };
    private const string ScriptResource = "Microsoft.AspNet.SignalR.Scripts.hubs.js";
    private readonly IHubManager _manager;
    private readonly IJavaScriptMinifier _javaScriptMinifier;
    private readonly Lazy<string> _generatedTemplate;

    public DefaultJavaScriptProxyGenerator(IDependencyResolver resolver)
      : this(resolver.Resolve<IHubManager>(), resolver.Resolve<IJavaScriptMinifier>())
    {
    }

    public DefaultJavaScriptProxyGenerator(
      IHubManager manager,
      IJavaScriptMinifier javaScriptMinifier)
    {
      this._manager = manager;
      this._javaScriptMinifier = javaScriptMinifier ?? (IJavaScriptMinifier) NullJavaScriptMinifier.Instance;
      this._generatedTemplate = new Lazy<string>((Func<string>) (() => DefaultJavaScriptProxyGenerator.GenerateProxy(this._manager, this._javaScriptMinifier, false)));
    }

    public string GenerateProxy(string serviceUrl)
    {
      serviceUrl = DefaultJavaScriptProxyGenerator.JavaScriptEncode(serviceUrl);
      return this._generatedTemplate.Value.Replace("{serviceUrl}", serviceUrl);
    }

    public string GenerateProxy(string serviceUrl, bool includeDocComments)
    {
      serviceUrl = DefaultJavaScriptProxyGenerator.JavaScriptEncode(serviceUrl);
      return DefaultJavaScriptProxyGenerator.GenerateProxy(this._manager, this._javaScriptMinifier, includeDocComments).Replace("{serviceUrl}", serviceUrl);
    }

    private static string GenerateProxy(
      IHubManager hubManager,
      IJavaScriptMinifier javaScriptMinifier,
      bool includeDocComments)
    {
      string str = DefaultJavaScriptProxyGenerator._templateFromResource.Value;
      StringBuilder sb = new StringBuilder();
      bool flag = true;
      foreach (HubDescriptor descriptor in (IEnumerable<HubDescriptor>) hubManager.GetHubs().OrderBy<HubDescriptor, string>((Func<HubDescriptor, string>) (h => h.Name)))
      {
        if (!flag)
        {
          sb.AppendLine(";");
          sb.AppendLine();
          sb.Append("    ");
        }
        DefaultJavaScriptProxyGenerator.GenerateType(hubManager, sb, descriptor, includeDocComments);
        flag = false;
      }
      if (sb.Length > 0)
        sb.Append(";");
      string source = str.Replace("/*hubs*/", sb.ToString());
      return javaScriptMinifier.Minify(source);
    }

    private static void GenerateType(
      IHubManager hubManager,
      StringBuilder sb,
      HubDescriptor descriptor,
      bool includeDocComments)
    {
      IEnumerable<MethodDescriptor> methods = DefaultJavaScriptProxyGenerator.GetMethods(hubManager, descriptor);
      string descriptorName = DefaultJavaScriptProxyGenerator.GetDescriptorName((Descriptor) descriptor);
      sb.AppendFormat("    proxies['{0}'] = this.createHubProxy('{1}'); ", (object) descriptorName, (object) descriptorName).AppendLine();
      sb.AppendFormat("        proxies['{0}'].client = {{ }};", (object) descriptorName).AppendLine();
      sb.AppendFormat("        proxies['{0}'].server = {{", (object) descriptorName);
      bool flag = true;
      foreach (MethodDescriptor method in methods)
      {
        if (!flag)
          sb.Append(",").AppendLine();
        DefaultJavaScriptProxyGenerator.GenerateMethod(sb, method, includeDocComments, descriptorName);
        flag = false;
      }
      sb.AppendLine();
      sb.Append("        }");
    }

    private static string GetDescriptorName(Descriptor descriptor)
    {
      string name = descriptor != null ? descriptor.Name : throw new ArgumentNullException(nameof (descriptor));
      if (!descriptor.NameSpecified)
        name = JsonUtility.CamelCase(name);
      return name;
    }

    private static IEnumerable<MethodDescriptor> GetMethods(
      IHubManager manager,
      HubDescriptor descriptor)
    {
      return manager.GetHubMethods(descriptor.Name).GroupBy<MethodDescriptor, string>((Func<MethodDescriptor, string>) (method => method.Name)).Select(overloads => new
      {
        overloads = overloads,
        oload = overloads.OrderBy<MethodDescriptor, int>((Func<MethodDescriptor, int>) (overload => overload.Parameters.Count)).FirstOrDefault<MethodDescriptor>()
      }).OrderBy(_param1 => _param1.oload.Name).Select(_param1 => _param1.oload);
    }

    private static void GenerateMethod(
      StringBuilder sb,
      MethodDescriptor method,
      bool includeDocComments,
      string hubName)
    {
      List<string> list1 = method.Parameters.Select<ParameterDescriptor, string>((Func<ParameterDescriptor, string>) (p => p.Name)).ToList<string>();
      sb.AppendLine();
      sb.AppendFormat("            {0}: function ({1}) {{", (object) DefaultJavaScriptProxyGenerator.GetDescriptorName((Descriptor) method), (object) DefaultJavaScriptProxyGenerator.Commas((IEnumerable<string>) list1)).AppendLine();
      if (includeDocComments)
      {
        sb.AppendFormat(Resources.DynamicComment_CallsMethodOnServerSideDeferredPromise, (object) method.Name, (object) method.Hub.Name).AppendLine();
        List<string> list2 = method.Parameters.Select<ParameterDescriptor, string>((Func<ParameterDescriptor, string>) (p => string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DynamicComment_ServerSideTypeIs, new object[3]
        {
          (object) p.Name,
          (object) DefaultJavaScriptProxyGenerator.MapToJavaScriptType(p.ParameterType),
          (object) p.ParameterType
        }))).ToList<string>();
        if (list2.Any<string>())
          sb.AppendLine(string.Join(Environment.NewLine, (IEnumerable<string>) list2));
      }
      sb.AppendFormat("                return proxies['{0}'].invoke.apply(proxies['{0}'], $.merge([\"{1}\"], $.makeArray(arguments)));", (object) hubName, (object) method.Name).AppendLine();
      sb.Append("             }");
    }

    private static string MapToJavaScriptType(Type type)
    {
      if (!type.IsPrimitive && !(type == typeof (string)))
        return "Object";
      if (type == typeof (string))
        return "String";
      if (((IEnumerable<Type>) DefaultJavaScriptProxyGenerator._numberTypes).Contains<Type>(type))
        return "Number";
      if (typeof (IEnumerable).IsAssignableFrom(type))
        return "Array";
      return ((IEnumerable<Type>) DefaultJavaScriptProxyGenerator._dateTypes).Contains<Type>(type) ? "Date" : string.Empty;
    }

    private static string Commas(IEnumerable<string> values) => DefaultJavaScriptProxyGenerator.Commas<string>(values, (Func<string, string>) (v => v));

    private static string Commas<T>(IEnumerable<T> values, Func<T, string> selector) => string.Join(", ", values.Select<T, string>(selector));

    private static string GetTemplateFromResource()
    {
      using (Stream manifestResourceStream = typeof (DefaultJavaScriptProxyGenerator).Assembly.GetManifestResourceStream("Microsoft.AspNet.SignalR.Scripts.hubs.js"))
        return new StreamReader(manifestResourceStream).ReadToEnd();
    }

    private static string JavaScriptEncode(string value)
    {
      value = JsonConvert.SerializeObject((object) value);
      return value.Substring(1, value.Length - 2);
    }
  }
}
