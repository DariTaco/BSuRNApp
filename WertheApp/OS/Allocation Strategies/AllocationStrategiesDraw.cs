using System;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using Xamarin.Forms;
using System.Diagnostics;
using System.Collections.Generic;

namespace WertheApp.OS.AllocationStrategies
{
    public class AllocationStrategiesDraw 
    {
        // canvas 
        private static SKCanvasView canvasView; // view containing the drawing
        private static SKCanvas canvas; // drawing
        private static SKImageInfo info; // canvas info
        private static float centerX, centerY, x1, x2, y1, y2; // canvas coordinates

        // painting tools
        private static float strokeWidth; // stroke Width for paint colors
        private static SKPaint sk_Background, sk_Black, sk_Text; //paint colors

        private static AllocationStrategiesAlgorithm algo;
       

        public AllocationStrategiesDraw(AllocationStrategiesAlgorithm p_algo)
        {
            
            // crate the canvas
            canvasView = new SKCanvasView();
            canvasView.PaintSurface += PaintSurface;
            canvasView.BackgroundColor = Color.WhiteSmoke;

            algo = p_algo;

        }

        /**********************************************************************
        ***********************************************************************
        draw on canvas */
        void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            //canvas object
            info = e.Info; //
            SKSurface surface = e.Surface;
            canvas = surface.Canvas;
 

            //Important! Otherwise the drawing will look messed up in iOS
            if (canvas != null) { canvas.Clear(); }

            MakeSKPaint();
            CalculateNeededVariables();

            /*********************HERE GOES THE DRAWING************************/
            /*important: the coordinate system starts in the upper left corner*/
            DrawMemory();

            //execute all drawing actions
            canvas.Flush();
        }

        /**********************************************************************
        *********************************************************************/
        private static void CalculateNeededVariables()
        {
            /*important: the coordinate system starts in the upper left corner*/
            strokeWidth = 5;
            centerX = info.Width / 2;
            centerY = info.Height / 2;
            x1 = strokeWidth / 2;
            y1 = strokeWidth / 2;
            x2 = info.Width - strokeWidth / 2;
            y2 = info.Height - strokeWidth / 2;

            Debug.WriteLine(string.Format($" centerX: {centerX}, centerY {centerY}, x1: {x1}, x2: {x2}, y1: {y1}, y2: {y2}"));
        }
        static float xPercent(float p)
        {
            float percent = (info.Width - strokeWidth / 2) * p;
            return percent;
        }
        static float yPercent(float p)
        {
            float percent = (info.Height - strokeWidth / 2) * p;
            return percent;
        }
        /**********************************************************************
        *********************************************************************/
        private static void DrawMemory()
        {
            // memory bounds in percent of the canvas
            float mX1 = 0.1f;
            float mX2 = 0.9f;
            float mY1 = 0.3f;
            float mY2 = 0.7f;
            float mWidth = mX2 - mX1;
            float mHeight = mY2 - mY1;
 

            //SKRect sk_memory = new SKRect(x1 + xPercent(0.1f), y1 + yPercent(0.3f), x2 - xPercent(0.1f), y2 - yPercent(0.3f));
            //SKRect sk_memory = new SKRect(xPercent(0.1f), yPercent(0.3f), xPercent(0.9f), yPercent(0.7f));

            // draw the outlines of the memory box
            SKRect sk_memory = new SKRect(xPercent(mX1), yPercent(mY1), xPercent(mX2), yPercent(mY2));
            canvas.DrawRect(sk_memory, sk_Black);

            //draw the gaps/parting lines to indicate memory fragmentation
            int totalMemorySize = algo.GetTotalMemorySize();
            List<int> fragmentsList = algo.GetFragmentsList();
            float relativeFragmentSize = mWidth / totalMemorySize;
            int fragmentsSoFar = 0;
            foreach (int fragment in fragmentsList)
            {
                fragmentsSoFar += fragment;
                float xLine = mX1 + (fragmentsSoFar * relativeFragmentSize);
                canvas.DrawLine(xPercent(xLine), yPercent(mY1), xPercent(xLine), yPercent(mY2), sk_Black); // fragmentation parting lines
                float xText = mX1 + ((fragmentsSoFar - (fragment / 2)) * relativeFragmentSize); // position in the middle of a fragmentation
                float yText = mY1 + (mHeight / 2);
                canvas.DrawText(fragment.ToString(), xPercent(xText), yPercent(yText), sk_Text); // size of fragment
            }


        }

        /**********************************************************************
        *********************************************************************/
        static private void MakeSKPaint()
        {

            sk_Background = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Color = new SKColor(67, 110, 238).WithAlpha(30)
            };

            sk_Black = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.Black.ToSKColor(),
                StrokeWidth = strokeWidth
            };

            sk_Text = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Black.ToSKColor(),
                TextAlign = SKTextAlign.Center,
                StrokeWidth = strokeWidth,
                TextSize = 50
            };
        }

        /**********************************************************************
        ***********************************************************************
        returns the canvas view */
        public SKCanvasView ReturnCanvas()
        {
            return canvasView;
        }
        /**********************************************************************
        ***********************************************************************
        redraws the canvas view */
        public static void Paint()
        {
            canvasView.InvalidateSurface();
        }
    }
}

