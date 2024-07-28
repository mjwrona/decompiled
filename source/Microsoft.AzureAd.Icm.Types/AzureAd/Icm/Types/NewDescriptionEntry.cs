// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.NewDescriptionEntry
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class NewDescriptionEntry
  {
    [DataMember]
    public string ChangedBy { get; set; }

    [DataMember]
    public string SubmittedBy { get; set; }

    [DataMember]
    public DescriptionEntryCause Cause { get; set; }

    [DataMember]
    public DateTime Date { get; set; }

    [DataMember]
    public DateTime SubmitDate { get; set; }

    [DataMember]
    public string Text { get; set; }

    [DataMember]
    public DescriptionTextRenderType RenderType { get; set; }
  }
}
