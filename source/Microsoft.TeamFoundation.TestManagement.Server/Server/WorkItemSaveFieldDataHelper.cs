// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.WorkItemSaveFieldDataHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class WorkItemSaveFieldDataHelper : IWorkItemSaveFieldDataHelper
  {
    private string m_userDisplayName;
    private WorkItemTrackingFieldService m_fields;

    public Dictionary<int, object> fieldDictionary { get; private set; }

    public IVssRequestContext TfsRequestContext { get; set; }

    public WorkItemSaveFieldDataHelper(
      Dictionary<int, object> fieldDictionary,
      IVssRequestContext requestContext)
    {
      this.fieldDictionary = fieldDictionary;
      this.TfsRequestContext = requestContext;
    }

    private WorkItemTrackingFieldService Fields
    {
      get
      {
        if (this.m_fields == null)
          this.m_fields = this.TfsRequestContext.GetService<WorkItemTrackingFieldService>();
        return this.m_fields;
      }
    }

    string IWorkItemSaveFieldDataHelper.UserDisplayName
    {
      get
      {
        if (this.m_userDisplayName == null)
        {
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          TeamFoundationIdentity foundationIdentity = tfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentity(tfsRequestContext, tfsRequestContext.UserContext, MembershipQuery.None, ReadIdentityOptions.IncludeReadFromSource);
          this.m_userDisplayName = foundationIdentity != null ? foundationIdentity.DisplayName : string.Empty;
        }
        return this.m_userDisplayName;
      }
    }

    bool IWorkItemSaveFieldDataHelper.IsDirty => true;

    bool IWorkItemSaveFieldDataHelper.HasField(int fieldId) => this.Fields.GetFieldById(this.TfsRequestContext, fieldId) != null;

    Dictionary<int, object> IWorkItemSaveFieldDataHelper.FieldUpdates => this.fieldDictionary;

    bool IWorkItemSaveFieldDataHelper.IsLongTextField(int fieldId) => this.Fields.GetFieldById(this.TfsRequestContext, fieldId).IsLongText;

    string IWorkItemSaveFieldDataHelper.GetFieldReferenceName(int fieldId) => this.Fields.GetFieldById(this.TfsRequestContext, fieldId).ReferenceName;

    string IWorkItemSaveFieldDataHelper.GetFieldName(int fieldId) => this.Fields.GetFieldById(this.TfsRequestContext, fieldId).Name;

    Type IWorkItemSaveFieldDataHelper.GetFieldSystemType(int fieldId) => this.Fields.GetFieldById(this.TfsRequestContext, fieldId).SystemType;
  }
}
