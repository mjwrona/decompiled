// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent25
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent25 : PublishedExtensionComponent24
  {
    public override PublishedExtension ProcessValidationResult(
      Guid extensionId,
      string version,
      Guid validationId,
      string message,
      bool success,
      int? fileId)
    {
      this.PrepareStoredProcedure("Gallery.prc_ProcessValidationResult");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid(nameof (validationId), validationId);
      this.BindString(nameof (message), message, 4000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean(nameof (success), success);
      this.BindNullableInt(nameof (fileId), fileId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_ProcessValidationResult", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        return this.ProcessExtensionResult(resultCollection, ExtensionQueryFlags.None).Values.FirstOrDefault<PublishedExtension>();
      }
    }

    public override ExtensionValidationResult GetValidationResult(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetValidationResult");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetValidationResult", requestContext))
      {
        resultCollection.AddBinder<ExtensionValidationResult>((ObjectBinder<ExtensionValidationResult>) new ExtensionValidationResultBinder());
        List<ExtensionValidationResult> items = resultCollection.GetCurrent<ExtensionValidationResult>().Items;
        return items.Count == 0 ? (ExtensionValidationResult) null : items.First<ExtensionValidationResult>();
      }
    }
  }
}
