// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.Contracts.ContainerAttributeMetadata
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi.Contracts
{
  public class ContainerAttributeMetadata
  {
    public ContainerAttributeMetadata()
    {
      this.ContainerPropertiesMap = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.ContainerItemRenameMap = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public Dictionary<string, object> ContainerPropertiesMap { get; set; }

    public Dictionary<string, string> ContainerItemRenameMap { get; set; }
  }
}
