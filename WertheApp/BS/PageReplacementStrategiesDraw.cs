using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Diagnostics; //Debug.WriteLine("");
using System.Collections.Generic;

//Ram array explanation
//[step, ram , 0] = pagenumber (range 0-9, -1 -> no page)
//[step, ram , 1] = r-bit (0 -> not set, 1 -> set)
//[step, ram , 2] = m-bit (0 -> not set, 1 -> set)
//[step, ram , 3] = pagefail (0 -> no pagefail, 1 -> pagefail without replacement, 2 -> pagefail with replacement)

namespace WertheApp.BS
{
    //TODO: IsStroke = false //somehow since the newest update this doesnt work anymore for ios
    public class PageReplacementStrategiesDraw
    {

        //VARIABLES
        private static SKCanvasView skiaview;
        private static float xe, ye;
        private static SKPaint sk_blackText, sk_blackTextSmall;
        private static SKPaint sk_PaintThin, sk_PaintFat, sk_Paint1;
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
                if(i == 0 || i == 1 || i == rows )
                { //first, second and last line is fat
                    canvas.DrawLine(new SKPoint(1 * xe, posRow * ye), new SKPoint(99 * xe, posRow * ye), sk_PaintFat);
                }
                else{
                    canvas.DrawLine(new SKPoint(1 * xe, posRow * ye), new SKPoint(99 * xe, posRow * ye), sk_PaintThin);
                }
                posRow += rowWidth;
            }

            canvas.DrawText("RAM", columnCenter * xe + xe, rowTextStart*ye, sk_blackText);
            //SKRect sk_r2 = new SKRect(0*xe,0*ye,99*xe,99*ye); //left , top, right, bottom
            //canvas.DrawRect(sk_r2, sk_Paint1); //left, top, right, bottom, color

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
                Color = new SKColor(0, 0, 0) //black
            };

            sk_PaintFat = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * 2 * xe,
                IsAntialias = true,
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

            //black neutral text
            sk_blackTextSmall = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * (columnWidth/4),
                IsAntialias = true,
                IsStroke = false, //TODO: somehow since the newest update this doesnt work anymore for ios
                TextAlign = SKTextAlign.Center
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

            //TODO: Maybe better pan gesture. reacts not good enough
            //add pan gesture recognizer
            double x = 0;
            double y = 0;
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += (s, e) => {
                // Handle the pan (only in zoomed in state)
                if (toggleZoom == 1 && e.StatusType != GestureStatus.Completed)
                {
                    //TODO: only when within screen bounds
                    if(true){
                        x = x + e.TotalX;
                        y = y + e.TotalY;
                        skiaview.TranslateTo(x, y);
                    }
                    Debug.WriteLine(e.TotalX + "," + e.TotalY);
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
            Debug.WriteLine(s);
        }
    }
}
