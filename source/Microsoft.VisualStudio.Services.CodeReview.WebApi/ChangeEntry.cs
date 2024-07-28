// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.ChangeEntry
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [DataContract]
  public class ChangeEntry
  {
    [DataMember(EmitDefaultValue = false)]
    public int? IterationId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? ChangeId { get; set; }

    [DataMember]
    public ChangeEntryFileInfo Base { get; set; }

    [DataMember]
    public ChangeEntryFileInfo Modified { get; set; }

    [DataMember]
    public ChangeType Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ExtendedChangeType { get; set; }

    [DataMember]
    public int ChangeTrackingId { get; set; }

    [DataMember]
    public int TotalChangesCount { get; set; }
  }
}
