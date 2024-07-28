// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemXmlConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct WorkItemXmlConstants
  {
    public const string WorkitemElement = "WorkItem";
    public const string ClassificationElement = "Classification";
    public const string RevisionFieldsElement = "RevisionFields";
    public const string DescriptionElement = "Description";
    public const string DiscussionElement = "Discussion";
    public const string ReferencesElement = "References";
    public const string RelatedLinksElement = "RelatedLinks";
    public const string RelatedIdElement = "RelatedID";
    public const string RelatedElement = "Related";
    public const string HyperLinksElement = "HyperLinks";
    public const string HyperLinkElement = "Hyperlink";
    public const string AttachmentsElement = "Attachments";
    public const string AttachmentElement = "Attachment";
    public const string ExternalLinksElement = "ExternalLinks";
    public const string ExternalLinkElement = "ExternalLink";
    public const string CommentElement = "Comment";
    public const string ErrorElement = "Error";
    public const string MessageElement = "Message";
    public const string NameElement = "Name";
    public const string IdElement = "ID";
    public const string UrlElement = "Url";
    public const string UriElement = "Uri";
    public const string ArtifactElement = "Artifact";
    public const string ChangedByElement = "ChangedBy";
    public const string AddedDateElement = "AddedDate";
    public const string TextElement = "Text";
    public const string EntryElement = "Entry";
    public const string ValueElement = "Value";
    public const string LongTextElement = "FieldX";
    public const string TimeZoneAttribute = "TimeZone";
    public const string OffsetAttribute = "Offset";
    public const string ViewColumnID = "ID";
    public const string ViewColumnRelatedID = "RelatedID";
    public const string ViewColumnChangedBy = "ChangedBy";
    public const string ViewColumnComment = "Comment";
    public const string ViewColumnLinkType = "LinkType";
    public const string ViewColumnAddedDate = "AddedDate";
    public const string ViewColumnWords = "Words";
    public const string ViewColumnUrl = "Url";
    public const string ViewColumnUri = "Uri";
    public const string ViewColumnArtifact = "ArtifactName";
  }
}
