﻿using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;

namespace WertheApp.CN
{

    public class CongestionControlDraw
    {
        //VARIABLES
        private static SKCanvasView skiaview;
        private static float xe, ye;
        private static int toggleZoom;

        private static SKPaint sk_blackText, sk_blackTextSmallVertical, sk_blackTextSmallHorizontal, 
                               sk_TextSlowStart, sk_TextCongestionControl, sk_TextFastRecovery;
        private static SKPaint sk_PaintSlowStart, sk_PaintCongestionControl, sk_FastRecovery;
        private static SKPaint sk_PaintVeryThin, sk_PaintThin, sk_PaintFat, sk_PaintFatBlack,
                               sk_PaintTahoe, sk_PaintTahoeFat, sk_PaintReno, sk_PaintRenoFat, 
                               sk_PaintthreshReno, sk_PaintthreshTahoe, sk_PaintthreshBlack;
        private static float textSize;
        private static float strokeWidth;
        private static float xWidth, yWidth;


        public static int stateR, stateT; //0 -> slow start, 1 -> congestion avoidance, 2 -> fast recovery
        public static int maxCwnd, numberOfRounds;

        //CONSTRUCTOR
        public CongestionControlDraw()
        {
            textSize = 5;
            strokeWidth = 0.2f;

            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;

            AddGestureRecognizers();

            stateR = CongestionControl.stateR;
            stateT = CongestionControl.stateT;
            maxCwnd = CongestionControl.maxCwnd;
            numberOfRounds = CongestionControl.numberOfRounds;

            xWidth = 92f / (numberOfRounds + 1);
            yWidth = 93f / (maxCwnd + 1);
            

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
            if(CongestionControl.renoOn && !CongestionControl.tahoeOn){
                state = stateR;
            }
            else if(CongestionControl.tahoeOn && !CongestionControl.renoOn){
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
                    canvas.DrawRect(sk_rBackground, sk_PaintCongestionControl); //left, top, right, bottom, color
                    canvas.DrawText("CONGESTION AVOIDANCE", 50f * xe + xe, 50f * ye, sk_TextCongestionControl);
                    break;
                case 2:
                    canvas.DrawRect(sk_rBackground, sk_FastRecovery); //left, top, right, bottom, color
                    canvas.DrawText("FAST RECOVERY", 50f * xe + xe, 50f * ye, sk_TextFastRecovery);
                    break;
                default: break;
            }

            //draw Graph
            canvas.DrawLine(new SKPoint(5 * xe, 95 * ye), new SKPoint(5 * xe, 2 * ye), sk_PaintThin);
            canvas.DrawLine(new SKPoint(5 * xe, 95* ye), new SKPoint(97 * xe, 95 * ye), sk_PaintThin);

            canvas.DrawLine(new SKPoint(4.3f * xe, 4 * ye), new SKPoint(5 * xe, 2 * ye), sk_PaintThin);
            canvas.DrawLine(new SKPoint(5.8f * xe, 4 * ye), new SKPoint(5 * xe, 2 * ye), sk_PaintThin);

            canvas.DrawLine(new SKPoint(96 * xe, 93.3f * ye), new SKPoint(97 * xe, 95 * ye), sk_PaintThin);
            canvas.DrawLine(new SKPoint(96 * xe, 96.7f * ye), new SKPoint(97 * xe, 95 * ye), sk_PaintThin);

            //zero point
            canvas.DrawText("0", 4 * xe, 99 * ye, sk_blackText);

            //numbers in Y-Achsis (rate)
            float posY = 95f - yWidth;
            for (int i = 1; i <= maxCwnd; i++)
            {
                if(i % 2 == 0){
                    canvas.DrawLine(new SKPoint(5 * xe, posY * ye), new SKPoint(97 * xe, posY * ye), sk_PaintVeryThin);
                }
                canvas.DrawLine(new SKPoint(4 * xe, posY * ye), new SKPoint(5 * xe, posY * ye), sk_PaintThin);
                if(i < 10){
                    canvas.DrawText(i.ToString(), 3 * xe, posY * ye, sk_blackTextSmallVertical);
                }else{
                    canvas.DrawText("1", 2 * xe, posY * ye, sk_blackTextSmallVertical);
                    canvas.DrawText((i % 10).ToString(), 3 * xe, posY * ye, sk_blackTextSmallVertical);
                }
                posY -= yWidth;
            }

            //numbers in X-Achsis (rounds)
            float posX = 5f + xWidth;
            for (int i = 1; i <= numberOfRounds; i++)
            {
                canvas.DrawLine(new SKPoint(posX * xe, 96 * ye), new SKPoint(posX * xe, 94 * ye), sk_PaintThin);
                canvas.DrawText(i.ToString(), posX * xe, 99 * ye, sk_blackTextSmallHorizontal);
                posX += xWidth;
            }

            //the words "cwnd" and "round"
            canvas.DrawText("cwnd", 2 * xe, 4 * ye, sk_blackText);
            canvas.DrawText("rnd", 98 * xe, 99 * ye, sk_blackText);

            //RENO
            if(CongestionControl.renoOn){
                //RENO: cwnd values dots and lines
                float posRateX = 5f;
                float posRateY = 95f;
                float posRateYOld = 95f;
                for (int i = 0; i <= CongestionControl.currentIndex; i++)
                {
                    posRateY = 95f - (yWidth * CongestionControl.reno[0,i]); //get value from array and convert it
                    if ((i - 1) >= 0)
                    {
                        posRateYOld = 95f - (yWidth * CongestionControl.reno[0, i - 1]);

                        if (CongestionControl.reno[1, i] != CongestionControl.reno[1, i - 1])
                        {
                            canvas.DrawLine(new SKPoint((posRateX - xWidth) * xe, posRateYOld * ye), new SKPoint(posRateX * xe, posRateY * ye), sk_PaintReno);
                        }
                        else
                        {
                            canvas.DrawLine(new SKPoint(posRateX* xe, posRateYOld * ye), new SKPoint(posRateX * xe, posRateY * ye), sk_PaintReno);
                        }

                    }
                    canvas.DrawPoint(posRateX * xe, posRateY * ye, sk_PaintRenoFat);
                    if(CongestionControl.reno[1,i] != CongestionControl.reno[1, i + 1])
                    {
                        posRateX += xWidth;
                    }
                }

                //RENO: threshold
                float posRateX3;
                float posRateY3;

                for (int i = 0; i <= CongestionControl.currentIndex; i++)
                {
                    posRateX3 = 5f + (xWidth * (CongestionControl.reno[1, i]));

                    //draw the new thresh for the new round 
                    posRateY3 = 95f - (yWidth * CongestionControl.ssthreshR[0, i]);
                    canvas.DrawLine(new SKPoint((posRateX3 - xWidth) * xe, posRateY3 * ye), new SKPoint(posRateX3 * xe, posRateY3 * ye), sk_PaintthreshReno);

                }
            }

            //TAHOE
            if(CongestionControl.tahoeOn){
                //TAHOE: cwnd values dots and lines
                float posRateX2 = 5f;
                float posRateY2 = 95f;
                float posRateY2Old = 95f;
                for (int i = 0; i <= CongestionControl.currentIndex; i++)
                {
                    posRateY2 = 95f - (yWidth * CongestionControl.tahoe[0,i]); //get value from array and convert it
                    if ((i - 1) >= 0)
                    {
                        posRateY2Old = 95f - (yWidth * CongestionControl.tahoe[0,i - 1]);
                        if (CongestionControl.tahoe[1, i] != CongestionControl.tahoe[1, i - 1])
                        {
                            canvas.DrawLine(new SKPoint((posRateX2 - xWidth) * xe, posRateY2Old * ye), new SKPoint(posRateX2 * xe, posRateY2 * ye), sk_PaintTahoe);
                        }
                        else
                        {
                            canvas.DrawLine(new SKPoint(posRateX2* xe, posRateY2Old * ye), new SKPoint(posRateX2 * xe, posRateY2 * ye), sk_PaintTahoe);
                        }

                    }
                    canvas.DrawPoint(posRateX2 * xe, posRateY2 * ye, sk_PaintTahoeFat);
                    if (CongestionControl.tahoe[1, i] != CongestionControl.tahoe[1, i + 1])
                    {
                        posRateX2 += xWidth;
                    }
                }

                //TAHOE: threshold
                float posRateX4;
                float posRateY4;

                for (int i = 0; i <= CongestionControl.currentIndex; i++)
                {
                    posRateX4 = 5f + (xWidth * CongestionControl.tahoe[1, i]);

                    //draw the new thresh for the new round 
                    posRateY4 = 95f - (yWidth * CongestionControl.ssthreshT[0,i]); //get value from array and convert it
                    canvas.DrawLine(new SKPoint((posRateX4 - xWidth) * xe, posRateY4 * ye), new SKPoint(posRateX4 * xe, posRateY4 * ye), sk_PaintthreshTahoe);
                }
            }

            //Black when both displayed and overlapping
            if(CongestionControl.tahoeOn && CongestionControl.renoOn){
                //cwnd
                float posRateX = 5f;
                float posRateY = 95f;
                float posRateYOld = 95f;
                int valT, valTOld;
                int valR, valROld;
      
                for (int i = 0; i <= CongestionControl.currentIndex; i++){
                    posRateY = 95f - (yWidth * CongestionControl.tahoe[0,i]); //get value from array and convert it
                    valT = CongestionControl.tahoe[0,i];
                    valR = CongestionControl.reno[0,i];

                    if ((i - 1) >= 0)
                    {
                        valROld = CongestionControl.reno[0,i - 1];
                        valTOld = CongestionControl.tahoe[0,i - 1];
                        if(valT == valR && valT != 0 && valTOld == valROld && valTOld != 0){
                            posRateYOld = 95f - (yWidth * CongestionControl.tahoe[0,i - 1]);

                        
                            if (CongestionControl.tahoe[1, i] != CongestionControl.tahoe[1, i - 1])
                            {
                                canvas.DrawLine(new SKPoint((posRateX - xWidth) * xe, posRateYOld * ye), new SKPoint(posRateX * xe, posRateY * ye), sk_PaintThin);
                            }
                            else
                            {
                                canvas.DrawLine(new SKPoint(posRateX * xe, posRateYOld * ye), new SKPoint(posRateX * xe, posRateY * ye), sk_PaintThin);
                            }
                        }
                    }

                    if (valT == valR && valT != 0){
                        canvas.DrawPoint(posRateX * xe, posRateY * ye, sk_PaintFatBlack);
                    }
                    
                    if (CongestionControl.tahoe[1, i] != CongestionControl.tahoe[1, i + 1])
                    {
                        posRateX += xWidth;
                    }

                }

                //thresh
                float posRateX2;
                float posRateY2;
                int ssthreshValT;
                int ssthreshValR;

                //since the rounds of tahoe and reno will always be the same, use tahoe
                for (int i = 0; i <= CongestionControl.currentIndex; i++)
                {
                    ssthreshValT = CongestionControl.ssthreshT[0, i];
                    ssthreshValR = CongestionControl.ssthreshR[0, i];

                    //draw the new thresh for the new round in black if reno and tahoe overlap
                    if (ssthreshValR == ssthreshValT)
                    {
                        posRateX2 = 5f + (xWidth * CongestionControl.tahoe[1, i]);
                        posRateY2 = 95f - (yWidth * CongestionControl.ssthreshT[0, i]); //get value from array and convert it
                        canvas.DrawLine(new SKPoint((posRateX2 - xWidth) * xe, posRateY2 * ye), new SKPoint(posRateX2 * xe, posRateY2 * ye), sk_PaintthreshBlack);
                    }

                }

            }

            //execute all drawing actions
            canvas.Flush();
        }

