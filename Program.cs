using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Filespec;
using iText.Forms.Fields;
using iText.Forms;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Drawing;
using iText.Kernel.Geom;
using Org.BouncyCastle.Security;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Colors;



namespace EmbedU3DinPDF
{
    internal class Program
    {
        static void WriteU3DToPDF(string u3d, string inputPDF="template.pdf")
        {
            string outputPDF = u3d.Split('.')[0] + ".pdf";
            using (PdfReader reader = new PdfReader(inputPDF))
            {
                using (PdfWriter writer = new PdfWriter(outputPDF))
                {
                    using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
                    {
                        PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                        PdfFormField formField = form.GetField("VIEW1");
                        Console.WriteLine(formField);
                        //form.AddField(button);
                        PdfPage firstPage = pdfDoc.GetPage(1);
                        var pageSize = firstPage.GetPageSize();
                        float width = pageSize.GetWidth();
                        float height = pageSize.GetHeight();
                        var rect = new iText.Kernel.Geom.Rectangle(50, 50, 925, 430);
                        PdfStream stream3D = new PdfStream(pdfDoc, new FileStream(u3d, FileMode.Open));
                        stream3D.Put(PdfName.Type, new PdfName("3D"));
                        stream3D.Put(PdfName.Subtype, new PdfName("U3D"));
                        stream3D.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
                        stream3D.Flush();

                        PdfDictionary dict3D = new PdfDictionary();
                        dict3D.Put(PdfName.Type, new PdfName("3DView"));
                        dict3D.Put(new PdfName("XN"), new PdfString("Default"));
                        dict3D.Put(new PdfName("IN"), new PdfString("Unnamed"));
                        dict3D.Put(new PdfName("MS"), PdfName.M);
                        dict3D.Put(new PdfName("C2W"), new PdfArray(new float[] { 1, 0, 0, 0, 0, -1, 0, 1, 0, 3, -235, 28 }));
                        dict3D.Put(PdfName.CO, new PdfNumber(235));


                        Pdf3DAnnotation annot = new Pdf3DAnnotation(rect, stream3D);
                        annot.SetContents(new PdfString("3D Model"));
                        annot.SetDefaultInitialView(dict3D);
                        //PdfPage page = pdfDoc.AddNewPage();
                        PageSize size = new PageSize(width, height);
                        PdfPage page = pdfDoc.GetPage(1).AddAnnotation(annot);
                        //PdfPage page = pdfDoc.AddNewPage(size).AddAnnotation(annot);
                        PdfCanvas canvas = new PdfCanvas(page);
                        canvas.SetFillColor(ColorConstants.GRAY);
                        canvas.Rectangle(rect);
                        canvas.Fill();
                        pdfDoc.Close();
                    }
                }
            }
        }
        static int Main(string[] args)
        {
            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
            if (args.Length < 1)
            {
                Console.WriteLine("Not enough args");
                Console.WriteLine("ARGS: <EmbedU3DinPDF.exe> <U3D File Path> {optional} <PDF Template Name>");
                Console.WriteLine("PDF Template Name Defaults to template.pdf");
                return 1;
            }
            WriteU3DToPDF(args[0]);
            return 0;
        }
    }
}
