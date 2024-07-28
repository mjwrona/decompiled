// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.OnPrem.ITfvcConverter
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B5DEEFA-3C5E-4BFB-92E2-3ADDA47952C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.OnPrem.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;

namespace Microsoft.TeamFoundation.CodeSense.Platform.OnPrem
{
  public interface ITfvcConverter
  {
    Item GetItem(string pathString, TfvcVersionDescriptor versionDescriptor, TfvcItem item);

    TfvcChange GetTfvcChange(Change change);

    TfvcChangeset GetTfvcChangeset(Changeset changeset);

    TfvcItemsCollection GetTfvcItem(Item item);
  }
}
