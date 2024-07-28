// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.EventHelper
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  internal static class EventHelper
  {
    internal static void FillChangedEventFromUpdate<T, U>(
      IVssRequestContext requestContext,
      ChangedEvent changed,
      T oldObject,
      U update)
      where T : IPropertyProvider
      where U : IUpdatePropertyProvider
    {
      requestContext.TraceEnter(0, "Build", "Notification", "");
      ILocationService service = requestContext.GetService<ILocationService>();
      changed.ChangedType = ChangedType.Updated;
      changed.ChangedBy = requestContext.DomainUserName;
      changed.TeamProjectCollectionUrl = service.GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker);
      changed.TimeZone = BisEvents.GetLocalTimeZoneName(DateTime.Now);
      changed.TimeZoneOffset = TFCommonUtil.GetLocalTimeZoneOffset(DateTime.Now);
      changed.Uri = update.Uri;
      changed.Name = update.Name;
      changed.ResourceType = typeof (T).Name;
      foreach (PropertyChange propertyChange in EventHelper.AttachedPropertiesDelta((IEnumerable<PropertyValue>) oldObject.Properties, (IEnumerable<PropertyValue>) update.AttachedProperties))
      {
        changed.PropertyChanges.Add(propertyChange);
        requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Property changed: Name={0} OldValue={1} NewValue={2}", (object) propertyChange.Name, (object) propertyChange.OldValue, (object) propertyChange.NewValue);
      }
      requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Processed changed event '{0}'", (object) changed.Uri);
      requestContext.TraceLeave(0, "Build", "Notification", "");
    }

    internal static void FillChangedEventFromDelta<T>(
      IVssRequestContext requestContext,
      ChangedEvent changed,
      T oldObject,
      T newObject)
      where T : IPropertyProvider
    {
      requestContext.TraceEnter(0, "Build", "Notification", "");
      ILocationService service = requestContext.GetService<ILocationService>();
      changed.ChangedType = ChangedType.Updated;
      changed.ChangedBy = requestContext.DomainUserName;
      changed.TeamProjectCollectionUrl = service.GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker);
      changed.TimeZone = BisEvents.GetLocalTimeZoneName(DateTime.Now);
      changed.TimeZoneOffset = TFCommonUtil.GetLocalTimeZoneOffset(DateTime.Now);
      changed.Uri = newObject.Uri;
      changed.Name = newObject.Name;
      changed.ResourceType = typeof (T).Name;
      foreach (PropertyChange propertyChange in EventHelper.AttachedPropertiesDelta((IEnumerable<PropertyValue>) oldObject.Properties, (IEnumerable<PropertyValue>) newObject.Properties))
      {
        changed.PropertyChanges.Add(propertyChange);
        requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Property changed: Name={0} OldValue={1} NewValue={2}", (object) propertyChange.Name, (object) propertyChange.OldValue, (object) propertyChange.NewValue);
      }
      requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Processed changed event '{0}'", (object) changed.Uri);
      requestContext.TraceLeave(0, "Build", "Notification", "");
    }

    internal static void FillAddedOrDeletedEvent<T>(
      IVssRequestContext requestContext,
      ChangedType action,
      ChangedEvent changed,
      T obj,
      HashSet<string> includedProperties)
      where T : IPropertyProvider
    {
      requestContext.TraceEnter(0, "Build", "Notification", nameof (FillAddedOrDeletedEvent));
      ILocationService service = requestContext.GetService<ILocationService>();
      changed.ChangedType = action;
      changed.ChangedBy = requestContext.DomainUserName;
      changed.TeamProjectCollectionUrl = service.GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker);
      changed.TimeZone = BisEvents.GetLocalTimeZoneName(DateTime.Now);
      changed.TimeZoneOffset = TFCommonUtil.GetLocalTimeZoneOffset(DateTime.Now);
      changed.Uri = obj.Uri;
      changed.Name = obj.Name;
      changed.ResourceType = typeof (T).Name;
      switch (action)
      {
        case ChangedType.Added:
          requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Processing add action for properties");
          foreach (PropertyChange propertyChange in EventHelper.CommonPropertiesAdded<T>(obj, includedProperties))
          {
            changed.PropertyChanges.Add(propertyChange);
            requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Property changed: Name={0} OldValue={1} NewValue={2}", (object) propertyChange.Name, (object) propertyChange.OldValue, (object) propertyChange.NewValue);
          }
          EventHelper.AddAttachedProperties(changed, (IEnumerable<PropertyValue>) obj.Properties);
          break;
        case ChangedType.Deleted:
          requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Processing delete action for properties");
          foreach (PropertyChange propertyChange in EventHelper.CommonPropertiesDeleted<T>(obj, includedProperties))
          {
            changed.PropertyChanges.Add(propertyChange);
            requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Property changed: Name={0} OldValue={1} NewValue={2}", (object) propertyChange.Name, (object) propertyChange.OldValue, (object) propertyChange.NewValue);
          }
          EventHelper.DeleteAttachedProperties(changed, (IEnumerable<PropertyValue>) obj.Properties);
          break;
      }
      requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Processed changed event '{0}'", (object) changed.Uri);
      requestContext.TraceLeave(0, "Build", "Notification", nameof (FillAddedOrDeletedEvent));
    }

    internal static void AddAttachedProperties(
      ChangedEvent changed,
      IEnumerable<PropertyValue> properties)
    {
      foreach (PropertyValue property in properties)
        changed.PropertyChanges.Add(new PropertyChange()
        {
          Name = property.PropertyName,
          OldValue = (string) null,
          NewValue = EventHelper.ObjectToString(property.Value)
        });
    }

    internal static void DeleteAttachedProperties(
      ChangedEvent changed,
      IEnumerable<PropertyValue> properties)
    {
      foreach (PropertyValue property in properties)
        changed.PropertyChanges.Add(new PropertyChange()
        {
          Name = property.PropertyName,
          NewValue = (string) null,
          OldValue = EventHelper.ObjectToString(property.Value)
        });
    }

    internal static IEnumerable<PropertyChange> AttachedPropertiesDelta(
      IEnumerable<PropertyValue> oldSet,
      IEnumerable<PropertyValue> newSet)
    {
      List<PropertyChange> propertyChangeList = new List<PropertyChange>();
      Dictionary<string, PropertyValue> dictionary = oldSet.ToDictionary<PropertyValue, string, PropertyValue>((Func<PropertyValue, string>) (x => x.PropertyName), (Func<PropertyValue, PropertyValue>) (x => x), (IEqualityComparer<string>) StringComparer.Ordinal);
      foreach (PropertyValue propertyValue1 in newSet)
      {
        PropertyValue propertyValue2;
        if (dictionary.TryGetValue(propertyValue1.PropertyName, out propertyValue2))
          dictionary.Remove(propertyValue1.PropertyName);
        if (propertyValue1.Value == null)
          propertyChangeList.Add(new PropertyChange()
          {
            Name = propertyValue1.PropertyName,
            OldValue = EventHelper.ObjectToString(propertyValue2?.Value),
            NewValue = (string) null
          });
        else if (propertyValue2 == null)
          propertyChangeList.Add(new PropertyChange()
          {
            Name = propertyValue1.PropertyName,
            OldValue = (string) null,
            NewValue = EventHelper.ObjectToString(propertyValue1?.Value)
          });
        else
          propertyChangeList.Add(new PropertyChange()
          {
            Name = propertyValue1.PropertyName,
            OldValue = EventHelper.ObjectToString(propertyValue2.Value),
            NewValue = EventHelper.ObjectToString(propertyValue1.Value)
          });
      }
      foreach (PropertyValue propertyValue in dictionary.Values)
        propertyChangeList.Add(new PropertyChange()
        {
          Name = propertyValue.PropertyName,
          OldValue = EventHelper.ObjectToString(propertyValue.Value),
          NewValue = (string) null
        });
      return (IEnumerable<PropertyChange>) propertyChangeList;
    }

    internal static IEnumerable<PropertyChange> CommonPropertiesDeltaFromObjects<T>(
      T oldObj,
      T newObj,
      HashSet<string> includeProperties)
    {
      List<PropertyChange> propertyChangeList = new List<PropertyChange>();
      foreach (PropertyInfo property in typeof (T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (includeProperties.Contains(property.Name))
        {
          object objA = property.GetValue((object) oldObj, (object[]) null);
          object objB = property.GetValue((object) newObj, (object[]) null);
          if (!object.Equals(objA, objB))
            propertyChangeList.Add(new PropertyChange()
            {
              Name = property.Name,
              OldValue = EventHelper.ObjectToString(objA),
              NewValue = EventHelper.ObjectToString(objB)
            });
        }
      }
      return (IEnumerable<PropertyChange>) propertyChangeList;
    }

    internal static IEnumerable<PropertyChange> CommonPropertiesDeltaFromUpdate<T, U>(
      T oldObj,
      U update,
      HashSet<string> includeProperties)
    {
      List<PropertyChange> propertyChangeList = new List<PropertyChange>();
      PropertyInfo[] properties = typeof (T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      Dictionary<string, PropertyInfo> dictionary1 = ((IEnumerable<PropertyInfo>) typeof (U).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).ToDictionary<PropertyInfo, string, PropertyInfo>((Func<PropertyInfo, string>) (x => x.Name), (Func<PropertyInfo, PropertyInfo>) (x => x), (IEqualityComparer<string>) StringComparer.Ordinal);
      PropertyInfo property = typeof (U).GetProperty("Fields");
      Dictionary<string, FieldInfo> dictionary2 = ((IEnumerable<FieldInfo>) property.PropertyType.GetFields()).ToDictionary<FieldInfo, string, FieldInfo>((Func<FieldInfo, string>) (x => x.Name), (Func<FieldInfo, FieldInfo>) (x => x), (IEqualityComparer<string>) StringComparer.Ordinal);
      object obj1 = property.GetValue((object) update, (object[]) null);
      foreach (PropertyInfo propertyInfo1 in properties)
      {
        string name = propertyInfo1.Name;
        if (includeProperties.Contains(name))
        {
          PropertyInfo propertyInfo2;
          dictionary1.TryGetValue(name, out propertyInfo2);
          FieldInfo fieldInfo;
          if (dictionary2.TryGetValue(name, out fieldInfo))
          {
            int int16_1 = (int) Convert.ToInt16(obj1, (IFormatProvider) CultureInfo.InvariantCulture);
            int int16_2 = (int) Convert.ToInt16(fieldInfo.GetValue((object) null), (IFormatProvider) CultureInfo.InvariantCulture);
            int num = int16_2;
            if ((int16_1 & num) == int16_2)
            {
              object obj2 = propertyInfo1.GetValue((object) oldObj, (object[]) null);
              object obj3 = propertyInfo2.GetValue((object) update, (object[]) null);
              propertyChangeList.Add(new PropertyChange()
              {
                Name = name,
                OldValue = EventHelper.ObjectToString(obj2),
                NewValue = EventHelper.ObjectToString(obj3)
              });
            }
          }
        }
      }
      return (IEnumerable<PropertyChange>) propertyChangeList;
    }

    internal static IEnumerable<PropertyChange> CommonPropertiesAdded<T>(
      T obj,
      HashSet<string> includeProperties)
    {
      List<PropertyChange> propertyChangeList = new List<PropertyChange>();
      foreach (PropertyInfo property in typeof (T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (includeProperties.Contains(property.Name))
        {
          object obj1 = property.GetValue((object) obj, (object[]) null);
          propertyChangeList.Add(new PropertyChange()
          {
            Name = property.Name,
            OldValue = (string) null,
            NewValue = EventHelper.ObjectToString(obj1)
          });
        }
      }
      return (IEnumerable<PropertyChange>) propertyChangeList;
    }

    internal static IEnumerable<PropertyChange> CommonPropertiesDeleted<T>(
      T obj,
      HashSet<string> includeProperties)
    {
      List<PropertyChange> propertyChangeList = new List<PropertyChange>();
      foreach (PropertyInfo property in typeof (T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (includeProperties.Contains(property.Name))
        {
          object obj1 = property.GetValue((object) obj, (object[]) null);
          propertyChangeList.Add(new PropertyChange()
          {
            Name = property.Name,
            OldValue = EventHelper.ObjectToString(obj1),
            NewValue = (string) null
          });
        }
      }
      return (IEnumerable<PropertyChange>) propertyChangeList;
    }

    private static string ObjectToString(object obj)
    {
      switch (obj)
      {
        case null:
          return (string) null;
        case string _:
          return obj as string;
        case IEnumerable _:
          string str = string.Empty;
          foreach (object obj1 in obj as IEnumerable)
            str = str + obj1.ToString() + ";";
          return str;
        default:
          return obj.ToString();
      }
    }
  }
}
