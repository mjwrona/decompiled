// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.IProcessFieldService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [DefaultServiceImplementation(typeof (ProcessFieldService))]
  public interface IProcessFieldService : IVssFrameworkService
  {
    IReadOnlyCollection<ProcessFieldResult> GetFields(
      IVssRequestContext requestContext,
      Guid processId);

    FieldEntry CreateField(
      IVssRequestContext requestContext,
      string name,
      string description,
      InternalFieldType type,
      Guid? processId = null,
      Guid? picklistId = null,
      string referenceName = null);

    FieldEntry UpdateField(
      IVssRequestContext requestContext,
      string fieldNameOrRefName,
      string description = null,
      bool convertToPicklist = false,
      bool? isIdentityFromProcess = null,
      bool makePicklistSuggestedValue = false);

    void DeleteField(IVssRequestContext requestContext, string referenceName);

    FieldEntry EnsureFieldExists(IVssRequestContext requestContext, string fieldReferenceName);

    IDictionary<string, string> GetAllOutOfBoxFieldReferenceNameToNameMappings(
      IVssRequestContext requestContext);

    IReadOnlyCollection<ProcessFieldDefinition> GetAllOutOfBoxFieldDefinitions(
      IVssRequestContext requestContext);

    FieldEntry ConvertFieldWithAllowedValuesToPicklist(
      IVssRequestContext requestContext,
      FieldEntry fieldEntry);

    bool IsSystemField(string fieldRefName);

    FieldEntry RestoreField(IVssRequestContext requestContext, string fieldNameOrRefName);

    FieldEntry SetFieldLocked(
      IVssRequestContext requestContext,
      string fieldNameOrRefName,
      bool isLocked);
  }
}
