// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DisabledRequestCheckingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class DisabledRequestCheckingService : VssBaseService, IVssFrameworkService
  {
    private IVssRegistryService m_registryService;
    private DisabledRequestCheckingService.DisabledRequest[] m_disabledRequests;
    private ILockName m_loadLockName;
    private Dictionary<string, GetPropertyValueDelegate<AspNetRequestContext>> m_propertyLookup = new Dictionary<string, GetPropertyValueDelegate<AspNetRequestContext>>((IEqualityComparer<string>) StringComparer.Ordinal);
    private static readonly string s_Area = nameof (DisabledRequestCheckingService);
    private static readonly string s_Layer = "TeamFoundationService";
    internal static readonly string s_disabledRequestsKey = "/Configuration/Application/DisabledRequests/**";

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      this.m_loadLockName = this.CreateLockName(requestContext, "load");
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), DisabledRequestCheckingService.s_disabledRequestsKey);
      this.m_registryService = requestContext.GetService<IVssRegistryService>();
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));

    public void CheckIfDisabled(IVssRequestContext requestContext, bool checkUserAgent)
    {
      if (!(requestContext.RootContext is AspNetRequestContext rootContext) && requestContext.UserAgent == null)
        return;
      DisabledRequestCheckingService.DisabledRequest[] disabledRequestArray = this.m_disabledRequests ?? this.LoadDisabledRequests(requestContext);
      string upperInvariant = requestContext.UserAgent != null ? requestContext.UserAgent.ToUpperInvariant() : (string) null;
      foreach (DisabledRequestCheckingService.DisabledRequest disabledRequest in disabledRequestArray)
      {
        if (disabledRequest.Filters.Count > 0)
        {
          string propertyName = string.Empty;
          string str = string.Empty;
          bool flag = false;
          foreach (DisabledRequestCheckingService.DisabledRequestPropertyFilter filter in disabledRequest.Filters)
          {
            propertyName = filter.Property;
            if (filter.IsUserAgent)
            {
              if (checkUserAgent)
                str = upperInvariant;
              else
                continue;
            }
            else if (rootContext != null)
            {
              if (!checkUserAgent)
                str = this.GetPropertyValue(rootContext, filter.Property).ToUpperInvariant();
              else
                continue;
            }
            if (disabledRequest.Negate)
            {
              flag = !PatternUtility.Match(str, filter.Value);
              if (flag)
                break;
            }
            else
            {
              flag = PatternUtility.Match(str, filter.Value);
              if (!flag)
                break;
            }
          }
          if (flag)
          {
            if (string.IsNullOrEmpty(disabledRequest.Message))
              throw new RequestDisabledException(propertyName, str);
            throw new RequestDisabledException(disabledRequest.Message);
          }
        }
      }
    }

    private DisabledRequestCheckingService.DisabledRequest[] LoadDisabledRequests(
      IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = this.m_registryService.ReadEntries(requestContext.Elevate(), (RegistryQuery) DisabledRequestCheckingService.s_disabledRequestsKey);
      DisabledRequestCheckingService.DisabledRequest[] array;
      using (requestContext.Lock(this.m_loadLockName))
      {
        Dictionary<string, DisabledRequestCheckingService.DisabledRequest> dictionary1 = new Dictionary<string, DisabledRequestCheckingService.DisabledRequest>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        Dictionary<string, GetPropertyValueDelegate<AspNetRequestContext>> dictionary2 = new Dictionary<string, GetPropertyValueDelegate<AspNetRequestContext>>((IDictionary<string, GetPropertyValueDelegate<AspNetRequestContext>>) this.m_propertyLookup, (IEqualityComparer<string>) StringComparer.Ordinal);
        foreach (RegistryEntry registryEntry in registryEntryCollection)
        {
          if (!string.IsNullOrEmpty(registryEntry.Value))
          {
            string fileName = Path.GetFileName(Path.GetDirectoryName(registryEntry.Path));
            DisabledRequestCheckingService.DisabledRequest disabledRequest;
            if (!dictionary1.TryGetValue(fileName, out disabledRequest))
            {
              disabledRequest = new DisabledRequestCheckingService.DisabledRequest();
              dictionary1.Add(fileName, disabledRequest);
            }
            if (StringComparer.OrdinalIgnoreCase.Equals(registryEntry.Name, "Message"))
              disabledRequest.Message = StringUtil.ReplaceResources(registryEntry.Value, out bool _);
            else if (StringComparer.OrdinalIgnoreCase.Equals(registryEntry.Name, "Negate"))
            {
              bool result;
              if (bool.TryParse(registryEntry.Value, out result))
                disabledRequest.Negate = result;
              else
                requestContext.Trace(0, TraceLevel.Warning, DisabledRequestCheckingService.s_Area, DisabledRequestCheckingService.s_Layer, "Couldn't parse 'Negate' boolean value '{0}'", (object) registryEntry.Value);
            }
            else if (StringComparer.OrdinalIgnoreCase.Equals(registryEntry.Name, "UserAgent"))
            {
              disabledRequest.Filters.Add(new DisabledRequestCheckingService.DisabledRequestPropertyFilter()
              {
                Property = registryEntry.Name,
                Value = registryEntry.Value.ToUpperInvariant(),
                IsUserAgent = true
              });
            }
            else
            {
              GetPropertyValueDelegate<AspNetRequestContext> propertyValueDelegate;
              if (!dictionary2.TryGetValue(registryEntry.Name, out propertyValueDelegate))
              {
                propertyValueDelegate = DisabledRequestCheckingService.GetDelegate<AspNetRequestContext>(registryEntry.Name);
                if (propertyValueDelegate != null)
                  dictionary2.Add(registryEntry.Name, propertyValueDelegate);
              }
              if (propertyValueDelegate != null)
                disabledRequest.Filters.Add(new DisabledRequestCheckingService.DisabledRequestPropertyFilter()
                {
                  Property = registryEntry.Name,
                  Value = registryEntry.Value.ToUpperInvariant()
                });
              else
                requestContext.Trace(0, TraceLevel.Warning, DisabledRequestCheckingService.s_Area, DisabledRequestCheckingService.s_Layer, "Couldn't initialize request property {0}", (object) registryEntry.Name);
            }
          }
        }
        array = dictionary1.Values.ToArray<DisabledRequestCheckingService.DisabledRequest>();
        this.m_disabledRequests = array;
        this.m_propertyLookup = dictionary2;
      }
      return array;
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      using (requestContext.Lock(this.m_loadLockName))
        this.m_disabledRequests = (DisabledRequestCheckingService.DisabledRequest[]) null;
    }

    private string GetPropertyValue(AspNetRequestContext toCheck, string requestProperty)
    {
      GetPropertyValueDelegate<AspNetRequestContext> propertyValueDelegate;
      if (this.m_propertyLookup.TryGetValue(requestProperty, out propertyValueDelegate))
        return propertyValueDelegate(toCheck) ?? string.Empty;
      toCheck.Trace(0, TraceLevel.Warning, DisabledRequestCheckingService.s_Area, DisabledRequestCheckingService.s_Layer, "Couldn't resolve request property {0}", (object) requestProperty);
      return string.Empty;
    }

    internal static GetPropertyValueDelegate<T> GetDelegate<T>(string propertyName)
    {
      string[] segments = propertyName.Split('.');
      Type type = typeof (T);
      Type[] typesRecursive = DisabledRequestCheckingService.GetTypesRecursive(segments, type);
      if (typesRecursive == null)
        return (GetPropertyValueDelegate<T>) null;
      DynamicMethod dynamicMethod = new DynamicMethod(propertyName, typeof (string), new Type[1]
      {
        typeof (T)
      }, typeof (T).Module);
      ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
      for (int index = 0; index < typesRecursive.Length; ++index)
        ilGenerator.DeclareLocal(typesRecursive[index]);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      for (int index = 0; index < segments.Length; ++index)
      {
        MethodInfo getMethod = type.GetProperty(segments[index], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(true);
        ilGenerator.EmitCall(OpCodes.Callvirt, getMethod, (Type[]) null);
        type = getMethod.ReturnType;
        ilGenerator.Emit(OpCodes.Stloc, index);
        ilGenerator.Emit(type.IsValueType ? OpCodes.Ldloca : OpCodes.Ldloc, index);
        if (!type.IsValueType)
        {
          Label label = ilGenerator.DefineLabel();
          ilGenerator.Emit(OpCodes.Brtrue_S, label);
          ilGenerator.Emit(OpCodes.Ldnull);
          ilGenerator.Emit(OpCodes.Ret);
          ilGenerator.MarkLabel(label);
          ilGenerator.Emit(OpCodes.Ldloc, index);
        }
      }
      if (type.IsValueType)
      {
        if (type.IsPrimitive)
        {
          ilGenerator.EmitCall(OpCodes.Call, type.GetMethod("ToString", Type.EmptyTypes), (Type[]) null);
        }
        else
        {
          ilGenerator.Emit(OpCodes.Constrained, type);
          ilGenerator.EmitCall(OpCodes.Callvirt, typeof (object).GetMethod("ToString", Type.EmptyTypes), (Type[]) null);
        }
      }
      else
        ilGenerator.EmitCall(OpCodes.Callvirt, typeof (object).GetMethod("ToString", Type.EmptyTypes), (Type[]) null);
      ilGenerator.Emit(OpCodes.Ret);
      return (GetPropertyValueDelegate<T>) dynamicMethod.CreateDelegate(typeof (GetPropertyValueDelegate<T>));
    }

    private static Type[] GetTypesRecursive(string[] segments, Type currentType)
    {
      Type[] typesRecursive = new Type[segments.Length];
      for (int index = 0; index < segments.Length; ++index)
      {
        PropertyInfo property = currentType.GetProperty(segments[index], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property == (PropertyInfo) null)
          return (Type[]) null;
        if (property.GetGetMethod(true) == (MethodInfo) null)
          return (Type[]) null;
        currentType = property.PropertyType;
        typesRecursive[index] = currentType;
      }
      return typesRecursive;
    }

    private class DisabledRequest
    {
      private List<DisabledRequestCheckingService.DisabledRequestPropertyFilter> m_filters = new List<DisabledRequestCheckingService.DisabledRequestPropertyFilter>();

      public List<DisabledRequestCheckingService.DisabledRequestPropertyFilter> Filters => this.m_filters;

      public string Message { get; set; }

      public bool Negate { get; set; }
    }

    private class DisabledRequestPropertyFilter
    {
      public string Property { get; set; }

      public string Value { get; set; }

      public bool IsUserAgent { get; set; }
    }
  }
}
