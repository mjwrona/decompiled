// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildCompatApiController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  public abstract class BuildCompatApiController : BuildApiController
  {
    protected T ExecuteCompatMethod<T>(
      DefinitionType? type,
      Func<T> buildMethod,
      Func<T> xamlMethod)
    {
      if (!type.HasValue)
      {
        try
        {
          return buildMethod();
        }
        catch (BuildNotFoundException ex)
        {
          return xamlMethod();
        }
      }
      else
      {
        DefinitionType? nullable = type;
        DefinitionType definitionType = DefinitionType.Xaml;
        return nullable.GetValueOrDefault() == definitionType & nullable.HasValue ? xamlMethod() : buildMethod();
      }
    }

    protected void ExecuteCompatMethod(DefinitionType? type, Action buildMethod, Action xamlMethod)
    {
      if (!type.HasValue)
      {
        try
        {
          buildMethod();
        }
        catch (BuildNotFoundException ex)
        {
          xamlMethod();
        }
      }
      else
      {
        DefinitionType? nullable = type;
        DefinitionType definitionType = DefinitionType.Xaml;
        if (nullable.GetValueOrDefault() == definitionType & nullable.HasValue)
          xamlMethod();
        else
          buildMethod();
      }
    }
  }
}
