// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemTypeMetadata
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public abstract class WorkItemTypeMetadata
  {
    private IDictionary<string, WorkItemField> m_fieldsMap;
    private ISet<string> m_states;
    private IDictionary<string, WorkItemTypeAction> m_actions;
    private XmlNode m_displayForm;
    private IDictionary<string, WorkItemTypeAction> m_addedActions;
    private IDictionary<string, ISet<string>> m_transitions;
    private IDictionary<string, XElement> m_addedFieldRefNameToXMLMap;
    private bool m_displayFormDirty;

    public string Name { get; private set; }

    public IEnumerable<WorkItemField> Fields => (IEnumerable<WorkItemField>) this.m_fieldsMap.Values;

    public WorkItemField GetField(string referenceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(referenceName, nameof (referenceName));
      WorkItemField workItemField;
      return this.m_fieldsMap.TryGetValue(referenceName, out workItemField) ? workItemField : (WorkItemField) null;
    }

    public IEnumerable<string> States => (IEnumerable<string>) this.m_states;

    public bool ContainsState(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      return this.m_states.Contains<string>(name, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
    }

    public string InitialState { get; private set; }

    public IEnumerable<WorkItemTypeAction> Actions => (IEnumerable<WorkItemTypeAction>) this.m_actions.Values;

    public WorkItemTypeAction GetAction(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      WorkItemTypeAction workItemTypeAction;
      return this.m_actions.TryGetValue(name, out workItemTypeAction) || this.m_addedActions != null && this.m_addedActions.TryGetValue(name, out workItemTypeAction) ? workItemTypeAction : (WorkItemTypeAction) null;
    }

    public virtual void AddAction(WorkItemTypeAction action)
    {
      ArgumentUtility.CheckForNull<WorkItemTypeAction>(action, nameof (action));
      if (this.GetAction(action.Name) != null)
        throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorActionAlreadyExists, (object) action.Name, (object) this.Name));
      if (!this.ContainsState(action.FromState))
        throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionWarningActionStateMissing, (object) action.Name, (object) this.Name, (object) action.FromState));
      if (!this.ContainsState(action.ToState))
        throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionWarningActionStateMissing, (object) action.Name, (object) this.Name, (object) action.ToState));
      if (!this.TransitionExists(action.FromState, action.ToState))
        throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionWarningWorkItemTypeTransactionMissing, (object) action.Name, (object) this.Name, (object) action.FromState, (object) action.ToState));
      this.m_addedActions[action.Name] = action;
    }

    public virtual void AddField(string workItemTypeRefName, string fieldRefName, XElement field = null)
    {
      if (this.m_addedFieldRefNameToXMLMap.ContainsKey(fieldRefName) || this.GetField(fieldRefName) != null)
        return;
      this.m_addedFieldRefNameToXMLMap.Add(fieldRefName, field);
    }

    public virtual XmlNode DisplayForm
    {
      get => this.m_displayForm;
      set
      {
        ArgumentUtility.CheckForNull<XmlNode>(value, nameof (DisplayForm));
        if (!this.m_displayFormDirty && !value.Equals((object) this.m_displayForm))
          this.m_displayFormDirty = true;
        this.m_displayForm = value;
      }
    }

    internal abstract void Save(IVssRequestContext requestContext, int projectId);

    internal virtual void Validate(
      IVssRequestContext requestContext,
      IProjectProvisioningContext provisioningContext)
    {
    }

    internal bool TransitionExists(string fromState, string toState)
    {
      ISet<string> source;
      return this.m_transitions.TryGetValue(fromState, out source) && source.Contains<string>(toState, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
    }

    protected bool DisplayFormIsDirty => this.m_displayFormDirty;

    protected IEnumerable<WorkItemTypeAction> AddedActions => (IEnumerable<WorkItemTypeAction>) this.m_addedActions.Values;

    protected IDictionary<string, XElement> AddedFieldRefNameToXMLMap => this.m_addedFieldRefNameToXMLMap;

    protected void Init(
      string name,
      IEnumerable<string> states,
      IEnumerable<WorkItemField> fields,
      IEnumerable<WorkItemTypeAction> actions,
      XmlNode displayForm,
      IEnumerable<WorkItemTypeTransition> transitions)
    {
      this.m_states = (ISet<string>) new HashSet<string>(states, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      this.m_fieldsMap = (IDictionary<string, WorkItemField>) new Dictionary<string, WorkItemField>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      this.m_actions = (IDictionary<string, WorkItemTypeAction>) new Dictionary<string, WorkItemTypeAction>((IEqualityComparer<string>) TFStringComparer.WorkItemActionName);
      this.m_addedActions = (IDictionary<string, WorkItemTypeAction>) new Dictionary<string, WorkItemTypeAction>((IEqualityComparer<string>) TFStringComparer.WorkItemActionName);
      this.m_transitions = (IDictionary<string, ISet<string>>) new Dictionary<string, ISet<string>>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      this.m_addedFieldRefNameToXMLMap = (IDictionary<string, XElement>) new Dictionary<string, XElement>();
      foreach (string state in states)
        this.m_states.Add(state);
      foreach (WorkItemField field in fields)
        this.m_fieldsMap[field.ReferenceName] = field;
      if (actions != null)
      {
        foreach (WorkItemTypeAction action in actions)
          this.m_actions[action.Name] = action;
      }
      this.m_displayForm = displayForm;
      this.Name = name;
      foreach (WorkItemTypeTransition transition in transitions)
      {
        if (string.IsNullOrEmpty(transition.FromState))
        {
          this.InitialState = transition.ToState;
        }
        else
        {
          ISet<string> stringSet;
          if (!this.m_transitions.TryGetValue(transition.FromState, out stringSet))
            this.m_transitions[transition.FromState] = stringSet = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
          stringSet.Add(transition.ToState);
        }
      }
    }
  }
}