        /**********************************************************************
        *********************************************************************/
        static private void MakeSKPaint()
        {
            //For Text
            sk_blackText = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * textSize/1.4f,
                IsAntialias = true,
                IsStroke = false,
                TextAlign = SKTextAlign.Center,
            };

            sk_blackTextSmallVertical = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * textSize / 1.8f,
                IsAntialias = true,
                IsStroke = false,
                TextAlign = SKTextAlign.Center,
            };

            sk_blackTextSmallHorizontal = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * textSize / 1.8f,
                IsAntialias = true,
                IsStroke = false, 
                TextAlign = SKTextAlign.Center,
            };

            sk_TextSlowStart = new SKPaint
            {
                Color = new SKColor(147, 230, 22).WithAlpha(50),
                TextSize = ye * textSize*3,
                IsAntialias = true,
                IsStroke = false, 
                TextAlign = SKTextAlign.Center,
                FakeBoldText = true
            };

            sk_TextCongestionControl = new SKPaint
            {
                Color = new SKColor(238, 130, 238).WithAlpha(40),
                TextSize = ye * textSize*3,
                IsAntialias = true,
                IsStroke = false, 
                TextAlign = SKTextAlign.Center,
                FakeBoldText = true
            };

            sk_TextFastRecovery = new SKPaint
            {
                Color = new SKColor(67, 110, 238).WithAlpha(30),
                TextSize = ye * textSize*3,
                IsAntialias = true,
                IsStroke = false, 
                TextAlign = SKTextAlign.Center,
                FakeBoldText = true
            };


            //For Lines
            sk_PaintVeryThin = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth / 4f * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 0) //black
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

            sk_PaintFatBlack = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * 4 * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 0) //black
            };

            sk_PaintReno = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(255, 0, 64) //red
            };

            sk_PaintRenoFat = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * 4 * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(255, 0, 64) //red
            };

            sk_PaintTahoe = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(2, 2, 219) //blue
            };

            sk_PaintTahoeFat = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * 4 * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(2, 2, 219) //blue
            };

            sk_PaintthreshReno = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(255, 0, 64) //red
            };

            sk_PaintthreshTahoe = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(2, 2, 219) //blue
            };

            sk_PaintthreshBlack = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 0) //blue
            };

            //For Background
            sk_PaintSlowStart = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(172, 255, 47).WithAlpha(30)
            };

            sk_PaintCongestionControl = new SKPaint
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
