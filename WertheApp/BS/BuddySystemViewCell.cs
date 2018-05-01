using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Diagnostics; //Debug.WriteLine("");

namespace WertheApp.BS
{
    public class BuddySystemViewCell : ViewCell
    {
        //VARIABLES
        private SKCanvasView skiaview;
        public static int dari; //determines backgroundcolor of the canvas

        private SKPaint sk_Paint1;
        private SKPaint sk_blackText;
        private SKPaint sk_greenText;
        private SKPaint sk_redText;

        private float xe, ye;


        //CONSTRUCTOR
        public BuddySystemViewCell()
        {
            // crate the canvas
            skiaview = new SKCanvasView();

            if(dari%2 == 0){
                skiaview.BackgroundColor = Color.Azure; 
            }else{
                skiaview.BackgroundColor = Color.WhiteSmoke;
            }
            dari++;

            // do the drawing
            skiaview.PaintSurface += (object sender, SKPaintSurfaceEventArgs e) =>
            {
                var surface = e.Surface;
                var surfaceWidth = e.Info.Width;
                var surfaceHeight = e.Info.Height;
                var canvas = surface.Canvas;
                canvas.Clear(); //Important! Otherwise the drawing will look messed up in iOS

                /*important: the coordinate system starts in the upper left corner*/
                float lborder = canvas.LocalClipBounds.Left;
                float tborder = canvas.LocalClipBounds.Top;
                float rborder = canvas.LocalClipBounds.Right;
                float bborder = canvas.LocalClipBounds.Bottom;

                xe = rborder / 100;
                ye = bborder / 100;

                makeSKPaint(); //depends on xe and ye and therfore has to be called after they were initialized


                SKRect sk_r = new SKRect(xe*2, ye*10, xe*80, ye*90); //left, top, right, bottom
                SKPoint sk_p1 = new SKPoint(xe * 50, ye * 10);
                SKPoint sk_p2 = new SKPoint(xe * 50, ye * 90);

                // draw on the canvas
                canvas.DrawRect(sk_r, sk_Paint1); //draw an rect with dimensions of sk_r and paint sk_1
                canvas.DrawLine(sk_p1, sk_p2, sk_Paint1); 
                canvas.DrawText("size: " + BuddySystem.absoluteMemorySize.ToString(), new SKPoint(xe*82, ye*50), sk_blackText);

                //execute all drawing actions
                canvas.Flush();
            };

            // assign the canvas to the cell
            View = skiaview;
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        private void makeSKPaint(){
            //create something to paint with (color, textsize, and even more....)
            sk_Paint1 = new SKPaint
            {
                IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(0, 0, 0) //black
            };

            //black neutral text
            sk_blackText = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye*30
                                
            };

            //green text for starting an process
            sk_greenText = new SKPaint
            {
                Color = SKColors.Green,
                TextSize = ye*30
            };

            //red text for ending an process
            sk_redText = new SKPaint
            {
                Color = SKColors.Red,
                TextSize = ye*30
            };
        }

        /**********************************************************************
        *********************************************************************/
		protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // update the canvas when the data changes
            skiaview.InvalidateSurface();
        }
    }
}
