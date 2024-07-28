// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Delta
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Microsoft.AspNet.OData
{
  [NonValidatingParameterBinding]
  public abstract class Delta : DynamicObject, IDelta
  {
    public abstract void Clear();

    public abstract bool TrySetPropertyValue(string name, object value);

    public abstract bool TryGetPropertyValue(string name, out object value);

    public abstract bool TryGetPropertyType(string name, out Type type);

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      if (binder == null)
        throw Error.ArgumentNull(nameof (binder));
      return this.TrySetPropertyValue(binder.Name, value);
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result) => binder != null ? this.TryGetPropertyValue(binder.Name, out result) : throw Error.ArgumentNull(nameof (binder));

    public abstract IEnumerable<string> GetChangedPropertyNames();

    public abstract IEnumerable<string> GetUnchangedPropertyNames();
  }
}
