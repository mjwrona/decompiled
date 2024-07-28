// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.EnvironmentExecutionPolicyExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class EnvironmentExecutionPolicyExtensions
  {
    public static void Validate(
      this EnvironmentExecutionPolicy executionPolicy,
      string environmentName)
    {
      if (executionPolicy == null)
        throw new ArgumentNullException(nameof (executionPolicy));
      List<string> values = new List<string>();
      if (executionPolicy.ConcurrencyCount < 0)
        values.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ExecutionPolicyConcurrencyCountErrorWithValidValuesSuggestionString, (object) executionPolicy.ConcurrencyCount, (object) environmentName));
      switch (executionPolicy.QueueDepthCount)
      {
        case 0:
        case 1:
          if (values.Count == 0)
            break;
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidValuesInExecutionPolicy, (object) string.Join(" ", (IEnumerable<string>) values)));
        default:
          values.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ExecutionPolicyErrorWithValidValuesSuggestionString, (object) "queueDepthCount", (object) executionPolicy.QueueDepthCount, (object) string.Join(", ", (object) 0, (object) 1)));
          goto case 0;
      }
    }
  }
}
