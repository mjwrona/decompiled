// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models.Rules
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models
{
  [DataContract]
  public class Rules
  {
    public Rules(IVssRequestContext requestContext, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType)
    {
      AdditionalWorkItemTypeProperties additionalProperties = workItemType.GetAdditionalProperties(requestContext);
      this.Transitions = additionalProperties.Transitions;
      this.TriggerList = additionalProperties.FieldRules.Select<WorkItemFieldRule, Guid>((Func<WorkItemFieldRule, Guid>) (fr => fr.Id)).ToArray<Guid>();
      this.StateColors = Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models.Rules.GetStateColors(requestContext, workItemType);
      this.FieldHelpTexts = (IDictionary<string, string>) workItemType.GetAdditionalProperties(requestContext).FieldHelpTexts.ToDictionary<KeyValuePair<int, string>, string, string>((Func<KeyValuePair<int, string>, string>) (k => k.Key.ToString()), (Func<KeyValuePair<int, string>, string>) (v => v.Value));
      this.Form = workItemType.GetFormLayout(requestContext);
    }

    [DataMember]
    public IDictionary<string, string> FieldHelpTexts { get; set; }

    [DataMember]
    public Layout Form { get; set; }

    [DataMember]
    public IDictionary<string, HashSet<string>> Transitions { get; set; }

    [DataMember]
    public IEnumerable<WorkItemFieldRule> FieldRules { get; set; }

    [DataMember]
    public Guid[] TriggerList { get; set; }

    [DataMember]
    public IEnumerable<WorkItemStateColor> StateColors { get; set; }

    private static IEnumerable<WorkItemStateColor> GetStateColors(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType)
    {
      return (IEnumerable<WorkItemStateColor>) requestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemStateColors(requestContext, workItemType.ProjectId, workItemType.Name);
    }
  }
}
