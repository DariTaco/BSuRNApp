using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Diagnostics; //Debug.WriteLine("");

namespace WertheApp.BS
{
    public class BuddySystemViewCell : ViewCell
    {
        private SKCanvasView skiaview;
        public static int dari;

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
            Debug.WriteLine(dari);


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

                float xe = rborder / 100;
                float ye = bborder / 100;

                //create something to paint with (color, textsize, and even more....)
                SKPaint sk_1 = new SKPaint
                {
                    IsStroke = true, //indicates whether to paint the stroke or the fill
                    StrokeWidth = 5,
                    IsAntialias = true,
                    Color = new SKColor(0, 0, 0) //black
                };

                SKRect sk_r = new SKRect(xe*2, ye*10, xe*80, ye*90); //left, top, right, bottom

                // draw on the canvas
                canvas.DrawRect(sk_r, sk_1); //draw an rect with dimensions of sk_r and paint sk_1
                //execute all drawing actions
                canvas.Flush();
            };

            // assign the canvas to the cell
            View = skiaview;
        }

		protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // update the canvas when the data changes
            skiaview.InvalidateSurface();
        }
    }
}
