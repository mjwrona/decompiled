// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlAzureLoginGenerator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class SqlAzureLoginGenerator
  {
    public static readonly string CollectionDbLoginPrefix = "tfsTPC-";
    public static readonly string TfsAccountDbLoginPrefix = "tfsACCT-";
    public static readonly string SpsAccountDbLoginPrefix = "spsACCT-";
    public static readonly string DataTierLoginPrefix = "tfsDT-";
    public static readonly string OpsLoginPrefix = "tfsOPS-";
    public static readonly string ConfigDbLoginPrefixWithoutService = "CFG-";
    public static readonly string DboLoginPrefixWithoutService = "DBO-";
    private static string s_area = "SqlSecurity";
    private static string s_layer = nameof (SqlAzureLoginGenerator);
    private static readonly int s_maxServiceNameLength = 24;

    public static string CreateConfigDbLoginName(string serviceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
      if (serviceName.Length > SqlAzureLoginGenerator.s_maxServiceNameLength)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Service name used for SQL Login can be no longer than {0} characters.", (object) SqlAzureLoginGenerator.s_maxServiceNameLength), nameof (serviceName));
      return SqlAzureLoginGenerator.CreateLoginName(serviceName + SqlAzureLoginGenerator.ConfigDbLoginPrefixWithoutService);
    }

    public static string CreateCollectionDbLoginName() => SqlAzureLoginGenerator.CreateLoginName(SqlAzureLoginGenerator.CollectionDbLoginPrefix);

    public static string CreateTfsAccountDbLoginName() => SqlAzureLoginGenerator.CreateLoginName(SqlAzureLoginGenerator.TfsAccountDbLoginPrefix);

    public static string CreateSpsAccountDbLoginName() => SqlAzureLoginGenerator.CreateLoginName(SqlAzureLoginGenerator.SpsAccountDbLoginPrefix);

    public static string CreateOpsLoginName() => SqlAzureLoginGenerator.CreateLoginName(SqlAzureLoginGenerator.OpsLoginPrefix);

    public static string CreateDataTierLoginName() => SqlAzureLoginGenerator.CreateLoginName(SqlAzureLoginGenerator.DataTierLoginPrefix);

    public static string CreateDboLoginName(string serviceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
      if (serviceName.Length > SqlAzureLoginGenerator.s_maxServiceNameLength)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Service name used for SQL Login can be no longer than {0} characters.", (object) SqlAzureLoginGenerator.s_maxServiceNameLength), nameof (serviceName));
      return SqlAzureLoginGenerator.CreateLoginName(serviceName + SqlAzureLoginGenerator.DboLoginPrefixWithoutService);
    }

    public static string CreateLoginPassword(string username, int? maxLength = null)
    {
      if (maxLength.HasValue)
        ArgumentUtility.CheckForOutOfRange(maxLength.Value, nameof (maxLength), 8);
      try
      {
        TeamFoundationTracingService.TraceEnterRaw(67201, SqlAzureLoginGenerator.s_area, SqlAzureLoginGenerator.s_layer, nameof (CreateLoginPassword), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
        byte[] numArray = new byte[15];
        using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
        {
          for (int index = 0; index < 100; ++index)
          {
            randomNumberGenerator.GetBytes(numArray);
            string password = Convert.ToBase64String(numArray);
            int length1 = password.Length;
            int? nullable = maxLength;
            int valueOrDefault = nullable.GetValueOrDefault();
            if (length1 > valueOrDefault & nullable.HasValue)
              password = password.Substring(0, maxLength.Value);
            if (SqlAzureLoginGenerator.PasswordMeetsComplexityRequirements(username, password))
            {
              int length2 = password.Length;
              TeamFoundationTracingService.TraceRaw(67202, TraceLevel.Info, SqlAzureLoginGenerator.s_area, SqlAzureLoginGenerator.s_layer, "Generated a random password with {0} characters.", (object) length2);
              return password;
            }
            TeamFoundationTracingService.TraceRaw(67203, TraceLevel.Warning, SqlAzureLoginGenerator.s_area, SqlAzureLoginGenerator.s_layer, "Generated password doesn't meet SQL Azure requirements. Generating another one.");
          }
        }
        TeamFoundationTracingService.TraceRaw(67204, TraceLevel.Error, SqlAzureLoginGenerator.s_area, SqlAzureLoginGenerator.s_layer, "Couldn't generate a valid SQL Azure password within 100 tries.");
        throw new ApplicationException("Couldn't generate a valid SQL Azure password within 100 tries.");
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(67209, SqlAzureLoginGenerator.s_area, SqlAzureLoginGenerator.s_layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(67210, SqlAzureLoginGenerator.s_area, SqlAzureLoginGenerator.s_layer, nameof (CreateLoginPassword));
      }
    }

    private static bool PasswordMeetsComplexityRequirements(string username, string password)
    {
      if (password.Length < 15 || string.Equals(username, password, StringComparison.OrdinalIgnoreCase))
        return false;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      foreach (char c in password)
      {
        if (c >= '0' && c <= '9')
          flag3 = true;
        else if (c >= 'A' && c <= 'Z')
          flag1 = true;
        else if (c >= 'a' && c <= 'z')
          flag2 = true;
        else if (!char.IsWhiteSpace(c))
          flag4 = true;
        if (flag1 & flag2 & flag3 & flag4)
          return true;
      }
      return false;
    }

    public static string CreateLoginName(string prefix)
    {
      try
      {
        TeamFoundationTracingService.TraceEnterRaw(67211, SqlAzureLoginGenerator.s_area, SqlAzureLoginGenerator.s_layer, "CreateLoginName (prefix={0})", (object) prefix);
        string loginName = prefix + Guid.NewGuid().ToString("N");
        TeamFoundationTracingService.TraceRaw(67212, TraceLevel.Verbose, SqlAzureLoginGenerator.s_area, SqlAzureLoginGenerator.s_layer, "Returning new loginName: {0}", (object) loginName);
        return loginName;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(67219, SqlAzureLoginGenerator.s_area, SqlAzureLoginGenerator.s_layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(67220, SqlAzureLoginGenerator.s_area, SqlAzureLoginGenerator.s_layer, nameof (CreateLoginName));
      }
    }

    public static bool IsValidLoginName(string prefix, string loginName) => loginName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
  }
}
