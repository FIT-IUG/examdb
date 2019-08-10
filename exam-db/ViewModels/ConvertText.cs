
using iDiTect.Converter;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;


namespace exam_db.ViewModels
{
    public class ConvertText
    {

        private string ConvertToText(byte[] bytes)
        {
            var sb = new StringBuilder();

            try
            {
                var reader = new PdfReader(bytes);
                var numberOfPages = reader.NumberOfPages;

                for (var currentPageIndex = 1; currentPageIndex <= numberOfPages; currentPageIndex++)
                {
                    sb.Append(PdfTextExtractor.GetTextFromPage(reader, currentPageIndex));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return sb.ToString();
        }
        public string ToPdf(ReadContent path)
        {
            var bytes = File.ReadAllBytes(path.path);
            string txt = ConvertToText(bytes);
            return txt;
        }
        public string XLS(ReadContent pathh)
        {
            XlsxToTxtConverter converter = new XlsxToTxtConverter();
            string x = "";
            //Load Excel document from stream
            using (Stream stream = File.OpenRead(pathh.path))
            {
                converter.Load(stream);
            }
            x = converter.SaveAsString();
            return x;
        }
        public string CSV(ReadContent path)
        {
            string x = "";
            CsvToTxtConverter converter = new CsvToTxtConverter();

            converter.Load(File.ReadAllText(path.path));
            x = converter.SaveAsString();
            return x;
        }
        public string TXT (ReadContent path)
        {
            string x = File.ReadAllText(path.path);
            return x;
        }
        public string GetTextFromWord(ReadContent pathh)
        {
            StringBuilder text = new StringBuilder();
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            object miss = System.Reflection.Missing.Value;
            object path = pathh.path;
            object readOnly = true;
            Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);

            for (int i = 0; i < docs.Paragraphs.Count; i++)
            {
                text.Append(" \r\n " + docs.Paragraphs[i + 1].Range.Text.ToString());
            }

            return text.ToString();
        }

    }
}