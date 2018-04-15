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
        public static int dari = 0;

        public BuddySystemViewCell()
        {
            // crate the canvas
            skiaview = new SKCanvasView();

            if(dari%2 == 0){
                skiaview.BackgroundColor = Color.Aquamarine; 
            }else{
                skiaview.BackgroundColor = Color.Firebrick;
            }
            dari++;


            // do the drawing
            skiaview.PaintSurface += (object sender, SKPaintSurfaceEventArgs e) =>
            {
                var surface = e.Surface;
                var surfaceWidth = e.Info.Width;
                var surfaceHeight = e.Info.Height;
                var canvas = surface.Canvas;

                float lborder = canvas.LocalClipBounds.Left;
                float tborder = canvas.LocalClipBounds.Top;
                float rborder = canvas.LocalClipBounds.Right;
                float bborder = canvas.LocalClipBounds.Bottom;

                //-1 -1 1281 134
                //-1 -1 2185 200
                /*TODO get absolute metrics of canvas*/

                Debug.WriteLine(lborder+ " " + tborder + " " + rborder + " " + bborder);


                //create something to paint with (color, textsize, and even more....)
                SKPaint sk_1 = new SKPaint
                {
                    IsStroke = true, //indicates whether to paint the stroke or the fill
                    StrokeWidth = 5,
                    IsAntialias = true,
                    Color = new SKColor(0, 0, 0) //black
                };

                SKRect sk_r = new SKRect(lborder+20, tborder+20, rborder-100, bborder); //left, top, right, bottom
                /*TODO the rect looks always the same in ios*/
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
