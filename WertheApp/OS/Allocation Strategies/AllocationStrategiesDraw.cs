using System;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using Xamarin.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

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
        private static SKPaint sk_Background, sk_Black, sk_Text, sk_TextWhite, sk_TextUsed,
            sk_ArrowRed, sk_ArrowGray, sk_UsedSpace; //paint colorssk
       

        public AllocationStrategiesDraw()
        {
            
            // crate the canvas
            canvasView = new SKCanvasView();
            canvasView.PaintSurface += PaintSurface;
            canvasView.BackgroundColor = App._viewBackground;
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

            CalculateNeededVariables();
            MakeSKPaint();

            /*********************HERE GOES THE DRAWING************************/
            /*important: the coordinate system starts in the upper left corner*/
            WriteInitialMemoryFragmentationOnCanvas();
            float relativeFragmentSize = DrawMemory();

            AllocationStrategiesAlgorithm.Status status = AllocationStrategiesAlgorithm.GetStatus();
            if (status == AllocationStrategiesAlgorithm.Status.searching || status == AllocationStrategiesAlgorithm.Status.successfull)
            {
                DrawRedArrow(relativeFragmentSize);


                int promIndex = AllocationStrategiesAlgorithm.GetMostPromisingIndex();
                if (promIndex != -1)
                {
                    DrawGrayArrow(promIndex, relativeFragmentSize);
                }
            }


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

            //Debug.WriteLine(string.Format($" centerX: {centerX}, centerY {centerY}, x1: {x1}, x2: {x2}, y1: {y1}, y2: {y2}"));
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
        private static float DrawMemory()
        {
            // memory bounds in percent of the canvas
            float mX1 = 0.1f;
            float mX2 = 0.9f;
            float mY1 = 0.3f;
            float mY2 = 0.7f;
            float mWidth = mX2 - mX1;
            float mHeight = mY2 - mY1;

            int totalMemorySize = AllocationStrategiesAlgorithm.GetTotalMemorySize();
            List<FragmentBlock> allFragmentsList = AllocationStrategiesAlgorithm.GetAllFragmentsList();
            float relativeFragmentSize = mWidth / totalMemorySize;
            int stepsSoFar = 0;


            foreach (FragmentBlock fragment in allFragmentsList)
            {
                // fragment size of 1 gets displayed as a .
                String fragmentSize = fragment.GetSize().ToString();
                if(fragmentSize == "1") { fragmentSize = "•"; }

                stepsSoFar += fragment.GetSize();
                float fragmentStart = mX1 + (stepsSoFar * relativeFragmentSize) - (fragment.GetSize() * relativeFragmentSize);
                float fragmentEnd = mX1 + (stepsSoFar * relativeFragmentSize);
                float xText = fragmentEnd - (fragmentEnd - fragmentStart) / 2; // position of text in the middle of fragment
               
                if (fragment.IsFree()) {
                    // draw size of fragment in the middle of memory
                    canvas.DrawText(fragmentSize, xPercent(xText), yPercent(0.5f), sk_Text); }
                else
                {
                    // draw size of fragment beneath the memory
                    //canvas.DrawText(fragmentSize, xPercent(xText), yPercent(0.8f), sk_TextWhite);
                    canvas.DrawText(fragmentSize, xPercent(xText), yPercent(0.8f), sk_Text);

                    // fill in used space
                    SKRect usedSpace = new SKRect(xPercent(fragmentStart), yPercent(mY1), xPercent(fragmentEnd), yPercent(mY2)); //left x1 top y1 right x2 bottom y2
                    canvas.DrawRect(usedSpace, sk_UsedSpace);
                }
            }

            // draw the outlines of the memory box
            SKRect sk_memory = new SKRect(xPercent(mX1), yPercent(mY1), xPercent(mX2), yPercent(mY2));
            canvas.DrawRect(sk_memory, sk_Black);
            canvas.DrawText("free", xPercent(0.05f), yPercent(0.5f), sk_Text);
            //canvas.DrawText("used", xPercent(0.05f), yPercent(0.8f), sk_TextWhite);
            canvas.DrawText("used", xPercent(0.05f), yPercent(0.8f), sk_Text);

            return relativeFragmentSize;
        }

        /**********************************************************************
        *********************************************************************/
        private static void DrawGrayArrow(int index, float relativeFragmentSize)
        {
            // determine position of fragment block 
            List<FragmentBlock> allFragmentsList = AllocationStrategiesAlgorithm.GetAllFragmentsList();
            float mX1 = 0.1f; // memory bound x1

            int stepsSoFar = 0;
            FragmentBlock fragment = null;
            for (int i = 0; i <= index; i++)
            {
                fragment = allFragmentsList.ElementAt(i);
                stepsSoFar += fragment.GetSize();
            }
            float fragmentStart = mX1 + (stepsSoFar * relativeFragmentSize) - (fragment.GetSize() * relativeFragmentSize);
            float fragmentEnd = mX1 + (stepsSoFar * relativeFragmentSize);
            float xPosition = fragmentEnd - (fragmentEnd - fragmentStart) / 2; // position of text in the middle of fragment


            // draw the arrow
            float arrowTip = 0.725f;
            canvas.DrawLine(xPercent(xPosition), yPercent(arrowTip + 0.125f), xPercent(xPosition), yPercent(arrowTip), sk_ArrowGray);
            SKPath arrow = new SKPath();
            arrow.MoveTo(xPercent(xPosition), yPercent(arrowTip));
            arrow.RLineTo(-yPercent(0.015f), +yPercent(0.03f));
            arrow.RLineTo(+yPercent(0.03f), 0);
            arrow.RLineTo(-yPercent(0.015f), -yPercent(0.03f));
            arrow.Close();
            canvas.DrawPath(arrow, sk_ArrowGray);
        }

        /**********************************************************************
        *********************************************************************/
        private static void DrawRedArrow(float relativeFragmentSize)
        {
            // determine position of fragment block
            int index = AllocationStrategiesAlgorithm.GetCurrentIndex(); //TODO: nach merge könnte der index nicht mehr existieren
            List<FragmentBlock> allFragmentsList = AllocationStrategiesAlgorithm.GetAllFragmentsList();
            float mX1 = 0.1f; // memory bound x1

            int stepsSoFar = 0;
            FragmentBlock fragment = null;
            for (int i = 0; i <= index; i++)
            {
                fragment = allFragmentsList.ElementAt(i);
                stepsSoFar += fragment.GetSize();
            }
            float fragmentStart = mX1 + (stepsSoFar * relativeFragmentSize) - (fragment.GetSize() * relativeFragmentSize);
            float fragmentEnd = mX1 + (stepsSoFar * relativeFragmentSize);
            float xPosition = fragmentEnd - (fragmentEnd - fragmentStart) / 2; // position of text in the middle of fragment

            // draw the arrow
            float arrowTip = 0.275f;
            canvas.DrawLine(xPercent(xPosition), yPercent(arrowTip-0.125f), xPercent(xPosition), yPercent(arrowTip), sk_ArrowRed);
            SKPath arrow = new SKPath();
            arrow.MoveTo(xPercent(xPosition), yPercent(arrowTip));
            arrow.RLineTo(yPercent(0.015f), -yPercent(0.03f));
            arrow.RLineTo(-yPercent(0.03f), 0);
            arrow.RLineTo(yPercent(0.015f), yPercent(0.03f));
            arrow.Close();
            canvas.DrawPath(arrow, sk_ArrowRed);
        }

        /**********************************************************************
        *********************************************************************/
        private static void WriteInitialMemoryFragmentationOnCanvas()
        {
            if(AllocationStrategiesAlgorithm.GetStatus() == AllocationStrategiesAlgorithm.Status.undefined)
            {
                canvas.DrawText("initial memory fragmentation", xPercent(0.5f), yPercent(0.2f), sk_Text);
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
                IsAntialias = true,
                Color = Color.Black.ToSKColor(),
                StrokeWidth = yPercent(0.01f)
            };

            sk_ArrowGray = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                IsAntialias = true,
                Color = Color.DarkSlateGray.ToSKColor(),
                StrokeWidth = strokeWidth
            };


            sk_ArrowRed = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                IsAntialias = true,
                Color = Color.DarkRed.ToSKColor(),
                StrokeWidth = strokeWidth
            };

            sk_Text = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Black.ToSKColor(),
                TextAlign = SKTextAlign.Center,
                StrokeWidth = strokeWidth,
                TextSize = yPercent(0.07f)
            };


            sk_TextWhite = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.White.ToSKColor(),
                TextAlign = SKTextAlign.Center,
                StrokeWidth = strokeWidth,
                TextSize = yPercent(0.07f)
            };

            sk_TextUsed = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(67, 110, 238).WithAlpha(150),//blue
                TextAlign = SKTextAlign.Center,
                StrokeWidth = strokeWidth,
                TextSize = yPercent(0.07f)
            };

            sk_UsedSpace = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                //Color = Color.Gray.ToSKColor(),
                Color = new SKColor(67, 110, 238).WithAlpha(80),//blue
                //Color = new SKColor(238, 130, 238).WithAlpha(80), //red
                StrokeWidth = yPercent(0.01f)
            };
        }

        /**********************************************************************
        ***********************************************************************
        returns the canvas view */
        public static SKCanvasView ReturnCanvas()
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

