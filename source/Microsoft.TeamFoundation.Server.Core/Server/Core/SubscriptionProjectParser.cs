// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.SubscriptionProjectParser
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class SubscriptionProjectParser
  {
    private Guid m_projectId = Guid.Empty;
    private const string s_area = "Notification";
    private const string s_layer = "SubscriptionProjectParser";
    private const string s_allowNewWorkItemAreaPathFeatureFlag = "AzureDevOps.Services.Notifications.AllowNewAreaPathFilterSyntax";
    private const string s_workItemAreaPath_OLD = "CoreFields/StringFields/Field[Name='Area Path']/";
    private const string s_workItemAreaPath = "CoreFields/StringFields/Field[ReferenceName='System.AreaPath']/";
    private const string s_workItemIterationPath = "CoreFields/StringFields/Field[Name='Iteration Path']/";
    private const string s_codeReviewAreaPath = "SourceWorkItem/AreaPath";
    private const string s_codeReviewChangedEvent = "CodeReviewChangedEvent";
    private const string s_workItemChangedEvent = "WorkItemChangedEvent";
    private const string c_workItemNodeRootPath = "\\";
    private const string c_checkinEvent = "CheckinEvent";
    private const string c_checkinArtifactXPath = "Artifacts/Artifact";
    private const string c_buildCompletedEvent = "BuildCompletedEvent";
    private const string c_buildCompletedDefinitionXPath = "tb1:Definition/@FullPath";
    private const string c_checkinArtifactServerItemXPath = "@ServerItem";
    private const string c_checkinArtifactFolderXPath = "@Folder";
    private const string c_checkinArtifactTeamProjectXPath = "@TeamProject";
    private const string c_checkinRootPath = "$/";
    private const string c_checkinArtifactProjectPathPattern = "\"\\$\\/(?<projectRef>[^\"\\/]+)(?<restOfPath>.*)\"";
    private const string c_checkinArtifactProjectPattern = "(?<caseSensitive>translate\\()?(?<name>@TeamProject)(,\\s*[\\'\"][^\\'\"]*[\\'\"],\\s*[\\'\"][^\\']*[\\'\"]\\))?\\s*=\\s*[\\'\"](?<value>[^\\'\"]*)[\\'\"]";
    private static readonly Regex s_checkinArtifactProjectPathRegex = new Regex("\"\\$\\/(?<projectRef>[^\"\\/]+)(?<restOfPath>.*)\"", RegexOptions.Compiled);
    private static readonly Regex s_checkinArtifactProjectRegex = new Regex("(?<caseSensitive>translate\\()?(?<name>@TeamProject)(,\\s*[\\'\"][^\\'\"]*[\\'\"],\\s*[\\'\"][^\\']*[\\'\"]\\))?\\s*=\\s*[\\'\"](?<value>[^\\'\"]*)[\\'\"]", RegexOptions.Compiled);
    private static readonly ISet<string> conditionProjectReferences = (ISet<string>) new SortedSet<string>()
    {
      "PortfolioProject",
      "TeamProject",
      "tb1:Build/@TeamProject",
      "SourceWorkItem/TeamProject",
      "TeamProjectCollectionName"
    };
    private static readonly string GitEventSpecialCase = "TeamProjectUri";
    private static readonly string GitPushEventSpecialCase = "PushNotification/TeamProjectUri";
    private static readonly string WitChangesFromSpecialCase = "CoreFields/StringFields/Field[Name='Team Project']/OldValue";
    private static readonly string WitChangesToSpecialCase = "CoreFields/StringFields/Field[Name='Team Project']/NewValue";

    public Guid GetProjectId(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription,
      Guid proposedProjectId,
      bool evaluateOnly = false)
    {
      Guid projectIdInternal = this.GetProjectIdInternal(requestContext, subscription, proposedProjectId);
      this.m_projectId = this.UpdatePathsReferencingProject(requestContext, subscription, SubscriptionProjectParser.UpdateOperation.ReplaceProjectNamesWithGuids, evaluateOnly);
      if (projectIdInternal != Guid.Empty)
        return projectIdInternal;
      return this.m_projectId != Guid.Empty ? this.m_projectId : proposedProjectId;
    }

    public Guid GetProjectId(
      IVssRequestContext requestContext,
      string eventType,
      string conditionString,
      Guid proposedProjectId,
      bool evaluateOnly,
      out string newSubscriptionCondition)
    {
      Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription = new Microsoft.VisualStudio.Services.Notifications.Server.Subscription()
      {
        SubscriberId = new Guid(),
        SubscriptionFilter = (ISubscriptionFilter) new ExpressionFilter(eventType),
        ConditionString = conditionString,
        Tag = (string) null
      };
      Guid projectId = this.GetProjectId(requestContext, subscription, proposedProjectId, evaluateOnly);
      subscription.ProjectId = projectId;
      newSubscriptionCondition = subscription.ConditionString;
      return projectId;
    }

    private Guid GetProjectIdInternal(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription,
      Guid proposedProjectId)
    {
      try
      {
        this.m_projectId = proposedProjectId;
        Condition condition = TeamFoundationEventConditionParser.GetParser(subscription.GetEventSerializerType(requestContext), subscription.ConditionString).Parse();
        string filterString;
        this.m_projectId = this.Parse(requestContext, condition, out filterString);
        subscription.ConditionString = filterString;
        return this.m_projectId != Guid.Empty ? this.m_projectId : proposedProjectId;
      }
      catch (XPathException ex)
      {
        throw new ArgumentException(ex.Message, "filterExpression");
      }
    }

    internal Guid Parse(
      IVssRequestContext requestContext,
      Condition condition,
      out string filterString)
    {
      return this.ParseInternal(requestContext, condition, out filterString);
    }

    private Guid ParseInternal(
      IVssRequestContext requestContext,
      Condition condition,
      out string filterString,
      SubscriptionProjectParser.UpdateOperation operation = SubscriptionProjectParser.UpdateOperation.None)
    {
      Guid guid = Guid.Empty;
      switch (condition)
      {
        case StringFieldCondition _:
          guid = this.ParseStringFieldCondition(requestContext, condition as StringFieldCondition, operation);
          break;
        case AndCondition _:
          guid = this.ParseAndCondition(requestContext, condition as AndCondition, out filterString, operation);
          break;
        case OrCondition _:
          guid = this.ParseOrCondition(requestContext, condition as OrCondition, out filterString, operation);
          break;
        default:
          NotCondition notCondition = condition as NotCondition;
          break;
      }
      filterString = condition.ToString();
      return guid;
    }

    private Guid ParseOrCondition(
      IVssRequestContext requestContext,
      OrCondition condition,
      out string filterString,
      SubscriptionProjectParser.UpdateOperation operation = SubscriptionProjectParser.UpdateOperation.None)
    {
      Guid objA = this.ParseInternal(requestContext, condition.Condition1, out filterString, operation);
      Guid guid = this.ParseInternal(requestContext, condition.Condition2, out filterString, operation);
      if (object.Equals((object) objA, (object) Guid.Empty) | object.Equals((object) guid, (object) Guid.Empty))
        return Guid.Empty;
      if (object.Equals((object) objA, (object) guid))
        return objA;
      requestContext.Trace(1002110, TraceLevel.Warning, "Notification", nameof (SubscriptionProjectParser), Resources.InvalidSubscriptionProjectReference());
      return Guid.Empty;
    }

    private Guid ParseAndCondition(
      IVssRequestContext requestContext,
      AndCondition condition,
      out string filterString,
      SubscriptionProjectParser.UpdateOperation operation = SubscriptionProjectParser.UpdateOperation.None)
    {
      Guid objA = this.ParseInternal(requestContext, condition.condition1, out filterString, operation);
      Guid guid = this.ParseInternal(requestContext, condition.condition2, out filterString, operation);
      bool flag1 = object.Equals((object) objA, (object) Guid.Empty);
      bool flag2 = object.Equals((object) guid, (object) Guid.Empty);
      if (!flag1 && !flag2 && !object.Equals((object) objA, (object) guid))
      {
        requestContext.Trace(1002110, TraceLevel.Warning, "Notification", nameof (SubscriptionProjectParser), Resources.InvalidSubscriptionProjectReference());
        return Guid.Empty;
      }
      if (!flag1)
        return objA;
      return !flag2 ? guid : Guid.Empty;
    }

    private Guid ParseStringFieldCondition(
      IVssRequestContext requestContext,
      StringFieldCondition condition,
      SubscriptionProjectParser.UpdateOperation operation = SubscriptionProjectParser.UpdateOperation.None)
    {
      ArgumentUtility.CheckForNull<StringFieldCondition>(condition, nameof (ParseStringFieldCondition));
      Guid stringFieldCondition = Guid.Empty;
      if (SubscriptionProjectParser.conditionProjectReferences.Contains<string>(condition.FieldName.Spelling, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
      {
        string spelling = condition.Target.Spelling;
        if (!string.Equals(spelling, "@@MyProjectName@@", StringComparison.InvariantCultureIgnoreCase) && condition.Operation != (byte) 15)
        {
          switch (operation)
          {
            case SubscriptionProjectParser.UpdateOperation.ReplaceProjectNamesWithGuids:
              Guid projectGuid;
              if (this.TryGetProjectGuidFromName(requestContext, spelling, out projectGuid))
              {
                condition.Target = (Token) new ConstantToken(projectGuid.ToString());
                if (condition.Operation == (byte) 12)
                {
                  stringFieldCondition = projectGuid;
                  break;
                }
                break;
              }
              break;
            case SubscriptionProjectParser.UpdateOperation.ReplaceProjectGuidsWithNames:
              Guid result = Guid.Empty;
              if (Guid.TryParse(spelling, out result))
              {
                string projectNameFromGuid = this.GetProjectNameFromGuid(requestContext, new Guid(spelling));
                condition.Target = (Token) new ConstantToken(projectNameFromGuid);
                break;
              }
              break;
          }
        }
      }
      else if (string.Equals(SubscriptionProjectParser.GitEventSpecialCase, condition.FieldName.Spelling, StringComparison.InvariantCultureIgnoreCase) || string.Equals(SubscriptionProjectParser.GitPushEventSpecialCase, condition.FieldName.Spelling, StringComparison.InvariantCultureIgnoreCase))
      {
        if (operation == SubscriptionProjectParser.UpdateOperation.ReplaceProjectNamesWithGuids)
          stringFieldCondition = ProjectInfo.GetProjectId(condition.Target.Spelling);
      }
      else if (string.Equals(SubscriptionProjectParser.WitChangesFromSpecialCase, condition.FieldName.Spelling, StringComparison.InvariantCultureIgnoreCase) || string.Equals(SubscriptionProjectParser.WitChangesToSpecialCase, condition.FieldName.Spelling, StringComparison.InvariantCultureIgnoreCase))
      {
        if (!string.Equals(condition.Target.Spelling, "@@MyProjectName@@"))
        {
          Guid empty = Guid.Empty;
          switch (operation)
          {
            case SubscriptionProjectParser.UpdateOperation.ReplaceProjectNamesWithGuids:
              if (this.TryGetProjectGuidFromName(requestContext, condition.Target.Spelling, out empty))
              {
                condition.Target = (Token) new XPathToken(empty.ToString());
                break;
              }
              break;
            case SubscriptionProjectParser.UpdateOperation.ReplaceProjectGuidsWithNames:
              if (Guid.TryParse(condition.Target.Spelling, out empty))
              {
                string projectNameFromGuid = this.GetProjectNameFromGuid(requestContext, empty);
                condition.Target = (Token) new ConstantToken(projectNameFromGuid);
                break;
              }
              break;
          }
        }
      }
      else if (requestContext.IsFeatureEnabled("AzureDevOps.Services.Notifications.AllowNewAreaPathFilterSyntax") && string.Equals("CoreFields/StringFields/Field[ReferenceName='System.AreaPath']/NewValue", condition.FieldName.Spelling, StringComparison.InvariantCultureIgnoreCase) || requestContext.IsFeatureEnabled("AzureDevOps.Services.Notifications.AllowNewAreaPathFilterSyntax") && string.Equals("CoreFields/StringFields/Field[ReferenceName='System.AreaPath']/OldValue", condition.FieldName.Spelling, StringComparison.InvariantCultureIgnoreCase) || string.Equals("CoreFields/StringFields/Field[Name='Area Path']/NewValue", condition.FieldName.Spelling, StringComparison.InvariantCultureIgnoreCase) || string.Equals("CoreFields/StringFields/Field[Name='Area Path']/OldValue", condition.FieldName.Spelling, StringComparison.InvariantCultureIgnoreCase) || string.Equals("CoreFields/StringFields/Field[Name='Iteration Path']/NewValue", condition.FieldName.Spelling, StringComparison.InvariantCultureIgnoreCase) || string.Equals("CoreFields/StringFields/Field[Name='Iteration Path']/OldValue", condition.FieldName.Spelling, StringComparison.InvariantCultureIgnoreCase) || string.Equals("SourceWorkItem/AreaPath", condition.FieldName.Spelling, StringComparison.InvariantCultureIgnoreCase))
      {
        if (operation == SubscriptionProjectParser.UpdateOperation.ReplaceProjectGuidsWithNames || operation == SubscriptionProjectParser.UpdateOperation.ReplaceProjectNamesWithGuids)
        {
          string projectNameToValidate;
          condition.Target = (Token) new ConstantToken(this.GetUpdatedProjectIdentifer(requestContext, condition.Target.Spelling, operation, out projectNameToValidate));
          if (!string.IsNullOrEmpty(projectNameToValidate))
          {
            condition.Target = (Token) new ConstantToken(this.GetNormalizedWorkItemNodePath(condition.Target.Spelling));
            stringFieldCondition = this.ValidateProjectName(requestContext, projectNameToValidate);
          }
        }
      }
      else if (this.IsProjectReferenceCheckinField(condition.FieldName.Spelling))
        stringFieldCondition = this.UpdateCheckinPathFieldCondition(requestContext, condition, operation);
      else if (this.IsProjectReferenceBuildCompleted(condition.FieldName.Spelling))
        stringFieldCondition = this.UpdateBuildDefinitionPathFieldCondition(requestContext, condition, operation);
      return stringFieldCondition;
    }

    internal virtual Guid ValidateProjectName(IVssRequestContext requestContext, string project)
    {
      bool flag1 = string.Equals(project, "@@MyProjectName@@");
      bool flag2 = !object.Equals((object) this.m_projectId, (object) Guid.Empty);
      if (flag1 & flag2)
        return this.m_projectId;
      if (flag1 && !flag2)
      {
        requestContext.Trace(1002110, TraceLevel.Warning, "Notification", nameof (SubscriptionProjectParser), Resources.InvalidSubscriptionToken());
        throw new InvalidSubscriptionException(Resources.InvalidSubscriptionToken());
      }
      int num = !flag1 & flag2 ? 1 : 0;
      return this.GetProjectGuidFromName(requestContext, project);
    }

    internal virtual Guid GetProjectGuidFromName(
      IVssRequestContext requestContext,
      string projectName)
    {
      Guid projectId;
      requestContext.GetService<IProjectService>().TryGetProjectId(requestContext, projectName, out projectId);
      return projectId;
    }

    private bool TryGetProjectGuidFromName(
      IVssRequestContext requestContext,
      string projectName,
      out Guid projectGuid)
    {
      projectGuid = Guid.Empty;
      if (string.Equals(projectName, "@@MyProjectName@@", StringComparison.InvariantCultureIgnoreCase))
        return false;
      projectGuid = this.GetProjectGuidFromName(requestContext, projectName);
      return projectGuid != Guid.Empty;
    }

    internal virtual string GetProjectNameFromGuid(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      string projectName = string.Empty;
      if (!object.Equals((object) projectId, (object) Guid.Empty))
        requestContext.GetService<IProjectService>().TryGetProjectName(requestContext, projectId, out projectName);
      return projectName;
    }

    private bool TryGetProjectNameFromGuid(
      IVssRequestContext requestContext,
      Guid projectId,
      out string projectName)
    {
      projectName = string.Empty;
      projectName = this.GetProjectNameFromGuid(requestContext, projectId);
      return !string.IsNullOrEmpty(projectName);
    }

    internal Guid UpdatePathsReferencingProject(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription,
      SubscriptionProjectParser.UpdateOperation operation,
      bool evaluateOnly = false)
    {
      requestContext.TraceEnter(1002120, "Notification", nameof (SubscriptionProjectParser), nameof (UpdatePathsReferencingProject));
      Guid projectGuid = Guid.Empty;
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>(subscription, nameof (subscription));
        if (!object.Equals((object) Guid.Empty, (object) subscription.ProjectId))
          this.m_projectId = subscription.ProjectId;
        if (operation != SubscriptionProjectParser.UpdateOperation.None)
        {
          string str;
          if (operation.Equals((object) SubscriptionProjectParser.UpdateOperation.ReplaceProjectGuidsWithNames) && this.TryReplaceProjectGuidsWithNames(requestContext, subscription, out str, out projectGuid))
            subscription.ConditionString = str;
          else if (operation.Equals((object) SubscriptionProjectParser.UpdateOperation.ReplaceProjectNamesWithGuids))
          {
            TeamFoundationEventConditionParser parser = TeamFoundationEventConditionParser.GetParser(subscription.GetEventSerializerType(requestContext), subscription.ConditionString);
            try
            {
              projectGuid = this.ParseInternal(requestContext, parser.Parse(), out str, operation);
              subscription.ConditionString = str;
            }
            catch (ArgumentException ex)
            {
              requestContext.Trace(1002122, TraceLevel.Warning, "Notification", nameof (SubscriptionProjectParser), "Bad field resulting in ArgumentException: " + subscription.ConditionString);
              return projectGuid;
            }
            catch (InvalidSubscriptionException ex)
            {
              requestContext.Trace(1002122, TraceLevel.Warning, "Notification", nameof (SubscriptionProjectParser), "Invalid subscription: " + subscription.ConditionString);
              if (!evaluateOnly)
                return projectGuid;
              throw;
            }
            catch (Exception ex)
            {
              requestContext.Trace(1002122, TraceLevel.Warning, "Notification", nameof (SubscriptionProjectParser), subscription.TraceTags, "Error while replacing project name with Guids. Exception: {0}", (object) ex);
            }
          }
        }
        return projectGuid;
      }
      finally
      {
        requestContext.TraceLeave(1002121, "Notification", nameof (SubscriptionProjectParser), nameof (UpdatePathsReferencingProject));
      }
    }

    private string GetUpdatedProjectIdentifer(
      IVssRequestContext requestContext,
      string conditionString,
      SubscriptionProjectParser.UpdateOperation operation,
      out string projectNameToValidate)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(conditionString, nameof (conditionString));
      projectNameToValidate = string.Empty;
      bool flag = conditionString.ElementAt<char>(0) == '\\';
      string str1 = conditionString.Substring(flag ? 1 : 0);
      if (requestContext.IsFeatureEnabled("AzureDevOps.Services.Notifications.AllowNewAreaPathFilterSyntax") && str1.IndexOf("CoreFields/StringFields/Field[ReferenceName='System.AreaPath']/") == 0 || str1.IndexOf("CoreFields/StringFields/Field[Name='Area Path']/") == 0 || str1.IndexOf("CoreFields/StringFields/Field[Name='Iteration Path']/") == 0)
        return conditionString;
      int length = str1.IndexOf('\\') != -1 ? str1.IndexOf('\\') : str1.Length;
      if (length == 0)
        return "\\";
      string str2 = str1.Substring(0, length);
      string projectName1;
      if (operation == SubscriptionProjectParser.UpdateOperation.ReplaceProjectNamesWithGuids)
      {
        Guid result;
        if (Guid.TryParse(str2, out result))
        {
          string projectName2;
          if (this.TryGetProjectNameFromGuid(requestContext, result, out projectName2))
            projectNameToValidate = projectName2;
          else
            requestContext.Trace(1002123, TraceLevel.Warning, "Notification", nameof (SubscriptionProjectParser), "ReplaceProjectNamesWithGuids: The supplied project Guid (" + str2 + ") is not valid.");
          return conditionString;
        }
        projectNameToValidate = str2;
        projectName1 = this.GetProjectGuidFromName(requestContext, str2).ToString();
      }
      else
      {
        if (operation != SubscriptionProjectParser.UpdateOperation.ReplaceProjectGuidsWithNames)
          throw new InvalidOperationException("The specified UpdateOperation is not supported.");
        Guid projectId;
        if (!Guid.TryParse(str2, out projectId))
        {
          if (this.TryGetProjectGuidFromName(requestContext, str2, out projectId))
            projectNameToValidate = str2;
          else
            requestContext.Trace(1002123, TraceLevel.Warning, "Notification", nameof (SubscriptionProjectParser), "ReplaceProjectGuidsWithNames: The supplied project name (" + str2 + ") is not valid.");
          return conditionString;
        }
        if (!this.TryGetProjectNameFromGuid(requestContext, projectId, out projectName1))
        {
          requestContext.Trace(1002123, TraceLevel.Warning, "Notification", nameof (SubscriptionProjectParser), "ReplaceProjectGuidsWithNames: The supplied project Guid (" + projectId.ToString() + ") is not valid.");
          return conditionString;
        }
        projectNameToValidate = projectName1;
      }
      if (string.IsNullOrEmpty(projectName1))
        return conditionString;
      string str3 = (flag ? "\\" : "") + projectName1;
      int startIndex = (flag ? 1 : 0) + str2.Length;
      string str4 = startIndex == conditionString.Length ? "" : conditionString.Substring(startIndex);
      return str3 + str4;
    }

    private bool TryReplaceProjectGuidsWithNames(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription,
      out string conditionString,
      out Guid projectGuid)
    {
      TeamFoundationEventConditionParser parser = TeamFoundationEventConditionParser.GetParser(subscription.GetEventSerializerType(requestContext), subscription.ConditionString);
      try
      {
        projectGuid = this.ParseInternal(requestContext, parser.Parse(), out conditionString, SubscriptionProjectParser.UpdateOperation.ReplaceProjectGuidsWithNames);
        return true;
      }
      catch (ArgumentException ex)
      {
        requestContext.Trace(1002124, TraceLevel.Warning, "Notification", nameof (SubscriptionProjectParser), "Bad field resulting in ArgumentException: " + subscription.ConditionString);
      }
      catch (InvalidSubscriptionException ex)
      {
        requestContext.Trace(1002124, TraceLevel.Warning, "Notification", nameof (SubscriptionProjectParser), "Invalid subscription: " + subscription.ConditionString);
      }
      catch (ProjectDoesNotExistException ex)
      {
        requestContext.Trace(1002124, TraceLevel.Warning, "Notification", nameof (SubscriptionProjectParser), "ProjectDoesNotExistException thrown for subscription: " + subscription.ConditionString);
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
        requestContext.Trace(1002124, TraceLevel.Warning, "Notification", nameof (SubscriptionProjectParser), "ProjectDoesNotExistWithNameException thrown for subscription: " + subscription.ConditionString);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1002124, "Notification", nameof (SubscriptionProjectParser), ex);
      }
      projectGuid = Guid.Empty;
      conditionString = string.Empty;
      return false;
    }

    private bool IsEligibleToUpdatePathsReferencingProject(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription)
    {
      if (subscription.SubscriptionFilter.EventType.Equals("CodeReviewChangedEvent") || subscription.SubscriptionFilter.EventType.Equals("WorkItemChangedEvent"))
        return requestContext.IsFeatureEnabled("AzureDevOps.Services.Notifications.AllowNewAreaPathFilterSyntax") && subscription.ConditionString.Contains("CoreFields/StringFields/Field[ReferenceName='System.AreaPath']/") || subscription.ConditionString.Contains("CoreFields/StringFields/Field[Name='Area Path']/") || subscription.ConditionString.Contains("CoreFields/StringFields/Field[Name='Iteration Path']/") || subscription.ConditionString.Contains("SourceWorkItem/AreaPath");
      if (subscription.SubscriptionFilter.EventType.Equals("CheckinEvent"))
        return subscription.ConditionString.Contains("Artifacts/Artifact");
      return subscription.SubscriptionFilter.EventType.Equals("BuildCompletedEvent") && subscription.ConditionString.Contains("tb1:Definition/@FullPath");
    }

    private string GetNormalizedWorkItemNodePath(string path)
    {
      if (string.IsNullOrWhiteSpace(path))
        return "\\";
      path = path.Trim();
      return path.StartsWith("\\") ? path : "\\" + path;
    }

    private Guid UpdateCheckinPathFieldCondition(
      IVssRequestContext requestContext,
      StringFieldCondition condition,
      SubscriptionProjectParser.UpdateOperation operation)
    {
      Guid projectGuid = Guid.Empty;
      string projectName = string.Empty;
      string empty = string.Empty;
      string spelling = !condition.FieldName.ToString().Contains("@TeamProject") ? SubscriptionProjectParser.s_checkinArtifactProjectPathRegex.Replace(condition.FieldName.Spelling, (MatchEvaluator) (match =>
      {
        string str1 = match.Groups["projectRef"].Value;
        if (string.IsNullOrWhiteSpace(str1))
          return match.Value;
        Guid projectId;
        string projectName1;
        if (Guid.TryParse(str1, out projectId))
        {
          if (this.TryGetProjectNameFromGuid(requestContext, projectId, out projectName1))
          {
            projectGuid = projectId;
            projectName = projectName1;
          }
        }
        else
        {
          projectName1 = str1;
          if (this.TryGetProjectGuidFromName(requestContext, projectName1, out projectId))
          {
            projectGuid = projectId;
            projectName = projectName1;
          }
        }
        if (projectGuid == Guid.Empty)
        {
          string str2 = match.Groups["restOfPath"].Value;
          if (str2 == null || !str2.StartsWith("/"))
            return match.Value;
          if (string.IsNullOrEmpty(projectName1))
            throw new ProjectDoesNotExistWithNameException(projectName1);
          throw new ProjectDoesNotExistException(str1);
        }
        projectGuid = this.ValidateProjectName(requestContext, projectName);
        if (operation == SubscriptionProjectParser.UpdateOperation.ReplaceProjectNamesWithGuids)
          return match.Value.Replace("$/" + str1, "$/" + projectGuid.ToString());
        return operation == SubscriptionProjectParser.UpdateOperation.ReplaceProjectGuidsWithNames ? match.Value.Replace("$/" + str1, "$/" + projectName1.ToLower()) : match.Value;
      })) : SubscriptionProjectParser.s_checkinArtifactProjectRegex.Replace(condition.FieldName.Spelling, (MatchEvaluator) (match =>
      {
        string str = match.Groups["value"].Value;
        if (string.IsNullOrWhiteSpace(str))
          return match.Value;
        Guid projectId;
        string projectName2;
        if (Guid.TryParse(str, out projectId))
        {
          if (this.TryGetProjectNameFromGuid(requestContext, projectId, out projectName2))
          {
            projectGuid = projectId;
            projectName = projectName2;
          }
        }
        else
        {
          projectName2 = str;
          if (this.TryGetProjectGuidFromName(requestContext, projectName2, out projectId))
          {
            projectGuid = projectId;
            projectName = projectName2;
          }
        }
        if (projectGuid != Guid.Empty)
        {
          if (operation == SubscriptionProjectParser.UpdateOperation.ReplaceProjectNamesWithGuids)
            return match.Value.Replace(str, projectGuid.ToString());
          if (operation == SubscriptionProjectParser.UpdateOperation.ReplaceProjectGuidsWithNames)
            return match.Value.Replace(str, projectName2.ToLower());
        }
        return match.Value;
      }));
      condition.SetFieldNameExternal((Token) new ConstantToken(condition.Operation, spelling));
      return projectGuid;
    }

    private Guid UpdateBuildDefinitionPathFieldCondition(
      IVssRequestContext requestContext,
      StringFieldCondition condition,
      SubscriptionProjectParser.UpdateOperation operation)
    {
      bool flag1 = condition.Operation == (byte) 15 && operation == SubscriptionProjectParser.UpdateOperation.ReplaceProjectGuidsWithNames;
      string[] strArray = condition.Target.Spelling.Split(new char[1]
      {
        '\\'
      }, StringSplitOptions.None);
      if (flag1)
      {
        if (strArray.Length < 5 || strArray[0].Length != 0 || strArray[1].Length != 0 || strArray[3].Length != 0)
          return Guid.Empty;
      }
      else if (strArray.Length < 2 || strArray[0].Length != 0)
        return Guid.Empty;
      int index1 = flag1 ? 2 : 1;
      int num = flag1 ? 4 : 0;
      bool flag2 = false;
      Guid empty = Guid.Empty;
      switch (operation)
      {
        case SubscriptionProjectParser.UpdateOperation.ReplaceProjectNamesWithGuids:
          if (this.TryGetProjectGuidFromName(requestContext, strArray[1], out empty))
          {
            strArray[1] = empty.ToString();
            flag2 = true;
            break;
          }
          break;
        case SubscriptionProjectParser.UpdateOperation.ReplaceProjectGuidsWithNames:
          if (Guid.TryParse(strArray[index1], out empty))
          {
            if (!flag1)
            {
              string projectName;
              if (this.TryGetProjectNameFromGuid(requestContext, empty, out projectName))
              {
                strArray[1] = projectName;
                flag2 = true;
                break;
              }
              break;
            }
            flag2 = true;
            break;
          }
          break;
      }
      if (flag2)
      {
        StringBuilder stringBuilder = new StringBuilder();
        for (int index2 = num; index2 < strArray.Length; ++index2)
        {
          stringBuilder.Append(strArray[index2]);
          if (index2 < strArray.Length - 1)
            stringBuilder.Append('\\');
        }
        condition.Target = (Token) new ConstantToken(stringBuilder.ToString());
      }
      return empty;
    }

    private bool IsProjectReferenceCheckinField(string fieldExpression)
    {
      if (!fieldExpression.StartsWith("Artifacts/Artifact"))
        return false;
      return fieldExpression.Contains("@ServerItem") || fieldExpression.Contains("@Folder") || fieldExpression.Contains("@TeamProject");
    }

    private bool IsProjectReferenceBuildCompleted(string fieldExpression) => string.Equals(fieldExpression, "tb1:Definition/@FullPath", StringComparison.Ordinal);

    internal enum UpdateOperation
    {
      ReplaceProjectNamesWithGuids,
      ReplaceProjectGuidsWithNames,
      None,
    }
  }
}
