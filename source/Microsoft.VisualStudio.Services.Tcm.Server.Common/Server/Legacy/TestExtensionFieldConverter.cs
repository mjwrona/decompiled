// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestExtensionFieldConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal static class TestExtensionFieldConverter
  {
    public static Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField Convert(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField field)
    {
      if (field == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField()
      {
        Field = TestExtensionFieldConverter.Convert(field.Field),
        Value = field.Value
      };
    }

    public static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField Convert(
      Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField field)
    {
      if (field == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField()
      {
        Field = TestExtensionFieldConverter.Convert(field.Field),
        Value = field.Value
      };
    }

    public static IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField> fields)
    {
      return fields == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField>) null : fields.Select<Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField>) (field => TestExtensionFieldConverter.Convert(field)));
    }

    public static IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField> fields)
    {
      return fields == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField>) null : fields.Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField, Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField, Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField>) (field => TestExtensionFieldConverter.Convert(field)));
    }

    public static Microsoft.TeamFoundation.TestManagement.Server.TestExtensionFieldDetails Convert(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionFieldDetails details)
    {
      if (details == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestExtensionFieldDetails) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestExtensionFieldDetails()
      {
        Id = details.Id,
        Name = details.Name,
        Type = details.Type,
        IsRunScoped = details.IsRunScoped,
        IsResultScoped = details.IsResultScoped,
        IsSystemField = details.IsSystemField
      };
    }

    public static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionFieldDetails Convert(
      Microsoft.TeamFoundation.TestManagement.Server.TestExtensionFieldDetails details)
    {
      if (details == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionFieldDetails) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionFieldDetails()
      {
        Id = details.Id,
        Name = details.Name,
        Type = details.Type,
        IsRunScoped = details.IsRunScoped,
        IsResultScoped = details.IsResultScoped,
        IsSystemField = details.IsSystemField
      };
    }
  }
}
