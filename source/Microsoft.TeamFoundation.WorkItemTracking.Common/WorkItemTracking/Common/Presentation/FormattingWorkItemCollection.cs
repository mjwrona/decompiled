// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Presentation.FormattingWorkItemCollection
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Presentation
{
  internal class FormattingWorkItemCollection
  {
    public string ProjectCollectionName { get; private set; }

    public Uri ProjectUri { get; private set; }

    public Guid? QueryId { get; private set; }

    public string QueryName { get; private set; }

    public IList<FormattingWorkItem> WorkItems { get; set; }

    public FormattingWorkItemCollection(
      string projectCollectionName,
      Uri projectUri,
      Guid? queryId,
      string queryName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectCollectionName, nameof (projectCollectionName));
      this.ProjectCollectionName = projectCollectionName;
      this.ProjectUri = projectUri;
      this.QueryId = queryId;
      this.QueryName = queryName;
      this.WorkItems = (IList<FormattingWorkItem>) new List<FormattingWorkItem>();
    }

    public FormattingWorkItemCollection(string projectCollectionName, Uri projectUri)
      : this(projectCollectionName, projectUri, new Guid?(), (string) null)
    {
    }
  }
}
