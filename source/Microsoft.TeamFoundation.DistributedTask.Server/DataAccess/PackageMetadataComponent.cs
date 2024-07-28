// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.PackageMetadataComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class PackageMetadataComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<PackageMetadataComponent>(1, true),
      (IComponentCreator) new ComponentCreator<PackageMetadataComponent2>(2),
      (IComponentCreator) new ComponentCreator<PackageMetadataComponent3>(3),
      (IComponentCreator) new ComponentCreator<PackageMetadataComponent4>(4)
    }, "DistributedTaskPackage", "DistributedTask");

    public PackageMetadataComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual PackageData AddPackage(
      string packageType,
      string platform,
      PackageVersion version,
      IDictionary<string, string> data)
    {
      return (PackageData) null;
    }

    public virtual void DeletePackage(string packageType, string platform, PackageVersion version)
    {
    }

    public virtual PackageData GetPackage(
      string packageType,
      string platform,
      PackageVersion version)
    {
      return (PackageData) null;
    }

    public virtual List<PackageData> GetPackages(string packageType, string platform, int top) => new List<PackageData>();
  }
}
