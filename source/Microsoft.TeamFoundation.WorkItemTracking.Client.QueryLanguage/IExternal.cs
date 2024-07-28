// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.IExternal
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IExternal
  {
    object FindTable(string name);

    object FindField(string name, string prefix, object tableTag);

    DataType GetFieldDataType(object fieldTag);

    void VerifyNode(Node node, NodeTableName tableContext, NodeFieldName fieldContext);

    Node OptimizeNode(Node node, NodeTableName tableContext, NodeFieldName fieldContext);

    object FindVariable(string name, NodeParameters parameters = null);

    DataType GetVariableDataType(string name);

    bool DoesMacroExtensionHandleOffset(string name);

    CultureInfo CultureInfo { get; }

    TimeZone TimeZone { get; }

    void ValidateParameters(
      string macroName,
      NodeTableName tableContext,
      NodeFieldName fieldContext,
      NodeParameters parameters);
  }
}
