// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.IntegrationSqlResourceComponent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal abstract class IntegrationSqlResourceComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static IntegrationSqlResourceComponent()
    {
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400013, new SqlExceptionFactory(typeof (CreateACENoObjectException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400014, new SqlExceptionFactory(typeof (CreateACENoActionException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400015, new SqlExceptionFactory(typeof (UnregisterProjectException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400016, new SqlExceptionFactory(typeof (RegisterProjectException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400017, new SqlExceptionFactory(typeof (InternalStoredProcedureException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400018, new SqlExceptionFactory(typeof (RegisterObjectExistsException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400019, new SqlExceptionFactory(typeof (RegisterObjectNoClassException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400020, new SqlExceptionFactory(typeof (RegisterObjectNoProjectException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400035, new SqlExceptionFactory(typeof (RegisterObjectProjectMismatchException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400021, new SqlExceptionFactory(typeof (RegisterObjectBadParentException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400027, new SqlExceptionFactory(typeof (ClassIdDoesNotExistException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400028, new SqlExceptionFactory(typeof (SecurityObjectDoesNotExistException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400029, new SqlExceptionFactory(typeof (SecurityActionDoesNotExistException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400033, new SqlExceptionFactory(typeof (BadParentObjectClassIdException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400034, new SqlExceptionFactory(typeof (CircularObjectInheritanceException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400032, new SqlExceptionFactory(typeof (AddProjectGroupProjectMismatchException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new AddProjectGroupProjectMismatchException(sqEr.ExtractString("group_name"), sqEr.ExtractString("member_name")))));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(400030, new SqlExceptionFactory(typeof (DeleteACEException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450000, new SqlExceptionFactory(typeof (NodeDoesNotExistException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450001, new SqlExceptionFactory(typeof (ProjectDoesNotExistException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ProjectDoesNotExistException(sqEr.ExtractString("project_id")))));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450002, new SqlExceptionFactory(typeof (ParentNodeDoesNotExistException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450003, new SqlExceptionFactory(typeof (ReclassificationNodeDoesNotExistException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450004, new SqlExceptionFactory(typeof (ProjectAlreadyExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ProjectAlreadyExistsException(sqEr.ExtractString("project_name")))));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450005, new SqlExceptionFactory(typeof (NodeAlreadyExistsException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450006, new SqlExceptionFactory(typeof (CannotModifyRootNodeException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450007, new SqlExceptionFactory(typeof (MoveArgumentOutOfRangeException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450008, new SqlExceptionFactory(typeof (CircularNodeReferenceException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450009, new SqlExceptionFactory(typeof (CannotChangeTreesException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450010, new SqlExceptionFactory(typeof (MaximumDepthExceededException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450011, new SqlExceptionFactory(typeof (ReclassifiedToDifferentTreeException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450012, new SqlExceptionFactory(typeof (ReclassifiedToSubTreeException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450013, new SqlExceptionFactory(typeof (ProjectNameNotRecognizedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ProjectNameNotRecognizedException(sqEr.ExtractString("project_name")))));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(450014, new SqlExceptionFactory(typeof (CannotAddDateToNonIterationException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(470006, new SqlExceptionFactory(typeof (SyncBadBaselineRevServiceException)));
      IntegrationSqlResourceComponent.s_sqlExceptionFactories.Add(470007, new SqlExceptionFactory(typeof (SyncSupersededBaselineRevServiceException)));
    }

    protected IntegrationSqlResourceComponent() => this.ContainerErrorCode = 50000;

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) IntegrationSqlResourceComponent.s_sqlExceptionFactories;

    public SqlParameter BindUniqueIdentifier(string parameterName, string id) => this.BindString(parameterName, id, 128, false, SqlDbType.VarChar);
  }
}
