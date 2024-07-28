// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingOperationClass
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ServicingOperationClass
  {
    public const string ApplyPatch = "ApplyPatch";
    public const string AssignCollection = "AssignCollection";
    public const string AssignHostingAccount = "AssignHostingAccount";
    public const string AttachCollection = "AttachCollection";
    public const string CreateCollection = "CreateCollection";
    public const string CreateHostingOrganization = "CreateHostingOrganization";
    public const string CreateProject = "CreateProject";
    public const string CreateSchema = "CreateSchema";
    public const string DataImport = "DataImport";
    public const string DeleteCollection = "DeleteCollection";
    public const string DeletePrivateArtifacts = "DeletePrivateArtifacts";
    public const string DeleteProject = "DeleteProject";
    public const string DetachCollection = "DetachCollection";
    public const string ExportHost = "ExportHost";
    public const string MigrateAccount = "MigrateAccount";
    public const string MoveHost = "MoveHost";
    public const string PreMigrateHost = "PreMigrateAccount";
    public const string PrepareCollection = "PrepareCollection";
    public const string PreCreateOrganization = "PreCreateOrganization";
    public const string ReparentCollection = "ReparentCollection";
    public const string UpgradeDatabase = "UpgradeDatabase";
    public const string UpgradeHost = "UpgradeHost";
    public const string UpdateCollectionProperties = "UpdateCollectionProperties";
    public const string UpgradeCollection = "UpgradeCollection";
    public static readonly string[] CollectionLevelOperationClasses = new string[2]
    {
      nameof (CreateProject),
      nameof (DeleteProject)
    };
    public static readonly ReadOnlyDictionary<string, OperationClassConcurrencyLimitInfo> ConcurrencyLimits = new ReadOnlyDictionary<string, OperationClassConcurrencyLimitInfo>((IDictionary<string, OperationClassConcurrencyLimitInfo>) new Dictionary<string, OperationClassConcurrencyLimitInfo>()
    {
      {
        nameof (CreateProject),
        new OperationClassConcurrencyLimitInfo(3, 10, JobPriorityLevel.Normal)
      },
      {
        nameof (DeleteProject),
        new OperationClassConcurrencyLimitInfo(10, 8, JobPriorityLevel.Normal)
      }
    });
  }
}
