// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.IDelta
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData
{
  public interface IDelta
  {
    IEnumerable<string> GetChangedPropertyNames();

    IEnumerable<string> GetUnchangedPropertyNames();

    bool TrySetPropertyValue(string name, object value);

    bool TryGetPropertyValue(string name, out object value);

    bool TryGetPropertyType(string name, out Type type);

    void Clear();
  }
}
