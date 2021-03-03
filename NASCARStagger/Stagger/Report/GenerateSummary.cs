using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stagger.Objects;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Stagger.Report
{
    class GenerateSummary
    {
        public GenerateSummary()
        { 
                
        }

        public void Generate(String filepath,List<DnumberSizeCount> D1 ,List<DnumberSizeCount> D2,String D1Name,String D2Name,List<Set> CalculatedSets)
        {
            Document document = new Document();

            FileStream newFileStream = new FileStream(filepath, FileMode.Create);

            PdfWriter.GetInstance(document, newFileStream);
            
            document.Open();
     
            PdfPTable datatable = new PdfPTable(3);
            datatable.DefaultCell.Padding = 0;
   

            float[] headerwidths = { 30, 30, 40 }; // percentage
            datatable.SetWidths(headerwidths);
            datatable.WidthPercentage = 100; // percentage

            datatable.DefaultCell.BorderWidth = 0;
            datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            datatable.AddCell("Dnumber #1: " + D1Name);
            datatable.AddCell("Dnumber #2: " + D2Name);

            if (CalculatedSets != null)
                datatable.AddCell("Sets");
            else
                datatable.AddCell("");
           
            datatable.HeaderRows = 1;  // this is the end of the table header
            datatable.DefaultCell.Border = 0;

            PdfPCell tempcell1 = new PdfPCell();
            tempcell1.Border = 0;
            tempcell1.BorderWidth = 0;
            tempcell1.AddElement(LoadDnumber(D1));
            datatable.AddCell(tempcell1);

            PdfPCell tempcell2 = new PdfPCell();
            tempcell2.Border = 0;
            tempcell2.BorderWidth = 0;
            tempcell2.AddElement(LoadDnumber(D2));
            datatable.AddCell(tempcell2);
            
            PdfPCell tempcell3 = new PdfPCell();
            tempcell3.Border = 0;
            tempcell3.BorderWidth = 0;
            
            if (CalculatedSets != null)
                tempcell3.AddElement(LoadCalc(CalculatedSets, D1Name, D2Name));
            
            datatable.AddCell(tempcell3);

            document.Add(datatable);

            document.Close();

            return;
        }
       
        public void GenerateSingle(String filepath, List<DnumberSizeCount> D1 ,String D1Name,List<Set> CalculatedSets)
        {
            Document document = new Document();

            FileStream newFileStream = new FileStream(filepath, FileMode.Create);

            PdfWriter.GetInstance(document, newFileStream);

            document.Open();

            PdfPTable datatable = new PdfPTable(2);

            datatable.DefaultCell.Padding = 2;
            float[] headerwidths = { 30, 40 }; // percentage
            datatable.SetWidths(headerwidths);
            datatable.WidthPercentage = 100; // percentage

            datatable.DefaultCell.BorderWidth = 0;
            datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            datatable.AddCell("Dnumber #1: " + D1Name);
           
            if (CalculatedSets != null)
                datatable.AddCell("Sets");
            else
                datatable.AddCell("");

            datatable.HeaderRows = 1;  // this is the end of the table header
            datatable.DefaultCell.Border = 0;

            PdfPCell tempcell1 = new PdfPCell();
            tempcell1.Border = 0;
            tempcell1.BorderWidth = 0;
            tempcell1.AddElement(LoadDnumber(D1));
            datatable.AddCell(tempcell1);

            PdfPCell tempcell3 = new PdfPCell();
            tempcell3.Border = 0;
            tempcell3.BorderWidth = 0;
          
            if (CalculatedSets != null)
                tempcell3.AddElement(LoadCalc(CalculatedSets, D1Name, ""));

            datatable.AddCell(tempcell3);

            document.Add(datatable);

            document.Close();

            return;
        }

        private PdfPTable LoadCalc(List<Set> CalculatedSets, String D1Name, String D2Name)
        {
            PdfPTable datatable = new PdfPTable(4);

            datatable.DefaultCell.Padding = 1;
            float[] headerwidths = { 25, 25, 25,25 }; // percentage
            datatable.SetWidths(headerwidths);
            datatable.WidthPercentage = 100; // percentage

            datatable.DefaultCell.BorderWidth = 0;
            datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            datatable.AddCell("Sets");
            datatable.AddCell(D1Name);
            datatable.AddCell(D2Name);
            datatable.AddCell("Stagger");

            datatable.HeaderRows = 1;  // this is the end of the table header

            datatable.DefaultCell.BorderWidth = 1;

            int max = CalculatedSets.Count;
            for (int i = 0; i < max; i++)
            {
                datatable.AddCell(CalculatedSets[i].SetCount.ToString());
                datatable.AddCell(CalculatedSets[i].LeftSize.ToString());
                datatable.AddCell(CalculatedSets[i].RightSize.ToString());
                datatable.AddCell(CalculatedSets[i].Stagger.ToString());
            }

            return datatable;
        }

        private PdfPTable LoadDnumber(List<DnumberSizeCount> D1)
        {
            
            // we add some meta information to the document

            PdfPTable datatable = new PdfPTable(3);

            datatable.DefaultCell.Padding = 1;
            float[] headerwidths = { 33,33,33 }; // percentage
            datatable.SetWidths(headerwidths);
            datatable.WidthPercentage = 100; // percentage

            datatable.DefaultCell.BorderWidth = 0;
            datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            datatable.AddCell("Size");
            datatable.AddCell("Count");
            datatable.AddCell("Used");

            datatable.HeaderRows = 1;  // this is the end of the table header

            datatable.DefaultCell.BorderWidth = 1;

            int max = D1.Count;
            for (int i = 0; i < max; i++)
            {
                    datatable.AddCell(D1[i].Size.ToString());
                    datatable.AddCell(D1[i].Count.ToString());
                    datatable.AddCell(D1[i].Used.ToString());
            }

            return datatable;
        }

      
    }
}
