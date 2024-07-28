// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.SoapExceptionUtilities
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Security;
using System.Threading;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Xml;

namespace Microsoft.TeamFoundation.Common
{
  public sealed class SoapExceptionUtilities
  {
    public static readonly string SqlNumber = nameof (SqlNumber);
    public static readonly string BaseExceptionName = nameof (BaseExceptionName);
    public static readonly string ExceptionMessage = nameof (ExceptionMessage);
    public static readonly string ServerTimeStamp = nameof (ServerTimeStamp);
    private static bool _fullDetails = false;

    private SoapExceptionUtilities()
    {
    }

    public static bool FullDetails
    {
      get => SoapExceptionUtilities._fullDetails;
      set => SoapExceptionUtilities._fullDetails = value;
    }

    [Obsolete("Method is obsolete")]
    public static bool IsFromSqlException(SoapException se) => typeof (SqlException).FullName == SoapExceptionUtilities.GetDetailAttribute(se, SoapExceptionUtilities.BaseExceptionName);

    [Obsolete("Method is obsolete")]
    public static bool IsFromSecurityException(SoapException se) => typeof (SecurityException).FullName == SoapExceptionUtilities.GetDetailAttribute(se, SoapExceptionUtilities.BaseExceptionName);

    [Obsolete("Method is obsolete")]
    public static bool IsFromArgumentException(SoapException se) => typeof (ArgumentException).FullName == SoapExceptionUtilities.GetDetailAttribute(se, SoapExceptionUtilities.BaseExceptionName);

    public static Exception ConvertToStronglyTypedException(SoapException se)
    {
      string baseExceptionName = SoapExceptionUtilities.GetBaseExceptionName(se);
      string exceptionMessage = SoapExceptionUtilities.GetExceptionMessage(se);
      if (baseExceptionName == typeof (ArgumentException).FullName)
        return (Exception) new ArgumentException(exceptionMessage);
      if (baseExceptionName == typeof (ArgumentNullException).FullName)
        return (Exception) new ArgumentNullException(exceptionMessage);
      if (baseExceptionName == typeof (SecurityException).FullName)
        return (Exception) new SecurityException(exceptionMessage);
      if (baseExceptionName == typeof (AuthorizationSubsystemException).FullName)
        return (Exception) new AuthorizationSubsystemException(exceptionMessage);
      if (baseExceptionName == typeof (GroupSecuritySubsystemException).FullName)
        return (Exception) new GroupSecuritySubsystemException(exceptionMessage);
      if (baseExceptionName == "Microsoft.TeamFoundation.Server.ServerException")
        return (Exception) new CommonStructureSubsystemException(exceptionMessage);
      if (baseExceptionName == typeof (CommonStructureSubsystemException).FullName)
        return (Exception) new CommonStructureSubsystemException(exceptionMessage);
      if (baseExceptionName == typeof (ProjectException).FullName)
        return (Exception) new ProjectException(exceptionMessage);
      if (baseExceptionName == typeof (SyncSupersededBaselineRevException).FullName)
        return (Exception) new SyncSupersededBaselineRevException(exceptionMessage);
      if (baseExceptionName == typeof (SyncBadBaselineRevException).FullName)
        return (Exception) new SyncBadBaselineRevException(exceptionMessage);
      if (baseExceptionName == typeof (SyncAccessDeniedException).FullName)
        return (Exception) new SyncAccessDeniedException(exceptionMessage);
      if (baseExceptionName == typeof (SyncSubsystemException).FullName)
        return (Exception) new SyncSubsystemException(exceptionMessage);
      if (baseExceptionName == typeof (TeamFoundationServiceUnavailableException).FullName)
        return (Exception) new TeamFoundationServiceUnavailableException(exceptionMessage, (Exception) se);
      return se.Message != exceptionMessage ? (Exception) new SoapException(exceptionMessage, se.Code, string.Empty, se.Detail, (Exception) null) : (Exception) se;
    }

    public static string GetExceptionMessage(SoapException e) => SoapExceptionUtilities.GetDetailAttribute(e, SoapExceptionUtilities.ExceptionMessage) ?? e.Message;

    public static string GetBaseExceptionName(SoapException exception) => SoapExceptionUtilities.GetDetailAttribute(exception, SoapExceptionUtilities.BaseExceptionName) ?? typeof (Exception).FullName;

    public static string GetDetailAttribute(SoapException exception, string name)
    {
      if (exception.Detail != null)
      {
        XmlAttribute attribute = exception.Detail.Attributes[name];
        if (attribute != null)
          return attribute.Value;
      }
      return (string) null;
    }

    public static int GetSqlNumber(SoapException exception)
    {
      string detailAttribute = SoapExceptionUtilities.GetDetailAttribute(exception, SoapExceptionUtilities.SqlNumber);
      return detailAttribute != null ? int.Parse(detailAttribute, (IFormatProvider) CultureInfo.InvariantCulture) : -1;
    }

    [Obsolete("Method is obsolete")]
    public static void ShowErrorDialog(IWin32Window owner, SoapException e)
    {
      CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
      MessageBoxOptions options = MessageBoxOptions.DefaultDesktopOnly;
      if (currentUiCulture.TextInfo.IsRightToLeft)
        options |= MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
      int num = (int) MessageBox.Show(owner, e.Message, ClientResources.ErrorTitle(), MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    }
  }
}
