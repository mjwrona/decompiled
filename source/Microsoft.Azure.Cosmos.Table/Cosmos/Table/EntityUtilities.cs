// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.EntityUtilities
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Table
{
  internal static class EntityUtilities
  {
    private static ConcurrentDictionary<Type, Func<object[], object>> compiledActivators = new ConcurrentDictionary<Type, Func<object[], object>>();

    internal static TElement ResolveEntityByType<TElement>(
      string partitionKey,
      string rowKey,
      DateTimeOffset timestamp,
      IDictionary<string, EntityProperty> properties,
      string etag)
    {
      ITableEntity tableEntity = (ITableEntity) EntityUtilities.InstantiateEntityFromType(typeof (TElement));
      tableEntity.PartitionKey = partitionKey;
      tableEntity.RowKey = rowKey;
      tableEntity.Timestamp = timestamp;
      tableEntity.ReadEntity(properties, (OperationContext) null);
      tableEntity.ETag = etag;
      return (TElement) tableEntity;
    }

    internal static DynamicTableEntity ResolveDynamicEntity(
      string partitionKey,
      string rowKey,
      DateTimeOffset timestamp,
      IDictionary<string, EntityProperty> properties,
      string etag)
    {
      DynamicTableEntity dynamicTableEntity = new DynamicTableEntity(partitionKey, rowKey);
      dynamicTableEntity.Timestamp = timestamp;
      dynamicTableEntity.ReadEntity(properties, (OperationContext) null);
      dynamicTableEntity.ETag = etag;
      return dynamicTableEntity;
    }

    internal static object InstantiateEntityFromType(Type type) => EntityUtilities.compiledActivators.GetOrAdd(type, new Func<Type, Func<object[], object>>(EntityUtilities.GenerateActivator))((object[]) null);

    private static Func<object[], object> GenerateActivator(Type type) => EntityUtilities.GenerateActivator(type, Type.EmptyTypes);

    private static Func<object[], object> GenerateActivator(Type type, Type[] ctorParamTypes)
    {
      ConstructorInfo constructor = type.GetConstructor(ctorParamTypes);
      ParameterInfo[] parameterInfoArray = !(constructor == (ConstructorInfo) null) ? constructor.GetParameters() : throw new InvalidOperationException("TableQuery Generic Type must provide a default parameterless constructor.");
      ParameterExpression array = Expression.Parameter(typeof (object[]), "args");
      Expression[] expressionArray = new Expression[parameterInfoArray.Length];
      for (int index1 = 0; index1 < parameterInfoArray.Length; ++index1)
      {
        Expression index2 = (Expression) Expression.Constant((object) index1);
        Type parameterType = parameterInfoArray[index1].ParameterType;
        Expression expression = (Expression) Expression.Convert((Expression) Expression.ArrayIndex((Expression) array, index2), parameterType);
        expressionArray[index1] = expression;
      }
      return (Func<object[], object>) Expression.Lambda(typeof (Func<object[], object>), (Expression) Expression.New(constructor, expressionArray), array).Compile();
    }
  }
}
