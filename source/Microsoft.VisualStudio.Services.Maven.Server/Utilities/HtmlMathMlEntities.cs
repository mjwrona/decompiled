// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.HtmlMathMlEntities
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  internal static class HtmlMathMlEntities
  {
    public static IReadOnlyDictionary<string, string> DefinedEntities { get; } = HtmlMathMlEntities.BuildFromResources();

    private static IReadOnlyDictionary<string, string> BuildFromResources()
    {
      Assembly assembly = typeof (HtmlMathMlEntities).Assembly;
      using (Stream manifestResourceStream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".htmlmathml.json"))
      {
        using (StreamReader streamReader = new StreamReader(manifestResourceStream))
          return JsonConvert.DeserializeObject<HtmlMathMlEntities.HtmlMathMlJsonFile>(streamReader.ReadToEnd()).Characters;
      }
    }

    private class HtmlMathMlJsonFile
    {
      public IReadOnlyDictionary<string, string> Characters { get; }

      public HtmlMathMlJsonFile(IReadOnlyDictionary<string, string> characters) => this.Characters = characters;
    }
  }
}
