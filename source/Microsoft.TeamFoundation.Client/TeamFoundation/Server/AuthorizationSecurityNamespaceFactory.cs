// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.AuthorizationSecurityNamespaceFactory
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server
{
  internal static class AuthorizationSecurityNamespaceFactory
  {
    internal static IEnumerable<SecurityNamespace> GetSecurityNamespaces(
      TfsTeamProjectCollection tfs)
    {
      List<SecurityNamespace> securityNamespaces = new List<SecurityNamespace>(5);
      IAuthorizationService service = tfs.GetService<IAuthorizationService>();
      securityNamespaces.Add((SecurityNamespace) new AuthorizationSecurityNamespace(tfs, new SecurityNamespaceDescription(AuthorizationSecurityConstants.NamespaceSecurityGuid, "NAMESPACE", (string) null, "Integration", AuthorizationSecurityConstants.SeparatorChar, -1, SecurityNamespaceStructure.Hierarchical, 0, 0, (IEnumerable<ActionDefinition>) AuthorizationSecurityNamespaceFactory.GetActionDefinitions(service, "NAMESPACE"))));
      securityNamespaces.Add((SecurityNamespace) new AuthorizationSecurityNamespace(tfs, new SecurityNamespaceDescription(AuthorizationSecurityConstants.ProjectSecurityGuid, "PROJECT", (string) null, "Integration", AuthorizationSecurityConstants.SeparatorChar, -1, SecurityNamespaceStructure.Hierarchical, 0, 0, (IEnumerable<ActionDefinition>) AuthorizationSecurityNamespaceFactory.GetActionDefinitions(service, "PROJECT"))));
      securityNamespaces.Add((SecurityNamespace) new AuthorizationSecurityNamespace(tfs, new SecurityNamespaceDescription(AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid, "CSS_NODE", (string) null, "Integration", AuthorizationSecurityConstants.SeparatorChar, -1, SecurityNamespaceStructure.Hierarchical, 0, 0, (IEnumerable<ActionDefinition>) AuthorizationSecurityNamespaceFactory.GetActionDefinitions(service, "CSS_NODE"))));
      if (!string.IsNullOrEmpty(tfs.GetService<ILocationService>().LocationForCurrentConnection("GroupSecurity2", new Guid("6448b75a-5ab4-492b-ba8d-5bd55b4ff523"))))
        securityNamespaces.Add((SecurityNamespace) new AuthorizationSecurityNamespace(tfs, new SecurityNamespaceDescription(AuthorizationSecurityConstants.IterationNodeSecurityGuid, "ITERATION_NODE", (string) null, "Integration", AuthorizationSecurityConstants.SeparatorChar, -1, SecurityNamespaceStructure.Hierarchical, 0, 0, (IEnumerable<ActionDefinition>) AuthorizationSecurityNamespaceFactory.GetActionDefinitions(service, "ITERATION_NODE"))));
      securityNamespaces.Add((SecurityNamespace) new AuthorizationSecurityNamespace(tfs, new SecurityNamespaceDescription(FrameworkSecurity.EventSubscriptionNamespaceId, "EVENT_SUBSCRIPTION", (string) null, "Integration", AuthorizationSecurityConstants.SeparatorChar, -1, SecurityNamespaceStructure.Hierarchical, 0, 0, (IEnumerable<ActionDefinition>) AuthorizationSecurityNamespaceFactory.GetActionDefinitions(service, "EVENT_SUBSCRIPTION"))));
      return (IEnumerable<SecurityNamespace>) securityNamespaces;
    }

    private static List<ActionDefinition> GetActionDefinitions(
      IAuthorizationService authorizationService,
      string classId)
    {
      string[] actionId = authorizationService.ListObjectClassActions(classId);
      string[] strArray = authorizationService.ListLocalizedActionNames(classId, actionId);
      List<ActionDefinition> actionDefinitions = new List<ActionDefinition>();
      for (int index = 0; index < actionId.Length; ++index)
      {
        int bit = 0;
        switch (classId)
        {
          case "NAMESPACE":
            string str1 = actionId[index];
            if (str1 != null)
            {
              switch (str1.Length)
              {
                case 12:
                  if (str1 == "GENERIC_READ")
                  {
                    bit = AuthorizationNamespacePermissions.GenericRead;
                    break;
                  }
                  break;
                case 13:
                  switch (str1[0])
                  {
                    case 'G':
                      if (str1 == "GENERIC_WRITE")
                      {
                        bit = AuthorizationNamespacePermissions.GenericWrite;
                        break;
                      }
                      break;
                    case 'T':
                      if (str1 == "TRIGGER_EVENT")
                      {
                        bit = AuthorizationNamespacePermissions.TriggerEvent;
                        break;
                      }
                      break;
                  }
                  break;
                case 15:
                  switch (str1[0])
                  {
                    case 'C':
                      if (str1 == "CREATE_PROJECTS")
                      {
                        bit = AuthorizationNamespacePermissions.CreateProjects;
                        break;
                      }
                      break;
                    case 'M':
                      if (str1 == "MANAGE_TEMPLATE")
                      {
                        bit = AuthorizationNamespacePermissions.ManageTemplate;
                        break;
                      }
                      break;
                  }
                  break;
                case 16:
                  switch (str1[0])
                  {
                    case 'D':
                      if (str1 == "DIAGNOSTIC_TRACE")
                      {
                        bit = AuthorizationNamespacePermissions.DiagnosticTrace;
                        break;
                      }
                      break;
                    case 'S':
                      if (str1 == "SYNCHRONIZE_READ")
                      {
                        bit = AuthorizationNamespacePermissions.SynchronizeRead;
                        break;
                      }
                      break;
                  }
                  break;
                case 20:
                  if (str1 == "ADMINISTER_WAREHOUSE")
                  {
                    bit = 0;
                    break;
                  }
                  break;
                case 23:
                  if (str1 == "MANAGE_TEST_CONTROLLERS")
                  {
                    bit = AuthorizationNamespacePermissions.ManageTestControllers;
                    break;
                  }
                  break;
              }
            }
            else
              break;
            break;
          case "PROJECT":
            string str2 = actionId[index];
            if (str2 != null)
            {
              switch (str2.Length)
              {
                case 6:
                  switch (str2[0])
                  {
                    case 'D':
                      if (str2 == "DELETE")
                      {
                        bit = AuthorizationProjectPermissions.Delete;
                        break;
                      }
                      break;
                    case 'R':
                      if (str2 == "RENAME")
                      {
                        bit = AuthorizationProjectPermissions.Rename;
                        break;
                      }
                      break;
                  }
                  break;
                case 11:
                  if (str2 == "START_BUILD")
                  {
                    bit = AuthorizationProjectPermissions.StartBuild;
                    break;
                  }
                  break;
                case 12:
                  switch (str2[0])
                  {
                    case 'G':
                      if (str2 == "GENERIC_READ")
                      {
                        bit = AuthorizationProjectPermissions.GenericRead;
                        break;
                      }
                      break;
                    case 'U':
                      if (str2 == "UPDATE_BUILD")
                      {
                        bit = AuthorizationProjectPermissions.UpdateBuild;
                        break;
                      }
                      break;
                  }
                  break;
                case 13:
                  if (str2 == "GENERIC_WRITE")
                  {
                    bit = AuthorizationProjectPermissions.GenericWrite;
                    break;
                  }
                  break;
                case 16:
                  if (str2 == "ADMINISTER_BUILD")
                  {
                    bit = AuthorizationProjectPermissions.AdministerBuild;
                    break;
                  }
                  break;
                case 17:
                  switch (str2[0])
                  {
                    case 'E':
                      if (str2 == "EDIT_BUILD_STATUS")
                      {
                        bit = AuthorizationProjectPermissions.EditBuildStatus;
                        break;
                      }
                      break;
                    case 'M':
                      if (str2 == "MANAGE_PROPERTIES")
                      {
                        bit = AuthorizationProjectPermissions.ManageProperties;
                        break;
                      }
                      break;
                    case 'U':
                      if (str2 == "UPDATE_VISIBILITY")
                      {
                        bit = AuthorizationProjectPermissions.UpdateVisibility;
                        break;
                      }
                      break;
                    case 'V':
                      if (str2 == "VIEW_TEST_RESULTS")
                      {
                        bit = AuthorizationProjectPermissions.ViewTestResults;
                        break;
                      }
                      break;
                  }
                  break;
                case 19:
                  if (str2 == "DELETE_TEST_RESULTS")
                  {
                    bit = AuthorizationProjectPermissions.DeleteTestResults;
                    break;
                  }
                  break;
                case 20:
                  if (str2 == "PUBLISH_TEST_RESULTS")
                  {
                    bit = AuthorizationProjectPermissions.PublishTestResults;
                    break;
                  }
                  break;
                case 21:
                  if (str2 == "BYPASS_PROPERTY_CACHE")
                  {
                    bit = AuthorizationProjectPermissions.BypassPropertyCache;
                    break;
                  }
                  break;
                case 24:
                  switch (str2[7])
                  {
                    case 'S':
                      if (str2 == "MANAGE_SYSTEM_PROPERTIES")
                      {
                        bit = AuthorizationProjectPermissions.ManageSystemProperties;
                        break;
                      }
                      break;
                    case 'T':
                      if (str2 == "MANAGE_TEST_ENVIRONMENTS")
                      {
                        bit = AuthorizationProjectPermissions.ManageTestEnvironments;
                        break;
                      }
                      break;
                  }
                  break;
                case 26:
                  if (str2 == "MANAGE_TEST_CONFIGURATIONS")
                  {
                    bit = AuthorizationProjectPermissions.ManageTestConfigurations;
                    break;
                  }
                  break;
              }
            }
            else
              break;
            break;
          case "CSS_NODE":
            string str3 = actionId[index];
            if (str3 != null)
            {
              switch (str3.Length)
              {
                case 6:
                  if (str3 == "DELETE")
                  {
                    bit = AuthorizationCssNodePermissions.Delete;
                    break;
                  }
                  break;
                case 12:
                  if (str3 == "GENERIC_READ")
                  {
                    bit = AuthorizationCssNodePermissions.GenericRead;
                    break;
                  }
                  break;
                case 13:
                  if (str3 == "GENERIC_WRITE")
                  {
                    bit = AuthorizationCssNodePermissions.GenericWrite;
                    break;
                  }
                  break;
                case 14:
                  if (str3 == "WORK_ITEM_READ")
                  {
                    bit = AuthorizationCssNodePermissions.WorkItemRead;
                    break;
                  }
                  break;
                case 15:
                  switch (str3[0])
                  {
                    case 'C':
                      if (str3 == "CREATE_CHILDREN")
                      {
                        bit = AuthorizationCssNodePermissions.CreateChildren;
                        break;
                      }
                      break;
                    case 'W':
                      if (str3 == "WORK_ITEM_WRITE")
                      {
                        bit = AuthorizationCssNodePermissions.WorkItemWrite;
                        break;
                      }
                      break;
                  }
                  break;
                case 17:
                  if (str3 == "MANAGE_TEST_PLANS")
                  {
                    bit = AuthorizationCssNodePermissions.ManageTestPlans;
                    break;
                  }
                  break;
                case 18:
                  if (str3 == "MANAGE_TEST_SUITES")
                  {
                    bit = AuthorizationCssNodePermissions.ManageTestSuites;
                    break;
                  }
                  break;
                case 22:
                  if (str3 == "WORK_ITEM_SAVE_COMMENT")
                  {
                    bit = AuthorizationCssNodePermissions.WorkItemSaveComment;
                    break;
                  }
                  break;
              }
            }
            else
              break;
            break;
          case "ITERATION_NODE":
            switch (actionId[index])
            {
              case "GENERIC_READ":
                bit = AuthorizationIterationNodePermissions.GenericRead;
                break;
              case "GENERIC_WRITE":
                bit = AuthorizationIterationNodePermissions.GenericWrite;
                break;
              case "CREATE_CHILDREN":
                bit = AuthorizationIterationNodePermissions.CreateChildren;
                break;
              case "DELETE":
                bit = AuthorizationIterationNodePermissions.Delete;
                break;
            }
            break;
          case "EVENT_SUBSCRIPTION":
            switch (actionId[index])
            {
              case "GENERIC_READ":
                bit = 1;
                break;
              case "GENERIC_WRITE":
                bit = 2;
                break;
              case "UNSUBSCRIBE":
                bit = 4;
                break;
            }
            break;
        }
        if (bit != 0)
          actionDefinitions.Add(new ActionDefinition(bit, actionId[index], strArray[index]));
      }
      return actionDefinitions;
    }
  }
}
