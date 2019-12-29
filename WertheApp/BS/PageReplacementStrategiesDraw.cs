using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Collections.Generic;

//Ram array explanation
//[step, ram , 0] = pagenumber (range 0-9, -1 -> no page)
//[step, ram , 1] = r-bit (0 -> not set, 1 -> set)
//[step, ram , 2] = m-bit (0 -> not set, 1 -> set)
//[step, ram , 3] = pagefail (0 -> no pagefail, 1 -> pagefail without replacement, 2 -> pagefail with replacement)
//[step, ram , 4] = r bit were reset in this step -> 1, else -> 0
//[step, ram , 5] = m bit was set in this step -> 1, else -> 0

namespace WertheApp.BS
{
    public class PageReplacementStrategiesDraw
    {

        //VARIABLES
        private static SKCanvasView skiaview;
        private static float xe, ye;
        private static SKPaint sk_blackText, sk_blackTextSmall, sk_redTextSmall;
        private static SKPaint sk_PaintThin, sk_PaintFat, sk_Paint1, sk_PaintRed;
        private static SKPaint sk_PaintYellow, sk_PaintPink;
        private static float rows, colums;
        private static float columnWidth, rowWidth, strokeWidth;// Width of a paint stroke
        private static float columnCenter, rowCenter, rowTextStart; //rowtextstart: it's not possible to center the text vertically, only horizontally...
        private static float textSize;
        private static int toggleZoom;

        private static int ramSize, discSize, sequenceLength;
        private static List<int> SequenceList;



