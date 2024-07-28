// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.IProcessTemplatePlugin
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  [InheritedExport]
  public interface IProcessTemplatePlugin
  {
    string PluginName { get; }

    bool IsMatch(string pluginName);

    ProcessTemplateValidatorResult Validate(
      IVssRequestContext requestContext,
      IDictionary<string, object> templateContext,
      Guid templateTypeId,
      IProcessTemplatePackage templatePackage,
      string taskListXmlPath,
      XDocument taskListXml,
      bool isTfsMigratorValidationContext = false,
      Dictionary<string, object> optionalParameters = null);
  }
}
