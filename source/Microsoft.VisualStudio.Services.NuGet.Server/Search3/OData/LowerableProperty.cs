// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.OData.LowerableProperty
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.Data.Edm;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3.OData
{
  internal class LowerableProperty
  {
    public IEdmProperty Property { get; }

    public bool HasToLower { get; }

    public LowerableProperty(IEdmProperty property, bool hasToLower)
    {
      this.Property = property ?? throw new ArgumentNullException(nameof (property));
      this.HasToLower = hasToLower;
    }
  }
}
