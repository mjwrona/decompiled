// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.EnvironmentOptionsExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class EnvironmentOptionsExtensions
  {
    private static readonly List<string> ValidEmailNotificationTypes = new List<string>()
    {
      "Always",
      "OnlyOnFailure",
      "Never"
    };

    public static void Validate(this EnvironmentOptions environmentOptions)
    {
      IDictionary<string, string> dictionary = environmentOptions != null ? environmentOptions.GetInvalidOptions() : throw new ArgumentNullException(nameof (environmentOptions));
      if (dictionary.Count != 0)
      {
        string str = string.Join(",", (IEnumerable<string>) dictionary.Keys);
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidValuesInEnvironmentOptions, (object) string.Join(",", (IEnumerable<string>) dictionary.Values), (object) str));
      }
    }

    private static IDictionary<string, string> GetInvalidOptions(
      this EnvironmentOptions environmentOptions)
    {
      Dictionary<string, string> invalidOptions = new Dictionary<string, string>();
      if (environmentOptions.TimeoutInMinutes < 0)
        invalidOptions.Add("TimeoutInMinutes", environmentOptions.TimeoutInMinutes.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!EnvironmentOptionsExtensions.ValidEmailNotificationTypes.Contains<string>(environmentOptions.EmailNotificationType, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        invalidOptions.Add("EmailNotificationType", environmentOptions.EmailNotificationType);
      }
      else
      {
        string[] source = new string[0];
        if (environmentOptions.EmailRecipients != null)
          source = environmentOptions.EmailRecipients.Split(';');
        if (((IEnumerable<string>) source).Any<string>())
        {
          string[] defaultEmailRecipients = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.RunOptionsConstants.DefaultEmailRecipients.Split(';');
          if (((IEnumerable<string>) source).Any<string>((Func<string, bool>) (e => !((IEnumerable<string>) defaultEmailRecipients).Contains<string>(e, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))))
            invalidOptions.Add("EmailRecipients", environmentOptions.EmailRecipients);
        }
        else if (!string.Equals(environmentOptions.EmailNotificationType, "Never", StringComparison.OrdinalIgnoreCase))
          invalidOptions.Add("EmailRecipients", environmentOptions.EmailRecipients);
      }
      return (IDictionary<string, string>) invalidOptions;
    }
  }
}
