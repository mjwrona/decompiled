// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RURequestProperty
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public abstract class RURequestProperty
  {
    private static Dictionary<string, RURequestProperty> s_propertyLibrary;

    public virtual RUStage PropertyKnownAt { get; protected set; } = RUStage.RequestReady;

    [Obsolete("PropertyKnownAtRequestReady is deprecated, please use PropertyKnownAt instead.", false)]
    public virtual bool PropertyKnownAtRequestReady { get; protected set; } = true;

    public virtual bool ShouldOutputEntityToTelemetry { get; protected set; } = true;

    internal static RURequestProperty GetRURequestPropertyFromString(string property)
    {
      if (RURequestProperty.s_propertyLibrary == null)
      {
        IEnumerable<Type> source;
        try
        {
          source = (IEnumerable<Type>) Assembly.GetAssembly(typeof (RURequestProperty)).GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
          source = ((IEnumerable<Type>) ex.Types).Where<Type>((Func<Type, bool>) (t => t != (Type) null));
        }
        Dictionary<string, RURequestProperty> dictionary = new Dictionary<string, RURequestProperty>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (Type type in source.Where<Type>((Func<Type, bool>) (t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof (RURequestProperty)))))
        {
          RURequestProperty instance = (RURequestProperty) Activator.CreateInstance(type);
          dictionary[instance.GetName()] = instance;
        }
        RURequestProperty.s_propertyLibrary = dictionary;
      }
      RURequestProperty ruRequestProperty;
      return !RURequestProperty.s_propertyLibrary.TryGetValue(property, out ruRequestProperty) ? (RURequestProperty) null : ruRequestProperty;
    }

    public abstract object GetRequestValue(IVssRequestContext requestContext);

    public virtual object GetXEventValue(XEventObjectBase xeventObject) => throw new VssServiceException("Not supported - Are you trying to bucket by " + this.GetName() + " using an XEvent resource?");

    public abstract object ConvertType(string comparand);

    public virtual bool EvaluateSpecialProperty(object requestValue, string specialProperty) => false;

    internal string GetName() => ((IEnumerable<string>) this.ToString().Split('_')).LastOrDefault<string>();
  }
}
