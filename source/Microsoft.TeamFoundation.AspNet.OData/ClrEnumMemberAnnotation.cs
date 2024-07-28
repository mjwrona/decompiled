// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ClrEnumMemberAnnotation
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData
{
  public class ClrEnumMemberAnnotation
  {
    private IDictionary<Enum, IEdmEnumMember> _map;
    private IDictionary<IEdmEnumMember, Enum> _reverseMap;

    public ClrEnumMemberAnnotation(IDictionary<Enum, IEdmEnumMember> map)
    {
      this._map = map != null ? map : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (map));
      this._reverseMap = (IDictionary<IEdmEnumMember, Enum>) new Dictionary<IEdmEnumMember, Enum>();
      foreach (KeyValuePair<Enum, IEdmEnumMember> keyValuePair in (IEnumerable<KeyValuePair<Enum, IEdmEnumMember>>) map)
        this._reverseMap.Add(keyValuePair.Value, keyValuePair.Key);
    }

    public IEdmEnumMember GetEdmEnumMember(Enum clrEnumMemberInfo)
    {
      IEdmEnumMember edmEnumMember;
      this._map.TryGetValue(clrEnumMemberInfo, out edmEnumMember);
      return edmEnumMember;
    }

    public Enum GetClrEnumMember(IEdmEnumMember edmEnumMember)
    {
      Enum clrEnumMember;
      this._reverseMap.TryGetValue(edmEnumMember, out clrEnumMember);
      return clrEnumMember;
    }
  }
}
