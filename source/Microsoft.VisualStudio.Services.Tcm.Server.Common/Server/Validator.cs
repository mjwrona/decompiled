// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Validator
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class Validator
  {
    private const string EventLogSource = "TFS Services";

    internal static string TranslateBuildUri(string uri)
    {
      Uri result;
      return Uri.TryCreate(uri, UriKind.Absolute, out result) ? result.GetLeftPart(UriPartial.Path) : uri;
    }

    internal static void CheckAndTrimString(ref string value, string propertyName, int maxLength)
    {
      if (value == null)
        return;
      value = value.Trim();
      if (value.Length == 0)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.PropertyCannotBeEmpty, (object) propertyName));
      if (value.Length > maxLength)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.PropertyTooLong, (object) propertyName, (object) maxLength));
    }

    internal static void CheckStartEndDatesInOrder(DateTime startDate, DateTime endDate)
    {
      if (startDate > endDate && startDate != DateTime.MinValue && endDate != DateTime.MinValue)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.StartDateAfterEndDateError, (object) startDate, (object) endDate));
    }

    internal static bool TryCheckDuplicateTests(int[] ids, out int duplicateTestId)
    {
      duplicateTestId = -1;
      HashSet<int> intSet = new HashSet<int>();
      for (int index = 0; index < ids.Length; ++index)
      {
        if (!intSet.Contains(ids[index]))
        {
          intSet.Add(ids[index]);
        }
        else
        {
          duplicateTestId = ids[index];
          return true;
        }
      }
      return false;
    }

    internal static string CheckAndGetProjectUriFromName(
      TestManagementRequestContext context,
      string projectName)
    {
      if (string.IsNullOrEmpty(projectName))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TeamProjectNotFound, (object) projectName));
      return Validator.GetProjectFromName(context.RequestContext, projectName, true).Uri;
    }

    internal static string CheckAndGetProjectUriFromId(
      TestManagementRequestContext context,
      Guid projectId)
    {
      if (projectId == new Guid())
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TeamProjectNotFound, (object) projectId));
      return Validator.GetProjectFromId(context.RequestContext, projectId, true).Uri;
    }

    internal static Guid CheckAndGetProjectGuidFromName(
      TestManagementRequestContext context,
      string projectName)
    {
      if (string.IsNullOrEmpty(projectName))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TeamProjectNotFound, (object) projectName));
      return Validator.GetProjectFromName(context.RequestContext, projectName, true).Id;
    }

    internal static GuidAndString CheckAndGetProjectFromName(
      TestManagementRequestContext context,
      string projectName)
    {
      if (string.IsNullOrEmpty(projectName))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TeamProjectNotFound, (object) projectName));
      ProjectInfo projectFromName = Validator.GetProjectFromName(context.RequestContext, projectName, true);
      return new GuidAndString(projectFromName.Uri, projectFromName.Id);
    }

    internal static void CheckUniqueName(string name, HashSet<string> uniqueNames)
    {
      name = name ?? string.Empty;
      Validator.CheckAndTrimString(ref name, "Name", 256);
      if (uniqueNames.Contains(name))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DuplicateNameError, (object) name));
      uniqueNames.Add(name);
    }

    internal static long CheckOverflowAndGetSafeValue(long value, long increment)
    {
      try
      {
        return (long) checked (Convert.ToUInt32(value) + Convert.ToUInt32(increment));
      }
      catch (OverflowException ex)
      {
        return Convert.ToInt64(uint.MaxValue);
      }
    }

    internal static TimeSpan CheckOverflowAndGetSafeValue(TimeSpan value, TimeSpan increment)
    {
      try
      {
        return value.Add(increment);
      }
      catch (OverflowException ex)
      {
        return TimeSpan.MaxValue;
      }
    }

    public static Guid GetProjectGuidFromName(IVssRequestContext context, string projectName)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetProjectGuidFromName), "Project")))
      {
        ProjectInfo projectFromName = Validator.GetProjectFromName(context, projectName, false);
        return projectFromName != null ? projectFromName.Id : Guid.Empty;
      }
    }

    public static string GetProjectNameFromProjectGuid(IVssRequestContext context, Guid projectId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetProjectNameFromProjectGuid), "Project")))
        return Validator.GetProjectFromId(context.Elevate(), projectId, false)?.Name;
    }

    private static ProjectInfo GetProjectFromName(
      IVssRequestContext context,
      string projectName,
      bool modifyException)
    {
      if (string.IsNullOrEmpty(projectName))
        return (ProjectInfo) null;
      ProjectInfo projectFromName = (ProjectInfo) null;
      try
      {
        projectFromName = context.GetService<IProjectService>().GetProject(context.Elevate(), projectName);
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
        if (modifyException)
          context.TraceException("Exceptions", (Exception) ex);
        else
          throw;
      }
      if (projectFromName == null || projectFromName.Id.Equals(Guid.Empty))
        throw new TeamProjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TeamProjectNotFound, (object) projectName));
      return projectFromName;
    }

    public static ProjectInfo GetProjectFromId(
      IVssRequestContext context,
      Guid projectId,
      bool modifyException)
    {
      if (projectId == Guid.Empty)
        return (ProjectInfo) null;
      ProjectInfo projectFromId = (ProjectInfo) null;
      try
      {
        projectFromId = context.GetService<IProjectService>().GetProject(context.Elevate(), projectId);
      }
      catch (ProjectDoesNotExistException ex)
      {
        if (modifyException)
          context.TraceException("Exceptions", (Exception) ex);
        else
          throw;
      }
      if (projectFromId == null || projectFromId.Id.Equals(Guid.Empty))
        throw new TeamProjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TeamProjectNotFound, (object) projectId));
      return projectFromId;
    }
  }
}
