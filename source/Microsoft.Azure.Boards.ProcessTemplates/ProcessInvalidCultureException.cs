// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessInvalidCultureException
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using System;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class ProcessInvalidCultureException : ProcessTemplateServiceException
  {
    public ProcessInvalidCultureException(
      string cultureName,
      int oemCodePage,
      Exception innerException)
      : base(Resources.ProcessInvalidCultureException((object) oemCodePage, (object) cultureName), innerException)
    {
      this.CultureName = cultureName;
      this.OemCodePage = oemCodePage;
    }

    public string CultureName { get; private set; }

    public int OemCodePage { get; private set; }
  }
}
