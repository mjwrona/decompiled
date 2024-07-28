// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Models.ItemDetailsMoreInfoModel
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Models
{
  [DataContract]
  public class ItemDetailsMoreInfoModel
  {
    [DataMember]
    public string VersionValue;
    [DataMember]
    public string PublisherValue;
    [DataMember]
    public string UniqueIdentifierValue;
    [DataMember]
    public string TwitterShareContents;
    [DataMember]
    public string EmailShareContents;
    [DataMember]
    public string EmailShareSubject;
    [DataMember]
    public bool IsPublic;
  }
}
