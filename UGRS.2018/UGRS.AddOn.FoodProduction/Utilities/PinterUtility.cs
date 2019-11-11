using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UGRS.AddOn.FoodProduction.Utilities
{
    public class PinterUtility
    {
        private static int gIntCurrentPageIndex;
        private static IList<Stream> gLstStreams;

       // [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        //public static extern bool SetDefaultPrinter(string pStrPrinterName);

        ///// <summary>
        ///// Verifica el estatus de la impresora.
        ///// </summary>
        ///// <param name="pStrPrinterName">Nombre de la impresora</param>
        //private static void PrinterStatus(string pStrPrinterName)
        //{
        //    PrintQueue lObjDefaultPrintQueue = null;
        //    try
        //    {
        //        lObjDefaultPrintQueue = LocalPrintServer.GetDefaultPrintQueue();

        //        if (lObjDefaultPrintQueue != null)
        //        {
        //            lObjDefaultPrintQueue.Refresh();

        //            PrintQueueStatus lEnmStatus = lObjDefaultPrintQueue.QueueStatus;

        //            if ((lEnmStatus & PrintQueueStatus.PaperProblem) == PrintQueueStatus.PaperProblem)
        //            {
        //                throw new Exception("La impresora tiene un problema con el papel");
        //            }
        //            if ((lEnmStatus & PrintQueueStatus.NoToner) == PrintQueueStatus.NoToner)
        //            {
        //                throw new Exception("La impresora no tiene toner");
        //            }
        //            if ((lEnmStatus & PrintQueueStatus.DoorOpen) == PrintQueueStatus.DoorOpen)
        //            {
        //                throw new Exception("La impresora tiene la puerta abierta");
        //            }
        //            if ((lEnmStatus & PrintQueueStatus.Error) == PrintQueueStatus.Error)
        //            {
        //                throw new Exception("La impresora tiene un estado de error");
        //            }
        //            if ((lEnmStatus & PrintQueueStatus.NotAvailable) == PrintQueueStatus.NotAvailable)
        //            {
        //                throw new Exception("La impresora no esta disponible");
        //            }
        //            if ((lEnmStatus & PrintQueueStatus.Offline) == PrintQueueStatus.Offline)
        //            {
        //                throw new Exception("La impresora está fuera de línea");
        //            }
        //            if ((lEnmStatus & PrintQueueStatus.OutOfMemory) == PrintQueueStatus.OutOfMemory)
        //            {
        //                throw new Exception("La impresora no tiene memoria suficiente");
        //            }
        //            if ((lEnmStatus & PrintQueueStatus.PaperOut) == PrintQueueStatus.PaperOut)
        //            {
        //                throw new Exception("La impresora no tiene papel");
        //            }
        //            if ((lEnmStatus & PrintQueueStatus.OutputBinFull) == PrintQueueStatus.OutputBinFull)
        //            {
        //                throw new Exception("Has a full output bin");
        //            }
        //            if ((lEnmStatus & PrintQueueStatus.PaperJam) == PrintQueueStatus.PaperJam)
        //            {
        //                throw new Exception("La impresora tiene atasco de papel");
        //            }
        //            if ((lEnmStatus & PrintQueueStatus.Paused) == PrintQueueStatus.Paused)
        //            {
        //                throw new Exception("La impresora está pausada");
        //            }
        //            /*if ((lEnmStatus & PrintQueueStatus.TonerLow) == PrintQueueStatus.TonerLow)
        //            {
        //                throw new Exception("Is low on toner");
        //            }*/
        //            if ((lEnmStatus & PrintQueueStatus.UserIntervention) == PrintQueueStatus.UserIntervention)
        //            {
        //                throw new Exception("Error en la impresora, contacte administrador");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        lObjDefaultPrintQueue.Dispose();
        //    }
        //}

        ///// <summary>
        ///// Rutina del render del reporte para guardar cada página del reporte
        ///// </summary>
        ///// <param name="pStrName"></param>
        ///// <param name="pStrFileNameExt"></param>
        ///// <param name="pObjEncoding"></param>
        ///// <param name="pStrMimeType"></param>
        ///// <param name="pBolWillSeek"></param>
        ///// <returns>Stream del reporte</returns>
        private static Stream CreateStream(string pStrName, string pStrFileNameExt, Encoding pObjEncoding, string pStrMimeType, bool pBolWillSeek)
        {
            Stream lObjStream = new MemoryStream();
            gLstStreams.Add(lObjStream);
            return lObjStream;
        }

        /// <summary>
        /// Exporta el reporte local en un archivo .emf (Enhanced Metafile).
        /// </summary>
        /// <param name="lObjReport">Reporte local</param>
        private static void Export(LocalReport lObjReport)
        {
            try
            {
                string lStrDeviceInfo =
                  @"<DeviceInfo>
                    <OutputFormat>EMF</OutputFormat>
                    <PageWidth>8.5in</PageWidth>
                    <PageHeight>11in</PageHeight>
                    <MarginTop>0in</MarginTop>
                    <MarginLeft>0in</MarginLeft>
                    <MarginRight>0in</MarginRight>
                    <MarginBottom>0in</MarginBottom>
                </DeviceInfo>";

                Warning[] lObjWarnings;
                gLstStreams = new List<Stream>();

                lObjReport.Render("Image", lStrDeviceInfo, CreateStream, out lObjWarnings);

                foreach (Stream stream in gLstStreams)
                    stream.Position = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //lObjReport.Dispose();
            }
        }

        /// <summary>
        /// Maneja el evento para impresión
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pObjev"></param>
        private static void PrintPage(object sender, PrintPageEventArgs pObjev)
        {
            Metafile lObjPageImage = null;
            try
            {
                lObjPageImage = new Metafile(gLstStreams[gIntCurrentPageIndex]);

                // Adjust rectangular area with printer margins.
                Rectangle lObjAdjustedRect = new Rectangle(
                    pObjev.PageBounds.Left - (int)pObjev.PageSettings.HardMarginX,
                    pObjev.PageBounds.Top - (int)pObjev.PageSettings.HardMarginY,
                    pObjev.PageBounds.Width,
                    pObjev.PageBounds.Height);

                // Draw a white background for the report
                pObjev.Graphics.FillRectangle(System.Drawing.Brushes.White, lObjAdjustedRect);

                // Draw the report content
                pObjev.Graphics.DrawImage(lObjPageImage, lObjAdjustedRect);

                // Prepare for the next page. Make sure we haven't hit the end.
                gIntCurrentPageIndex++;
                pObjev.HasMorePages = (gIntCurrentPageIndex < gLstStreams.Count);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lObjPageImage.Dispose();
            }
        }

        /// <summary>
        /// Construye el objeto PrintDocument
        /// </summary>
        private static void Print()
        {
            PrintDocument lObjPrintDoc = null;
            try
            {
                //Logs.Write(Logs.Types.Debug, typeof(PrinterHelper).ToString(), "[Imprimir] Comenzó impresión de documento en: " + Properties.Settings.Default.PrinterName);

                if (gLstStreams == null || gLstStreams.Count == 0)
                {
                    //Logs.Write(Logs.Types.Error, typeof(PrinterHelper).ToString(), "[Imprimir] No se encontró el archivo para imprimir");
                    throw new Exception("No se encontró el archivo para imprimir");
                }

                lObjPrintDoc = new PrintDocument();
                if (!lObjPrintDoc.PrinterSettings.IsValid)
                {
                    //Logs.Write(Logs.Types.Error, typeof(PrinterHelper).ToString(), "[Imprimir] No se encontró la impresora principal");
                    throw new Exception("No se encontró la impresora principal");
                }
                else
                {
                    //string lStrPrinterName = string.Empty;
                    //if (string.IsNullOrEmpty(Properties.Settings.Default.PrinterName))
                    //    lStrPrinterName = lObjPrintDoc.PrinterSettings.PrinterName;
                    //else
                    //    lStrPrinterName = Properties.Settings.Default.PrinterName;

                    //PrinterStatus(lStrPrinterName);

                    lObjPrintDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                    gIntCurrentPageIndex = 0;
                    lObjPrintDoc.Print();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lObjPrintDoc.Dispose();
            }
        }

        /// <summary>
        /// Exporta el reporte local .rdlc a un archivo .emf para su impresión.
        /// </summary>
        /// <param name="pObjReport">Reporte local a imprimir</param>
        public static void PrintReport(LocalReport pObjReport)
        {
            try
            {
                Export(pObjReport);
                Print();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// Elimina todos los stream.
        /// </summary>
        private static void Dispose()
        {
            if (gLstStreams != null)
            {
                foreach (Stream lObjStream in gLstStreams)
                    lObjStream.Dispose();

                gLstStreams = null;
            }
        }
    }

    /////// <summary>
    /////// The ReportPrintDocument will print all of the pages of a ServerReport or LocalReport.
    /////// The pages are rendered when the print document is constructed.  Once constructed,
    /////// call Print() on this class to begin printing.
    /////// </summary>
    //class ReportPrintDocument : PrintDocument
    //{
    //    private PageSettings pageSettings;
    //    private int currentPage;
    //    private List<Stream> pages = new List<Stream>();

    //    public ReportPrintDocument(ServerReport serverReport)
    //        : this((Report)serverReport)
    //    {
    //        RenderAllServerReportPages(serverReport);
    //    }

    //    public ReportPrintDocument(LocalReport localReport)
    //        : this((Report)localReport)
    //    {
    //        RenderAllLocalReportPages(localReport);
    //    }

    //    private ReportPrintDocument(Report report)
    //    {
    //        // Set the page settings to the default defined in the report
    //        ReportPageSettings reportPageSettings = report.GetDefaultPageSettings();

    //        // The page settings object will use the default printer unless
    //        // PageSettings.PrinterSettings is changed.  This assumes there
    //        // is a default printer.
    //        pageSettings = new PageSettings();
    //        pageSettings.Landscape = reportPageSettings.IsLandscape;
    //        pageSettings.PaperSize = reportPageSettings.PaperSize;
    //        pageSettings.Margins = reportPageSettings.Margins;


    //        ////int num = pageSettings.Landscape ? pageSettings.PaperSize.Height : pageSettings.PaperSize.Width;
    //        ////int num2 = pageSettings.Landscape ? pageSettings.PaperSize.Width : pageSettings.PaperSize.Height;
    //        ////foreach (PaperSize paperSize in this.PrinterSettings.PaperSizes) {
    //        ////    if (num == paperSize.Width && num2 == paperSize.Height) {
    //        ////        pageSettings.Landscape = false;
    //        ////        pageSettings.PaperSize = paperSize;
    //        ////        break;
    //        ////    }
    //        ////    if (num == paperSize.Height && num2 == paperSize.Width) {
    //        ////        pageSettings.Landscape = true;
    //        ////        pageSettings.PaperSize = paperSize;
    //        ////        break;
    //        ////    }
    //        ////}

    //    }

    //    protected override void Dispose(bool disposing)
    //    {
    //        base.Dispose(disposing);

    //        if (disposing)
    //        {
    //            foreach (Stream s in pages)
    //            {
    //                s.Dispose();
    //            }

    //            pages.Clear();
    //        }
    //    }

    //    protected override void OnBeginPrint(PrintEventArgs e)
    //    {
    //        base.OnBeginPrint(e);
    //        currentPage = 0;
    //    }

    //    protected override void OnPrintPage(PrintPageEventArgs e)
    //    {
    //        base.OnPrintPage(e);

    //        Stream pageToPrint = pages[currentPage];
    //        pageToPrint.Position = 0;

    //        // Load each page into a Metafile to draw it.
    //        using (Metafile pageMetaFile = new Metafile(pageToPrint))
    //        {
    //            Rectangle adjustedRect = new Rectangle(
    //                    e.PageBounds.Left - (int)e.PageSettings.HardMarginX,
    //                    e.PageBounds.Top - (int)e.PageSettings.HardMarginY,
    //                    e.PageBounds.Width,
    //                    e.PageBounds.Height);

    //            // Draw a white background for the report
    //            e.Graphics.FillRectangle(System.Drawing.Brushes.White, adjustedRect);

    //            // Draw the report content
    //            e.Graphics.DrawImage(pageMetaFile, adjustedRect);

    //            // Prepare for next page.  Make sure we haven't hit the end.
    //            currentPage++;
    //            e.HasMorePages = currentPage < pages.Count;
    //        }
    //    }

    //    protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
    //    {
    //        e.PageSettings = (PageSettings)pageSettings.Clone();
    //    }

    //    private void RenderAllServerReportPages(ServerReport serverReport)
    //    {
    //        string deviceInfo = CreateEMFDeviceInfo();

    //        // Generating Image renderer pages one at a time can be expensive.  In order
    //        // to generate page 2, the server would need to recalculate page 1 and throw it
    //        // away.  Using PersistStreams causes the server to generate all the pages in
    //        // the background but return as soon as page 1 is complete.
    //        var firstPageParameters = new NameValueCollection();
    //        firstPageParameters.Add("rs:PersistStreams", "True");

    //        // GetNextStream returns the next page in the sequence from the background process
    //        // started by PersistStreams.
    //        var nonFirstPageParameters = new NameValueCollection();
    //        nonFirstPageParameters.Add("rs:GetNextStream", "True");

    //        string mimeType;
    //        string fileExtension;
    //        Stream pageStream = serverReport.Render("IMAGE", deviceInfo, firstPageParameters, out mimeType, out fileExtension);

    //        // The server returns an empty stream when moving beyond the last page.
    //        while (pageStream.Length > 0)
    //        {
    //            pages.Add(pageStream);

    //            pageStream = serverReport.Render("IMAGE", deviceInfo, nonFirstPageParameters, out mimeType, out fileExtension);
    //        }
    //    }

    //    private void RenderAllLocalReportPages(LocalReport localReport)
    //    {
    //        string deviceInfo = CreateEMFDeviceInfo();

    //        Warning[] warnings;
    //        localReport.Render("IMAGE", deviceInfo, LocalReportCreateStreamCallback, out warnings);
    //    }

    //    private Stream LocalReportCreateStreamCallback(
    //        string name,
    //        string extension,
    //        Encoding encoding,
    //        string mimeType,
    //        bool willSeek)
    //    {
    //        var stream = new MemoryStream();
    //        pages.Add(stream);
    //        return stream;
    //    }

    //    private string CreateEMFDeviceInfo()
    //    {
    //        int width = pageSettings.Landscape ? pageSettings.PaperSize.Height : pageSettings.PaperSize.Width;
    //        int height = pageSettings.Landscape ? pageSettings.PaperSize.Width : pageSettings.PaperSize.Height;
    //        return string.Format(CultureInfo.GetCultureInfo("en-US"),
    //            "<DeviceInfo>" +
    //            "<OutputFormat>emf</OutputFormat>" +
    //            "<StartPage>0</StartPage><EndPage>0</EndPage>" +
    //            "<MarginTop>{0}</MarginTop><MarginLeft>{1}</MarginLeft>" +
    //            "<MarginRight>{2}</MarginRight><MarginBottom>{3}</MarginBottom>" +
    //            "<PageHeight>{4}</PageHeight><PageWidth>{5}</PageWidth>" +
    //            "</DeviceInfo>",
    //            ToInches(pageSettings.Margins.Top),
    //            ToInches(pageSettings.Margins.Left),
    //            ToInches(pageSettings.Margins.Right),
    //            ToInches(pageSettings.Margins.Bottom),
    //            ToInches(height),
    //            ToInches(width));
    //    }

    //    private static string ToInches(int hundrethsOfInch)
    //    {
    //        double inches = hundrethsOfInch / 100.0;
    //        return inches.ToString(CultureInfo.GetCultureInfo("en-US")) + "in";
    //    }
    //}

    /*
    internal sealed class ReportPrintDocument : PrintDocument
    {
        private List<Stream> pages = new List<Stream>();

        private PageSettings m_pageSettings;

        private int m_currentPage;

        private int m_endPage;

        private int m_hardMarginX = -1;

        private int m_hardMarginY = -1;

        internal ReportPrintDocument(PageSettings pageSettings)
        {
            this.m_pageSettings = pageSettings;
        }

        protected override void OnBeginPrint(PrintEventArgs e)
        {
            base.OnBeginPrint(e);
            this.m_hardMarginX = -1;
            this.m_hardMarginY = -1;
            switch (base.PrinterSettings.PrintRange) {
                case PrintRange.AllPages:
                    this.m_currentPage = 1;
                    this.m_endPage = base.PrinterSettings.MaximumPage;
                    return;
                case PrintRange.SomePages:
                    this.m_currentPage = base.PrinterSettings.FromPage;
                    this.m_endPage = base.PrinterSettings.ToPage;
                    return;
            }
            throw new NotSupportedException();
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);
            if (this.m_hardMarginX == -1) {
                this.m_hardMarginX = (int)e.PageSettings.HardMarginX;
                this.m_hardMarginY = (int)e.PageSettings.HardMarginY;
            }
            Stream stream = new MemoryStream();
            if (stream != null) {
                MetaFilePage metaFilePage = new MetaFilePage(stream, this.m_pageSettings);
                Rectangle destRect = new Rectangle(e.PageBounds.Left - this.m_hardMarginX, e.PageBounds.Top - this.m_hardMarginY, e.PageBounds.Width, e.PageBounds.Height);
                metaFilePage.Draw(e.Graphics, destRect);
                this.m_currentPage++;
                e.HasMorePages = (this.m_currentPage <= this.m_endPage);// && this.m_fileManager.Count >= this.m_currentPage);
                return;
            }
            e.Cancel = true;
        }

        protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
        {
            e.PageSettings = (PageSettings)this.m_pageSettings.Clone();
        }
    }

    internal sealed class MetaFilePage
    {
        private Metafile m_metaFile;

        private float m_pageWidth;

        private float m_pageHeight;

        public bool NeedsFrame
        {
            get
            {
                return true;
            }
        }

        public int ExternalMargin
        {
            get
            {
                return 20;
            }
        }

        public bool DrawInPixels
        {
            get
            {
                return true;
            }
        }

        public bool IsRequireFullRedraw
        {
            get
            {
                return false;
            }
        }

        public MetaFilePage(Stream metaFileStream, PageSettings pageSettings)
        {
            metaFileStream.Position = 0L;
            this.m_metaFile = new Metafile(metaFileStream);
            if (pageSettings.Landscape) {
                this.m_pageWidth = (float)pageSettings.PaperSize.Height / 100f;
                this.m_pageHeight = (float)pageSettings.PaperSize.Width / 100f;
                return;
            }
            this.m_pageWidth = (float)pageSettings.PaperSize.Width / 100f;
            this.m_pageHeight = (float)pageSettings.PaperSize.Height / 100f;
        }

        public void Draw(Graphics g, PointF scrollOffset, bool testMode)
        {
            this.Draw(g, new Rectangle(0, 0, Global.InchToPixels(this.m_pageWidth, g.DpiX), Global.InchToPixels(this.m_pageHeight, g.DpiY)));
        }

        public void GetPageSize(Graphics g, out float width, out float height)
        {
            width = (float)(Global.InchToPixels(this.m_pageWidth, g.DpiX) + 2 * this.ExternalMargin);
            height = (float)(Global.InchToPixels(this.m_pageHeight, g.DpiY) + 2 * this.ExternalMargin);
        }

        public void Draw(Graphics g, Rectangle destRect)
        {
            using (Brush brush = new SolidBrush(Color.White)) {
                g.FillRectangle(brush, destRect);
            }
            g.DrawImage(this.m_metaFile, destRect);
        }
    }
    */
    //internal static class Global
    //{
    //    public static int ToPixels(double inMM, double dpi)
    //    {
    //        return (int)(inMM * dpi / 25.4);
    //    }

    //    public static float ToMillimeters(int pixels, double dpi)
    //    {
    //        return (float)((double)pixels * 25.4 / dpi);
    //    }

    //    public static float ToMillimeters(float pixels, double dpi)
    //    {
    //        return (float)((double)pixels * 25.4 / dpi);
    //    }

    //    public static int InchToPixels(float value, float dpi)
    //    {
    //        return (int)(value * dpi);
    //    }
    //}
}
