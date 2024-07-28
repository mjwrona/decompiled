// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.TableEntity
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Table
{
  public class TableEntity : ITableEntity
  {
    private static ConcurrentDictionary<Type, Func<object, OperationContext, IDictionary<string, EntityProperty>>> compiledWriteCache = new ConcurrentDictionary<Type, Func<object, OperationContext, IDictionary<string, EntityProperty>>>();
    private static ConcurrentDictionary<Type, Action<object, OperationContext, IDictionary<string, EntityProperty>>> compiledReadCache = new ConcurrentDictionary<Type, Action<object, OperationContext, IDictionary<string, EntityProperty>>>();
    private static ConcurrentDictionary<Type, Dictionary<string, EdmType>> propertyResolverCache = new ConcurrentDictionary<Type, Dictionary<string, EdmType>>();
    private static bool disablePropertyResolverCache = false;

    static TableEntity() => TableEntity.DisableCompiledSerializers();

    public TableEntity()
    {
    }

    public TableEntity(string partitionKey, string rowKey)
    {
      this.PartitionKey = partitionKey;
      this.RowKey = rowKey;
    }

    public string PartitionKey { get; set; }

    public string RowKey { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public string ETag { get; set; }

    public virtual void ReadEntity(
      IDictionary<string, EntityProperty> properties,
      OperationContext operationContext)
    {
      if (this.CompiledRead == null)
        this.CompiledRead = TableEntity.compiledReadCache.GetOrAdd(this.GetType(), new Func<Type, Action<object, OperationContext, IDictionary<string, EntityProperty>>>(TableEntity.CompileReadAction));
      this.CompiledRead((object) this, operationContext, properties);
    }

    public static void ReadUserObject(
      object entity,
      IDictionary<string, EntityProperty> properties,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNull(nameof (entity), entity);
      TableEntity.compiledReadCache.GetOrAdd(entity.GetType(), new Func<Type, Action<object, OperationContext, IDictionary<string, EntityProperty>>>(TableEntity.CompileReadAction))(entity, operationContext, properties);
    }

    public static TResult ConvertBack<TResult>(
      IDictionary<string, EntityProperty> properties,
      OperationContext operationContext)
    {
      return EntityPropertyConverter.ConvertBack<TResult>(properties, operationContext);
    }

    public static TResult ConvertBack<TResult>(
      IDictionary<string, EntityProperty> properties,
      EntityPropertyConverterOptions entityPropertyConverterOptions,
      OperationContext operationContext)
    {
      return EntityPropertyConverter.ConvertBack<TResult>(properties, entityPropertyConverterOptions, operationContext);
    }

    public virtual IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
    {
      if (this.CompiledWrite == null)
        this.CompiledWrite = TableEntity.compiledWriteCache.GetOrAdd(this.GetType(), new Func<Type, Func<object, OperationContext, IDictionary<string, EntityProperty>>>(TableEntity.CompileWriteFunc));
      return this.CompiledWrite((object) this, operationContext);
    }

    public static IDictionary<string, EntityProperty> WriteUserObject(
      object entity,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNull(nameof (entity), entity);
      return TableEntity.compiledWriteCache.GetOrAdd(entity.GetType(), new Func<Type, Func<object, OperationContext, IDictionary<string, EntityProperty>>>(TableEntity.CompileWriteFunc))(entity, operationContext);
    }

    public static IDictionary<string, EntityProperty> Flatten(
      object entity,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNull(nameof (entity), entity);
      return (IDictionary<string, EntityProperty>) EntityPropertyConverter.Flatten(entity, operationContext);
    }

    public static IDictionary<string, EntityProperty> Flatten(
      object entity,
      EntityPropertyConverterOptions entityPropertyConverterOptions,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNull(nameof (entity), entity);
      return (IDictionary<string, EntityProperty>) EntityPropertyConverter.Flatten(entity, entityPropertyConverterOptions, operationContext);
    }

    internal static bool ShouldSkipProperty(
      PropertyInfo property,
      OperationContext operationContext)
    {
      string name = property.Name;
      if (name == "PartitionKey" || name == "RowKey" || name == "Timestamp" || name == "ETag")
        return true;
      MethodInfo setMethod = property.SetMethod;
      MethodInfo getMethod = property.GetMethod;
      if (setMethod == (MethodInfo) null || !setMethod.IsPublic || getMethod == (MethodInfo) null || !getMethod.IsPublic)
      {
        Logger.LogInformational(operationContext, "Omitting property '{0}' from serialization/de-serialization because the property's getter/setter are not public.", (object) property.Name);
        return true;
      }
      if (setMethod.IsStatic)
        return true;
      if (!Attribute.IsDefined((MemberInfo) property, typeof (IgnorePropertyAttribute)))
        return false;
      Logger.LogInformational(operationContext, "Omitting property '{0}' from serialization/de-serialization because IgnoreAttribute has been set on that property.", (object) property.Name);
      return true;
    }

    private static void ReadNoOpAction(
      object obj,
      OperationContext ctx,
      IDictionary<string, EntityProperty> dict)
    {
    }

    private static IDictionary<string, EntityProperty> WriteNoOpFunc(
      object obj,
      OperationContext ctx)
    {
      return (IDictionary<string, EntityProperty>) new Dictionary<string, EntityProperty>();
    }

    private static MethodInfo GetKeyOrNullFromDictionaryMethodInfo { get; set; }

    private static MethodInfo DictionaryAddMethodInfo { get; set; }

    private static MethodInfo EntityProperty_CreateFromObjectMethodInfo { get; set; }

    private static MethodInfo EntityPropertyIsNullInfo { get; set; }

    private static PropertyInfo EntityPropertyPropTypePInfo { get; set; }

    private static PropertyInfo EntityProperty_StringPI { get; set; }

    private static PropertyInfo EntityProperty_BinaryPI { get; set; }

    private static PropertyInfo EntityProperty_BoolPI { get; set; }

    private static PropertyInfo EntityProperty_DateTimeOffsetPI { get; set; }

    private static PropertyInfo EntityProperty_DoublePI { get; set; }

    private static PropertyInfo EntityProperty_GuidPI { get; set; }

    private static PropertyInfo EntityProperty_Int32PI { get; set; }

    private static PropertyInfo EntityProperty_Int64PI { get; set; }

    private static PropertyInfo EntityProperty_PropTypePI { get; set; }

    private static MethodInfo EntityProperty_PropTypeGetter { get; set; }

    private static void DisableCompiledSerializers()
    {
      TableEntity.EntityProperty_CreateFromObjectMethodInfo = typeof (EntityProperty).FindStaticMethods("CreateEntityPropertyFromObject").First<MethodInfo>((Func<MethodInfo, bool>) (m =>
      {
        ParameterInfo[] parameters = m.GetParameters();
        return parameters.Length == 2 && parameters[0].ParameterType == typeof (object) && parameters[1].ParameterType == typeof (Type);
      }));
      TableEntity.GetKeyOrNullFromDictionaryMethodInfo = typeof (TableEntity).FindStaticMethods("GetValueByKeyFromDictionary").First<MethodInfo>((Func<MethodInfo, bool>) (m => m.GetParameters().Length == 3));
      TableEntity.DictionaryAddMethodInfo = typeof (Dictionary<string, EntityProperty>).FindMethod("Add", new Type[2]
      {
        typeof (string),
        typeof (EntityProperty)
      });
      TableEntity.EntityProperty_StringPI = typeof (EntityProperty).FindProperty("StringValue");
      TableEntity.EntityProperty_BinaryPI = typeof (EntityProperty).FindProperty("BinaryValue");
      TableEntity.EntityProperty_BoolPI = typeof (EntityProperty).FindProperty("BooleanValue");
      TableEntity.EntityProperty_DateTimeOffsetPI = typeof (EntityProperty).FindProperty("DateTimeOffsetValue");
      TableEntity.EntityProperty_DoublePI = typeof (EntityProperty).FindProperty("DoubleValue");
      TableEntity.EntityProperty_GuidPI = typeof (EntityProperty).FindProperty("GuidValue");
      TableEntity.EntityProperty_Int32PI = typeof (EntityProperty).FindProperty("Int32Value");
      TableEntity.EntityProperty_Int64PI = typeof (EntityProperty).FindProperty("Int64Value");
      TableEntity.EntityProperty_PropTypePI = typeof (EntityProperty).FindProperty("PropertyType");
      TableEntity.EntityProperty_PropTypeGetter = TableEntity.EntityProperty_PropTypePI.FindGetProp();
      TableEntity.EntityPropertyIsNullInfo = typeof (EntityProperty).FindProperty("IsNull").GetGetMethod(true);
    }

    internal Func<object, OperationContext, IDictionary<string, EntityProperty>> CompiledWrite { get; set; }

    internal Action<object, OperationContext, IDictionary<string, EntityProperty>> CompiledRead { get; set; }

    private static Action<object, OperationContext, IDictionary<string, EntityProperty>> CompileReadAction(
      Type type)
    {
      PropertyInfo[] properties = type.GetProperties();
      ParameterExpression instanceParam = Expression.Parameter(typeof (object), "instance");
      ParameterExpression parameterExpression4 = Expression.Parameter(typeof (OperationContext), "ctx");
      ParameterExpression parameterExpression5 = Expression.Parameter(typeof (IDictionary<string, EntityProperty>), "dict");
      ParameterExpression parameterExpression6 = Expression.Variable(typeof (EntityProperty), "entityProp");
      ParameterExpression left = Expression.Variable(typeof (string), "propName");
      List<Expression> expressionList = new List<Expression>();
      foreach (PropertyInfo property in ((IEnumerable<PropertyInfo>) properties).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => !TableEntity.ShouldSkipProperty(p, (OperationContext) null))))
      {
        Expression expressionByType = TableEntity.GeneratePropertyReadExpressionByType(type, property, (Expression) instanceParam, (Expression) parameterExpression6);
        if (expressionByType != null)
        {
          expressionList.Add((Expression) Expression.Assign((Expression) left, (Expression) Expression.Constant((object) property.Name)));
          expressionList.Add((Expression) Expression.Assign((Expression) parameterExpression6, (Expression) Expression.Call(TableEntity.GetKeyOrNullFromDictionaryMethodInfo, (Expression) left, (Expression) parameterExpression5, (Expression) parameterExpression4)));
          expressionList.Add((Expression) Expression.IfThen((Expression) Expression.NotEqual((Expression) parameterExpression6, (Expression) Expression.Constant((object) null)), expressionByType));
        }
      }
      if (expressionList.Count == 0)
        return new Action<object, OperationContext, IDictionary<string, EntityProperty>>(TableEntity.ReadNoOpAction);
      return ((Expression<Action<object, OperationContext, IDictionary<string, EntityProperty>>>) ((parameterExpression1, parameterExpression2, parameterExpression3) => Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[2]
      {
        parameterExpression6,
        left
      }, (IEnumerable<Expression>) expressionList))).Compile();
    }

    private static Func<object, OperationContext, IDictionary<string, EntityProperty>> CompileWriteFunc(
      Type type)
    {
      PropertyInfo[] properties = type.GetProperties();
      ParameterExpression parameterExpression3 = Expression.Parameter(typeof (object), "instance");
      ParameterExpression parameterExpression4 = Expression.Parameter(typeof (OperationContext), "ctx");
      ParameterExpression left1 = Expression.Variable(typeof (EntityProperty), "entityProp");
      ParameterExpression left2 = Expression.Variable(typeof (string), "propName");
      ParameterExpression parameterExpression5 = Expression.Variable(typeof (Dictionary<string, EntityProperty>), "dictVar");
      List<Expression> expressionList = new List<Expression>();
      expressionList.Add((Expression) Expression.Assign((Expression) parameterExpression5, (Expression) Expression.New(typeof (Dictionary<string, EntityProperty>))));
      foreach (PropertyInfo property in ((IEnumerable<PropertyInfo>) properties).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => !TableEntity.ShouldSkipProperty(p, (OperationContext) null))))
      {
        expressionList.Add((Expression) Expression.Assign((Expression) left1, (Expression) Expression.Call(TableEntity.EntityProperty_CreateFromObjectMethodInfo, (Expression) Expression.Convert((Expression) Expression.Call((Expression) Expression.Convert((Expression) parameterExpression3, type), property.FindGetProp()), typeof (object)), (Expression) Expression.Constant((object) property.PropertyType))));
        expressionList.Add((Expression) Expression.Assign((Expression) left2, (Expression) Expression.Constant((object) property.Name)));
        expressionList.Add((Expression) Expression.IfThen((Expression) Expression.NotEqual((Expression) left1, (Expression) Expression.Constant((object) null)), (Expression) Expression.Call((Expression) parameterExpression5, TableEntity.DictionaryAddMethodInfo, (Expression) left2, (Expression) left1)));
      }
      if (expressionList.Count == 1)
        return new Func<object, OperationContext, IDictionary<string, EntityProperty>>(TableEntity.WriteNoOpFunc);
      expressionList.Add((Expression) parameterExpression5);
      return (Func<object, OperationContext, IDictionary<string, EntityProperty>>) ((Expression<Func<object, OperationContext, Dictionary<string, EntityProperty>>>) ((parameterExpression1, parameterExpression2) => Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[3]
      {
        parameterExpression5,
        left1,
        left2
      }, (IEnumerable<Expression>) expressionList))).Compile();
    }

    private static Expression GeneratePropertyReadExpressionByType(
      Type type,
      PropertyInfo property,
      Expression instanceParam,
      Expression currentEntityProperty)
    {
      if (property.PropertyType == typeof (string))
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.String)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_StringPI.FindGetProp())));
      if (property.PropertyType == typeof (byte[]))
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.Binary)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_BinaryPI.FindGetProp())));
      if (property.PropertyType == typeof (bool?))
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.Boolean)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_BoolPI.FindGetProp())));
      if (property.PropertyType == typeof (bool))
      {
        MethodInfo getProp1 = typeof (bool?).FindProperty("HasValue").FindGetProp();
        MethodInfo getProp2 = typeof (bool?).FindProperty("Value").FindGetProp();
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.Boolean)), (Expression) Expression.IfThen((Expression) Expression.IsTrue((Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_BoolPI.FindGetProp()), getProp1)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_BoolPI.FindGetProp()), getProp2))));
      }
      if (property.PropertyType == typeof (DateTime?))
      {
        MethodInfo getProp3 = typeof (DateTimeOffset?).FindProperty("HasValue").FindGetProp();
        MethodInfo getProp4 = typeof (DateTimeOffset?).FindProperty("Value").FindGetProp();
        MethodInfo getProp5 = typeof (DateTimeOffset).FindProperty("UtcDateTime").FindGetProp();
        ParameterExpression left = Expression.Variable(typeof (DateTime?), "tempVal");
        ConditionalExpression conditionalExpression = Expression.IfThenElse((Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_DateTimeOffsetPI.FindGetProp()), getProp3), (Expression) Expression.Assign((Expression) left, (Expression) Expression.TypeAs((Expression) Expression.Call((Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_DateTimeOffsetPI.FindGetProp()), getProp4), getProp5), typeof (DateTime?))), (Expression) Expression.Assign((Expression) left, (Expression) Expression.TypeAs((Expression) Expression.Constant((object) null), typeof (DateTime?))));
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.DateTime)), (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
        {
          left
        }, (Expression) conditionalExpression, (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) left)));
      }
      if (property.PropertyType == typeof (DateTime))
      {
        MethodInfo getProp6 = typeof (DateTimeOffset?).FindProperty("Value").FindGetProp();
        MethodInfo getProp7 = typeof (DateTimeOffset).FindProperty("UtcDateTime").FindGetProp();
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.DateTime)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call((Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_DateTimeOffsetPI.FindGetProp()), getProp6), getProp7)));
      }
      if (property.PropertyType == typeof (DateTimeOffset?))
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.DateTime)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_DateTimeOffsetPI.FindGetProp())));
      if (property.PropertyType == typeof (DateTimeOffset))
      {
        MethodInfo getProp8 = typeof (DateTimeOffset?).FindProperty("HasValue").FindGetProp();
        MethodInfo getProp9 = typeof (DateTimeOffset?).FindProperty("Value").FindGetProp();
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.DateTime)), (Expression) Expression.IfThen((Expression) Expression.IsTrue((Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_DateTimeOffsetPI.FindGetProp()), getProp8)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_DateTimeOffsetPI.FindGetProp()), getProp9))));
      }
      if (property.PropertyType == typeof (double?))
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.Double)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_DoublePI.FindGetProp())));
      if (property.PropertyType == typeof (double))
      {
        MethodInfo getProp10 = typeof (double?).FindProperty("HasValue").FindGetProp();
        MethodInfo getProp11 = typeof (double?).FindProperty("Value").FindGetProp();
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.Double)), (Expression) Expression.IfThen((Expression) Expression.IsTrue((Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_DoublePI.FindGetProp()), getProp10)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_DoublePI.FindGetProp()), getProp11))));
      }
      if (property.PropertyType == typeof (Guid?))
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.Guid)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_GuidPI.FindGetProp())));
      if (property.PropertyType == typeof (Guid))
      {
        MethodInfo getProp12 = typeof (Guid?).FindProperty("HasValue").FindGetProp();
        MethodInfo getProp13 = typeof (Guid?).FindProperty("Value").FindGetProp();
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.Guid)), (Expression) Expression.IfThen((Expression) Expression.IsTrue((Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_GuidPI.FindGetProp()), getProp12)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_GuidPI.FindGetProp()), getProp13))));
      }
      if (property.PropertyType == typeof (int?))
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.Int32)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_Int32PI.FindGetProp())));
      if (property.PropertyType == typeof (int))
      {
        MethodInfo getProp14 = typeof (int?).FindProperty("HasValue").FindGetProp();
        MethodInfo getProp15 = typeof (int?).FindProperty("Value").FindGetProp();
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.Int32)), (Expression) Expression.IfThen((Expression) Expression.IsTrue((Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_Int32PI.FindGetProp()), getProp14)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_Int32PI.FindGetProp()), getProp15))));
      }
      if (property.PropertyType == typeof (long?))
        return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.Int64)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_Int64PI.FindGetProp())));
      if (!(property.PropertyType == typeof (long)))
        return (Expression) null;
      MethodInfo getProp16 = typeof (long?).FindProperty("HasValue").FindGetProp();
      MethodInfo getProp17 = typeof (long?).FindProperty("Value").FindGetProp();
      return (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_PropTypeGetter), (Expression) Expression.Constant((object) EdmType.Int64)), (Expression) Expression.IfThen((Expression) Expression.IsTrue((Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_Int64PI.FindGetProp()), getProp16)), (Expression) Expression.Call((Expression) Expression.Convert(instanceParam, type), property.FindSetProp(), (Expression) Expression.Call((Expression) Expression.Call(currentEntityProperty, TableEntity.EntityProperty_Int64PI.FindGetProp()), getProp17))));
    }

    private static EntityProperty GetValueByKeyFromDictionary(
      string key,
      IDictionary<string, EntityProperty> dict,
      OperationContext operationContext)
    {
      EntityProperty keyFromDictionary;
      dict.TryGetValue(key, out keyFromDictionary);
      if (keyFromDictionary == null)
        Logger.LogInformational(operationContext, "Omitting property '{0}' from de-serialization because there is no corresponding entry in the dictionary provided.", (object) key);
      return keyFromDictionary;
    }

    internal static ConcurrentDictionary<Type, Dictionary<string, EdmType>> PropertyResolverCache
    {
      get => TableEntity.propertyResolverCache;
      set => TableEntity.propertyResolverCache = value;
    }

    public static bool DisablePropertyResolverCache
    {
      get => TableEntity.disablePropertyResolverCache;
      set
      {
        if (value)
          TableEntity.propertyResolverCache.Clear();
        TableEntity.disablePropertyResolverCache = value;
      }
    }
  }
}
