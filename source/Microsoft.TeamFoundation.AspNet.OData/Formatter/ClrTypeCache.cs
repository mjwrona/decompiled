// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.ClrTypeCache
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Concurrent;

namespace Microsoft.AspNet.OData.Formatter
{
  internal class ClrTypeCache
  {
    private ConcurrentDictionary<Type, IEdmTypeReference> _cache = new ConcurrentDictionary<Type, IEdmTypeReference>();

    public IEdmTypeReference GetEdmType(Type clrType, IEdmModel model)
    {
      IEdmTypeReference edmTypeReference;
      if (!this._cache.TryGetValue(clrType, out edmTypeReference))
      {
        edmTypeReference = model.GetEdmTypeReference(clrType);
        this._cache[clrType] = edmTypeReference;
      }
      return edmTypeReference;
    }
  }
}
