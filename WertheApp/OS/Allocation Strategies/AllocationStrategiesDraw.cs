using System;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using Xamarin.Forms;
using System.Diagnostics;

namespace WertheApp.OS.AllocationStrategies
{
    public class AllocationStrategiesDraw 
    {
        private static SKCanvasView canvasView; // view containing the drawing
        private static SKCanvas canvas; // drawing
        private static SKImageInfo info; // canvas info

        private static float strokeWidth; // stroke Width for paint colors
        private static SKPaint sk_Background, sk_Black; //paint colors

        public AllocationStrategiesDraw()
        {
            
            // crate the canvas
            canvasView = new SKCanvasView();
            canvasView.PaintSurface += PaintSurface;
            canvasView.BackgroundColor = Color.WhiteSmoke;

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

            var centerX = info.Width / 2;
            var centerY = info.Height / 2;
            var x1 = strokeWidth / 2;
            var y1 = strokeWidth / 2;
            var x2 = info.Width - strokeWidth / 2;
            var y2 = info.Height - strokeWidth / 2;




            //Important! Otherwise the drawing will look messed up in iOS
            if (canvas != null) { canvas.Clear(); }

            MakeSKPaint(); 

            /*********************HERE GOES THE DRAWING************************/
            /*important: the coordinate system starts in the upper left corner*/

            SKRect sk_memory = new SKRect(x1 + xPercent(0.1f), y1 + yPercent(0.3f), x2 - xPercent(0.1f), y2 - yPercent(0.3f)); 
            canvas.DrawRect(sk_memory, sk_Black); 

            //execute all drawing actions
            canvas.Flush();
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
        static private void MakeSKPaint()
        {

            strokeWidth = 5;
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

