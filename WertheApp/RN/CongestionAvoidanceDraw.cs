using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Diagnostics; //Debug.WriteLine("");
using System.Collections.Generic;
namespace WertheApp.RN
{
    //TODO: IsStroke = false //somehow since the newest update this doesnt work anymore for ios
    public class CongestionAvoidanceDraw
    {
        //VARIABLES
        private static SKCanvasView skiaview;
        private static float xe, ye;
        private static int toggleZoom;

        private static SKPaint sk_blackText, sk_blackTextSmallVertical, sk_blackTextSmallHorizontal, sk_TextSlowStart, sk_TextCongestionAvoidance, sk_TextFastRecovery;
        private static SKPaint sk_PaintSlowStart, sk_PaintCongestionAvoidance, sk_FastRecovery;
        private static SKPaint sk_PaintThin, sk_PaintFat;
        private static float textSize;
        private static float strokeWidth;
        private static float xWidth, yWidth;


        public static int stateR, stateT; //0 -> slow start, 1 -> congestion avoidance, 2 -> fast recovery
        public static int maxRate, numberOfSteps;

        //CONSTRUCTOR
        public CongestionAvoidanceDraw()
        {
            textSize = 5;
            strokeWidth = 0.2f;

            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;

            AddGestureRecognizers();

            stateR = CongestionAvoidance.stateR;
            stateT = CongestionAvoidance.stateT;
            maxRate = CongestionAvoidance.maxRate;
            numberOfSteps = CongestionAvoidance.numberOfSteps;

            xWidth = 92f / (numberOfSteps + 1);
            yWidth = 93f / (maxRate + 1);
            

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

            /*********************HERE GOES THE DRAWING************************/

            //draw Background (which indicates the current state) . Do not draw Background if Tahoe and Reno are displayed
            int state = 3;
            if(CongestionAvoidance.renoOn && !CongestionAvoidance.tahoeOn){
                state = stateR;
            }
            else if(CongestionAvoidance.tahoeOn && !CongestionAvoidance.renoOn){
                state = stateT;
            }
            SKRect sk_rBackground = new SKRect(00 * xe, 0 * ye, 100 * xe, 100 * ye); //left , top, right, bottom
            switch (state)
            {
                case 0:
                    canvas.DrawRect(sk_rBackground, sk_PaintSlowStart); //left, top, right, bottom, color
                    canvas.DrawText("SLOW START", 50f * xe + xe, 50f * ye, sk_TextSlowStart);
                    break;
                case 1:
                    canvas.DrawRect(sk_rBackground, sk_PaintCongestionAvoidance); //left, top, right, bottom, color
                    canvas.DrawText("CONGESTION AVOIDANCE", 50f * xe + xe, 50f * ye, sk_TextCongestionAvoidance);
                    break;
                case 2:
                    canvas.DrawRect(sk_rBackground, sk_FastRecovery); //left, top, right, bottom, color
                    canvas.DrawText("FAST RECOVERY", 50f * xe + xe, 50f * ye, sk_TextFastRecovery);
                    break;
                default: break;
            }

            //draw Graph
            canvas.DrawLine(new SKPoint(5 * xe, 95 * ye), new SKPoint(5 * xe, 2 * ye), sk_PaintFat);
            canvas.DrawLine(new SKPoint(5 * xe, 95* ye), new SKPoint(97 * xe, 95 * ye), sk_PaintFat);

            canvas.DrawLine(new SKPoint(4 * xe, 4 * ye), new SKPoint(5 * xe, 2 * ye), sk_PaintFat);
            canvas.DrawLine(new SKPoint(6 * xe, 4 * ye), new SKPoint(5 * xe, 2 * ye), sk_PaintFat);

            canvas.DrawLine(new SKPoint(96 * xe, 93 * ye), new SKPoint(97 * xe, 95 * ye), sk_PaintFat);
            canvas.DrawLine(new SKPoint(96 * xe, 97 * ye), new SKPoint(97 * xe, 95 * ye), sk_PaintFat);

            //zero point
            canvas.DrawText("0", 4 * xe, 99 * ye, sk_blackText);

            //numbers in Y-Achsis
            float posY = 95f - yWidth;
            for (int i = 1; i <= maxRate; i++)
            {
                canvas.DrawLine(new SKPoint(4 * xe, posY * ye), new SKPoint(5 * xe, posY * ye), sk_PaintThin);
                if(i < 10){
                    canvas.DrawText(i.ToString(), 3 * xe, posY * ye, sk_blackTextSmallVertical);
                }else{
                    canvas.DrawText("1", 2 * xe, posY * ye, sk_blackTextSmallVertical);
                    canvas.DrawText((i % 10).ToString(), 3 * xe, posY * ye, sk_blackTextSmallVertical);
                }
                posY -= yWidth;
            }

            //numbers in X-Achsis
            float posX = 5f + xWidth;
            for (int i = 1; i <= numberOfSteps; i++)
            {
                canvas.DrawLine(new SKPoint(posX * xe, 96 * ye), new SKPoint(posX * xe, 94 * ye), sk_PaintThin);
                canvas.DrawText(i.ToString(), posX * xe, 99 * ye, sk_blackTextSmallHorizontal);
                posX += xWidth;
            }

            //rate and step
            canvas.DrawText("rate", 2 * xe, 4 * ye, sk_blackText);
            canvas.DrawText("step", 98 * xe, 99 * ye, sk_blackText);
            // canvas.DrawText("HELLO WORLD", 20 * xe, 90 * ye, sk_blackTextSmall);

            //execute all drawing actions
            canvas.Flush();
        }

