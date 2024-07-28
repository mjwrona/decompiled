// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityExtensionTableValuedParameters
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IdentityExtensionTableValuedParameters
  {
    internal static IdentityExtensionTableValuedParametersBinder[] binders = new IdentityExtensionTableValuedParametersBinder[3]
    {
      new IdentityExtensionTableValuedParametersBinder(),
      (IdentityExtensionTableValuedParametersBinder) new IdentityExtensionTableValuedParametersBinder2(),
      (IdentityExtensionTableValuedParametersBinder) new IdentityExtensionTableValuedParametersBinder3()
    };

    public static SqlParameter BindIdentityExtensionTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows)
    {
      return IdentityExtensionTableValuedParameters.GetBinder().BindIdentityExtensionTable(component, parameterName, rows);
    }

    public static SqlParameter BindIdentityExtensionTableForIdentityComponent11OrLater(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows)
    {
      return IdentityExtensionTableValuedParameters.GetBinder().BindIdentityExtensionTableForIdentityComponent11OrLater(component, parameterName, rows);
    }

    public static SqlParameter BindIdentityExtensionTableForIdentityComponent19OrLater(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      bool allowMetadataUpdates)
    {
      return IdentityExtensionTableValuedParameters.GetBinder().BindIdentityExtensionTableForIdentityComponent19OrLater(component, parameterName, rows, allowMetadataUpdates);
    }

    public static SqlParameter BindIdentityExtensionTableForIdentityComponent21OrLater(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      bool allowMetadataUpdates)
    {
      return IdentityExtensionTableValuedParameters.GetBinder(1).BindIdentityExtensionTableForIdentityComponent19OrLater(component, parameterName, rows, allowMetadataUpdates);
    }

    public static SqlParameter BindIdentityExtensionTableForIdentityComponent35OrLater(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      bool allowMetadataUpdates)
    {
      return IdentityExtensionTableValuedParameters.GetBinder(2).BindIdentityExtensionTableForIdentityComponent19OrLater(component, parameterName, rows, allowMetadataUpdates);
    }

    public static IdentityExtensionTableValuedParametersBinder GetBinder(int version = 0) => IdentityExtensionTableValuedParameters.binders[version];
  }
}
