// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlFileSource
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public sealed class YamlFileSource
  {
    private IDictionary<string, InputValue> sourceData;

    public YamlFileSource(YamlFileSourceTypes type) => this.Type = type;

    [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Its a source type and not object type.")]
    public YamlFileSourceTypes Type { get; private set; }

    public IDictionary<string, InputValue> SourceData
    {
      get
      {
        if (this.sourceData == null)
          this.sourceData = (IDictionary<string, InputValue>) new Dictionary<string, InputValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.sourceData;
      }
    }
  }
}
