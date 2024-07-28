// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Models.ItemActionModel
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Models
{
  [DataContract]
  public class ItemActionModel
  {
    [DataMember]
    public string ActionHelpLink;
    [DataMember]
    public string ActionHelpText;
    [DataMember]
    public string FullyQualifiedName;
    [DataMember]
    public string GalleryUrl;
    [DataMember]
    public string InstallTitle;
    [DataMember]
    public string InstallInstructions;
    [DataMember]
    public string CopyCommand;
    [DataMember]
    public string CopyButtonTooltip;
    [DataMember]
    public string CopyButtonText;
    [DataMember]
    public string CopiedAnimationText;
    [DataMember]
    public string MoreInfoLink;
    [DataMember]
    public string MoreInfoText;
    [DataMember]
    public string UnpublishedText;
    [DataMember]
    public string AcqButtonText;
    [DataMember]
    public string AcqButtonLink;
    [DataMember]
    public bool IsAcqDisabled;
    [DataMember]
    public string SearchTarget;
    [DataMember]
    public bool AcqLinkNewTab;
    [DataMember]
    public string ActionDescriptionHtml;
  }
}
