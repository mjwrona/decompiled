// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DataRetention.DataRetentionPagesResources
// Assembly: Microsoft.TeamFoundation.VersionControl.Server.DataRetention, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACAD6A6E-265A-4AD6-8B83-B0DD75035C8B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.DataRetention.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.VersionControl.Server.DataRetention
{
  internal static class DataRetentionPagesResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (DataRetentionPagesResources).GetTypeInfo().Assembly);
    public const string DeleteWorkspace = "DeleteWorkspace";
    public const string DeleteShelveset = "DeleteShelveset";
    public const string MustProvideWorkspaceNameAndOwner = "MustProvideWorkspaceNameAndOwner";
    public const string MustProvideShelvesetNameAndOwner = "MustProvideShelvesetNameAndOwner";
    public const string MustProviderShelvesetOwnerAndCutoffDate = "MustProviderShelvesetOwnerAndCutoffDate";
    public const string TableServerUri = "TableServerUri";
    public const string TableProjectCollection = "TableProjectCollection";
    public const string TableName = "TableName";
    public const string TableOwner = "TableOwner";
    public const string TableComputer = "TableComputer";
    public const string TableComment = "TableComment";
    public const string TableLastAccessDate = "TableLastAccessDate";
    public const string TableCreationDate = "TableCreationDate";
    public const string TableCutoffDate = "TableCutoffDate";
    public const string TableWFStatus = "TableWFStatus";
    public const string TableWFServerPath = "TableWFServerPath";
    public const string TableWFLocalPath = "TableWFLocalPath";
    public const string TableWFActive = "TableWFActive";
    public const string TableWFCloaked = "TableWFCloaked";
    public const string TablePCName = "TablePCName";
    public const string TablePCChange = "TablePCChange";
    public const string TablePCFolder = "TablePCFolder";
    public const string ChangeTypeAdd = "ChangeTypeAdd";
    public const string ChangeTypeBranch = "ChangeTypeBranch";
    public const string ChangeTypeDelete = "ChangeTypeDelete";
    public const string ChangeTypeEdit = "ChangeTypeEdit";
    public const string ChangeTypeFileType = "ChangeTypeFileType";
    public const string ChangeTypeLock = "ChangeTypeLock";
    public const string ChangeTypeMerge = "ChangeTypeMerge";
    public const string ChangeTypeNone = "ChangeTypeNone";
    public const string ChangeTypeRename = "ChangeTypeRename";
    public const string ChangeTypeRollback = "ChangeTypeRollback";
    public const string ChangeTypeUndelete = "ChangeTypeUndelete";
    public const string ChangeTypeSourceRename = "ChangeTypeSourceRename";
    public const string ErrorInTable = "ErrorInTable";
    public const string DeletionSuccessful = "DeletionSuccessful";
    public const string RenewalSuccessful = "RenewalSuccessful";
    public const string AlreadyUpToDate = "AlreadyUpToDate";
    public const string ExtendAllShelvesets = "ExtendAllShelvesets";
    public const string MultipleShelvesets = "MultipleShelvesets";

    public static ResourceManager Manager => DataRetentionPagesResources.s_resMgr;

    public static string Get(string resourceName) => DataRetentionPagesResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    public static string Get(string resourceName, CultureInfo culture) => culture == null ? DataRetentionPagesResources.Get(resourceName) : DataRetentionPagesResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) DataRetentionPagesResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? DataRetentionPagesResources.GetInt(resourceName) : (int) DataRetentionPagesResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) DataRetentionPagesResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? DataRetentionPagesResources.GetBool(resourceName) : (bool) DataRetentionPagesResources.s_resMgr.GetObject(resourceName, culture);

    public static string Format(string resourceName, params object[] args) => DataRetentionPagesResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    public static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = DataRetentionPagesResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }
  }
}
