// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandGetAnnotationProperty
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandGetAnnotationProperty : VersionControlCommand
  {
    private List<Annotation> m_result = new List<Annotation>();

    internal CommandGetAnnotationProperty(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    internal void Execute(string annotationName, string targetServerItem, int version) => this.Execute(annotationName, targetServerItem, version, RecursionType.None);

    internal void Execute(
      string annotationName,
      string targetServerItem,
      int version,
      RecursionType recursionType)
    {
      if (targetServerItem == null)
      {
        targetServerItem = "$/";
        recursionType = RecursionType.OneLevel;
      }
      int changesetId = version == 0 ? this.m_versionControlRequestContext.VersionControlService.GetLatestChangeset(this.m_versionControlRequestContext) : version;
      StreamingCollection<Item> streamingCollection = new StreamingCollection<Item>()
      {
        HandleExceptions = false
      };
      using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
      {
        ResultCollection resultCollection = versionedItemComponent.QueryItems(ItemPathPair.FromServerItem(targetServerItem), changesetId, recursionType, DeletedState.NonDeleted, ItemType.Any, 0);
        resultCollection.NextResult();
        foreach (Item obj in resultCollection.GetCurrent<Item>().Items)
        {
          if (version == 0)
            obj.QueryUnversionedProperties = true;
          streamingCollection.Add((object) obj);
        }
      }
      string str1 = annotationName + "." + "Annotation";
      string str2 = annotationName + "." + "AnnotationComment";
      string str3 = annotationName + "." + "AnnotationLastModifiedDate";
      PropertyMerger<Item> propertyMerger = new PropertyMerger<Item>(this.m_versionControlRequestContext, new string[3]
      {
        str1,
        str2,
        str3
      }, (VersionControlCommand) null, VersionControlPropertyKinds.Annotation);
      propertyMerger.Execute(streamingCollection);
      propertyMerger.TryMergeNextPage();
      foreach (Item obj in streamingCollection)
      {
        if (obj.Attributes != null)
        {
          string annotationValue = string.Empty;
          string empty = string.Empty;
          DateTime lastModifiedDate = new DateTime();
          foreach (PropertyValue attribute in obj.Attributes)
          {
            if (attribute.PropertyName.Equals(str1, StringComparison.Ordinal))
              annotationValue = (string) attribute.Value;
            else if (attribute.PropertyName.Equals(str2, StringComparison.Ordinal))
              empty = (string) attribute.Value;
            else if (attribute.PropertyName.Equals(str3, StringComparison.Ordinal))
              lastModifiedDate = (DateTime) attribute.Value;
          }
          if (this.m_versionControlRequestContext.VersionControlService.GetAllowAsynchronousCheckoutInServerWorkspaces(this.m_versionControlRequestContext) && (annotationName.Equals("GetLatestOnCheckout", StringComparison.OrdinalIgnoreCase) || annotationName.Equals("ExclusiveCheckout", StringComparison.OrdinalIgnoreCase)))
            annotationValue = bool.FalseString;
          this.Result.Add(new Annotation(obj.ServerItem, annotationName, annotationValue, empty, lastModifiedDate));
        }
      }
    }

    public override void ContinueExecution()
    {
    }

    internal List<Annotation> Result => this.m_result;

    protected override void Dispose(bool disposing)
    {
    }
  }
}
