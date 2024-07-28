// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ITeamProjectPickerDefaultSelectionProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Client
{
  public interface ITeamProjectPickerDefaultSelectionProvider
  {
    Uri GetDefaultServerUri();

    Guid? GetDefaultCollectionId(Uri instanceUri);

    IEnumerable<string> GetDefaultProjects(Guid collectionId);
  }
}
