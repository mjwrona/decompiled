// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.SelectExpandWrapper`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  [JsonConverter(typeof (SelectExpandWrapperConverter))]
  internal class SelectExpandWrapper<TElement> : SelectExpandWrapper
  {
    public TElement Instance
    {
      get => (TElement) this.UntypedInstance;
      set => this.UntypedInstance = (object) value;
    }

    protected override Type GetElementType() => this.UntypedInstance != null ? this.UntypedInstance.GetType() : typeof (TElement);
  }
}
