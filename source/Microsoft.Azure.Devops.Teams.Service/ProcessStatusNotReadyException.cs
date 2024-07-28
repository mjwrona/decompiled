// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Teams.Service.ProcessStatusNotReadyException
// Assembly: Microsoft.Azure.Devops.Teams.Service, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 643BB522-27FA-4CE7-8A1E-31D95F8AC409
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Teams.Service.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Devops.Teams.Service
{
  [Serializable]
  public class ProcessStatusNotReadyException : TeamFoundationServiceException
  {
    public ProcessStatusNotReadyException(string templateName)
      : base(Resources.ProcessTemplateStatusNotReady((object) templateName), 402369)
    {
    }
  }
}
