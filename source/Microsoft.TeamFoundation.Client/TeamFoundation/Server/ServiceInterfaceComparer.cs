// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.ServiceInterfaceComparer
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server
{
  internal class ServiceInterfaceComparer : IComparer<ServiceInterface>
  {
    public int Compare(ServiceInterface first, ServiceInterface second) => VssStringComparer.ServiceInterface.Compare(first.Name, second.Name);
  }
}
