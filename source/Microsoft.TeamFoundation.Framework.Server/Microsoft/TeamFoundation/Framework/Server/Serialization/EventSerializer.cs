// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Serialization.EventSerializer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server.Serialization
{
  public class EventSerializer : IEventSerializer
  {
    public Dictionary<string, object> ToObjectDict<T>(T e) => EventSerializer.ObjectSerializerImpl<T>.Serialize(e);

    private class ObjectGettersContract<T>
    {
      public IReadOnlyDictionary<string, Func<T, object>> Getters { get; }

      public ObjectGettersContract()
      {
        Dictionary<string, Func<T, object>> dictionary = new Dictionary<string, Func<T, object>>();
        foreach (PropertyInfo property in typeof (T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
          if (property.GetCustomAttribute<IgnoreDataMemberAttribute>() == null && property.GetCustomAttribute<JsonIgnoreAttribute>() == null)
          {
            ParameterExpression parameterExpression;
            dictionary[property.Name] = Expression.Lambda<Func<T, object>>((Expression) Expression.Convert((Expression) Expression.Property((Expression) parameterExpression, property), typeof (object)), parameterExpression).Compile();
          }
        }
        this.Getters = (IReadOnlyDictionary<string, Func<T, object>>) dictionary;
      }
    }

    private static class ObjectSerializerImpl<T>
    {
      private static readonly EventSerializer.ObjectGettersContract<T> Contract = new EventSerializer.ObjectGettersContract<T>();

      public static Dictionary<string, object> Serialize(T data)
      {
        Dictionary<string, object> dictionary = new Dictionary<string, object>(EventSerializer.ObjectSerializerImpl<T>.Contract.Getters.Count);
        foreach (KeyValuePair<string, Func<T, object>> getter in (IEnumerable<KeyValuePair<string, Func<T, object>>>) EventSerializer.ObjectSerializerImpl<T>.Contract.Getters)
          dictionary[getter.Key] = getter.Value(data);
        return dictionary;
      }
    }
  }
}
