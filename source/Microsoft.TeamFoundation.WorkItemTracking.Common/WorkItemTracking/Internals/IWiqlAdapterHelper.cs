// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.IWiqlAdapterHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IWiqlAdapterHelper
  {
    CultureInfo CultureInfo { get; }

    TimeZone TimeZone { get; }

    string UserDisplayName { get; }

    int GetTreeID(string path, TreeStructureType type);

    bool IsSupported(string feature);

    object FindField(string name, string prefix, object tableTag);

    int GetFieldId(object fieldTag);

    string GetFieldReferenceName(object fieldTag);

    string GetFieldFriendlyName(object fieldTag);

    bool GetFieldIsQueryable(object fieldTag);

    bool GetFieldCanSortBy(object fieldTag);

    InternalFieldUsages GetFieldUsage(object fieldTag);

    InternalFieldType GetFieldType(object fieldTag);

    Type GetFieldSystemType(object fieldTag);

    bool GetFieldSupportsTextQuery(object fieldTag);

    bool GetFieldIsLongText(object fieldTag);

    string GetFieldFriendlyName(string fieldName);

    int GetFieldPsFieldType(string fieldName);

    InternalFieldType GetFieldType(string fieldName);

    bool HasLinkType(string linkTypeName);

    int GetLinkTypeId(string linkTypeName);

    bool GetLinkTypeIsForward(string linkTypeName);

    int GetLinkTypeTopology(string linkTypeName);

    IEnumerable<int> GetAllLinkTypeIds();

    List<object> GetSortFieldList(NodeSelect nodeSelect);

    List<object> GetDisplayFieldList(NodeSelect nodeSelect);

    void SetDisplayFieldList(NodeSelect nodeSelect, IEnumerable<object> list);

    void SetSortFieldList(NodeSelect nodeSelect, IEnumerable<object> list);

    DataType GetVariableType(string name);

    object GetVariableValue(string name, NodeParameters parameters);

    bool RewriteCondition(NodeCondition condition, out Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node rewritten);

    bool DoesMacroExtensionHandleOffset(string macroName);

    bool IsSupportedMacro(string name);

    void ValidateParameters(
      string macroName,
      NodeTableName tableContext,
      NodeFieldName fieldContext,
      NodeParameters parameters);
  }
}
