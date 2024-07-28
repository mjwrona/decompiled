// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.ItemAssociationResult
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Microsoft.VisualStudio.Services.ItemStore.Common.Telemetry;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class ItemAssociationResult
  {
    public AssociationsStatus AssociationStatus { get; set; }

    public ItemUploaderRecord ItemUploaderRecord { get; set; }
  }
}
