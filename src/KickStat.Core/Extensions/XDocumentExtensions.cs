using System.Text;
using System.Xml.Linq;


namespace KickStat;

public static class XDocumentExtensions
{
    public static string? ToStringWithXmlDeclaration(this XDocument? xDocument, SaveOptions options = SaveOptions.None)
    {
        if (xDocument == null)
            return null;

        var sBuilder = new StringBuilder();
        using (var writer = new StringWriter(sBuilder))
            xDocument.Save(writer, options);

        return sBuilder.ToString();
    }
}