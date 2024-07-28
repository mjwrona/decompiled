// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceTypeMetadata
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServiceTypeMetadata
  {
    public readonly Guid ServiceType;
    public readonly string Name;
    public readonly string PublicDisplayName;
    public readonly string ShortName;
    public readonly bool DataImportStatusPageIgnore;

    public ServiceTypeMetadata(
      Guid serviceName,
      string name,
      string publicDisplayName,
      string shortName,
      bool dataImportStatusPageIgnore = false)
    {
      this.ServiceType = serviceName;
      this.Name = name;
      this.PublicDisplayName = publicDisplayName;
      this.ShortName = shortName;
      this.DataImportStatusPageIgnore = dataImportStatusPageIgnore;
    }
  }
}
