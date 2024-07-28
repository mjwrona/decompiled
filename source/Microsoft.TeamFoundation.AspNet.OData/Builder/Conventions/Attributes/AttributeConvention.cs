// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.AttributeConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal abstract class AttributeConvention : IConvention
  {
    private static Dictionary<MemberInfo, ICollection<Attribute>> attributesCache = new Dictionary<MemberInfo, ICollection<Attribute>>();

    protected AttributeConvention(Func<Attribute, bool> attributeFilter, bool allowMultiple)
    {
      if (attributeFilter == null)
        throw Error.ArgumentNull(nameof (attributeFilter));
      this.AllowMultiple = allowMultiple;
      this.AttributeFilter = attributeFilter;
    }

    public Func<Attribute, bool> AttributeFilter { get; private set; }

    public bool AllowMultiple { get; private set; }

    public Attribute[] GetAttributes(MemberInfo member)
    {
      if (member == (MemberInfo) null)
        throw Error.ArgumentNull(nameof (member));
      bool flag = false;
      ICollection<Attribute> list;
      lock (AttributeConvention.attributesCache)
        flag = AttributeConvention.attributesCache.TryGetValue(member, out list);
      if (!flag)
      {
        list = (ICollection<Attribute>) member.GetCustomAttributes(true).OfType<Attribute>().ToList<Attribute>();
        lock (AttributeConvention.attributesCache)
          AttributeConvention.attributesCache[member] = list;
      }
      ICollection<Attribute> array = (ICollection<Attribute>) list.Where<Attribute>(this.AttributeFilter).ToArray<Attribute>();
      if (!this.AllowMultiple && array.Count > 1)
        throw Error.Argument(nameof (member), SRResources.MultipleAttributesFound, (object) member.Name, (object) TypeHelper.GetReflectedType(member).Name, (object) array.First<Attribute>().GetType().Name);
      return array.ToArray<Attribute>();
    }
  }
}
