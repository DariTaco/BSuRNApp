using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Diagnostics; //Debug.WriteLine("");
using System.Collections.Generic;

namespace WertheApp.RN
{
    public class DijkstraDraw
    {
        //VARIABLES
        private static SKCanvasView skiaview;
        private static float xe, ye;

        private static SKPaint sk_blackText, sk_test, sk_PaintBlack;
        private static float textSize, strokeWidth;

        private static int currentStep;
        private static int maxStep;

        //CONSTRUCTOR
        public DijkstraDraw()
        {
            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;

            textSize = 5;
            strokeWidth = 0.2f;
            currentStep = 0;
            maxStep = 20;
        }


        //METHODS
        /**********************************************************************
        *********************************************************************/
        // do the drawing
        void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            //canvas object
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            //Important! Otherwise the drawing will look messed up in iOS
            if (canvas != null) { canvas.Clear(); }

            //calculate some stuff and make the paint
            CalculateNeededNumbers(canvas);
            MakeSKPaint(); //depends on xe and ye and therfore has to be called after they were initialized

            /*********************HERE GOES THE DRAWING************************/
            DrawBackground(canvas);


            //execute all drawing actions
            canvas.Flush();
        }

        /**********************************************************************
        *********************************************************************/
        public static void DrawBackground(SKCanvas canvas)
        {
            SKRect sk_rBackground = new SKRect(00 * xe, 0 * ye, 100 * xe, 100 * ye); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground, sk_test); //left, top, right, bottom, color
            canvas.DrawLine(new SKPoint(0 * xe, 50 * ye), new SKPoint(100 * xe, 50 * ye), sk_PaintBlack);
        }

        /**********************************************************************
        *********************************************************************/
        public static bool NextStep()
        {
            currentStep++;

            if (currentStep >= maxStep)
            {
                return false;
            }
            return true;

        }

        /**********************************************************************
        *********************************************************************/
        public static bool PreviousStep()
        {
            currentStep--;
            if (currentStep <= 0)
            {
                return false;
            }
            return true;
        }

        /**********************************************************************
        *********************************************************************/
        public static void Restart()
        {
            currentStep = 0;
        }

        /**********************************************************************
        *********************************************************************/
        public static void GoToEnd()
        {
            currentStep = maxStep;
        }
        /**********************************************************************
        *********************************************************************/
        static void CalculateNeededNumbers(SKCanvas canvas)
        {
            /*important: the coordinate system starts in the upper left corner*/
            float lborder = canvas.LocalClipBounds.Left;
            float tborder = canvas.LocalClipBounds.Top;
            float rborder = canvas.LocalClipBounds.Right;
            float bborder = canvas.LocalClipBounds.Bottom;

            xe = rborder / 100; //using the variable surfacewidth instead would mess everything up
            ye = bborder / 100;
        }

        /**********************************************************************
        *********************************************************************/
        public static SKCanvasView ReturnCanvas()
        {
            return skiaview;
        }

        /**********************************************************************
        *********************************************************************/
        //redraws the canvas
        public static void Paint()
        {
            // update the canvas when the data changes
            skiaview.InvalidateSurface();

        }

        /**********************************************************************
        *********************************************************************/
        static private void MakeSKPaint()
        {
            //For Text
            sk_blackText = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * textSize / 1.8f,
                IsAntialias = true,
                IsStroke = false, //TODO: somehow since the newest update this doesnt work anymore for ios
                TextAlign = SKTextAlign.Center,
                IsVerticalText = false
            };

            sk_test = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(238, 130, 238).WithAlpha(30)
            };


            sk_PaintBlack = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth / 1f * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 0) //black
            };
        }
    }
}
