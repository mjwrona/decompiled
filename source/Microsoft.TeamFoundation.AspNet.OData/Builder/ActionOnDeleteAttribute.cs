// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.ActionOnDeleteAttribute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.AspNet.OData.Builder
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  public sealed class ActionOnDeleteAttribute : Attribute
  {
    public ActionOnDeleteAttribute(EdmOnDeleteAction onDeleteAction) => this.OnDeleteAction = onDeleteAction;

    public EdmOnDeleteAction OnDeleteAction { get; private set; }
  }
}
