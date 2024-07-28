// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.IProcessTemplatePackage
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using System;
using System.Xml.Linq;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public interface IProcessTemplatePackage
  {
    string Name { get; }

    Guid TypeId { get; }

    ProcessVersion Version { get; }

    XDocument RootDocument { get; }

    XDocument GetDocument(string packagePath);

    bool TryGetDocument(string packagePath, out XDocument doc);

    void UpdateDocument(string packagePath, XDocument document);
  }
}
