// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileServiceContentValidationComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileServiceContentValidationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<FileServiceContentValidationComponent>(1, true),
      (IComponentCreator) new ComponentCreator<FileServiceContentValidationComponent2>(2),
      (IComponentCreator) new ComponentCreator<FileServiceContentValidationComponent3>(3),
      (IComponentCreator) new ComponentCreator<FileServiceContentValidationComponent4>(4)
    }, "FileContentValidation");
    private static readonly string s_area = "FileContentValidation";

    public FileServiceContentValidationComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual List<FileServiceContentValidationMetadata> QueryUnvalidatedFiles(
      int dataspaceId,
      int fileIdWatermark,
      int batchSize = 1000)
    {
      return new List<FileServiceContentValidationMetadata>();
    }

    public virtual void SaveMetadata(FileServiceContentValidationMetadata entry)
    {
    }

    public virtual void CleanupMetadata(int retentionPeriodInDays = 14)
    {
    }

    public virtual int GetWatermark(int dataspaceId) => 0;

    public virtual void SetWatermark(int dataspaceId, int fileIdWatermark)
    {
    }

    protected override string TraceArea => FileServiceContentValidationComponent.s_area;
  }
}
