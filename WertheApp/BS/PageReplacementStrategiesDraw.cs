using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Diagnostics; //Debug.WriteLine("");
using System.Collections.Generic;
namespace WertheApp.BS
{
    public class PageReplacementStrategiesDraw
    {

        //VARIABLES
        private static SKCanvasView skiaview;
        private static float xe, ye;
        private static SKPaint sk_blackText, sk_blackTextSmall;
        private static SKPaint sk_PaintVertical, sk_PaintHorizontal, sk_Paint1;
        private static int ramSize, discSize, sequenceLength;
        private static List<int> SequenceList;
        private static float colums;
        private static float rows;
        private static float columnWidth;
        private static float rowWidth;
        private static float strokeWidth;// Width of a paint stroke
        private static int toggleZoom;

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
            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;

            //add tap Gestrue Recognizer for zooming in and out
            toggleZoom = 0;
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.NumberOfTapsRequired = 2; // double-tap
            tapGestureRecognizer.Tapped += (s, e) => {
                // handle the tap
                if(toggleZoom == 0){
                    skiaview.ScaleTo(2);
                    toggleZoom = 1;
                }else{
                    skiaview.ScaleTo(1);
                    toggleZoom = 0;
                }

            };
            skiaview.GestureRecognizers.Add(tapGestureRecognizer);

            //TODO: FINDE HERAUS; WIE MAN HIN UND HER SCHIEBT
            //add pan gesture recognizer
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += (s, e) => {
                // Handle the pan
            };
            skiaview.GestureRecognizers.Add(panGesture);
            

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

            //Draw rows and colums
            float posCol = 1;
            for (int i = 0; i <= colums + 1; i++){
                canvas.DrawLine(new SKPoint(posCol * xe, 1 * ye), new SKPoint(posCol * xe, 99 * ye), sk_PaintVertical);
                posCol += columnWidth;
            }

            float posRow = 1;
            for (int i = 0; i <= rows +1 ; i++){
                canvas.DrawLine(new SKPoint(1 * xe, posRow * ye), new SKPoint(99 * xe, posRow * ye), sk_PaintHorizontal);
                posRow += rowWidth;
            }

            SKRect sk_r2 = new SKRect(0*xe,0*ye,99*xe,99*ye); //left , top, right, bottom
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

            sk_PaintVertical = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth*xe,
                IsAntialias = true,
                Color = new SKColor(0, 0, 0) //black
            };

            sk_PaintHorizontal = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * ye,
                IsAntialias = true,
                Color = new SKColor(0, 0, 0) //black
            };

            //black neutral text
            sk_blackText = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * 30
            };

            //black neutral text
            sk_blackTextSmall = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * 20
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
