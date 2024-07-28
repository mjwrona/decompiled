// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.TelemetryConfigurationFactory
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal class TelemetryConfigurationFactory
  {
    private const string AddElementName = "Add";
    private const string TypeAttributeName = "Type";
    private static readonly MethodInfo LoadInstancesDefinition = typeof (TelemetryConfigurationFactory).GetRuntimeMethods().First<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == "LoadInstances"));
    private static readonly XNamespace XmlNamespace = (XNamespace) "http://schemas.microsoft.com/ApplicationInsights/2013/Settings";
    private static TelemetryConfigurationFactory instance;

    protected TelemetryConfigurationFactory()
    {
    }

    public static TelemetryConfigurationFactory Instance
    {
      get => TelemetryConfigurationFactory.instance ?? (TelemetryConfigurationFactory.instance = new TelemetryConfigurationFactory());
      set => TelemetryConfigurationFactory.instance = value;
    }

    public virtual void Initialize(TelemetryConfiguration configuration)
    {
      configuration.ContextInitializers.Add((IContextInitializer) new SdkVersionPropertyContextInitializer());
      configuration.TelemetryInitializers.Add((ITelemetryInitializer) new TimestampPropertyInitializer());
      string text = PlatformSingleton.Current.ReadConfigurationXml();
      if (!string.IsNullOrEmpty(text))
      {
        XDocument xml = XDocument.Parse(text);
        TelemetryConfigurationFactory.LoadFromXml(configuration, xml);
      }
      TelemetryConfigurationFactory.InitializeComponents(configuration);
    }

    protected static object CreateInstance(Type interfaceType, string typeName)
    {
      Type type = TelemetryConfigurationFactory.GetType(typeName);
      object obj = !(type == (Type) null) ? Activator.CreateInstance(type) : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' could not be loaded.", new object[1]
      {
        (object) typeName
      }));
      return interfaceType.IsAssignableFrom(obj.GetType()) ? obj : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' does not implement the required interface {1}.", new object[2]
      {
        (object) type.AssemblyQualifiedName,
        (object) interfaceType.FullName
      }));
    }

    protected static void LoadFromXml(TelemetryConfiguration configuration, XDocument xml) => TelemetryConfigurationFactory.LoadInstance(xml.Element(TelemetryConfigurationFactory.XmlNamespace + "ApplicationInsights"), typeof (TelemetryConfiguration), (object) configuration);

    protected static object LoadInstance(XElement definition, Type expectedType, object instance)
    {
      if (definition != null)
      {
        XAttribute xattribute = definition.Attribute((XName) "Type");
        if (xattribute != null)
        {
          if (instance == null || instance.GetType() != TelemetryConfigurationFactory.GetType(xattribute.Value))
            instance = TelemetryConfigurationFactory.CreateInstance(expectedType, xattribute.Value);
        }
        else if (!definition.Elements().Any<XElement>() && !definition.Attributes().Any<XAttribute>())
          TelemetryConfigurationFactory.LoadInstanceFromValue(definition, expectedType, ref instance);
        else if (instance == null && !expectedType.IsAbstract())
          instance = Activator.CreateInstance(expectedType);
        else if (instance == null)
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' element does not have a Type attribute, does not specify a value and is not a valid collection type", new object[1]
          {
            (object) definition.Name.LocalName
          }));
        if (instance != null)
        {
          TelemetryConfigurationFactory.LoadProperties(definition, instance);
          Type elementType;
          if (TelemetryConfigurationFactory.GetCollectionElementType(instance.GetType(), out elementType))
            TelemetryConfigurationFactory.LoadInstancesDefinition.MakeGenericMethod(elementType).Invoke((object) null, new object[2]
            {
              (object) definition,
              instance
            });
        }
      }
      return instance;
    }

    protected static void LoadInstances<T>(XElement definition, ICollection<T> instances)
    {
      if (definition == null)
        return;
      foreach (XElement element in definition.Elements(TelemetryConfigurationFactory.XmlNamespace + "Add"))
      {
        object instance = (object) null;
        XAttribute xattribute = element.Attribute((XName) "Type");
        if (xattribute != null)
        {
          Type type = TelemetryConfigurationFactory.GetType(xattribute.Value);
          instance = (object) instances.FirstOrDefault<T>((Func<T, bool>) (i => i.GetType() == type));
        }
        int num = instance == null ? 1 : 0;
        object obj = TelemetryConfigurationFactory.LoadInstance(element, typeof (T), instance);
        if (num != 0)
          instances.Add((T) obj);
      }
    }

    protected static void LoadProperties(XElement instanceDefinition, object instance)
    {
      List<XElement> list = TelemetryConfigurationFactory.GetPropertyDefinitions(instanceDefinition).ToList<XElement>();
      if (list.Count <= 0)
        return;
      Type type = instance.GetType();
      Dictionary<string, PropertyInfo> dictionary = ((IEnumerable<PropertyInfo>) type.GetProperties()).ToDictionary<PropertyInfo, string>((Func<PropertyInfo, string>) (p => p.Name));
      foreach (XElement definition in list)
      {
        string localName = definition.Name.LocalName;
        PropertyInfo propertyInfo;
        if (dictionary.TryGetValue(localName, out propertyInfo))
        {
          object instance1 = propertyInfo.GetValue(instance, (object[]) null);
          object obj = TelemetryConfigurationFactory.LoadInstance(definition, propertyInfo.PropertyType, instance1);
          if (propertyInfo.CanWrite)
            propertyInfo.SetValue(instance, obj, (object[]) null);
        }
        else if (!(instance is TelemetryConfiguration))
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' is not a valid property name for type {1}.", new object[2]
          {
            (object) localName,
            (object) type.AssemblyQualifiedName
          }));
      }
    }

    private static void InitializeComponents(TelemetryConfiguration configuration)
    {
      TelemetryConfigurationFactory.InitializeComponent((object) configuration.TelemetryChannel, configuration);
      TelemetryConfigurationFactory.InitializeComponents((IEnumerable) configuration.TelemetryModules, configuration);
      TelemetryConfigurationFactory.InitializeComponents((IEnumerable) configuration.TelemetryInitializers, configuration);
      TelemetryConfigurationFactory.InitializeComponents((IEnumerable) configuration.ContextInitializers, configuration);
    }

    private static void InitializeComponents(
      IEnumerable components,
      TelemetryConfiguration configuration)
    {
      foreach (object component in components)
        TelemetryConfigurationFactory.InitializeComponent(component, configuration);
    }

    private static void InitializeComponent(object component, TelemetryConfiguration configuration)
    {
      if (!(component is ISupportConfiguration supportConfiguration))
        return;
      supportConfiguration.Initialize(configuration);
    }

    private static void LoadInstanceFromValue(
      XElement definition,
      Type expectedType,
      ref object instance)
    {
      if (string.IsNullOrEmpty(definition.Value))
      {
        instance = typeof (ValueType).IsAssignableFrom(expectedType) ? Activator.CreateInstance(expectedType) : (object) null;
      }
      else
      {
        try
        {
          instance = Convert.ChangeType((object) definition.Value.Trim(), expectedType, (IFormatProvider) CultureInfo.InvariantCulture);
        }
        catch (InvalidCastException ex)
        {
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}' element has unexpected contents: '{1}'.", new object[2]
          {
            (object) definition.Name.LocalName,
            (object) definition.Value
          }), (Exception) ex);
        }
      }
    }

    private static Type GetType(string typeName) => TelemetryConfigurationFactory.GetManagedType(typeName);

    private static Type GetWindowsRuntimeType(string typeName)
    {
      try
      {
        return Type.GetType(typeName + ", ContentType=WindowsRuntime");
      }
      catch (IOException ex)
      {
        return (Type) null;
      }
    }

    private static Type GetManagedType(string typeName)
    {
      try
      {
        return Type.GetType(typeName);
      }
      catch (IOException ex)
      {
        return (Type) null;
      }
    }

    private static bool GetCollectionElementType(Type type, out Type elementType)
    {
      Type type1 = ((IEnumerable<Type>) type.GetInterfaces()).FirstOrDefault<Type>((Func<Type, bool>) (i => i.IsGenericType() && i.GetGenericTypeDefinition() == typeof (ICollection<>)));
      elementType = type1 != (Type) null ? type1.GetGenericArguments()[0] : (Type) null;
      return elementType != (Type) null;
    }

    private static IEnumerable<XElement> GetPropertyDefinitions(XElement instanceDefinition) => instanceDefinition.Attributes().Where<XAttribute>((Func<XAttribute, bool>) (a => !a.IsNamespaceDeclaration && a.Name.LocalName != "Type")).Select<XAttribute, XElement>((Func<XAttribute, XElement>) (a => new XElement(a.Name, (object) a.Value))).Concat<XElement>(instanceDefinition.Elements().Where<XElement>((Func<XElement, bool>) (e => e.Name.LocalName != "Add")));
  }
}
