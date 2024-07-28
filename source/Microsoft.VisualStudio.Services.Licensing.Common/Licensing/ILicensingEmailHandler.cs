// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ILicensingEmailHandler
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.EmailNotification;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [InheritedExport]
  public interface ILicensingEmailHandler
  {
    void SendEmailNotification(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      INotificationEmailData data);
  }
}
