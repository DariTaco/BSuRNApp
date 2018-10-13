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
        private static SKPaint sk_Paint1;

        //CONSTRUCTOR
        public PageReplacementStrategiesDraw()
        {
            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;
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


            SKRect sk_r2 = new SKRect(1*xe,1*ye,99*xe,99*ye); //left , top, right, bottom
            canvas.DrawRect(sk_r2, sk_Paint1); //left, top, right, bottom, color


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
    }
}