        //CONSTRUCTOR
        public PageReplacementStrategiesDraw(int rS, int dS, int sL, List<int> l)
        {
            ramSize = rS;
            discSize = dS;
            sequenceLength = sL;
            SequenceList = new List<int>(l);

            colums = sequenceLength + 1;
            rows = ramSize + discSize + 1;
            columnWidth = 98f / colums;
            rowWidth = 98f / rows;
            strokeWidth = 0.2f;
            columnCenter = columnWidth / 2;
            rowCenter = rowWidth / 2;

            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;

            //adjust textsize and positioning
            if (rowWidth > columnWidth)
            {
                textSize = columnWidth;
            }
            else
            {
                textSize = rowWidth;
            }
            rowTextStart = rowCenter + (textSize / 2);

            AddGestureRecognizers();

           
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        // do the drawing
        static void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            var surfaceWidth = info.Width;
            var surfaceHeight = info.Height;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            if (canvas != null)
            {
                canvas.Clear(); //Important! Otherwise the drawing will look messed up in iOS
            }

            /*important: the coordinate system starts in the upper left corner*/
            float lborder = canvas.LocalClipBounds.Left;
            float tborder = canvas.LocalClipBounds.Top;
            float rborder = canvas.LocalClipBounds.Right;
            float bborder = canvas.LocalClipBounds.Bottom;

            xe = rborder / 100; //using the variable surfacewidth instead would mess everything up
            ye = bborder / 100;

            MakeSKPaint(); //depends on xe and ye and therfore has to be called after they were initialized


            /*********************HERE GOES THE DRAWING************************/

            //draw background for ram and disc: pink and yellow
            float bgRamY1 = 1 + rowWidth;
            float bgRamY2 = bgRamY1 + (rowWidth * ramSize);
            float bgDiscY1 = bgRamY2;
            float bgDiscY2 = bgDiscY1 + (rowWidth * discSize);
            SKRect sk_rBackgroundRam = new SKRect(1 * xe, bgRamY1* ye, 99 * xe, bgRamY2 * ye); //left , top, right, bottom
            SKRect sk_rBackgroundDisc = new SKRect(1 * xe, bgDiscY1 * ye, 99 * xe, bgDiscY2 * ye); //left , top, right, bottom
            canvas.DrawRect(sk_rBackgroundRam, sk_PaintPink); //left, top, right, bottom, color
            canvas.DrawRect(sk_rBackgroundDisc, sk_PaintYellow);


            //Draw rows and colums
            float posCol = 1;
            for (int i = 0; i <= colums ; i++){
                if(i == 0 || i == 1 || i == colums )
                { //first, second and last line is fat
                    canvas.DrawLine(new SKPoint(posCol * xe, 1 * ye), new SKPoint(posCol * xe, 99 * ye), sk_PaintFat);
                }
                else{
                    canvas.DrawLine(new SKPoint(posCol * xe, 1 * ye), new SKPoint(posCol * xe, 99 * ye), sk_PaintThin);
                }
                posCol += columnWidth;
            }
            float posRow = 1;
            for (int i = 0; i <= rows ; i++){
                if(i == 0 || i == 1 || i == rows || i == ramSize + 1)
                { //first, second and last line is fat, also the seperating line between ram and disc
                    canvas.DrawLine(new SKPoint(1 * xe, posRow * ye), new SKPoint(99 * xe, posRow * ye), sk_PaintFat);
                }
                else{
                    canvas.DrawLine(new SKPoint(1 * xe, posRow * ye), new SKPoint(99 * xe, posRow * ye), sk_PaintThin);
                }
                posRow += rowWidth;
            }

            //draw the words RAM and DISC
            float posText1 = rowTextStart + rowWidth;
            for (int i = 0; i < ramSize; i++){
                canvas.DrawText("RAM", columnCenter * xe + xe, posText1 * ye, sk_blackText);
                posText1 += rowWidth;
            }
            float posText2 = rowTextStart + (rowWidth * (ramSize + 1));
            for (int i = 0; i < discSize; i++){
                canvas.DrawText("DISC", columnCenter * xe + xe, posText2 * ye, sk_blackText);
                posText2 += rowWidth;
            }

            //draw the page sequence
            float posText3 = 1 + columnCenter + columnWidth;
            foreach (var p in SequenceList)
            {
                canvas.DrawText(p.ToString(), posText3 * xe, rowTextStart * ye, sk_blackText);
                posText3 += columnWidth;
            }

            //draw Ram: for every step thats done yet, look for pages in ram
            float posXText4 = 1 + columnCenter + columnWidth;
            float posYText4 = rowTextStart + rowWidth;
            for (int step = 0; step <= PageReplacementStrategies.currentStep; step++){
                for (int ram = 0; ram <= PageReplacementStrategies.ram.GetUpperBound(1); ram++){
                    int page = PageReplacementStrategies.ram[step, ram, 0];
                    if(page != -1){
                        canvas.DrawText(page.ToString(), posXText4 * xe, posYText4 * ye, sk_blackText);
                    }

                    posYText4 += rowWidth;
                }
                posYText4 = rowTextStart + rowWidth;
                posXText4 += columnWidth;
            }
            //draw Disc: for evry step thats done yet, look for pages in disc
            float posXText5 = 1 + columnCenter + columnWidth;
            float posYText5 = rowTextStart + (rowWidth * (ramSize + 1));
            for (int step = 0; step <= PageReplacementStrategies.currentStep; step++)
            {
                for (int disc = 0; disc <= PageReplacementStrategies.disc.GetUpperBound(1); disc++)
                {
                    int page = PageReplacementStrategies.disc[step, disc];
                    if (page != -1)
                    {
                        canvas.DrawText(page.ToString(), posXText5 * xe, posYText5 * ye, sk_blackText);
                    }

                    posYText5 += rowWidth;
                }
                posYText5 = rowTextStart + (rowWidth * (ramSize + 1));
                posXText5 += columnWidth;
            }

            //prepare a red square
            float posPFX1 = 1 + columnWidth + (columnWidth / 6) * 1;
            float posPFX2 = posPFX1 + (columnWidth / 6) * 4;
            float posPFY1 = 1 + rowWidth + (rowWidth / 10) * 1;
            float posPFY2 = posPFY1 + (rowWidth / 10) * 8;

            //prepare a red circle
            float radius = 0; 
            float cx = 1 + columnWidth + columnCenter;
            float cy = 1 + rowWidth + rowCenter;
            if (rowWidth > columnWidth){ radius = columnWidth / 2.6f * xe;}
            else{ radius = rowWidth / 2.2f * ye;}


            //draw pagefails
            for (int step = 0; step <= PageReplacementStrategies.currentStep; step++){
                if(PageReplacementStrategies.ram[step, 0, 3] == 2){
                    //with replacement(circle)
                    canvas.DrawCircle(cx * xe, cy * ye, radius, sk_PaintRed); //center x, center y, radius, paint
                }
                else if(PageReplacementStrategies.ram[step, 0, 3] == 1){
                    //without replacement (square)
                    SKRect sk_Pagefail = new SKRect(posPFX1 * xe, posPFY1 * ye, posPFX2 * xe, posPFY2 * ye); //left , top, right, bottom
                    canvas.DrawRect(sk_Pagefail, sk_PaintRed); //left, top, right, bottom, color
                }
                posPFX1 += columnWidth;
                posPFX2 = posPFX1 + (columnWidth / 6) * 4;
                cx += columnWidth;
            }

            //draw M-Bits and R-Bits
            float posXMbit = 1 + columnWidth + columnCenter - columnCenter / 2;
            float posYMbit = 1 + rowWidth + rowCenter + rowCenter / 2 ;
            float posXRbit = 1 + columnWidth + columnCenter - columnCenter / 2;
            float posYRbit = 1 + rowWidth + rowCenter - rowCenter / 2 ;
            if (PageReplacementStrategies.strategy == "RNU FIFO Second Chance" || PageReplacementStrategies.strategy == "RNU FIFO"){
                for (int step = 0; step <= PageReplacementStrategies.currentStep; step++){
                    for (int ram = 0; ram <= PageReplacementStrategies.ram.GetUpperBound(1); ram++){
                        if(PageReplacementStrategies.ram[step, ram, 0] != -1){
                            String rBitValue = PageReplacementStrategies.ram[step, ram, 1].ToString();
                            String mBitValue = PageReplacementStrategies.ram[step, ram, 2].ToString();
                            if(PageReplacementStrategies.ram[step, ram, 4] == 0){
                                canvas.DrawText(rBitValue, posXRbit * xe, posYRbit * ye, sk_blackTextSmall);
                            }else{
                                canvas.DrawText(rBitValue, posXRbit * xe, posYRbit * ye, sk_redTextSmall);
                            }
                            if(PageReplacementStrategies.ram[step,ram,5] == 0){
                                canvas.DrawText(mBitValue, posXMbit * xe, posYMbit * ye, sk_blackTextSmall);
                            }else{
                                canvas.DrawText(mBitValue, posXMbit * xe, posYMbit * ye, sk_redTextSmall);
                            }
                        }
                        posYMbit += rowWidth;
                        posYRbit += rowWidth;
                    }
                    posXMbit += columnWidth;
                    posXRbit += columnWidth;
                    posYMbit = 1 + rowWidth + rowCenter + rowCenter / 2;
                    posYRbit = 1 + rowWidth + rowCenter - rowCenter / 2;
                }

            }


            //execute all drawing actions
            canvas.Flush();
        }


