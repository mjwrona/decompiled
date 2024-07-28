// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ResourceContext`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;

namespace Microsoft.AspNet.OData
{
  public class ResourceContext<TStructuredType> : ResourceContext
  {
    [Obsolete("Resource instance might not be available when the incoming uri has a $select. Use the EdmObject property instead.")]
    public TStructuredType ResourceInstance
    {
      get => (TStructuredType) base.ResourceInstance;
      set => this.ResourceInstance = (object) value;
    }
  }
}
