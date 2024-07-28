// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryMacroExtension
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [InheritedExport]
  public abstract class QueryMacroExtension
  {
    public static void DefaultValidate(
      string macroName,
      NodeTableName tableContext,
      NodeFieldName fieldContext,
      NodeParameters parameters)
    {
      if (parameters.Arguments.Any<NodeItem>())
        throw new SyntaxException(ServerResources.MacroDoesNotAcceptParameters((object) macroName));
    }

    public virtual bool IsMatch(string macroName) => TFStringComparer.WorkItemQueryText.Equals(this.Name, macroName);

    public virtual void Validate(
      IVssRequestContext requestContext,
      NodeTableName tableContext,
      NodeFieldName fieldContext,
      NodeParameters parameters)
    {
      QueryMacroExtension.DefaultValidate(this.Name, tableContext, fieldContext, parameters);
    }

    public virtual bool Rewrite(
      IVssRequestContext requestContext,
      NodeCondition condition,
      out Node rewritten)
    {
      rewritten = (Node) null;
      return false;
    }

    public abstract object GetValue(
      IVssRequestContext requestContext,
      string macro,
      ProjectInfo project,
      WebApiTeam team = null,
      NodeParameters parameters = null,
      TimeZone timeZone = null,
      CultureInfo cultureInfo = null);

    public abstract string Name { get; }

    public abstract object DefaultValue { get; }

    public abstract DataType DataType { get; }

    public virtual bool DoesMacroExtensionHandleOffset => false;
  }
}
