// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandSetAnnotationProperty
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandSetAnnotationProperty : VersionControlCommand
  {
    public CommandSetAnnotationProperty(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(
      string targetServerItem,
      int version,
      string annotationName,
      string annotationValue,
      string comment,
      DateTime? lastModifiedDateTime)
    {
      List<Item> objList = new List<Item>();
      int changesetId = version == 0 ? this.m_versionControlRequestContext.VersionControlService.GetLatestChangeset(this.m_versionControlRequestContext) : version;
      using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
      {
        ResultCollection resultCollection = versionedItemComponent.QueryItems(ItemPathPair.FromServerItem(targetServerItem), changesetId, RecursionType.None, DeletedState.NonDeleted, ItemType.Any, 0);
        resultCollection.NextResult();
        objList = resultCollection.GetCurrent<Item>().Items;
      }
      if (objList.Count == 0)
        return;
      Item obj = objList[0];
      if (version == 0)
        obj.ChangesetId = 0;
      PropertyValue[] propertyValueArray = CommandSetAnnotationProperty.AnnotationPropertyValues(annotationName, annotationValue, comment, lastModifiedDateTime);
      this.PropertyService.SetProperties(this.RequestContext, new ArtifactSpec(VersionControlPropertyKinds.Annotation, obj.ItemId, obj.ChangesetId, obj.ItemDataspaceId), (IEnumerable<PropertyValue>) propertyValueArray);
    }

    public override void ContinueExecution()
    {
    }

    private ITeamFoundationPropertyService PropertyService => this.m_versionControlRequestContext.VersionControlService.GetPropertyService(this.m_versionControlRequestContext);

    private static PropertyValue[] AnnotationPropertyValues(
      string annotationName,
      string annotationValue,
      string comment,
      DateTime? lastModifiedDate)
    {
      return new PropertyValue[3]
      {
        new PropertyValue(annotationName + "." + "Annotation", (object) annotationValue),
        new PropertyValue(annotationName + "." + "AnnotationComment", (object) comment),
        new PropertyValue(annotationName + "." + "AnnotationLastModifiedDate", (object) lastModifiedDate)
      };
    }

    protected override void Dispose(bool disposing)
    {
    }
  }
}
