// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.TagFilter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class TagFilter
  {
    public static bool IsMatchingTagFilters(
      IVssRequestContext requestContext,
      ContainerImageTrigger definitionTrigger,
      string containerImagePushTag)
    {
      bool flag;
      if (definitionTrigger == null)
        flag = false;
      else if (definitionTrigger.TagFilters.IsNullOrEmpty<TagFilter>())
      {
        flag = true;
      }
      else
      {
        flag = false;
        foreach (TagFilter tagFilter in (IEnumerable<TagFilter>) definitionTrigger.TagFilters)
        {
          try
          {
            flag = TagFilter.IsMatchingRegexPattern(containerImagePushTag, tagFilter?.Pattern);
            if (flag)
            {
              string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ContainerImageTagMatchesRegex, (object) containerImagePushTag, (object) tagFilter.Pattern, (object) definitionTrigger.Alias);
              requestContext.Trace(1961043, TraceLevel.Info, "ReleaseManagementService", "JobLayer", message);
              break;
            }
            string message1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ContainerImageTagDoesNotMatchRegex, (object) containerImagePushTag, (object) tagFilter.Pattern, (object) definitionTrigger.Alias);
            requestContext.Trace(1961043, TraceLevel.Info, "ReleaseManagementService", "JobLayer", message1);
          }
          catch (RegexMatchTimeoutException ex)
          {
            string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.RegexTimeoutForContainerImageTag, (object) containerImagePushTag, (object) tagFilter?.Pattern, (object) definitionTrigger.Alias, (object) ex.Message);
            requestContext.Trace(1961043, TraceLevel.Error, "ReleaseManagementService", "JobLayer", message);
            throw new InvalidRequestException(message);
          }
        }
      }
      return flag;
    }

    public static void ValidateTagFilters(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ContainerImageTrigger trigger)
    {
      if (trigger == null || trigger.TagFilters.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.TagFilter>())
        return;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.TagFilter tagFilter in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.TagFilter>) trigger.TagFilters)
      {
        string pattern = tagFilter.Pattern;
        string errorMessage;
        if (!TagFilter.IsValidRegex(pattern, out errorMessage))
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidRegexInTagFilter, pattern == null ? (object) string.Empty : (object) pattern, (object) trigger.Alias, (object) errorMessage));
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required to get the error message along with the validation")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This is to validate the Regex pattern")]
    [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification = "Calling Regex constructor to validate the pattern")]
    public static bool IsValidRegex(string pattern, out string errorMessage)
    {
      if (pattern.IsNullOrEmpty<char>())
      {
        errorMessage = string.Empty;
        return true;
      }
      try
      {
        Regex regex = new Regex(pattern);
        errorMessage = string.Empty;
        return true;
      }
      catch (Exception ex)
      {
        errorMessage = ex.Message;
        return false;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want the Regex matching to happen gracefully so that the job does not fail.")]
    public bool IsMatchingTagFilter(
      IVssRequestContext requestContext,
      string containerImageTag,
      string artifactAlias)
    {
      bool flag = false;
      try
      {
        flag = TagFilter.IsMatchingRegexPattern(containerImageTag, this.Pattern);
        if (flag)
        {
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ContainerImageTagMatchesRegex, (object) containerImageTag, (object) this.Pattern, (object) artifactAlias);
          requestContext.Trace(1980006, TraceLevel.Info, "ReleaseManagementService", "Pipeline", message);
        }
        else
        {
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ContainerImageTagDoesNotMatchRegex, (object) containerImageTag, (object) this.Pattern, (object) artifactAlias);
          requestContext.Trace(1980006, TraceLevel.Info, "ReleaseManagementService", "Pipeline", message);
        }
      }
      catch (RegexMatchTimeoutException ex)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.RegexTimeoutForContainerImageTag, (object) containerImageTag, (object) this.Pattern, (object) artifactAlias, (object) ex.Message);
        requestContext.Trace(1980006, TraceLevel.Error, "ReleaseManagementService", "Pipeline", message);
      }
      catch (Exception ex)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.RegexMatchingErrorForContainerImageTag, (object) containerImageTag, (object) this.Pattern, (object) artifactAlias, (object) ex.Message);
        requestContext.Trace(1980006, TraceLevel.Error, "ReleaseManagementService", "Pipeline", message);
      }
      return flag;
    }

    public string Pattern { get; set; }

    public override bool Equals(object obj) => obj != null && !(this.GetType() != obj.GetType()) && string.Equals(this.Pattern, ((TagFilter) obj).Pattern);

    public override int GetHashCode()
    {
      string pattern = this.Pattern;
      return pattern == null ? 0 : pattern.GetHashCode();
    }

    private static bool IsMatchingRegexPattern(string tag, string pattern)
    {
      bool flag = false;
      if (string.IsNullOrEmpty(pattern) || Regex.IsMatch(tag, pattern, RegexOptions.None, TimeSpan.FromSeconds(1.0)))
        flag = true;
      return flag;
    }
  }
}
