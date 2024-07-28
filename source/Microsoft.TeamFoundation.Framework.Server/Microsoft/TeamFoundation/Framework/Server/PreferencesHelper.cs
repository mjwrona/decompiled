// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PreferencesHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class PreferencesHelper
  {
    private static Dictionary<string, PreferencesHelper.Validator> s_validators = new Dictionary<string, PreferencesHelper.Validator>((IEqualityComparer<string>) StringComparer.Ordinal);
    private static Dictionary<string, PreferencesHelper.Fixer> s_fixers;
    private const string Auto = "auto";

    public static T Load<T>(Dictionary<string, string> entries) where T : BasePreferences
    {
      BasePreferences instance = (BasePreferences) Activator.CreateInstance(typeof (T));
      foreach (KeyValuePair<string, string> entry in entries)
      {
        if (!string.IsNullOrWhiteSpace(entry.Key))
        {
          PropertyInfo property = instance.GetType().GetProperty(entry.Key, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
          if (property != (PropertyInfo) null && !string.IsNullOrWhiteSpace(entry.Value))
          {
            if (!string.Equals(entry.Value, "auto", StringComparison.OrdinalIgnoreCase))
            {
              try
              {
                PreferencesHelper.SetProperty(instance, property, entry.Value);
                PreferencesHelper.Validate(instance, property, true, true, "preferences");
              }
              catch (Exception ex)
              {
                TeamFoundationTrace.TraceException("Exception thrown while loading user/account preferences", nameof (Load), ex);
              }
            }
          }
        }
      }
      return (T) instance;
    }

    public static Dictionary<string, string> ComputeDelta(
      BasePreferences newPreferences,
      BasePreferences existingPreferences,
      bool merge,
      out string[] entriesToDelete)
    {
      Dictionary<string, string> delta = new Dictionary<string, string>();
      List<string> stringList = new List<string>();
      foreach (PropertyInfo property in newPreferences.GetType().GetProperties())
      {
        object obj1 = property.GetValue((object) newPreferences, (object[]) null);
        if (existingPreferences == null)
        {
          delta.Add(property.Name, PreferencesHelper.ToString(property, obj1));
        }
        else
        {
          object obj2 = property.GetValue((object) existingPreferences, (object[]) null);
          if (obj1 == null)
          {
            if (obj2 != null)
            {
              if (merge)
                property.SetValue((object) newPreferences, obj2, (object[]) null);
              else
                stringList.Add(property.Name);
            }
          }
          else if (!obj1.Equals(obj2))
            delta.Add(property.Name, PreferencesHelper.ToString(property, obj1));
        }
      }
      entriesToDelete = stringList.ToArray();
      return delta;
    }

    public static void WriteXml(BasePreferences preferences, XmlWriter writer)
    {
      foreach (PropertyInfo property in preferences.GetType().GetProperties())
      {
        object obj = property.GetValue((object) preferences, (object[]) null);
        if (obj != null)
        {
          writer.WriteStartElement(property.Name);
          writer.WriteValue(PreferencesHelper.ToString(property, obj));
          writer.WriteEndElement();
        }
      }
    }

    public static void ReadXml(BasePreferences preferences, XmlReader reader)
    {
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num != 0)
        return;
      while (reader.NodeType == XmlNodeType.Element)
      {
        string name = reader.Name;
        PropertyInfo property = preferences.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
        if (property != (PropertyInfo) null)
          PreferencesHelper.SetProperty(preferences, property, XmlUtility.StringFromXmlElement(reader));
        else
          reader.ReadOuterXml();
      }
      reader.ReadEndElement();
    }

    public static void Validate(
      BasePreferences preferences,
      bool allowNullProperty,
      string argumentName)
    {
      if (preferences == null)
        throw new ArgumentNullException(argumentName);
      foreach (PropertyInfo property in preferences.GetType().GetProperties())
        PreferencesHelper.Validate(preferences, property, false, allowNullProperty, argumentName);
    }

    private static void Validate(
      BasePreferences preferences,
      PropertyInfo pInfo,
      bool fixInvalidProperty,
      bool allowNullProperty,
      string argumentName)
    {
      object obj = pInfo.GetValue((object) preferences, (object[]) null);
      if (obj == null)
      {
        if (!allowNullProperty)
          throw new ArgumentException(FrameworkResources.PreferenceCannotBeEmpty((object) pInfo.Name), argumentName);
      }
      else
      {
        PreferencesHelper.Validator validator;
        if (!PreferencesHelper.s_validators.TryGetValue(pInfo.Name, out validator))
          return;
        try
        {
          validator(obj, argumentName, pInfo.Name);
        }
        catch (Exception ex1)
        {
          TeamFoundationTrace.TraceException(ex1);
          if (fixInvalidProperty)
          {
            PreferencesHelper.Fixer fixer;
            if (PreferencesHelper.s_fixers.TryGetValue(pInfo.Name, out fixer))
            {
              try
              {
                pInfo.SetValue((object) preferences, fixer(obj), (object[]) null);
                return;
              }
              catch (Exception ex2)
              {
                TeamFoundationTrace.TraceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Failed to fix property {0} from value: {1}", (object) pInfo.Name, obj), nameof (Validate), ex2);
              }
            }
            pInfo.SetValue((object) preferences, (object) null, (object[]) null);
          }
          else
            throw;
        }
      }
    }

    private static void CheckSpecificCulture(
      object value,
      string argumentName,
      string propertyName)
    {
      if ((((CultureInfo) value).CultureTypes & CultureTypes.SpecificCultures) == (CultureTypes) 0)
        throw new ArgumentException(FrameworkResources.InvalidSpecificCultureValue((object) propertyName), argumentName);
    }

    private static object ConvertToSpecificCulture(object value) => (object) CultureInfo.ReadOnly(CultureInfo.CreateSpecificCulture(((CultureInfo) value).Name));

    private static void CheckSystemTimeZone(object value, string argumentName, string propertyName)
    {
      string id = ((TimeZoneInfo) value).Id;
      try
      {
        TimeZoneInfo.FindSystemTimeZoneById(id);
      }
      catch
      {
        throw new ArgumentException(FrameworkResources.InvalidSystemTimeZoneValue((object) propertyName), argumentName);
      }
    }

    private static string ToString(PropertyInfo pInfo, object value)
    {
      string str = (string) null;
      Type propertyType = pInfo.PropertyType;
      TypeConverter converter = TypeDescriptor.GetConverter(propertyType);
      if (converter.CanConvertFrom(typeof (string)))
        str = converter.ConvertToInvariantString(value);
      else if (propertyType.Equals(typeof (TimeZoneInfo)))
        str = ((TimeZoneInfo) value).Id;
      else if (value is string)
        return value as string;
      return str != null ? str : throw new NotImplementedException();
    }

    private static void SetProperty(BasePreferences preferences, PropertyInfo pInfo, string value)
    {
      object obj = (object) null;
      Type propertyType = pInfo.PropertyType;
      if (propertyType.IsAssignableFrom(typeof (string)))
      {
        obj = (object) value;
      }
      else
      {
        TypeConverter converter = TypeDescriptor.GetConverter(propertyType);
        if (converter.CanConvertFrom(typeof (string)))
          obj = converter.ConvertFromInvariantString(value);
        else if (propertyType.Equals(typeof (TimeZoneInfo)))
          obj = (object) TimeZoneInfo.FindSystemTimeZoneById(value);
      }
      if (obj == null)
        throw new NotImplementedException();
      pInfo.SetValue((object) preferences, obj, (object[]) null);
    }

    static PreferencesHelper()
    {
      PreferencesHelper.s_validators.Add("Culture", new PreferencesHelper.Validator(PreferencesHelper.CheckSpecificCulture));
      PreferencesHelper.s_validators.Add("TimeZone", new PreferencesHelper.Validator(PreferencesHelper.CheckSystemTimeZone));
      PreferencesHelper.s_fixers = new Dictionary<string, PreferencesHelper.Fixer>((IEqualityComparer<string>) StringComparer.Ordinal);
      PreferencesHelper.s_fixers.Add("Culture", new PreferencesHelper.Fixer(PreferencesHelper.ConvertToSpecificCulture));
    }

    private delegate void Validator(object value, string argumentName, string propertyName);

    private delegate object Fixer(object value);
  }
}
