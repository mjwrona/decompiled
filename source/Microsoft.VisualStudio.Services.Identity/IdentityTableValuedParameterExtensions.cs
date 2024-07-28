// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityTableValuedParameterExtensions
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IdentityTableValuedParameterExtensions
  {
    private static IdentityTableValuedParametersBinder binder;
    private static readonly IdentityTableValuedParametersBinder DefaultBinder = new IdentityTableValuedParametersBinder();

    public static SqlParameter BindIdentityTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParameterExtensions.Binder.BindIdentityTable(component, parameterName, rows, propertiesToUpdate);
    }

    public static SqlParameter BindIdentityTable2(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParameterExtensions.Binder.BindIdentityTable2(component, parameterName, rows, propertiesToUpdate);
    }

    public static SqlParameter BindIdentityTable3(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParameterExtensions.Binder.BindIdentityTable3(component, parameterName, rows, propertiesToUpdate);
    }

    public static SqlParameter BindIdentityTable4(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParameterExtensions.Binder.BindIdentityTable4(component, parameterName, rows, propertiesToUpdate);
    }

    public static SqlParameter BindIdentityTable5(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParameterExtensions.Binder.BindIdentityTable5(component, parameterName, rows, propertiesToUpdate);
    }

    public static SqlParameter BindIdentityTable6(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParameterExtensions.Binder.BindIdentityTable6(component, parameterName, rows, propertiesToUpdate);
    }

    public static SqlParameter BindIdentityTable7(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParameterExtensions.Binder.BindIdentityTable7(component, parameterName, rows, propertiesToUpdate);
    }

    public static SqlParameter BindIdentityTable9(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParameterExtensions.Binder.BindIdentityTable9(component, parameterName, rows, propertiesToUpdate);
    }

    public static SqlParameter BindIdentityTable10(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate,
      IdentityBindingConfig bindingConfig)
    {
      return IdentityTableValuedParameterExtensions.Binder.BindIdentityTable10(component, parameterName, rows, propertiesToUpdate, bindingConfig);
    }

    public static SqlParameter BindIdentityTable12(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate,
      IdentityBindingConfig bindingConfig)
    {
      return IdentityTableValuedParameterExtensions.Binder.BindIdentityTable12(component, parameterName, rows, propertiesToUpdate, bindingConfig);
    }

    public static IdentityTableValuedParametersBinder Binder
    {
      get => IdentityTableValuedParameterExtensions.binder ?? IdentityTableValuedParameterExtensions.DefaultBinder;
      set => IdentityTableValuedParameterExtensions.binder = value;
    }
  }
}
