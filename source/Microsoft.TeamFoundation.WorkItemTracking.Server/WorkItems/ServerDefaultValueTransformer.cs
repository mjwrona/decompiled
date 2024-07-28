// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.ServerDefaultValueTransformer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class ServerDefaultValueTransformer : IServerDefaultValueTransformer
  {
    private IVssRequestContext m_requestContext;
    private string m_currentUserDisplayName;

    public ServerDefaultValueTransformer(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      this.m_requestContext = requestContext;
      this.CurrentIdentity = identity;
    }

    public ServerDefaultValueTransformer(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.m_requestContext = requestContext;
      this.CurrentIdentity = requestContext.GetUserIdentity();
    }

    public Microsoft.VisualStudio.Services.Identity.Identity CurrentIdentity { get; private set; }

    public string CurrentUser
    {
      get
      {
        if (this.m_currentUserDisplayName == null)
        {
          ConstantRecord constantRecord = this.m_requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().GetConstantRecords(this.m_requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            this.CurrentIdentity.Id
          }).FirstOrDefault<ConstantRecord>();
          this.m_currentUserDisplayName = constantRecord == null ? this.CurrentIdentity.GetLegacyDistinctDisplayName() : constantRecord.DisplayText;
          WorkItemIdentity workItemIdentity;
          if (WorkItemIdentityHelper.AddIdentityToWorkItemIdentityMap(this.m_requestContext, this.CurrentIdentity).TryGetValue(this.CurrentIdentity.Id, out workItemIdentity))
          {
            workItemIdentity.DistinctDisplayName = this.m_currentUserDisplayName;
            WorkItemIdentityHelper.AddIdentitiesToDistinctDisplayNameMap(this.m_requestContext, (IEnumerable<WorkItemIdentity>) new WorkItemIdentity[1]
            {
              workItemIdentity
            });
          }
        }
        return this.m_currentUserDisplayName;
      }
    }

    public object TransformValue(object value, InternalFieldType fieldType)
    {
      if (value is ServerDefaultFieldValue)
      {
        switch (((ServerDefaultFieldValue) value).Type)
        {
          case ServerDefaultType.ServerDateTime:
            return (object) DateTime.SpecifyKind(SqlDateTime.MinValue.Value, DateTimeKind.Utc);
          case ServerDefaultType.CallerIdentity:
            return (object) this.CurrentUser;
          case ServerDefaultType.RandomGuid:
            return (object) Guid.NewGuid();
        }
      }
      return value;
    }

    public T TransformValue<T>(object value, InternalFieldType fieldType)
    {
      value = this.TransformValue(value, fieldType);
      return CommonWITUtils.ConvertValue<T>(value);
    }
  }
}