        /**********************************************************************
        *********************************************************************/
        static private void MakeSKPaint()
        {
  
            sk_Paint1 = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(0, 0, 0) //black
            };

            sk_PaintThin = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 0) //black
            };

            sk_PaintFat = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * 2 * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 0) //black
            };

            //black neutral text
            sk_blackText = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * textSize,
                IsAntialias =true,
                IsStroke = false, 
                TextAlign = SKTextAlign.Center
                
            };

            //black small text
            sk_blackTextSmall = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * textSize/2,
                IsAntialias = true,
                IsStroke = false, 
                TextAlign = SKTextAlign.Center,
                IsVerticalText =true
            };

            //red small text
            sk_redTextSmall = new SKPaint
            {
                Color = SKColors.OrangeRed,
                TextSize = ye * textSize / 2,
                IsAntialias = true,
                IsStroke = false,
                TextAlign = SKTextAlign.Center,
                IsVerticalText = true
            };

            sk_PaintPink = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(238, 130, 238).WithAlpha(30)
            };

            sk_PaintYellow = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(255, 255, 0).WithAlpha(30)
            };

            sk_PaintRed = new SKPaint
            {
                Color = SKColors.Red,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth * xe,
                IsAntialias = true,

            };
        }

        /**********************************************************************
        *********************************************************************/
        public static SKCanvasView ReturnCanvas (){
            return skiaview;
        }

        /**********************************************************************
        *********************************************************************/
        public static void Paint (){
            // update the canvas when the data changes
            skiaview.InvalidateSurface();

        }

        private static void AddGestureRecognizers(){
            //add tap Gestrue Recognizer for zooming in and out
            toggleZoom = 0;
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.NumberOfTapsRequired = 2; // double-tap
            tapGestureRecognizer.Tapped += (s, e) => {
                // handle the tap
                if (toggleZoom == 0)
                {
                    skiaview.ScaleTo(2);
                    toggleZoom = 1;
                }
                else
                {

                    skiaview.ScaleTo(1);
                    toggleZoom = 0;
                    skiaview.TranslateTo(0, 0);
                }

            };
            skiaview.GestureRecognizers.Add(tapGestureRecognizer);

            //add pan gesture recognizer
            double x = 0;
            double y = 0;
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += (s, e) => {

                // Handle the pan (only in zoomed in state)
                if (toggleZoom == 1 && e.StatusType != GestureStatus.Completed)
                {
                    //only when within screen bounds or 20% more or less
                    if(x + e.TotalX >= skiaview.Width*1.2f/2*-1 
                       && x + e.TotalX <= skiaview.Width*1.2f/2
                       && y + e.TotalY >= skiaview.Height*1.2f/2*-1
                       && y+ e.TotalY <= skiaview.Height*1.2f/2){
                        x = x + e.TotalX;
                        y = y + e.TotalY;
                        skiaview.TranslateTo(x, y);
                    }
                }
            };
            skiaview.GestureRecognizers.Add(panGesture);
        }
        /**********************************************************************
        *********************************************************************/
        public static void PrintSequenceList()
        {
            String s = "SEQUENCELIST";
            foreach (var p in SequenceList)
            {
                s += ", " + p;
            }
            //Debug.WriteLine(s);
        }
    }
}
