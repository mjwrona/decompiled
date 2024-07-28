// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.PropertyContainer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal abstract class PropertyContainer
  {
    private static List<Type> SingleExpandedPropertyTypes = new List<Type>()
    {
      typeof (PropertyContainer.SingleExpandedProperty<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext0<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext1<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext2<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext3<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext4<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext5<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext6<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext7<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext8<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext9<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext10<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext11<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext12<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext13<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext14<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext15<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext16<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext17<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext18<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext19<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext20<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext21<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext22<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext23<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext24<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext25<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext26<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext27<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext28<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext29<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext30<>),
      typeof (PropertyContainer.SingleExpandedPropertyWithNext31<>)
    };
    private static List<Type> CollectionExpandedPropertyTypes = new List<Type>()
    {
      typeof (PropertyContainer.CollectionExpandedProperty<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext0<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext1<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext2<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext3<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext4<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext5<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext6<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext7<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext8<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext9<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext10<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext11<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext12<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext13<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext14<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext15<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext16<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext17<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext18<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext19<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext20<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext21<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext22<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext23<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext24<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext25<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext26<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext27<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext28<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext29<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext30<>),
      typeof (PropertyContainer.CollectionExpandedPropertyWithNext31<>)
    };
    private static List<Type> AutoSelectedNamedPropertyTypes = new List<Type>()
    {
      typeof (PropertyContainer.AutoSelectedNamedProperty<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext0<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext1<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext2<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext3<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext4<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext5<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext6<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext7<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext8<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext9<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext10<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext11<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext12<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext13<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext14<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext15<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext16<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext17<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext18<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext19<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext20<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext21<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext22<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext23<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext24<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext25<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext26<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext27<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext28<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext29<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext30<>),
      typeof (PropertyContainer.AutoSelectedNamedPropertyWithNext31<>)
    };
    private static List<Type> NamedPropertyTypes = new List<Type>()
    {
      typeof (PropertyContainer.NamedProperty<>),
      typeof (PropertyContainer.NamedPropertyWithNext0<>),
      typeof (PropertyContainer.NamedPropertyWithNext1<>),
      typeof (PropertyContainer.NamedPropertyWithNext2<>),
      typeof (PropertyContainer.NamedPropertyWithNext3<>),
      typeof (PropertyContainer.NamedPropertyWithNext4<>),
      typeof (PropertyContainer.NamedPropertyWithNext5<>),
      typeof (PropertyContainer.NamedPropertyWithNext6<>),
      typeof (PropertyContainer.NamedPropertyWithNext7<>),
      typeof (PropertyContainer.NamedPropertyWithNext8<>),
      typeof (PropertyContainer.NamedPropertyWithNext9<>),
      typeof (PropertyContainer.NamedPropertyWithNext10<>),
      typeof (PropertyContainer.NamedPropertyWithNext11<>),
      typeof (PropertyContainer.NamedPropertyWithNext12<>),
      typeof (PropertyContainer.NamedPropertyWithNext13<>),
      typeof (PropertyContainer.NamedPropertyWithNext14<>),
      typeof (PropertyContainer.NamedPropertyWithNext15<>),
      typeof (PropertyContainer.NamedPropertyWithNext16<>),
      typeof (PropertyContainer.NamedPropertyWithNext17<>),
      typeof (PropertyContainer.NamedPropertyWithNext18<>),
      typeof (PropertyContainer.NamedPropertyWithNext19<>),
      typeof (PropertyContainer.NamedPropertyWithNext20<>),
      typeof (PropertyContainer.NamedPropertyWithNext21<>),
      typeof (PropertyContainer.NamedPropertyWithNext22<>),
      typeof (PropertyContainer.NamedPropertyWithNext23<>),
      typeof (PropertyContainer.NamedPropertyWithNext24<>),
      typeof (PropertyContainer.NamedPropertyWithNext25<>),
      typeof (PropertyContainer.NamedPropertyWithNext26<>),
      typeof (PropertyContainer.NamedPropertyWithNext27<>),
      typeof (PropertyContainer.NamedPropertyWithNext28<>),
      typeof (PropertyContainer.NamedPropertyWithNext29<>),
      typeof (PropertyContainer.NamedPropertyWithNext30<>),
      typeof (PropertyContainer.NamedPropertyWithNext31<>)
    };

    public Dictionary<string, object> ToDictionary(
      IPropertyMapper propertyMapper,
      bool includeAutoSelected = true)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      this.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      return dictionary;
    }

    public abstract void ToDictionaryCore(
      Dictionary<string, object> dictionary,
      IPropertyMapper propertyMapper,
      bool includeAutoSelected);

    public static Expression CreatePropertyContainer(IList<NamedPropertyExpression> properties)
    {
      Expression propertyContainer = (Expression) null;
      if (properties.Count >= 1)
      {
        NamedPropertyExpression property = properties.First<NamedPropertyExpression>();
        int num1 = properties.Count - 1;
        List<Expression> source = new List<Expression>();
        int num2 = PropertyContainer.SingleExpandedPropertyTypes.Count - 1;
        int num3 = 0;
        for (int parts = num2; parts > 0; --parts)
        {
          int leftSize = PropertyContainer.GetLeftSize(num1 - num3, parts);
          source.Add(PropertyContainer.CreatePropertyContainer((IList<NamedPropertyExpression>) properties.Skip<NamedPropertyExpression>(1 + num3).Take<NamedPropertyExpression>(leftSize).ToList<NamedPropertyExpression>()));
          num3 += leftSize;
        }
        propertyContainer = PropertyContainer.CreateNamedPropertyCreationExpression(property, (IList<Expression>) source.Where<Expression>((Func<Expression, bool>) (e => e != null)).ToList<Expression>());
      }
      return propertyContainer;
    }

    private static int GetLeftSize(int count, int parts) => count % parts != 0 ? count / parts + 1 : count / parts;

    private static Expression CreateNamedPropertyCreationExpression(
      NamedPropertyExpression property,
      IList<Expression> expressions)
    {
      Type namedPropertyType = PropertyContainer.GetNamedPropertyType(property, expressions);
      List<MemberBinding> bindings = new List<MemberBinding>();
      bindings.Add((MemberBinding) Expression.Bind((MemberInfo) namedPropertyType.GetProperty("Name"), property.Name));
      bool? countOption;
      if (!property.PageSize.HasValue)
      {
        countOption = property.CountOption;
        if (!countOption.HasValue)
        {
          bindings.Add((MemberBinding) Expression.Bind((MemberInfo) namedPropertyType.GetProperty("Value"), property.Value));
          goto label_8;
        }
      }
      bindings.Add((MemberBinding) Expression.Bind((MemberInfo) namedPropertyType.GetProperty("Collection"), property.Value));
      if (property.PageSize.HasValue)
        bindings.Add((MemberBinding) Expression.Bind((MemberInfo) namedPropertyType.GetProperty("PageSize"), (Expression) Expression.Constant((object) property.PageSize)));
      countOption = property.CountOption;
      if (countOption.HasValue)
      {
        countOption = property.CountOption;
        if (countOption.Value)
          bindings.Add((MemberBinding) Expression.Bind((MemberInfo) namedPropertyType.GetProperty("TotalCount"), ExpressionHelpers.ToNullable(property.TotalCount)));
      }
label_8:
      for (int index = 0; index < expressions.Count; ++index)
        bindings.Add((MemberBinding) Expression.Bind((MemberInfo) namedPropertyType.GetProperty("Next" + index.ToString((IFormatProvider) CultureInfo.CurrentCulture)), expressions[index]));
      if (property.NullCheck != null)
        bindings.Add((MemberBinding) Expression.Bind((MemberInfo) namedPropertyType.GetProperty("IsNull"), property.NullCheck));
      return (Expression) Expression.MemberInit(Expression.New(namedPropertyType), (IEnumerable<MemberBinding>) bindings);
    }

    private static Type GetNamedPropertyType(
      NamedPropertyExpression property,
      IList<Expression> expressions)
    {
      Type type1;
      int? pageSize;
      bool? countOption;
      if (property.NullCheck != null)
      {
        type1 = PropertyContainer.SingleExpandedPropertyTypes[expressions.Count];
      }
      else
      {
        pageSize = property.PageSize;
        if (!pageSize.HasValue)
        {
          countOption = property.CountOption;
          if (!countOption.HasValue)
          {
            type1 = !property.AutoSelected ? PropertyContainer.NamedPropertyTypes[expressions.Count] : PropertyContainer.AutoSelectedNamedPropertyTypes[expressions.Count];
            goto label_6;
          }
        }
        type1 = PropertyContainer.CollectionExpandedPropertyTypes[expressions.Count];
      }
label_6:
      pageSize = property.PageSize;
      Type type2;
      if (!pageSize.HasValue)
      {
        countOption = property.CountOption;
        if (!countOption.HasValue)
        {
          type2 = property.Value.Type;
          goto label_10;
        }
      }
      type2 = TypeHelper.GetInnerElementType(property.Value.Type);
label_10:
      Type type3 = type2;
      return type1.MakeGenericType(type3);
    }

    private class SingleExpandedPropertyWithNext0<T> : PropertyContainer.SingleExpandedProperty<T>
    {
      public PropertyContainer Next0 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next0.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext1<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext0<T>
    {
      public PropertyContainer Next1 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next1.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext2<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext1<T>
    {
      public PropertyContainer Next2 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next2.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext3<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext2<T>
    {
      public PropertyContainer Next3 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next3.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext4<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext3<T>
    {
      public PropertyContainer Next4 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next4.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext5<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext4<T>
    {
      public PropertyContainer Next5 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next5.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext6<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext5<T>
    {
      public PropertyContainer Next6 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next6.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext7<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext6<T>
    {
      public PropertyContainer Next7 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next7.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext8<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext7<T>
    {
      public PropertyContainer Next8 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next8.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext9<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext8<T>
    {
      public PropertyContainer Next9 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next9.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext10<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext9<T>
    {
      public PropertyContainer Next10 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next10.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext11<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext10<T>
    {
      public PropertyContainer Next11 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next11.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext12<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext11<T>
    {
      public PropertyContainer Next12 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next12.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext13<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext12<T>
    {
      public PropertyContainer Next13 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next13.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext14<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext13<T>
    {
      public PropertyContainer Next14 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next14.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext15<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext14<T>
    {
      public PropertyContainer Next15 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next15.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext16<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext15<T>
    {
      public PropertyContainer Next16 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next16.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext17<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext16<T>
    {
      public PropertyContainer Next17 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next17.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext18<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext17<T>
    {
      public PropertyContainer Next18 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next18.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext19<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext18<T>
    {
      public PropertyContainer Next19 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next19.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext20<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext19<T>
    {
      public PropertyContainer Next20 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next20.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext21<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext20<T>
    {
      public PropertyContainer Next21 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next21.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext22<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext21<T>
    {
      public PropertyContainer Next22 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next22.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext23<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext22<T>
    {
      public PropertyContainer Next23 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next23.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext24<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext23<T>
    {
      public PropertyContainer Next24 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next24.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext25<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext24<T>
    {
      public PropertyContainer Next25 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next25.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext26<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext25<T>
    {
      public PropertyContainer Next26 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next26.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext27<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext26<T>
    {
      public PropertyContainer Next27 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next27.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext28<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext27<T>
    {
      public PropertyContainer Next28 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next28.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext29<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext28<T>
    {
      public PropertyContainer Next29 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next29.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext30<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext29<T>
    {
      public PropertyContainer Next30 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next30.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class SingleExpandedPropertyWithNext31<T> : 
      PropertyContainer.SingleExpandedPropertyWithNext30<T>
    {
      public PropertyContainer Next31 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next31.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext0<T> : 
      PropertyContainer.CollectionExpandedProperty<T>
    {
      public PropertyContainer Next0 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next0.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext1<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext0<T>
    {
      public PropertyContainer Next1 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next1.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext2<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext1<T>
    {
      public PropertyContainer Next2 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next2.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext3<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext2<T>
    {
      public PropertyContainer Next3 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next3.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext4<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext3<T>
    {
      public PropertyContainer Next4 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next4.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext5<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext4<T>
    {
      public PropertyContainer Next5 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next5.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext6<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext5<T>
    {
      public PropertyContainer Next6 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next6.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext7<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext6<T>
    {
      public PropertyContainer Next7 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next7.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext8<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext7<T>
    {
      public PropertyContainer Next8 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next8.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext9<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext8<T>
    {
      public PropertyContainer Next9 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next9.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext10<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext9<T>
    {
      public PropertyContainer Next10 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next10.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext11<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext10<T>
    {
      public PropertyContainer Next11 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next11.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext12<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext11<T>
    {
      public PropertyContainer Next12 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next12.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext13<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext12<T>
    {
      public PropertyContainer Next13 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next13.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext14<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext13<T>
    {
      public PropertyContainer Next14 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next14.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext15<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext14<T>
    {
      public PropertyContainer Next15 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next15.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext16<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext15<T>
    {
      public PropertyContainer Next16 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next16.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext17<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext16<T>
    {
      public PropertyContainer Next17 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next17.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext18<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext17<T>
    {
      public PropertyContainer Next18 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next18.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext19<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext18<T>
    {
      public PropertyContainer Next19 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next19.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext20<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext19<T>
    {
      public PropertyContainer Next20 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next20.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext21<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext20<T>
    {
      public PropertyContainer Next21 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next21.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext22<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext21<T>
    {
      public PropertyContainer Next22 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next22.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext23<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext22<T>
    {
      public PropertyContainer Next23 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next23.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext24<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext23<T>
    {
      public PropertyContainer Next24 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next24.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext25<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext24<T>
    {
      public PropertyContainer Next25 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next25.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext26<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext25<T>
    {
      public PropertyContainer Next26 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next26.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext27<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext26<T>
    {
      public PropertyContainer Next27 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next27.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext28<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext27<T>
    {
      public PropertyContainer Next28 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next28.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext29<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext28<T>
    {
      public PropertyContainer Next29 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next29.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext30<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext29<T>
    {
      public PropertyContainer Next30 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next30.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class CollectionExpandedPropertyWithNext31<T> : 
      PropertyContainer.CollectionExpandedPropertyWithNext30<T>
    {
      public PropertyContainer Next31 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next31.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext0<T> : 
      PropertyContainer.AutoSelectedNamedProperty<T>
    {
      public PropertyContainer Next0 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next0.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext1<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext0<T>
    {
      public PropertyContainer Next1 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next1.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext2<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext1<T>
    {
      public PropertyContainer Next2 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next2.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext3<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext2<T>
    {
      public PropertyContainer Next3 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next3.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext4<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext3<T>
    {
      public PropertyContainer Next4 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next4.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext5<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext4<T>
    {
      public PropertyContainer Next5 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next5.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext6<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext5<T>
    {
      public PropertyContainer Next6 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next6.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext7<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext6<T>
    {
      public PropertyContainer Next7 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next7.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext8<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext7<T>
    {
      public PropertyContainer Next8 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next8.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext9<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext8<T>
    {
      public PropertyContainer Next9 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next9.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext10<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext9<T>
    {
      public PropertyContainer Next10 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next10.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext11<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext10<T>
    {
      public PropertyContainer Next11 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next11.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext12<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext11<T>
    {
      public PropertyContainer Next12 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next12.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext13<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext12<T>
    {
      public PropertyContainer Next13 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next13.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext14<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext13<T>
    {
      public PropertyContainer Next14 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next14.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext15<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext14<T>
    {
      public PropertyContainer Next15 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next15.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext16<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext15<T>
    {
      public PropertyContainer Next16 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next16.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext17<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext16<T>
    {
      public PropertyContainer Next17 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next17.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext18<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext17<T>
    {
      public PropertyContainer Next18 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next18.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext19<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext18<T>
    {
      public PropertyContainer Next19 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next19.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext20<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext19<T>
    {
      public PropertyContainer Next20 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next20.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext21<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext20<T>
    {
      public PropertyContainer Next21 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next21.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext22<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext21<T>
    {
      public PropertyContainer Next22 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next22.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext23<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext22<T>
    {
      public PropertyContainer Next23 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next23.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext24<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext23<T>
    {
      public PropertyContainer Next24 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next24.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext25<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext24<T>
    {
      public PropertyContainer Next25 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next25.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext26<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext25<T>
    {
      public PropertyContainer Next26 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next26.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext27<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext26<T>
    {
      public PropertyContainer Next27 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next27.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext28<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext27<T>
    {
      public PropertyContainer Next28 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next28.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext29<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext28<T>
    {
      public PropertyContainer Next29 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next29.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext30<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext29<T>
    {
      public PropertyContainer Next30 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next30.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class AutoSelectedNamedPropertyWithNext31<T> : 
      PropertyContainer.AutoSelectedNamedPropertyWithNext30<T>
    {
      public PropertyContainer Next31 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next31.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext0<T> : PropertyContainer.NamedProperty<T>
    {
      public PropertyContainer Next0 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next0.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext1<T> : PropertyContainer.NamedPropertyWithNext0<T>
    {
      public PropertyContainer Next1 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next1.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext2<T> : PropertyContainer.NamedPropertyWithNext1<T>
    {
      public PropertyContainer Next2 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next2.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext3<T> : PropertyContainer.NamedPropertyWithNext2<T>
    {
      public PropertyContainer Next3 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next3.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext4<T> : PropertyContainer.NamedPropertyWithNext3<T>
    {
      public PropertyContainer Next4 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next4.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext5<T> : PropertyContainer.NamedPropertyWithNext4<T>
    {
      public PropertyContainer Next5 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next5.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext6<T> : PropertyContainer.NamedPropertyWithNext5<T>
    {
      public PropertyContainer Next6 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next6.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext7<T> : PropertyContainer.NamedPropertyWithNext6<T>
    {
      public PropertyContainer Next7 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next7.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext8<T> : PropertyContainer.NamedPropertyWithNext7<T>
    {
      public PropertyContainer Next8 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next8.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext9<T> : PropertyContainer.NamedPropertyWithNext8<T>
    {
      public PropertyContainer Next9 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next9.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext10<T> : PropertyContainer.NamedPropertyWithNext9<T>
    {
      public PropertyContainer Next10 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next10.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext11<T> : PropertyContainer.NamedPropertyWithNext10<T>
    {
      public PropertyContainer Next11 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next11.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext12<T> : PropertyContainer.NamedPropertyWithNext11<T>
    {
      public PropertyContainer Next12 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next12.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext13<T> : PropertyContainer.NamedPropertyWithNext12<T>
    {
      public PropertyContainer Next13 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next13.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext14<T> : PropertyContainer.NamedPropertyWithNext13<T>
    {
      public PropertyContainer Next14 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next14.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext15<T> : PropertyContainer.NamedPropertyWithNext14<T>
    {
      public PropertyContainer Next15 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next15.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext16<T> : PropertyContainer.NamedPropertyWithNext15<T>
    {
      public PropertyContainer Next16 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next16.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext17<T> : PropertyContainer.NamedPropertyWithNext16<T>
    {
      public PropertyContainer Next17 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next17.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext18<T> : PropertyContainer.NamedPropertyWithNext17<T>
    {
      public PropertyContainer Next18 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next18.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext19<T> : PropertyContainer.NamedPropertyWithNext18<T>
    {
      public PropertyContainer Next19 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next19.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext20<T> : PropertyContainer.NamedPropertyWithNext19<T>
    {
      public PropertyContainer Next20 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next20.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext21<T> : PropertyContainer.NamedPropertyWithNext20<T>
    {
      public PropertyContainer Next21 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next21.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext22<T> : PropertyContainer.NamedPropertyWithNext21<T>
    {
      public PropertyContainer Next22 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next22.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext23<T> : PropertyContainer.NamedPropertyWithNext22<T>
    {
      public PropertyContainer Next23 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next23.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext24<T> : PropertyContainer.NamedPropertyWithNext23<T>
    {
      public PropertyContainer Next24 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next24.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext25<T> : PropertyContainer.NamedPropertyWithNext24<T>
    {
      public PropertyContainer Next25 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next25.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext26<T> : PropertyContainer.NamedPropertyWithNext25<T>
    {
      public PropertyContainer Next26 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next26.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext27<T> : PropertyContainer.NamedPropertyWithNext26<T>
    {
      public PropertyContainer Next27 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next27.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext28<T> : PropertyContainer.NamedPropertyWithNext27<T>
    {
      public PropertyContainer Next28 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next28.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext29<T> : PropertyContainer.NamedPropertyWithNext28<T>
    {
      public PropertyContainer Next29 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next29.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext30<T> : PropertyContainer.NamedPropertyWithNext29<T>
    {
      public PropertyContainer Next30 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next30.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    private class NamedPropertyWithNext31<T> : PropertyContainer.NamedPropertyWithNext30<T>
    {
      public PropertyContainer Next31 { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
        this.Next31.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      }
    }

    internal class NamedProperty<T> : PropertyContainer
    {
      public string Name { get; set; }

      public T Value { get; set; }

      public bool AutoSelected { get; set; }

      public override void ToDictionaryCore(
        Dictionary<string, object> dictionary,
        IPropertyMapper propertyMapper,
        bool includeAutoSelected)
      {
        if (this.Name == null || !includeAutoSelected && this.AutoSelected)
          return;
        string key = propertyMapper.MapProperty(this.Name);
        if (string.IsNullOrEmpty(key))
          throw Error.InvalidOperation(SRResources.InvalidPropertyMapping, (object) this.Name);
        dictionary.Add(key, this.GetValue());
      }

      public virtual object GetValue() => (object) this.Value;
    }

    private class AutoSelectedNamedProperty<T> : PropertyContainer.NamedProperty<T>
    {
      public AutoSelectedNamedProperty() => this.AutoSelected = true;
    }

    private class SingleExpandedProperty<T> : PropertyContainer.NamedProperty<T>
    {
      public bool IsNull { get; set; }

      public override object GetValue() => !this.IsNull ? (object) this.Value : (object) null;
    }

    private class CollectionExpandedProperty<T> : PropertyContainer.NamedProperty<T>
    {
      public int PageSize { get; set; }

      public long? TotalCount { get; set; }

      public IEnumerable<T> Collection { get; set; }

      public override object GetValue() => !this.TotalCount.HasValue ? (object) new TruncatedCollection<T>(this.Collection, this.PageSize) : (object) new TruncatedCollection<T>(this.Collection, this.PageSize, this.TotalCount);
    }
  }
}
