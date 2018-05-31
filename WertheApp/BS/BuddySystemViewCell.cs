using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Diagnostics; //Debug.WriteLine("");
using System.Collections.Generic;

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
        private SKPaint sk_processColor;

        private float xe, ye;

        private List<BuddySystemBlock> buddySystem;

        //CONSTRUCTOR
        /*TODO constructor with parameters is somehow not possible. give buddySystem List to constructor*/
        public BuddySystemViewCell()
        {
            //TODO get buddySystem List through constructor
            buddySystem = new List<BuddySystemBlock>(BuddySystem.buddySystem);
            //note: List<BuddySystemBlock> budd<System  = BuddySystem.buddySystem would not copy the list
            /*TODO gerade im Moment verweist die kopierte Lise noch auf dieselben Elemente man sollte deswegen die Elementeigenschaften 
            sofort rauskopieren und dann verwenden. bevor sie geändert werden. zb. getfree()*/
            /*TODO: Eingabe: 5 dann 2 , dann A beenden. Bleibt hängen*/
            /*TODO: keine Ahnung wieso, aber eingezeichnete Prozesse verschwinden immer nach ein paar eingaben*/
            //IDEE!!! im Konstruktor einfach eine komplett neue buddylist machen die die blocks kopiert und jeweils einen neuen extra erstellt.
            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;

            //changing background color for better clarity
            if (dari % 2 == 0)
            {
                skiaview.BackgroundColor = Color.Azure;
            }
            else
            {
                skiaview.BackgroundColor = Color.WhiteSmoke;
            }
            dari++;


            // assign the canvas to the cell
            this.View = skiaview;
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        // do the drawing
        void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            var surfaceWidth = info.Width;
            var surfaceHeight = info.Height;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            if (canvas != null)
            {
                canvas.Clear(); //Important! Otherwise the drawing will look messed up in iOS
            } //Important! Otherwise the drawing will look messed up in iOS

            /*important: the coordinate system starts in the upper left corner*/
            float lborder = canvas.LocalClipBounds.Left;
            float tborder = canvas.LocalClipBounds.Top;
            float rborder = canvas.LocalClipBounds.Right;
            float bborder = canvas.LocalClipBounds.Bottom;

            this.xe = rborder / 100;
            this.ye = bborder / 100;

            MakeSKPaint(); //depends on xe and ye and therfore has to be called after they were initialized


            // draw on the canvas
            //rect for memory 80units long
            SKRect sk_r = new SKRect(xe * 2, ye * 10, xe * 82, ye * 90); //left, top, right, bottom
            canvas.DrawRect(sk_r, sk_Paint1); //draw an rect with dimensions of sk_r and paint sk_1

            //calculate units for memory
            float memoryUnit = (float)(80f / BuddySystem.absoluteMemorySize);

            //Draw blocks in memory
            float startValue = xe * 2;
            for (int i = 0; i < buddySystem.Count; i++)
            {
                //block
                int blockSize = buddySystem[i].GetBlockSize();
                float value = memoryUnit * blockSize;
                SKPoint sk_p1 = new SKPoint(startValue + xe * value, ye * 10);
                SKPoint sk_p2 = new SKPoint(startValue + xe * value, ye * 90);
                canvas.DrawLine(sk_p1, sk_p2, sk_Paint1);

                startValue = startValue + xe * value;

            }

            //draw processes in memory
            float startValue2 = xe * 2;
            for (int i = 0; i < buddySystem.Count; i++)
            {
                int blockSize2 = buddySystem[i].GetBlockSize();
                float value2 = memoryUnit * blockSize2;
                //process
                int processSize = buddySystem[i].GetProcessSize();
                float processValue = memoryUnit * processSize;
                bool isFree = buddySystem[i].GetFree();
                if (true)
                {
                    SKRect sk_r2 = new SKRect(startValue2, ye * 10 + ye, xe * 82 - xe, ye * 90 - ye);
                    canvas.DrawRect(sk_r2, sk_processColor); //left, top, right, bottom, color
                }

                startValue2 = startValue2 + xe * value2;
            }

            //text
            canvas.DrawText("size: " + BuddySystem.absoluteMemorySize.ToString(), new SKPoint(xe * 84, ye * 50), sk_blackText);

            //execute all drawing actions
            canvas.Flush();
        }
    

        private void MakeSKPaint(){
            //create something to paint with (color, textsize, and even more....)
            sk_Paint1 = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(0, 0, 0) //black
            };

            //process color
            sk_processColor = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                //IsStroke = true,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(20, 200, 0)
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
		/*protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // update the canvas when the data changes
            skiaview.InvalidateSurface();
        }
        */
    }
}
