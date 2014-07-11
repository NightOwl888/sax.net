// SAXTest.java - test application for SAX2

namespace SaxTest {
  using System;
  using System.IO;
  using System.Text;

  using Sax.Net;
  using Sax.Net.Helpers;

  /// <summary>
  ///   Test class for SAX2.
  /// </summary>
  public class SAXTest : IContentHandler, IErrorHandler {
    void IContentHandler.SetDocumentLocator(ILocator locator) {
      Console.WriteLine("  EVENT: setDocumentLocator");
    }

    public void StartDocument() {
      Console.WriteLine("  EVENT: startDocument");
    }

    public void EndDocument() {
      Console.WriteLine("  EVENT: endDocument");
    }

    public void StartPrefixMapping(String prefix, String uri) {
      Console.WriteLine("  EVENT: startPrefixMapping " + prefix + " = " + uri);
    }

    public void EndPrefixMapping(String prefix) {
      Console.WriteLine("  EVENT: endPrefixMapping " + prefix);
    }

    public void StartElement(String namespaceURI, String localName, String qName, IAttributes atts) {
      Console.WriteLine("  EVENT: startElement " + makeNSName(namespaceURI, localName, qName));
      int attLen = atts.Length;
      for (int i = 0; i < attLen; i++) {
        char[] ch = atts.GetValue(i).ToCharArray();
        Console.WriteLine("    Attribute " + makeNSName(atts.GetUri(i), atts.GetLocalName(i), atts.GetQName(i)) + '='
                          + escapeData(ch, 0, ch.Length));
      }
    }

    public void EndElement(String namespaceURI, String localName, String qName) {
      Console.WriteLine("  EVENT: endElement " + makeNSName(namespaceURI, localName, qName));
    }

    public void Characters(char[] ch, int start, int length) {
      Console.WriteLine("  EVENT: characters " + escapeData(ch, start, length));
    }

    public void IgnorableWhitespace(char[] ch, int start, int length) {
      Console.WriteLine("  EVENT: ignorableWhitespace " + escapeData(ch, start, length));
    }

    public void ProcessingInstruction(String target, String data) {
      Console.WriteLine("  EVENT: processingInstruction " + target + ' ' + data);
    }

    public void SkippedEntity(String name) {
      Console.WriteLine("  EVENT: skippedEntity " + name);
    }

    public void Warning(SAXParseException e) {
      Console.WriteLine("  EVENT: warning " + e.Message + ' ' + e.SystemId + ' ' + e.LineNumber + ' ' + e.ColumnNumber);
    }

    public void Error(SAXParseException e) {
      Console.WriteLine("  EVENT: error " + e.Message + ' ' + e.SystemId + ' ' + e.LineNumber + ' ' + e.ColumnNumber);
    }

    public void FatalError(SAXParseException e) {
      Console.WriteLine("  EVENT: fatal error " + e.Message + ' ' + e.SystemId + ' ' + e.LineNumber + ' '
                        + e.ColumnNumber);
    }

    /// <summary>
    ///   Main application entry point.
    /// </summary>
    /// <param name="args"></param>
    public static void Main(params string[] args) {
      Console.WriteLine("************************************" + "************************************");
      Console.WriteLine("* Testing SAX2");
      Console.WriteLine("************************************" + "************************************");
      Console.Write("\n");

      IXmlReader reader = XmlReaderFactory.Current.CreateXmlReader();
      Console.WriteLine("XMLReader created successfully\n");

      //
      // Check features.
      //
      Console.WriteLine("Checking defaults for some well-known features:");
      checkFeature(reader, "http://xml.org/sax/features/namespaces");
      checkFeature(reader, "http://xml.org/sax/features/namespace-prefixes");
      checkFeature(reader, "http://xml.org/sax/features/string-interning");
      checkFeature(reader, "http://xml.org/sax/features/validation");
      checkFeature(reader, "http://xml.org/sax/features/external-general-entities");
      checkFeature(reader, "http://xml.org/sax/features/external-parameter-entities");
      Console.Write("\n");

      //
      // Assign handlers.
      //
      Console.WriteLine("Creating and assigning handlers\n");
      var handler = new SAXTest();
      reader.ContentHandler = handler;
      reader.ErrorHandler = handler;

      //
      // Parse documents.
      //
      if (args.Length > 0) {
        foreach (string arg in args) {
          String systemId = makeAbsoluteURL(arg);
          Console.WriteLine("Trying file " + systemId);
          try {
            reader.Parse(systemId);
          } catch (SAXException e1) {
            Console.WriteLine(systemId + " failed with XML error: " + e1.Message);
          } catch (IOException e2) {
            Console.WriteLine(systemId + " failed with I/O error: " + e2.Message);
          }
          Console.Write("\n");
        }
      } else {
        Console.WriteLine("No documents supplied on command line; " + "parsing skipped.");
      }

      //
      // Done.
      //
      Console.WriteLine("SAX2 test finished.");
    }

    /// <summary>
    /// Check and display the value of a feature.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="name"></param>
    private static void checkFeature(IXmlReader reader, String name) {
      try {
        Console.WriteLine("  " + name + " = " + reader.GetFeature(name));
      } catch (SAXNotRecognizedException e) {
        Console.WriteLine("XMLReader does not recognize feature " + name);
      } catch (SAXNotSupportedException e) {
        Console.WriteLine("XMLReader recognizes feature " + name + " but does not support checking its value");
      }
    }

    /// <summary>
    ///   Construct an absolute URL if necessary.
    ///   This method is useful for relative file paths on a command
    ///   line; it converts them to absolute file: URLs, using the
    ///   correct path separator.  This method is based on an
    ///   original suggestion by James Clark.
    /// </summary>
    /// <param name="url">The (possibly relative) URL.</param>
    /// <returns>An absolute URL of some sort.</returns>
    private static String makeAbsoluteURL(String url) {
      Uri baseURL;

      String currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
      String fileSep = "/";
      String file = currentDirectory.Replace(fileSep[0], '/') + '/';

      if (file[0] != '/') {
        file = "/" + file;
      }

      try {
        baseURL = new Uri("file:" + file);
        return new Uri(baseURL, url).ToString();
      } catch (UriFormatException e) {
        Console.Error.Write(url + ": " + e);
        return url;
      }
    }

    private static String makeNSName(String uri, String localName, String qName) {
      if (uri.Equals("")) {
        uri = "[none]";
      }
      if (localName.Equals("")) {
        localName = "[none]";
      }
      if (qName.Equals("")) {
        qName = "[none]";
      }
      return uri + '/' + localName + '/' + qName;
    }

    private static String escapeData(char[] ch, int start, int length) {
      var buf = new StringBuilder();
      for (int i = start; i < start + length; i++) {
        switch (ch[i]) {
          case '\n':
            buf.Append("\\n");
            break;
          case '\t':
            buf.Append("\\t");
            break;
          case '\r':
            buf.Append("\\r");
            break;
          default:
            buf.Append(ch[i]);
            break;
        }
      }
      return buf.ToString();
    }
  }

  // end of SAXTest.java
}