        /**********************************************************************
        *********************************************************************/
        static private void MakeSKPaint()
        {
            sk_blackText = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * textSize/1.3f,
                IsAntialias = true,
                IsStroke = false, //TODO: somehow since the newest update this doesnt work anymore for ios
                TextAlign = SKTextAlign.Center,
                IsVerticalText = false
            };

            sk_blackTextSmallVertical = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * textSize / 1.8f,
                IsAntialias = true,
                IsStroke = false, //TODO: somehow since the newest update this doesnt work anymore for ios
                TextAlign = SKTextAlign.Center,
                IsVerticalText = true
            };

            sk_blackTextSmallHorizontal = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * textSize / 1.8f,
                IsAntialias = true,
                IsStroke = false, //TODO: somehow since the newest update this doesnt work anymore for ios
                TextAlign = SKTextAlign.Center,
                IsVerticalText = false
            };

            sk_TextSlowStart = new SKPaint
            {
                Color = new SKColor(147, 230, 22).WithAlpha(50),
                TextSize = ye * textSize*3,
                IsAntialias = true,
                IsStroke = false, //TODO: somehow since the newest update this doesnt work anymore for ios
                TextAlign = SKTextAlign.Center,
                FakeBoldText = true
            };

            sk_TextCongestionAvoidance = new SKPaint
            {
                Color = new SKColor(238, 130, 238).WithAlpha(40),
                TextSize = ye * textSize*3,
                IsAntialias = true,
                IsStroke = false, //TODO: somehow since the newest update this doesnt work anymore for ios
                TextAlign = SKTextAlign.Center,
                FakeBoldText = true
            };

            sk_TextFastRecovery = new SKPaint
            {
                Color = new SKColor(67, 110, 238).WithAlpha(30),
                TextSize = ye * textSize*3,
                IsAntialias = true,
                IsStroke = false, //TODO: somehow since the newest update this doesnt work anymore for ios
                TextAlign = SKTextAlign.Center,
                FakeBoldText = true
            };


            sk_PaintThin = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 0) //black
            };

            sk_PaintFat = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * 2 * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 0) //black
            };

            sk_PaintSlowStart = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(172, 255, 47).WithAlpha(30)
            };

            sk_PaintCongestionAvoidance = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(238, 130, 238).WithAlpha(30)
            };

            sk_FastRecovery = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(67, 110, 238).WithAlpha(30)
            };
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
        private static void AddGestureRecognizers()
        {
            //add tap Gestrue Recognizer for zooming in and out
            toggleZoom = 0;
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.NumberOfTapsRequired = 2; // double-tap
            tapGestureRecognizer.Tapped += (s, e) => {
                // handle the tap
                if (toggleZoom == 0)
                {
                    skiaview.ScaleTo(2);
                    toggleZoom = 1;
                }
                else
                {

                    skiaview.ScaleTo(1);
                    toggleZoom = 0;
                    skiaview.TranslateTo(0, 0);
                }

            };
            skiaview.GestureRecognizers.Add(tapGestureRecognizer);

            //add pan gesture recognizer
            double x = 0;
            double y = 0;
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += (s, e) => {

                // Handle the pan (only in zoomed in state)
                if (toggleZoom == 1 && e.StatusType != GestureStatus.Completed)
                {
                    //only when within screen bounds or 20% more or less
                    if (x + e.TotalX >= skiaview.Width * 1.2f / 2 * -1
                       && x + e.TotalX <= skiaview.Width * 1.2f / 2
                       && y + e.TotalY >= skiaview.Height * 1.2f / 2 * -1
                       && y + e.TotalY <= skiaview.Height * 1.2f / 2)
                    {
                        x = x + e.TotalX;
                        y = y + e.TotalY;
                        skiaview.TranslateTo(x, y);
                    }
                }
            };
            skiaview.GestureRecognizers.Add(panGesture);
        }
    }
}
