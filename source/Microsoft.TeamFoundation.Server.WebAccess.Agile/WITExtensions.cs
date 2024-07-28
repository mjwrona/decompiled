// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.WITExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public static class WITExtensions
  {
    internal static IDictionary<string, IDictionary<string, ISet<string>>> GetStateTransitions(
      this WebAccessWorkItemService witService,
      IVssRequestContext requestContext,
      string projectName,
      IEnumerable<string> childWorkItems,
      IEnumerable<string> displayStates)
    {
      return WITExtensions.Implementation.Instance.GetStateTransitions(witService, requestContext, projectName, childWorkItems, displayStates);
    }

    internal static IWorkItemType GetWorkItemType(
      this WebAccessWorkItemService witService,
      IVssRequestContext requestContext,
      Project project,
      string workItemTypeName)
    {
      return WITExtensions.Implementation.Instance.GetWorkItemType(witService, requestContext, project, workItemTypeName);
    }

    public class Implementation
    {
      private static WITExtensions.Implementation s_instance;

      protected Implementation()
      {
      }

      public static WITExtensions.Implementation Instance
      {
        get
        {
          if (WITExtensions.Implementation.s_instance == null)
            WITExtensions.Implementation.s_instance = new WITExtensions.Implementation();
          return WITExtensions.Implementation.s_instance;
        }
        internal set => WITExtensions.Implementation.s_instance = value;
      }

      internal virtual IDictionary<string, IDictionary<string, ISet<string>>> GetStateTransitions(
        WebAccessWorkItemService witService,
        IVssRequestContext requestContext,
        string projectName,
        IEnumerable<string> childWorkItems,
        IEnumerable<string> displayStates)
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
        ArgumentUtility.CheckForNull<IEnumerable<string>>(childWorkItems, nameof (childWorkItems));
        ArgumentUtility.CheckForNull<IEnumerable<string>>(displayStates, nameof (displayStates));
        IDictionary<string, IDictionary<string, ISet<string>>> stateTransitions = (IDictionary<string, IDictionary<string, ISet<string>>>) new Dictionary<string, IDictionary<string, ISet<string>>>();
        Project project = witService.GetProject(requestContext, projectName);
        foreach (string childWorkItem in childWorkItems)
        {
          IDictionary<string, HashSet<string>> transitions = witService.GetWorkItemType(requestContext, project, childWorkItem).GetExtendedProperties(requestContext).Transitions;
          Dictionary<string, ISet<string>> dictionary = new Dictionary<string, ISet<string>>(transitions.Count, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
          foreach (string displayState in displayStates)
          {
            HashSet<string> collection;
            if (!dictionary.ContainsKey(displayState) && transitions.TryGetValue(displayState, out collection))
            {
              HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) collection, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
              stringSet.Remove(displayState);
              dictionary.Add(displayState, (ISet<string>) stringSet);
            }
          }
          stateTransitions.Add(childWorkItem, (IDictionary<string, ISet<string>>) dictionary);
        }
        return stateTransitions;
      }

      internal virtual IWorkItemType GetWorkItemType(
        WebAccessWorkItemService witService,
        IVssRequestContext requestContext,
        Project project,
        string workItemTypeName)
      {
        return witService.GetWorkItemTypes(requestContext, project.Guid).FirstOrDefault<IWorkItemType>((Func<IWorkItemType, bool>) (p => TFStringComparer.WorkItemTypeName.Equals(p.Name, workItemTypeName))) ?? throw new LegacyConfigurationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AgileServerResources.TaskBoardController_WorkItemTypeNotFound, (object) workItemTypeName));
      }
    }
  }
}
