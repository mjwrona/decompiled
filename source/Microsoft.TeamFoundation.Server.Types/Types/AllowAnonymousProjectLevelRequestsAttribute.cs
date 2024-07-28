// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.AllowAnonymousProjectLevelRequestsAttribute
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using System;

namespace Microsoft.TeamFoundation.Server.Types
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class AllowAnonymousProjectLevelRequestsAttribute : Attribute
  {
  }
}
