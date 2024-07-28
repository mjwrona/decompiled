// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.EntityRenameEventData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System.ComponentModel.Composition;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  [Export(typeof (ChangeEventData))]
  public class EntityRenameEventData : ChangeEventData
  {
    private EntityRenameEventData()
    {
    }

    public EntityRenameEventData(ExecutionContext executionContext)
      : base(executionContext)
    {
    }

    [DataMember(Order = 0)]
    public string OldEntityName { get; set; }

    [DataMember(Order = 1)]
    public string NewEntityName { get; set; }

    [DataMember(Order = 2)]
    public string FieldUpdateEventIndexingUnitType { get; set; }
  }
}
