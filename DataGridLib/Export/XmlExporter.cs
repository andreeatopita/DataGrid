using DataGridLib.Export.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DataGridLib.Export;

namespace DataGridLib.Export;


public class XmlExporter : IGridExporter
{
    public string Extension => "xml";

    public void Export(IReadOnlyList<string> headers, IReadOnlyList<string[]> rows, string filePath, GridPage? pageExp = null)
    {
        //export ca si XML
        var setting = new XmlWriterSettings
        {
            Indent = true
        };

        try
        {
            //scrie direct in fisierul de la filepath
            using XmlWriter xml = XmlWriter.Create(filePath, setting);
            xml.WriteStartDocument();

            xml.WriteStartElement("DataGrid");

            if (pageExp != null)
            {
                GridPage c = pageExp;
                xml.WriteAttributeString("page", c.CurrentPage.ToString());
                xml.WriteAttributeString("totalPages", c.TotalPages.ToString());
                xml.WriteAttributeString("pageSize", c.PageSize.ToString());
                xml.WriteAttributeString("totalItems", c.TotalItems.ToString());
                xml.WriteAttributeString("itemsOnPage", rows.Count.ToString());
            }

            foreach (string[] row in rows)
            {
                xml.WriteStartElement("Row");
                //pt fiecare coloana din header
                for (int i = 0; i < headers.Count; i++)
                {
                    string tag = NormalizeHeader(headers[i]);
                    xml.WriteStartElement(tag);
                    //textul sau null ( sir gol )
                    xml.WriteString(row[i] ?? string.Empty);
                    xml.WriteEndElement(); //inchid tag 
                }
                xml.WriteEndElement(); //row
            }
            xml.WriteEndElement(); //grid
            xml.WriteEndDocument();
        }
        catch(IOException ex)
        {
            Console.Error.WriteLine($"Cannot write XML {filePath}: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.Error.WriteLine($"Access denied for {filePath}: {ex.Message}");
        }
    }

    private static string NormalizeHeader(string header)
    {
        //daca headerul e gol:column
        if (string.IsNullOrWhiteSpace(header))
            header = "Column";

        //spatiu:underscore
        header = header.Replace(" ", "_");

        //tai # si pun num 
        if (header.StartsWith("#"))
            header = "Num" + header.Substring(1);

        return header;
    }
}
