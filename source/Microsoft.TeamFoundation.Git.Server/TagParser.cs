// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TagParser
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class TagParser
  {
    public static void Parse(Stream contentStream, ITagParserHandler handler)
    {
      StreamReader streamReader = new StreamReader(contentStream, Encoding.UTF8);
      string line;
      TagParser.ReadLineAndHeader(streamReader, "object", true, out line);
      string sha1IdString = line.Substring(0, 40);
      Sha1Id objectId;
      try
      {
        objectId = new Sha1Id(sha1IdString);
      }
      catch (ArgumentException ex)
      {
        throw new TagObjectFailedToParseException("Tag parse failed due to invalid object id of " + sha1IdString + ".");
      }
      TagParser.ReadLineAndHeader(streamReader, "type", true, out line);
      GitPackObjectType packType;
      try
      {
        packType = GitObjectTypeExtensions.GetPackType(line);
      }
      catch (InvalidOperationException ex)
      {
        throw new TagObjectFailedToParseException("Tag parse failed due to invalid object type of " + line + ".");
      }
      handler.OnReferencedObject(new ObjectIdAndType(objectId, packType));
      TagParser.ReadLineAndHeader(streamReader, "tag", true, out line);
      handler.OnName(line);
      if (TagParser.ReadLineAndHeader(streamReader, "tagger", false, out line))
      {
        IdentityAndDate result;
        if (!IdentityAndDate.TryParse(line, out result))
          throw new TagObjectFailedToParseException("Tag parse failed due to invalid identity id of " + line + ".");
        handler.OnTagger(result);
      }
      while (!string.IsNullOrEmpty(line))
        line = streamReader.ReadLine();
      handler.OnComment(streamReader.ReadToEnd());
    }

    private static bool ReadLineAndHeader(
      StreamReader streamReader,
      string expectedHeader,
      bool throwIfDifferent,
      out string line)
    {
      line = streamReader.ReadLine();
      if (string.IsNullOrEmpty(line))
      {
        if (throwIfDifferent)
          throw new TagObjectFailedToParseException("Tag parse failed due to unexpected null or empty tag header when we expected " + expectedHeader);
        return false;
      }
      int length = line.IndexOf(' ');
      if (length < 0 || length == line.Length - 1)
        throw new TagObjectFailedToParseException("Tag parse failed due to missing space separator. Line: " + line);
      string str = line.Substring(0, length);
      if (!expectedHeader.Equals(str, StringComparison.Ordinal))
      {
        if (throwIfDifferent)
          throw new TagObjectFailedToParseException("Tag parse failed due to different header than expected. Expected: " + expectedHeader + " Actual: " + str);
        return false;
      }
      line = line.Substring(length + 1);
      return true;
    }
  }
}
