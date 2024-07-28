// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemCommentVersionModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class WorkItemCommentVersionModel : BaseSecuredObjectModel
  {
    private WorkItemCommentVersionModel(
      WorkItemCommentVersionRecord commentVersion,
      IdentityReference createdBy,
      IdentityReference createdOnBehalfOf,
      IdentityReference modifiedBy,
      ISecuredObject securedObject)
      : base(securedObject)
    {
      this.CommentId = commentVersion.CommentId;
      this.WorkItemId = commentVersion.WorkItemId;
      this.Version = commentVersion.Version;
      this.Format = commentVersion.Format;
      this.Text = commentVersion.Text;
      this.RenderedText = commentVersion.RenderedText;
      this.OriginalFormat = commentVersion.OriginalFormat;
      this.OriginalText = commentVersion.OriginalText;
      this.OriginalRenderedText = commentVersion.OriginalRenderedText;
      this.CreatedBy = this.CreatedBy;
      this.CreatedDate = commentVersion.CreatedDate;
      this.CreatedOnBehalfOf = createdOnBehalfOf;
      this.CreatedOnBehalfDate = new DateTime?(commentVersion.CreatedOnBehalfDate);
      this.ModifiedBy = modifiedBy;
      this.ModifiedDate = commentVersion.ModifiedDate;
      this.IsDeleted = commentVersion.IsDeleted;
    }

    private static IdentityReference GetIdentityReference(
      IVssRequestContext requestContext,
      ISecuredObject securedObject,
      Guid identityId,
      IDictionary<Guid, IdentityReference> identityRefsById)
    {
      IdentityReference identityReference;
      if (!identityRefsById.TryGetValue(identityId, out identityReference))
      {
        string witIdentityName = InternalsResourceStrings.Get("UnknownUser");
        ConstantIdentityRef constantIdentityRef = new ConstantIdentityRef(securedObject);
        constantIdentityRef.Id = identityId.ToString();
        constantIdentityRef.DisplayName = witIdentityName;
        identityReference = new IdentityReference((IdentityRef) constantIdentityRef, witIdentityName);
      }
      return identityReference;
    }

    private static IdentityReference GetIdentityReference(
      IVssRequestContext requestContext,
      ISecuredObject securedObject,
      string identity,
      IDictionary<Guid, IdentityReference> identityRefsById)
    {
      Guid result;
      if (Guid.TryParse(identity, out result))
        return WorkItemCommentVersionModel.GetIdentityReference(requestContext, securedObject, result, identityRefsById);
      ConstantIdentityRef constantIdentityRef = new ConstantIdentityRef(securedObject);
      constantIdentityRef.Id = Guid.Empty.ToString();
      constantIdentityRef.DisplayName = identity;
      return new IdentityReference((IdentityRef) constantIdentityRef, identity);
    }

    [DataMember]
    public int CommentId { get; private set; }

    [DataMember]
    public int WorkItemId { get; private set; }

    [DataMember]
    public int Version { get; private set; }

    [DataMember]
    public byte Format { get; private set; }

    [DataMember]
    public string Text { get; private set; }

    [DataMember]
    public string RenderedText { get; private set; }

    [DataMember]
    public byte OriginalFormat { get; private set; }

    [DataMember]
    public string OriginalText { get; private set; }

    [DataMember]
    public string OriginalRenderedText { get; private set; }

    [DataMember]
    public IdentityReference CreatedBy { get; private set; }

    [DataMember]
    public DateTime CreatedDate { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityReference CreatedOnBehalfOf { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? CreatedOnBehalfDate { get; private set; }

    [DataMember]
    public IdentityReference ModifiedBy { get; private set; }

    [DataMember]
    public DateTime ModifiedDate { get; private set; }

    [DataMember]
    public bool IsDeleted { get; private set; }

    public static WorkItemCommentVersionModel FromCommentVersionRecord(
      IVssRequestContext requestContext,
      WorkItemCommentVersionRecord commentVersion,
      IDictionary<Guid, IdentityReference> identityRefs,
      ISecuredObject securedObject)
    {
      IdentityReference identityReference1 = WorkItemCommentVersionModel.GetIdentityReference(requestContext, securedObject, commentVersion.CreatedBy, identityRefs);
      IdentityReference identityReference2 = WorkItemCommentVersionModel.GetIdentityReference(requestContext, securedObject, commentVersion.CreatedOnBehalfOf, identityRefs);
      IdentityReference identityReference3 = WorkItemCommentVersionModel.GetIdentityReference(requestContext, securedObject, commentVersion.ModifiedBy, identityRefs);
      return new WorkItemCommentVersionModel(commentVersion, identityReference1, identityReference2, identityReference3, securedObject);
    }
  }
}
