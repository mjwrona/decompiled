// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.ExtensionRequestComponent2
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components
{
  internal class ExtensionRequestComponent2 : ExtensionRequestComponent
  {
    public override ResultCollection DeleteExtensionRequests(
      List<RequestedExtension> requestedExtensions)
    {
      try
      {
        this.PrepareStoredProcedure("Extension.prc_DeleteExtensionRequests");
        this.BindExtensionRequestTable("@extensionRequests", (IEnumerable<RequestedExtension>) requestedExtensions);
        this.ExecuteNonQuery();
        return (ResultCollection) null;
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (DeleteExtensionRequests));
      }
    }
  }
}
