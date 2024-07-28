// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.PackageContainer
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class PackageContainer
  {
    public PackageContainer()
    {
    }

    public PackageContainer(
      Guid containerId,
      string name,
      PackageContainerType type,
      string token = null,
      byte[] securityHashCode = null,
      bool isDeleted = false)
    {
      this.ContainerId = containerId;
      this.Name = name;
      this.Type = type;
      this.Token = token;
      this.SecurityHashCode = securityHashCode;
      this.IsDeleted = isDeleted;
    }

    public Guid ContainerId { get; set; }

    public string Name { get; set; }

    public PackageContainerType Type { get; set; }

    public string Token { get; set; }

    public byte[] SecurityHashCode { get; set; }

    public bool IsDeleted { get; set; }

    public override int GetHashCode() => this.ContainerId.GetHashCode();

    public override string ToString() => JsonConvert.SerializeObject((object) this, Formatting.None, (JsonConverter) new StringEnumConverter());
  }
}
