// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceTypeMapper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ServiceTypeMapper
  {
    private static readonly ServiceTypeMetadata[] s_serviceTypes = new ServiceTypeMetadata[17]
    {
      new ServiceTypeMetadata(new Guid("951917ac-a960-4999-8464-e3f0aa25b381"), "SPS", "Other services", "SPS", true),
      new ServiceTypeMetadata(new Guid("0000003e-0000-8888-8000-000000000000"), "DataImport", "Data Migration", "DIS", true),
      new ServiceTypeMetadata(new Guid("00025394-6065-48CA-87D9-7F5672854EF7"), "TFS", "Boards, Repos, and Pipelines", "TFS"),
      new ServiceTypeMetadata(new Guid("00000028-0000-8888-8000-000000000000"), "ExtensionManagement", "Other services", "EMS", true),
      new ServiceTypeMetadata(new Guid("00000003-0000-8888-8000-000000000000"), "Service Hooks", "Other services", "SH", true),
      new ServiceTypeMetadata(new Guid("0000000D-0000-8888-8000-000000000000"), "Release Management", "Pipelines", "RM"),
      new ServiceTypeMetadata(new Guid("0000000F-0000-8888-8000-000000000000"), "CodeLens", "Repos", "CodeLens", true),
      new ServiceTypeMetadata(new Guid("00000010-0000-8888-8000-000000000000"), "ALMSearch", "Other services", "ALMSearch", true),
      new ServiceTypeMetadata(new Guid("0000003C-0000-8888-8000-000000000000"), "Analytics", "Analytics", "AX"),
      new ServiceTypeMetadata(new Guid("00000064-0000-8888-8000-000000000000"), "Audit", "Other services", "Audit", true),
      new ServiceTypeMetadata(new Guid("00000016-0000-8888-8000-000000000000"), "Artifact", "Artifacts", "Artifact", true),
      new ServiceTypeMetadata(new Guid("00000019-0000-8888-8000-000000000000"), "Blobstore", "Artifacts (1)", "Blobstore"),
      new ServiceTypeMetadata(new Guid("00000030-0000-8888-8000-000000000000"), "Package", "Artifacts (2)", "pkgs"),
      new ServiceTypeMetadata(new Guid("00000036-0000-8888-8000-000000000000"), "Feeds", "Artifacts (3)", "Feeds"),
      new ServiceTypeMetadata(new Guid("00000001-0000-8888-8000-000000000000"), "SPS", "Core services", "SPS", true),
      new ServiceTypeMetadata(new Guid("00000054-0000-8888-8000-000000000000"), "TCM", "Test Plans", "TCM"),
      new ServiceTypeMetadata(new Guid("00000041-0000-8888-8000-000000000000"), "Aex", "Other services", "Aex", true)
    };

    public static ServiceTypeMetadata GetServiceTypeMetadataByServiceType(Guid serviceType) => ((IEnumerable<ServiceTypeMetadata>) ServiceTypeMapper.s_serviceTypes).FirstOrDefault<ServiceTypeMetadata>((Func<ServiceTypeMetadata, bool>) (s => s.ServiceType == serviceType));

    public static ServiceTypeMetadata GetServiceTypeMetadataByName(string name) => ((IEnumerable<ServiceTypeMetadata>) ServiceTypeMapper.s_serviceTypes).FirstOrDefault<ServiceTypeMetadata>((Func<ServiceTypeMetadata, bool>) (s => s.ShortName.Equals(name, StringComparison.InvariantCultureIgnoreCase) || s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
  }
}
