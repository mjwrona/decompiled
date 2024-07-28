// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EntityResCategoryAttribute
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.ComponentModel;

namespace Microsoft.OData.Edm
{
  [AttributeUsage(AttributeTargets.All)]
  internal sealed class EntityResCategoryAttribute : CategoryAttribute
  {
    public EntityResCategoryAttribute(string category)
      : base(category)
    {
    }

    protected override string GetLocalizedString(string value) => EntityRes.GetString(value);
  }
}
